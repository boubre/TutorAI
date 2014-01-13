using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib
{
    public class BridgeUItoBackEnd
    {
        // Returns the number of interesting problems
        public static int AnalyzeFigure(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
        {
            //
            // Precompute all coordinate-based interesting relations (problem goal nodes)
            // These become the basis for the template-based problem generation (these are the goals)
            //
            Precompute(figure, givens);

            // Handle givens that strengthen the intrinsic parts of the figure
            givens = DoGivensStrengthenFigure(figure, givens);

            //
            // Instantiate to build the hypergraph for the particular figure
            //
            ConstructHypergraph(figure, givens);

            Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> pebbler = ConstructForwardPebblingHypergraph(figure, givens);

            if (Utilities.DEBUG)
            {
                pebbler.DebugDumpClauses();
            }

            KeyValuePair<List<ProblemAnalyzer.Problem>, List<ProblemAnalyzer.Problem>> problems = GenerateTemplateProblems();

            List<ProblemAnalyzer.Problem> candidateProbs = new List<ProblemAnalyzer.Problem>();
            candidateProbs.AddRange(problems.Key);
            candidateProbs.AddRange(problems.Value);

            ProblemAnalyzer.InterestingProblemCalculator calculator = new ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens);
            //List<ProblemAnalyzer.Problem> interestingProblems = calculator.DetermineInterestingProblems(candidateProbs);

            //if (Utilities.DEBUG)
            //{
            //    System.Diagnostics.Debug.WriteLine("Interesting Problems (" + interestingProblems.Count + "):");
            //    foreach (ProblemAnalyzer.Problem interesting in interestingProblems)
            //    {
            //        System.Diagnostics.Debug.WriteLine(interesting);
            //    }
            //}

            ProblemAnalyzer.QueryFeatureVector query = new ProblemAnalyzer.QueryFeatureVector();

            ProblemAnalyzer.ProblemGroupingStructure problemSpacePartitions = new ProblemAnalyzer.ProblemGroupingStructure(graph);

            // Do not keep forward and backward problems distinct
            problemSpacePartitions.ConstructPartitions(problems.Key, query);
            problemSpacePartitions.ConstructPartitions(problems.Value, query);

            if (Utilities.DEBUG)
            {
                problemSpacePartitions.DumpPartitions(query);
            }
            return 0; //  interestingProblems.Count;
        }

        //
        // Modify the given information to account for redundancy in stated nodes
        // That is, does given information strengthen a figure node?
        //
        private static List<ConcreteAST.GroundedClause> DoGivensStrengthenFigure(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
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
        // Use threads to precompute all forward relations and strengthening 
        //
        private static Precomputer.CoordinatePrecomputer precomputer = null;

        private static void Precompute(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
        {
            precomputer = new Precomputer.CoordinatePrecomputer(figure);

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
        // Construct the main Hypergraph
        //
        private static Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;
        private static void ConstructHypergraph(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
        {
            GenericInstantiator.Instantiator.Clear(); // Resets all saved data to allow multiple problems
            GenericInstantiator.Instantiator instantiator = new GenericInstantiator.Instantiator();

            // Build the hypergraph through instantiation
            graph = instantiator.Instantiate(figure, givens);

            if (Utilities.DEBUG)
            {
                graph.DumpNonEquationClauses();
                graph.DumpEquationClauses();
            }
        }

        //
        // Create the Pebbler version of the hypergraph for analysis
        //
        private static Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> pebbler = null;
        private static ProblemAnalyzer.PathGenerator pathGenerator = null;
        private static Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> ConstructForwardPebblingHypergraph(List<ConcreteAST.GroundedClause> intrinsic, List<ConcreteAST.GroundedClause> givens)
        {
            // Create the Pebbler version of the hypergraph (all integer representation) and provide the original hypergraph for reference
            pebbler = graph.GetForwardPebblerHypergraph();
            pebbler.SetOriginalHypergraph(graph);

            // This is the shared worklist for generating paths for when new nodes are pebbled 
            //Pebbler.SharedPebbledNodeList sharedData = new Pebbler.SharedPebbledNodeList();

            // Create and / or indicate these two objects work on the shared list object
            pathGenerator = new ProblemAnalyzer.PathGenerator(graph /*, sharedData */);
//            pebbler.SetSharedList(sharedData);

            // Pebble all of the intrinsic figure nodes
            List<int> intrinsicSet = CollectGraphIndices(intrinsic);
            // Pebble all givens
            List<int> givenSet = CollectGraphIndices(givens);

            // Set the nodes to begin pebbling
//            pebbler.SetSourceNodes(srcsToPebble);

            pebbler.GenerateAllPaths(intrinsicSet, givenSet);

/*
            // Create producer and consumer threads for problem path / solution generation
            Thread pebblerProducer = new Thread(new ThreadStart(pebbler.GenerateAllPaths));
            Thread pathGeneratorConsumer = new Thread(new ThreadStart(pathGenerator.GenerateAllPaths));

            // Start the threads
            try
            {
                pebblerProducer.Start();
                // Allow production to occur;
                // if pebbling is so poor that nothing is produced, this delay prevents an infinite wait.
                Thread.Sleep(150);

                //
                // This begins the fixpoint computation based on pebbling, but also refining problem selection
                // because templated problems have already been generated
                //
                pathGeneratorConsumer.Start();

                // Join both threads with no timeout
                pebblerProducer.Join();
                pathGeneratorConsumer.Join();
            }
            catch (ThreadStateException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
* */

            //
            // End Analysis Code
            //
            if (Utilities.DEBUG)
            {

                Debug.WriteLine("Forward Vertices after pebbling:");
                for (int i = 0; i < pebbler.vertices.Length; i++)
                {
                    StringBuilder strLocal = new StringBuilder();
                    strLocal.Append(pebbler.vertices[i].id + ": pebbled(");
                    if (pebbler.vertices[i].pebble == Pebbler.PebblerColorType.NO_PEBBLE) strLocal.Append("NONE");
                    if (pebbler.vertices[i].pebble == Pebbler.PebblerColorType.RED_FORWARD) strLocal.Append("RED");
                    if (pebbler.vertices[i].pebble == Pebbler.PebblerColorType.BLUE_BACKWARD) strLocal.Append("BLUE");
                    if (pebbler.vertices[i].pebble == Pebbler.PebblerColorType.PURPLE_BOTH) strLocal.Append("PURPLE");
                    if (pebbler.vertices[i].pebble == Pebbler.PebblerColorType.BLACK_EDGE) strLocal.Append("BLACK");
                    strLocal.Append(")");
                    Debug.WriteLine(strLocal.ToString());
                }
            }

            return pebbler;
        }

        //
        // Generate all of the problems based on the precomputed values (these precomputations are the problem goals)
        //
        private static KeyValuePair<List<ProblemAnalyzer.Problem>, List<ProblemAnalyzer.Problem>> GenerateTemplateProblems()
        {
            GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator templateProblemGenerator =
                new ProblemAnalyzer.TemplateProblemGenerator(graph, pebbler, pathGenerator);

            List<ConcreteAST.GroundedClause> allClauses = new List<ConcreteAST.GroundedClause>();
            precomputer.GetPrecomputedRelations().ForEach(r => allClauses.Add(r));
            precomputer.GetStrengthenedClauses().ForEach(r => allClauses.Add(r));

            return templateProblemGenerator.Generate(allClauses);
        }

        // Acquire the index of the clause in the hypergraph based only on structure
        private static int StructuralIndex(ConcreteAST.GroundedClause g)
        {
            List<Hypergraph.HyperNode<ConcreteAST.GroundedClause, int>> vertices = graph.vertices;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data.StructurallyEquals(g)) return v;
            }

            return -1;
        }

        private static List<int> CollectGraphIndices(List<ConcreteAST.GroundedClause> clauses)
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

        //private static Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> backwardPebbler = null;
        //private static ProblemAnalyzer.PathGenerator backwardPathGenerator = null;
        //private static void GenerateBackwardTemplateProblems(List<ConcreteAST.GroundedClause> intrinsic)
        //{
        //    // Create the Pebbler version of the hypergraph (all integer representation) and provide the original hypergraph for reference
        //    backwardPebbler = graph.GetBackwardPebblerHypergraph();
        //    backwardPebbler.SetOriginalHypergraph(graph);

        //    // This is the shared worklist for generating paths for when new nodes are pebbled 
        //    Pebbler.SharedPebbledNodeList sharedData = new Pebbler.SharedPebbledNodeList();

        //    // Create and / or indicate these two objects work on the shared list object
        //    //backwardPathGenerator = new ProblemAnalyzer.PathGenerator(graph, sharedData);
        //    //backwardPebbler.SetSharedList(sharedData);

        //    // Find the deepest tempalte problem in the list (deepest means high hypergraph index value)
        //    List<ConcreteAST.GroundedClause> allClauses = new List<ConcreteAST.GroundedClause>();
        //    precomputer.GetPrecomputedRelations().ForEach(r => allClauses.Add(r));
        //    precomputer.GetStrengthenedClauses().ForEach(r => allClauses.Add(r));
        //    List<int> precomputedIndices = CollectGraphIndices(allClauses);

        //    // Construct the subset of the powerset of precomputed notions with a limit on result size
        //    // The order is largest first
        //    int MAX_PEBBLING_SIZE_SET = 4;
        //    List<List<int>> powersetIndices = Utilities.ConstructPowerSetWithNoEmpty(precomputedIndices.Count, MAX_PEBBLING_SIZE_SET);

        //    StringBuilder str = new StringBuilder();
        //    str.Append("Powerset: { ");
        //    foreach (List<int> set in powersetIndices)
        //    {
        //        str.Append(" { ");
        //        foreach (int val in set) { str.Append(val + " "); }
        //        str.Append("} ");
        //    }
        //    str.Append(" } ");

        //    Debug.WriteLine(str.ToString());

        //    //
        //    // Create pebbler graph, pebble the graph, and generate the problems
        //    //
        //    // CTA: This needs to be threaded
        //    //
        //    foreach (List<int> precomputed in powersetIndices)
        //    {
        //        // Pebble all of the intrinsic figure nodes
        //        List<int> srcsToPebble = CollectGraphIndices(intrinsic);

        //        // Reset the Pebbler graph so no nodes are pebbled
        //        backwardPebbler.ClearPebbles();

        //        // Pebble all nodes in the powerset of precomputed, forward template problems
        //        foreach (int preIndex in precomputed)
        //        {
        //            srcsToPebble.Add(precomputedIndices[preIndex]);
        //        }

        //        // Set the nodes to begin pebbling
        //        //backwardPebbler.SetSourceNodes(srcsToPebble);

        //        //backwardPebbler.GenerateAllPaths();

        //        Debug.WriteLine("Backward Vertices after pebbling:");
        //        for (int i = 0; i < backwardPebbler.vertices.Length; i++)
        //        {
        //            StringBuilder strLocal = new StringBuilder();
        //            strLocal.Append(backwardPebbler.vertices[i].id + ": pebbled(");
        //            if (backwardPebbler.vertices[i].pebble == Pebbler.PebblerColorType.NO_PEBBLE) strLocal.Append("NONE");
        //            if (backwardPebbler.vertices[i].pebble == Pebbler.PebblerColorType.RED_FORWARD) strLocal.Append("RED");
        //            if (backwardPebbler.vertices[i].pebble == Pebbler.PebblerColorType.BLUE_BACKWARD) strLocal.Append("BLUE");
        //            if (backwardPebbler.vertices[i].pebble == Pebbler.PebblerColorType.PURPLE_BOTH) strLocal.Append("PURPLE");
        //            if (backwardPebbler.vertices[i].pebble == Pebbler.PebblerColorType.BLACK_EDGE) strLocal.Append("BLACK");
        //            strLocal.Append(")  " + graph.vertices[i].data.ToString());
        //            Debug.WriteLine(strLocal.ToString());
        //        }

        //        GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator templateProblemGenerator =
        //                 new ProblemAnalyzer.TemplateProblemGenerator(graph, backwardPebbler, backwardPathGenerator);

        //        templateProblemGenerator.Generate(allClauses);

        //        return; // for testing purposes only
        //    }
        //}

        //private static ProblemAnalyzer.PathGenerator GeneratePaths(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph)
        //{
        //    ProblemAnalyzer.PathGenerator pathGenerator = null;

        //    // Create the Pebbler version of the hypergraph (all integer representation) and provide the original hypergraph for reference
        //    Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> forwardPebbler = graph.GetPebblerHypergraph();
        //    pebbler.SetOriginalHypergraph(graph);

        //    // This is the shared worklist for generating paths for when new nodes are pebbled 
        //    Pebbler.SharedPebbledNodeList sharedData = new Pebbler.SharedPebbledNodeList();

        //    // Create and / or indicate these two objects work on the shared list object
        //    pathGenerator = new ProblemAnalyzer.PathGenerator(graph, sharedData);
        //    pebbler.SetSharedList(sharedData);

        //    // For testing purposes, pebble from this start node only
        //    int start = 0;
        //    int stop = 75;
        //    stop = stop < graph.Size() ? stop : graph.Size() - 1;
        //    List<int> srcs = new List<int>();
        //    for (int i = start; i <= stop; i++)
        //    {
        //        srcs.Add(i);
        //    }

        //    // Set the nodes to begin pebbling
        //    pebbler.SetSourceNodes(srcs);

        //    pebbler.GenerateAllPaths();

        //    //// Create producer and consumer threads for problem path / solution generation
        //    //Thread pebblerProducer = new Thread(new ThreadStart(pebbler.GenerateAllPaths));
        //    //Thread pathGeneratorConsumer = new Thread(new ThreadStart(pathGenerator.GenerateAllPaths));

        //    //// Start the threads
        //    //try
        //    //{
        //    //    pebblerProducer.Start();
        //    //    // Allow production to occur;
        //    //    // if pebbling is so poor that nothing is produced, this delay prevents an infinite wait.
        //    //    Thread.Sleep(150);
        //    //    pathGeneratorConsumer.Start();

        //    //    // Join both threads with no timeout
        //    //    pebblerProducer.Join();
        //    //    pathGeneratorConsumer.Join();
        //    //}
        //    //catch (ThreadStateException e)
        //    //{
        //    //    Debug.WriteLine(e);
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Debug.WriteLine(e);
        //    //}

        //    //
        //    // End Analysis Code
        //    //
        //    Debug.WriteLine("Vertices after pebbling:");
        //    for (int i = 0; i < pebbler.vertices.Length; i++)
        //    {
        //        Debug.WriteLine(pebbler.vertices[i].id + ": pebbled(" + pebbler.vertices[i].pebbled + ")");
        //    }

        //    Debug.WriteLine("Number of Unique Paths / Problems: " + pathGenerator.GetProblems().Count);

        //    return pathGenerator;
        //}

        //private static void AnalyzePaths(ProblemAnalyzer.PathGenerator generator, Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph)
        //{
        //    // generator.PrintAllProblemsAndSolutions();

        //    List<ProblemAnalyzer.Problem> problems = generator.GetProblems();

        //    // This query defaults to goal isomorphism currently
        //    ProblemAnalyzer.QueryFeatureVector query = new ProblemAnalyzer.QueryFeatureVector();

        //    ProblemAnalyzer.ProblemGroupingStructure problemSpacePartitions = new ProblemAnalyzer.ProblemGroupingStructure(graph);

        //    problemSpacePartitions.ConstructPartitions(problems, query);

        //    problemSpacePartitions.DumpPartitions(query);
        //}
    }
}
