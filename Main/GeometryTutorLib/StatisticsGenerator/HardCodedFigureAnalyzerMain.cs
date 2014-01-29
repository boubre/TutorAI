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
        private Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;
        private GenericInstantiator.Instantiator instantiator;
        private Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> pebblerGraph;
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

            // Create the precomputer object for coordinate-based pre-comutation analysis
            precomputer = new Precomputer.CoordinatePrecomputer(figure);
            instantiator = new GenericInstantiator.Instantiator();
            //queryVector = new ProblemAnalyzer.QueryFeatureVector(givens.Count);
        }

        // Returns: <number of interesting problems, number of original problems generated>
        public StatisticsGenerator.FigureStatisticsAggregator AnalyzeFigure()
        {
            StatisticsGenerator.FigureStatisticsAggregator figureStats = new StatisticsGenerator.FigureStatisticsAggregator();

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
            KeyValuePair<List<ProblemAnalyzer.Problem>, List<ProblemAnalyzer.Problem>> problems = GenerateTemplateProblems();

            // Combine the problems together into one list
            List<ProblemAnalyzer.Problem> candidateProbs = new List<ProblemAnalyzer.Problem>();
            candidateProbs.AddRange(problems.Key);
            candidateProbs.AddRange(problems.Value);

            figureStats.totalBackwardProblemsGenerated = problems.Value.Count;
            figureStats.totalProblemsGenerated = candidateProbs.Count;

            // Determine which, if any, of the problems are interesting (using definition that 100% of the givens are used)
            interestingCalculator = new ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens, goals);
            List<ProblemAnalyzer.Problem> interestingProblems = interestingCalculator.DetermineInterestingProblems(candidateProbs);
            figureStats.totalInterestingProblems = interestingProblems.Count;

            // Validate that the original book problems were generated.
            Validate(interestingProblems, figureStats);

            // Stop timing before we generate all of the statistics
            figureStats.stopwatch.Stop();

            // Construct partitions based on different queries
            GenerateStatistics(interestingProblems, figureStats);

            return figureStats;
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
                        currentGiven = new ConcreteAST.Strengthened(component, given, "Given");
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
        private KeyValuePair<List<ProblemAnalyzer.Problem>, List<ProblemAnalyzer.Problem>> GenerateTemplateProblems()
        {
            templateProblemGenerator = new ProblemAnalyzer.TemplateProblemGenerator(graph, pebblerGraph, pathGenerator);

            // Generate the problem pairs
            return templateProblemGenerator.Generate(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses(), givens);
        }

        //
        // Given, the list of generated, interesting problems, validate (for soundness) the fact that the original book problem was generated.
        // Do so by constructing a goal-based isomorphism partitioning and check that there exists a problem with the same given set.
        //
        private void Validate(List<ProblemAnalyzer.Problem> problems, FigureStatisticsAggregator figureStats)
        {
            ProblemAnalyzer.QueryFeatureVector query = ProblemAnalyzer.QueryFeatureVector.ConstructGoalIsomorphismQueryVector();

            ProblemAnalyzer.PartitionedProblemSpace goalBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, query);
            
            goalBasedPartitions.ConstructPartitions(problems);

            // Validate that we have generated all of the original problems from the text.
            List<ProblemAnalyzer.Problem> generatedBookProblems = goalBasedPartitions.ValidateOriginalProblems(givens, goals);
            figureStats.totalBookProblemsGenerated = generatedBookProblems.Count;

            if (Utilities.PROBLEM_GEN_DEBUG)
            {
                goalBasedPartitions.DumpPartitions();
            }
            if (Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("\nAll " + generatedBookProblems.Count + " Book-specified problems: \n");
                foreach (ProblemAnalyzer.Problem bookProb in generatedBookProblems)
                {
                    Debug.WriteLine(bookProb.ConstructProblemAndSolution(graph));
                }
            }

            figureStats.goalPartitionSummary = goalBasedPartitions.GetGoalBasedPartitionSummary();
        }

        //
        // We may analyze the interesting problems constructing various partitions and queries
        //
        private void GenerateStatistics(List<ProblemAnalyzer.Problem> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            GenerateAverages(problems, figureStats);
            GenerateIsomorphicStatistics(problems, figureStats);
        }

        private void GenerateAverages(List<ProblemAnalyzer.Problem> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            int totalWidth = 0;
            int totalLength = 0;
            int totalDeductiveSteps = 0;

            foreach (ProblemAnalyzer.Problem problem in problems)
            {
                totalWidth += problem.GetWidth();
                totalLength += problem.GetLength();
                totalDeductiveSteps += problem.GetNumDeductiveSteps();
            }

            figureStats.averageProblemWidth = ((double)(totalWidth)) / problems.Count;
            figureStats.averageProblemLength =((double)(totalLength)) / problems.Count;
            figureStats.averageProblemDeductiveSteps = ((double)(totalDeductiveSteps)) / problems.Count;
        }

        private void GenerateIsomorphicStatistics(List<ProblemAnalyzer.Problem> problems, StatisticsGenerator.FigureStatisticsAggregator figureStats)
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
            // 0-2 Easy
            // 3-5 Medium
            // 6-10 Difficult
            // 10+ Extreme
            //
            ProblemAnalyzer.QueryFeatureVector difficultyQuery = ProblemAnalyzer.QueryFeatureVector.ConstructDeductiveBasedIsomorphismQueryVector(ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds());

            ProblemAnalyzer.PartitionedProblemSpace difficultyBasedPartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, difficultyQuery);

            difficultyBasedPartitions.ConstructPartitions(problems);

            figureStats.difficultyPartitionSummary = difficultyBasedPartitions.GetDeductivePartitionSummary();
        }
    }
}
