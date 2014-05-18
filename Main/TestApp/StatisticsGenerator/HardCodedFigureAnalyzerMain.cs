using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class HardCodedFigureAnalyzerMain
    {
        // The problem parameters to analyze
        private List<ConcreteAST.GroundedClause> figure;
        private List<ConcreteAST.GroundedClause> givens;

        private Precomputer.CoordinatePrecomputer precomputer;
        private Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        private GenericInstantiator.Instantiator instantiator;
        private Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> pebblerGraph;
        private ProblemAnalyzer.PathGenerator pathGenerator;
        private GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator templateProblemGenerator = null;
        private ProblemAnalyzer.InterestingProblemCalculator interestingCalculator;
//        private ProblemAnalyzer.QueryFeatureVector queryVector;
//        private ProblemAnalyzer.PartitionedProblemSpace problemSpacePartitions;
        private List<ConcreteAST.GroundedClause> goals;

        public HardCodedFigureAnalyzerMain(List<ConcreteAST.GroundedClause> fs, List<ConcreteAST.GroundedClause> gs, List<ConcreteAST.GroundedClause> gls)
        {
            this.figure = fs;
            this.givens = gs;
            this.goals = gls;

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\givensCount.txt", true))
            //{
            //    file.WriteLine(this.givens.Count);
            //}

            // Create the precomputer object for coordinate-based pre-comutation analysis
            precomputer = new Precomputer.CoordinatePrecomputer(figure);
            instantiator = new GenericInstantiator.Instantiator();
            //queryVector = new ProblemAnalyzer.QueryFeatureVector(givens.Count);
        }

        // Returns: <number of interesting problems, number of original problems generated>
        public StatisticsGenerator.FigureStatisticsAggregator AnalyzeFigure()
        {
            StatisticsGenerator.FigureStatisticsAggregator figureStats = new StatisticsGenerator.FigureStatisticsAggregator();

            // For statistical analysis, count the number of each particular type of implicit facts.
            CountIntrisicProperties(figureStats);

            // Start timing
            figureStats.stopwatch.Start();

            // Precompute all coordinate-based interesting relations (problem goal nodes)
            // These become the basis for the template-based problem generation (these are the goals)
            Precompute();

            // Handle givens that strengthen the intrinsic parts of the figure; modifies if needed
            givens = DoGivensStrengthenFigure();


            // Use a worklist technique to instantiate nodes to construct the hypergraph for this figure
            ConstructHypergraph();

            // Create the integer-based hypergraph representation
            ConstructPebblingHypergraph();

            // Pebble that hypergraph
            Pebble();

            // Analyze paths in the hypergraph to generate the pair of <forward problems, backward problems> (precomputed nodes are goals)
            KeyValuePair<List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>, List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>> problems = GenerateTemplateProblems();

            // Combine the problems together into one list
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> candidateProbs = new List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>();
            candidateProbs.AddRange(problems.Key);
            //candidateProbs.AddRange(problems.Value); // converse

            figureStats.totalBackwardProblemsGenerated = problems.Value.Count;
            figureStats.totalProblemsGenerated = candidateProbs.Count;

            // Determine which, if any, of the problems are interesting (using definition that 100% of the givens are used)
            interestingCalculator = new ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens, goals);
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> interestingProblems = interestingCalculator.DetermineInterestingProblems(candidateProbs);
            figureStats.totalInterestingProblems = interestingProblems.Count;

            // Explicit number of facts: hypergraph size - figure facts
            figureStats.totalExplicitFacts = graph.vertices.Count - figure.Count;

            // Validate that the original book problems were generated.
            Validate(interestingProblems, figureStats);

            // Stop timing before we generate all of the statistics
            figureStats.stopwatch.Stop();


            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> strictlyInteresting = GetStrictlyInteresting(interestingProblems);

            // Construct partitions based on different queries
            // GenerateStatistics(interestingProblems, figureStats, strictlyInteresting);

            //
            // Is this figure complete? That is, do the assumptions define the figure completely?
            //
            figureStats.isComplete = templateProblemGenerator.DefinesFigure(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses());

            // Debug.WriteLine("Explicit Complete: " + figureStats.isComplete);

            // Construct the K-goal problems
            // CalculateKnonStrictCardinalities(interestingCalculator, interestingProblems, figureStats);

            return figureStats;
        }

        //
        // Given the problems with at least one assumption, construct ALL such combinations to form (I, G).
        //
        private void CalculateKnonStrictCardinalities(ProblemAnalyzer.InterestingProblemCalculator interestingCalculator,
                                                      List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems,
                                                      StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            // K-G  container: index 0 is 1-G, index 1 is 2-G, etc.
            List<List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>> KmgProblems = new List<List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>>();

            //
            // Create the new set of multigoal problems each with 1 goal: 1-G
            //
            List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>> mgProblems = new List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>();
            foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation> new1GProblem = new ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>();
                new1GProblem.AddProblem(problem);
                mgProblems.Add(new1GProblem);
            }

            // Add the 1-G problems to the K-G problem set.
            KmgProblems.Add(mgProblems);


            // Construct all of the remaining 
            CalculateKnonStrictCardinalities(KmgProblems, problems, FigureStatisticsAggregator.MAX_K);

            //
            // Now that we have 1, 2, ..., MAX_K -G multigoal problems, we must filter them.
            // That is, are the problems strictly interesting?
            //
            // Filtered K-G  container: index 0 is 1-G, index 1 is 2-G, etc.
            List<List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>> filteredKmgProblems = new List<List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>>();

            foreach (List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>> originalKgProblems in KmgProblems)
            {
                filteredKmgProblems.Add(interestingCalculator.DetermineStrictlyInterestingMultiGoalProblems(originalKgProblems));
            }

            // Calculate the final numbers: counts of the k-G Strictly interesting problems.
            StringBuilder str = new StringBuilder();
            for (int k = 1; k <= FigureStatisticsAggregator.MAX_K; k++)
            {
                figureStats.kGcardinalities[k] = filteredKmgProblems[k - 1].Count;

                str.AppendLine(k + "-G: " + figureStats.kGcardinalities[k]);
            }

            Debug.WriteLine(str);

            if (Utilities.PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine(str);
            }
        }

        // Calculate k-G; a set of goals with k propositions
        private void CalculateKnonStrictCardinalities(List<List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>> kgProblems,
                                                      List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> interesting, int MAX_K)
        {
            for (int k = 2; k <= MAX_K; k++)
            {
                // (k-1)-G list: 
                List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>> kMinus1Problems = kgProblems[k-2];
                List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>> kProblems = new List<ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>>();

                // For each (k-1)-G problem, add each interesting problem, in turn.
                foreach(ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation> kMinus1Problem in kMinus1Problems)
                {
                    if (kMinus1Problem.givens.Count < givens.Count)
                    {
                        // For each k-G, make a copy of the (k-1)-G problem and add the interesting problem to it.
                        foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> sgProblem in interesting)
                        {
                            // Make a copy
                            ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation> newKGproblem = new ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation>(kMinus1Problem);

                            // Add the interesting problem to it; if the add is successful, the k-G problem is added to 
                            if (newKGproblem.AddProblem(sgProblem))
                            {
                                // Debug.WriteLine(kMinus1Problem.ToString() + " + " + sgProblem.ToString() + " = " + newKGproblem.ToString());

                                // Is this problem, based on (Givens, Goals) in this list already?
                                bool alreadyExists = false;
                                foreach (ProblemAnalyzer.MultiGoalProblem<Hypergraph.EdgeAnnotation> kProblem in kProblems)
                                {
                                    if (kProblem.HasSameGivensGoals(newKGproblem))
                                    {
                                        alreadyExists = true;
                                        break;
                                    }
                                }

                                if (!alreadyExists) kProblems.Add(newKGproblem);
                            }
                        }
                    }
                }

                // Add the complete set of k-G problems to the list
                kgProblems.Add(kProblems);
            }
        }


        private List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> GetStrictlyInteresting(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems)
        {
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> strictlyInteresting = new List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>();

            foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                if (problem.interestingPercentage >= 100) strictlyInteresting.Add(problem);
            }

            return strictlyInteresting;
        }

        //
        // For statistical analysis only count the number of occurrences of each intrisic property.
        //
        private void CountIntrisicProperties(StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            foreach (ConcreteAST.GroundedClause clause in figure)
            {
                figureStats.totalProperties++;
                if (clause is ConcreteAST.Point) figureStats.numPoints++;
                else if (clause is ConcreteAST.InMiddle) figureStats.numInMiddle++;
                else if (clause is ConcreteAST.Segment) figureStats.numSegments++;
                else if (clause is ConcreteAST.Intersection) figureStats.numIntersections++;
                else if (clause is ConcreteAST.Triangle) figureStats.numTriangles++;
                else if (clause is ConcreteAST.Angle) figureStats.numAngles++;
                else if (clause is ConcreteAST.Quadrilateral) figureStats.numQuadrilaterals++;
                else if (clause is ConcreteAST.Circle) figureStats.numCircles++;
                else
                {
                    Debug.WriteLine("Did not count " + clause);
                    figureStats.totalProperties--;
                }
            }
        }

        //
        // Use threads to precompute all forward relations and strengthening 
        //
        private void Precompute()
        {
            //precomputer.CalculateRelations();
            //precomputer.CalculateStrengthening();

            Thread precomputeRelations = new Thread(new ThreadStart(precomputer.CalculateRelations));
            Thread precomputeStrengthening = new Thread(new ThreadStart(precomputer.CalculateStrengthening));

            // Start and indicate thread joins for these short computations threads
            try
            {
                precomputeRelations.Start();
                precomputeStrengthening.Start();
                precomputeRelations.Join();
                precomputeStrengthening.Join();
            }
            catch (ThreadStateException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        //
        // Modify the given information to account for redundancy in stated nodes
        // That is, does given information strengthen a figure node?
        //
        private List<ConcreteAST.GroundedClause> DoGivensStrengthenFigure()
        {
            List<ConcreteAST.GroundedClause> modifiedGivens = new List<ConcreteAST.GroundedClause>();
            ConcreteAST.GroundedClause currentGiven = null;

            foreach (ConcreteAST.GroundedClause given in givens)
            {
                currentGiven = given;
                foreach (ConcreteAST.GroundedClause component in figure)
                {
                    if (component.CanBeStrengthenedTo(given))
                    {
                        currentGiven = new ConcreteAST.Strengthened(component, given);
                        break;
                    }
                }
                modifiedGivens.Add(currentGiven);
            }

            return modifiedGivens;
        }

        //
        // Construct the main Hypergraph
        //
        private void ConstructHypergraph()
        {
            // Resets all saved data to allow multiple problems
            GenericInstantiator.Instantiator.Clear();

            // Build the hypergraph through instantiation
            graph = instantiator.Instantiate(figure, givens);

            if (Utilities.DEBUG)
            {
                graph.DumpNonEquationClauses();
                graph.DumpEquationClauses();
            }
        }

        //
        // Create the Pebbler version of the hypergraph for analysis (integer-based hypergraph)
        //
        private void ConstructPebblingHypergraph()
        {
            // Create the Pebbler version of the hypergraph (all integer representation) from the original hypergraph
            pebblerGraph = graph.GetPebblerHypergraph();

            // Provide the original hypergraph for reference
            pebblerGraph.SetOriginalHypergraph(graph);
        }

        //
        // Actually perform pebbling on the integer hypergraph (pebbles forward and backward)
        //
        private void Pebble()
        {
            pathGenerator = new ProblemAnalyzer.PathGenerator(graph);

            // Acquire the integer values of the intrinsic / figure nodes
            List<int> intrinsicSet = Utilities.CollectGraphIndices(graph, figure);

            // Acquire the integer values of the givens (from the original 
            List<int> givenSet = Utilities.CollectGraphIndices(graph, givens);

            // Perform pebbling based on the <figure, given> pair.
            pebblerGraph.Pebble(intrinsicSet, givenSet);

            if (Utilities.PEBBLING_DEBUG)
            {
//                Debug.WriteLine("Forward Vertices after pebbling:");
//                for (int i = 0; i < pebblerGraph.vertices.Length; i++)
//                {
//                    StringBuilder strLocal = new StringBuilder();
//                    strLocal.Append(pebblerGraph.vertices[i].id + ": pebbled(");
//                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.NO_PEBBLE) strLocal.Append("NONE");
//                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.RED_FORWARD) strLocal.Append("RED");
//                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.BLUE_BACKWARD) strLocal.Append("BLUE");
////                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.PURPLE_BOTH) strLocal.Append("PURPLE");
//                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.BLACK_EDGE) strLocal.Append("BLACK");
//                    strLocal.Append(")");
//                    Debug.WriteLine(strLocal.ToString());
//                }

                pebblerGraph.DebugDumpEdges();
            }
        }

        //
        // Generate all of the problems based on the precomputed values (these precomputations are the problem goals)
        //
        private KeyValuePair<List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>, List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>> GenerateTemplateProblems()
        {
            templateProblemGenerator = new ProblemAnalyzer.TemplateProblemGenerator(graph, pebblerGraph, pathGenerator);

            // Generate the problem pairs
            return templateProblemGenerator.Generate(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses(), givens);
        }

        //
        // Given, the list of generated, interesting problems, validate (for soundness) the fact that the original book problem was generated.
        // Do so by constructing a goal-based isomorphism partitioning and check that there exists a problem with the same given set.
        //
        private void Validate(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, FigureStatisticsAggregator figureStats)
        {
            ProblemAnalyzer.QueryFeatureVector query = ProblemAnalyzer.QueryFeatureVector.ConstructGoalIsomorphismQueryVector();

            ProblemAnalyzer.PartitionedProblemSpace goalBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, query);
            
            goalBasedPartitions.ConstructPartitions(problems);

            // Validate that we have generated all of the original problems from the text.
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> generatedBookProblems = goalBasedPartitions.ValidateOriginalProblems(givens, goals);
            figureStats.totalBookProblemsGenerated = generatedBookProblems.Count;

            if (Utilities.PROBLEM_GEN_DEBUG)
            {
                goalBasedPartitions.DumpPartitions();
            }
            if (Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("\nAll " + generatedBookProblems.Count + " Book-specified problems: \n");
                foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> bookProb in generatedBookProblems)
                {
                    Debug.WriteLine(bookProb.ConstructProblemAndSolution(graph));
                }
            }

            figureStats.goalPartitionSummary = goalBasedPartitions.GetGoalBasedPartitionSummary();
        }

        //
        // We may analyze the interesting problems constructing various partitions and queries
        //
        private void GenerateStatistics(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems,
            StatisticsGenerator.FigureStatisticsAggregator figureStats,
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> strictlyInteresting)
        {
            GenerateAverages(problems, figureStats);
            GenerateIsomorphicStatistics(problems, figureStats);

            GenerateStrictAverages(strictlyInteresting, figureStats);
            GenerateStrictIsomorphicStatistics(strictlyInteresting, figureStats);

            GeneratePaperQuery(strictlyInteresting, figureStats);
        }

        private void GenerateAverages(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            int totalWidth = 0;
            int totalLength = 0;
            int totalDeductiveSteps = 0;

            foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                totalWidth += problem.GetWidth();
                totalLength += problem.GetLength();
                totalDeductiveSteps += problem.GetNumDeductiveSteps();
            }

            figureStats.averageProblemWidth = ((double)(totalWidth)) / problems.Count;
            figureStats.averageProblemLength =((double)(totalLength)) / problems.Count;
            figureStats.averageProblemDeductiveSteps = ((double)(totalDeductiveSteps)) / problems.Count;
        }

        private void GenerateStrictAverages(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            figureStats.totalStrictInterestingProblems = problems.Count;

            int totalWidth = 0;
            int totalLength = 0;
            int totalDeductiveSteps = 0;

            foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                totalWidth += problem.GetWidth();
                totalLength += problem.GetLength();
                totalDeductiveSteps += problem.GetNumDeductiveSteps();
            }

            figureStats.strictAverageProblemWidth = ((double)(totalWidth)) / problems.Count;
            figureStats.strictAverageProblemLength = ((double)(totalLength)) / problems.Count;
            figureStats.strictAverageProblemDeductiveSteps = ((double)(totalDeductiveSteps)) / problems.Count;
        }

        private void GenerateIsomorphicStatistics(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            //
            // Determine number of problems based on SOURCE isomorphism
            //
            ProblemAnalyzer.QueryFeatureVector sourceQuery = ProblemAnalyzer.QueryFeatureVector.ConstructSourceIsomorphismQueryVector();

            ProblemAnalyzer.PartitionedProblemSpace sourceBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, sourceQuery);

            sourceBasedPartitions.ConstructPartitions(problems);

            figureStats.sourcePartitionSummary = sourceBasedPartitions.GetPartitionSummary();

            //
            // Determine number of problems based on DIFFICULTY of the problems (easy, medium difficult, extreme) based on the number of deductions
            //
            // Construct the partitions:
            // 25% Easy
            // 50% Medium
            // 75% Difficult
            // 100% Extreme
            //
            ProblemAnalyzer.QueryFeatureVector difficultyQuery = ProblemAnalyzer.QueryFeatureVector.ConstructDeductiveBasedIsomorphismQueryVector(ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds());

            ProblemAnalyzer.PartitionedProblemSpace difficultyBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, difficultyQuery);

            difficultyBasedPartitions.ConstructPartitions(problems);

            figureStats.difficultyPartitionSummary = difficultyBasedPartitions.GetDifficultyPartitionSummary();

            //
            // Determine number of interesting problems based percentage of givens covered.
            //
            // Construct the partitions:
            // 0-2 Easy
            // 3-5 Medium
            // 6-10 Difficult
            // 10+ Extreme
            //
            ProblemAnalyzer.QueryFeatureVector interestingQuery = ProblemAnalyzer.QueryFeatureVector.ConstructInterestingnessIsomorphismQueryVector(ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds());

            ProblemAnalyzer.PartitionedProblemSpace interestingBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, interestingQuery);

            interestingBasedPartitions.ConstructPartitions(problems);

            figureStats.interestingPartitionSummary = interestingBasedPartitions.GetInterestingPartitionSummary();
        }

        private void GenerateStrictIsomorphicStatistics(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            //
            // Determine number of problems based on DIFFICULTY of the problems (easy, medium difficult, extreme) based on the number of deductions
            //
            // Construct the partitions:
            // 25% Easy
            // 50% Medium
            // 75% Difficult
            // 100% Extreme
            //
            ProblemAnalyzer.QueryFeatureVector difficultyQuery = ProblemAnalyzer.QueryFeatureVector.ConstructDeductiveBasedIsomorphismQueryVector(ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds());

            ProblemAnalyzer.PartitionedProblemSpace difficultyBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, difficultyQuery);

            difficultyBasedPartitions.ConstructPartitions(problems);

            figureStats.strictDifficultyPartitionSummary = difficultyBasedPartitions.GetDifficultyPartitionSummary();

            //
            // Determine number of interesting problems based percentage of givens covered.
            //
            // Construct the partitions:
            // 0-2 Easy
            // 3-5 Medium
            // 6-10 Difficult
            // 10+ Extreme
            //
            ProblemAnalyzer.QueryFeatureVector interestingQuery = ProblemAnalyzer.QueryFeatureVector.ConstructInterestingnessIsomorphismQueryVector(ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds());

            ProblemAnalyzer.PartitionedProblemSpace interestingBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, interestingQuery);

            interestingBasedPartitions.ConstructPartitions(problems);

            figureStats.strictInterestingPartitionSummary = interestingBasedPartitions.GetInterestingPartitionSummary();
        }

        private void GeneratePaperQuery(List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> sat = new List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>();
            int query = 0;
            int query2 = 0;
            int query3 = 0;
            int query4 = 0;

            foreach (ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                bool WRITE_PROBLEMS = true;
                int width = problem.GetWidth();
                int steps = problem.GetNumDeductiveSteps();
                int depth = problem.GetLength();

                if (6 <= steps && steps <= 10 && 4 <= width && width <= 8)
                {
                    query++;
                    if (WRITE_PROBLEMS)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\singleProblemQuery.txt", true))
                        {
                            file.WriteLine(problem.ConstructProblemAndSolution(graph));
                        }
                    }
                }



                //if (3 <= steps && steps <= 7 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query2++;

                //    if (WRITE_PROBLEMS)
                //    {
                //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryProblems.txt", true))
                //        {
                //            file.WriteLine(problem.ConstructProblemAndSolution(graph));
                //        }
                //    }
                //}
                //if (steps >= 10 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query4++;
                //}

                //if (steps == 6 && depth == 4 && width == 5 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query3++;
                //}
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\teacherQuery.txt", true))
            {
                file.WriteLine(query);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent2.txt", true))
            {
                file.WriteLine(query2);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent3.txt", true))
            {
                file.WriteLine(query3);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent4.txt", true))
            {
                file.WriteLine(query4);
            }
        }
    }
}
