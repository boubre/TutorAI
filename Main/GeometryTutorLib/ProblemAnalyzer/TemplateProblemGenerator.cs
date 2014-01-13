using System;
using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Hypergraph;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class TemplateProblemGenerator
    {
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;
        Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> pebblerGraph;
        ProblemAnalyzer.PathGenerator pathGenerator;

        public TemplateProblemGenerator(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> g,
                                               Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause> pebblerG,
                                               ProblemAnalyzer.PathGenerator generator)
        {
            graph = g;
            pebblerGraph = pebblerG;
            pathGenerator = generator;
        }

        //
        // Acquire the index of the clause in the hypergraph based only on structure
        //
        private int StructuralIndex(GroundedClause g)
        {
            //
            // Handle general case
            //
            List<HyperNode<GroundedClause, int>> vertices = graph.vertices;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data.StructurallyEquals(g)) return v;

                if (vertices[v].data is Strengthened)
                {
                    if ((vertices[v].data as Strengthened).strengthened.StructurallyEquals(g)) return v;
                }
            }

            //
            // Handle strengthening by seeing if the clause is found without a 'strengthening' component
            //
            Strengthened streng = g as Strengthened;
            if (streng != null)
            {
                int index = StructuralIndex(streng.strengthened);
                if (index != -1) return index;
            }

            return -1;
        }

        //
        // Generate all forward and backward problems based on the template nodes in the input list
        //
        public KeyValuePair<List<Problem>, List<Problem>> Generate(List<GroundedClause> descriptorsAndStrengthened)
        {
            List<Problem> forwardProblems = new List<Problem>();
            List<Problem> backwardProblems = new List<Problem>();

            // Generate the set of forward and backward problems for each template clause
            foreach (GroundedClause clause in descriptorsAndStrengthened)
            {
                // We need to restrict problems generated based on goal nodes; don't want an obvious notion as a goal
                if (clause.IsAbleToBeAGoalNode())
                {
                    KeyValuePair<List<Problem>, List<Problem>> forwardBackwardProblemPair = GenerateProblems(clause);
                    forwardProblems.AddRange(forwardBackwardProblemPair.Key);
                    backwardProblems.AddRange(forwardBackwardProblemPair.Value);
                }
            }

            return new KeyValuePair<List<Problem>, List<Problem>>(forwardProblems, backwardProblems);
        }

        //
        // Generate all problems using the input template clause as a starting point.
        //
        public KeyValuePair<List<Problem>, List<Problem>> GenerateProblems(GroundedClause clause)
        {
            List<Problem> forwardProblems = new List<Problem>();
            List<Problem> backwardProblems = new List<Problem>();

            // Find the integer clause ID representation in the standard hypergraph
            int nodeIndex = StructuralIndex(clause);

            if (nodeIndex == -1)
            {
                if (Utilities.DEBUG)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Did not find precomputed node in the hypergraph: " + clause.ToString());
                }
                return new KeyValuePair<List<Problem>, List<Problem>>(forwardProblems, backwardProblems);
            }

            if (Utilities.DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Found template node; will generate problems (" + nodeIndex + "): " + clause.ToString());
            }

            //
            // Is this is a forward pebbled node? If so, generate the forward set of problems to this node.
            //
            if (pebblerGraph.IsNodePebbledForward(nodeIndex))
            {
                if (Utilities.DEBUG) System.Diagnostics.Debug.WriteLine("Forward");
                forwardProblems = pathGenerator.GenerateForwardProblemsUsingBackwardPathToLeaves(pebblerGraph, nodeIndex);  
            }

            //
            // Is this is a backward pebbled node? If so, generate the backward set of problems to this node.
            //
            if (pebblerGraph.IsNodePebbledBackward(nodeIndex))
            {
                if (Utilities.DEBUG) System.Diagnostics.Debug.WriteLine("Backward");
                backwardProblems = pathGenerator.GenerateBackwardProblemsUsingBackwardPathToNonLeaves(pebblerGraph, nodeIndex);
            }

            return new KeyValuePair<List<Problem>, List<Problem>>(forwardProblems, backwardProblems);
        }
    }
}