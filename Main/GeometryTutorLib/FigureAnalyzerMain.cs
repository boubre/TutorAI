using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib
{
    public class FigureAnalyzerMain
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
        private ProblemAnalyzer.QueryFeatureVector queryVector;
        private ProblemAnalyzer.PartitionedProblemSpace problemSpacePartitions;
        private List<ConcreteAST.GroundedClause> goals;

        public FigureAnalyzerMain(List<ConcreteAST.GroundedClause> fs, List<ConcreteAST.GroundedClause> gs, List<ConcreteAST.GroundedClause> gls)
        {
            this.figure = fs;
            this.givens = gs;
            this.goals = gls;

            // Create the precomputer object for coordinate-based pre-comutation analysis
            precomputer = new Precomputer.CoordinatePrecomputer(figure);
            instantiator = new GenericInstantiator.Instantiator();
            queryVector = new ProblemAnalyzer.QueryFeatureVector(givens.Count);
        }

        // Returns: <number of interesting problems, number of original problems generated>
        public KeyValuePair<int, int> AnalyzeFigure()
        {
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

            // Determine which, if any, of the problems are interesting (using definition that 100% of the givens are used)
            interestingCalculator = new ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens, goals, queryVector);
            List<ProblemAnalyzer.Problem> interestingProblems = interestingCalculator.DetermineInterestingProblems(candidateProbs);

            // Partition the problem-space based on the query vector defined (by us or the user)
            problemSpacePartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph);
            problemSpacePartitions.ConstructPartitions(interestingProblems, queryVector);

            // Validate that we have generated all of the original problems from the text.
            List<ProblemAnalyzer.Problem> generatedBookProblems = problemSpacePartitions.ValidateOriginalProblems(givens, goals);

            if (Utilities.DEBUG) problemSpacePartitions.DumpPartitions(queryVector);

            if (Utilities.DEBUG)
            {
                Debug.WriteLine("\nAll " + generatedBookProblems.Count + " Book-specified problems: \n");
                foreach (ProblemAnalyzer.Problem bookProb in generatedBookProblems)
                {
                    Debug.WriteLine(bookProb.ConstructProblemAndSolution(graph));
                }
            }

            return new KeyValuePair<int,int>(interestingProblems.Count, generatedBookProblems.Count);
        }

        //
        // Use threads to precompute all forward relations and strengthening 
        //
        private void Precompute()
        {
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
            List<int> intrinsicSet = CollectGraphIndices(figure);

            // Acquire the integer values of the givens (from the original 
            List<int> givenSet = CollectGraphIndices(givens);

            // Perform pebbling based on the <figure, given> pair.
            pebblerGraph.Pebble(intrinsicSet, givenSet);

            if (Utilities.PEBBLING_DEBUG)
            {
                Debug.WriteLine("Forward Vertices after pebbling:");
                for (int i = 0; i < pebblerGraph.vertices.Length; i++)
                {
                    StringBuilder strLocal = new StringBuilder();
                    strLocal.Append(pebblerGraph.vertices[i].id + ": pebbled(");
                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.NO_PEBBLE) strLocal.Append("NONE");
                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.RED_FORWARD) strLocal.Append("RED");
                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.BLUE_BACKWARD) strLocal.Append("BLUE");
                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.PURPLE_BOTH) strLocal.Append("PURPLE");
                    if (pebblerGraph.vertices[i].pebble == Pebbler.PebblerColorType.BLACK_EDGE) strLocal.Append("BLACK");
                    strLocal.Append(")");
                    Debug.WriteLine(strLocal.ToString());
                }

                pebblerGraph.DebugDumpClauses();
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
        // Acquire the index of the clause in the hypergraph based only on structure
        //
        private int StructuralIndex(ConcreteAST.GroundedClause g)
        {
            List<Hypergraph.HyperNode<ConcreteAST.GroundedClause, int>> vertices = graph.vertices;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data.StructurallyEquals(g)) return v;
            }

            return -1;
        }

        //
        // Acquires the hypergraph index value of the given nodes using structural equality
        //
        private List<int> CollectGraphIndices(List<ConcreteAST.GroundedClause> clauses)
        {
            List<int> indices = new List<int>();

            foreach (ConcreteAST.GroundedClause gc in clauses)
            {
                int index = StructuralIndex(gc);
                if (index != -1)
                {
                    indices.Add(index);
                }
                else
                {
                    Debug.WriteLine("We expect to find the given node (we did not): " + gc.ToString());
                }
            }

            return indices;
        }
    }
}
