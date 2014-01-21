﻿using System;
using System.Linq;
using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Hypergraph;
using System.Diagnostics;

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
        // Generate all forward and backward problems based on the template nodes in the input list
        //
        public KeyValuePair<List<Problem>, List<Problem>> Generate(List<Descriptor> descriptors, List<Strengthened> strengthened, List<GroundedClause> givens)
        {
            // Combine the precomputed descriptors and strengthened clauses together into one list
            List<GroundedClause> descriptorsAndStrengthened = new List<GroundedClause>();
            descriptors.ForEach(r => descriptorsAndStrengthened.Add(r));
            strengthened.ForEach(r => descriptorsAndStrengthened.Add(r));

            List<Problem> forwardList = GenerateProblems(descriptorsAndStrengthened, pebblerGraph.forwardPebbledEdges, true);
            List<Problem> backwardList = GenerateProblems(givens, pebblerGraph.backwardPebbledEdges, false);

            return new KeyValuePair<List<Problem>, List<Problem>>(forwardList, backwardList);
        }

        private List<Problem> GenerateProblems(List<GroundedClause> goalClauses, Pebbler.HyperEdgeMultiMap edgeDatabase, bool forward)
        {
            // Find all the node indices and sort them in increasing order
            List<int> clauseIndices = new List<int>();
            foreach (GroundedClause clause in goalClauses)
            {
                // We need to restrict problems generated based on goal nodes; don't want an obvious notion as a goal
                if (clause.IsAbleToBeAGoalNode())
                {
                    // Find the integer clause ID representation in the standard hypergraph
                    int nodeIndex = Utilities.StructuralIndex(graph, clause);

                    if (nodeIndex == -1)
                    {
                        if (Utilities.PROBLEM_GEN_DEBUG)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: Did not find precomputed node in the hypergraph: " + clause.ToString());
                        }
                    }
                    else
                    {
                        clauseIndices.Add(nodeIndex);
                    }
                }
            }

            // Sort in ascending order
            clauseIndices.Sort();

            // The resultant structure of problems
            ProblemHashMap problems = new ProblemHashMap(edgeDatabase, graph.vertices.Count);

            // Generate all the problems based on the node indices
            foreach (int goalNode in clauseIndices)
            {
                if (Utilities.PROBLEM_GEN_DEBUG)
                {
                    System.Diagnostics.Debug.WriteLine("Template node; will generate problems (" + goalNode + "): " + graph.vertices[goalNode].data.ToString());
                }

                if (forward && pebblerGraph.IsNodePebbledForward(goalNode))
                {
                    if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("Forward");
                    pathGenerator.GenerateProblemsUsingBackwardPathToLeaves(problems, edgeDatabase, goalNode);
                }
                // BACKWARD
                else if (!forward && pebblerGraph.IsNodePebbledBackward(goalNode))
                {
                    if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("Backward");
                    pathGenerator.GenerateProblemsUsingBackwardPathToLeaves(problems, edgeDatabase, goalNode);
                }
            }

            return FilterForMinimalAndRedundantProblems(problems.GetAll());
        }

        //private List<Problem> GenerateBackwardProblems(List<GroundedClause> givens)
        //{
        //    ProblemHashMap backwardProblems = new ProblemHashMap(pebblerGraph.backwardPebbledEdges, graph.vertices.Count);

        //    // Find all the node indices and sort them in increasing order
        //    List<int> clauseIndices = new List<int>();
        //    foreach (GroundedClause clause in goalClauses)
        //    {
        //        // We need to restrict problems generated based on goal nodes; don't want an obvious notion as a goal
        //        if (clause.IsAbleToBeAGoalNode())
        //        {
        //            // Find the integer clause ID representation in the standard hypergraph
        //            int nodeIndex = Utilities.StructuralIndex(graph, clause);

        //            if (nodeIndex == -1)
        //            {
        //                if (Utilities.PROBLEM_GEN_DEBUG)
        //                {
        //                    System.Diagnostics.Debug.WriteLine("ERROR: Did not find precomputed node in the hypergraph: " + clause.ToString());
        //                }
        //            }
        //            else
        //            {
        //                clauseIndices.Add(nodeIndex);
        //            }
        //        }
        //    }

        //    // Sort in ascending order
        //    clauseIndices.Sort();

        //    // Generate all the problems based on the node indices
        //    foreach (int goalNode in clauseIndices)
        //    {
        //        if (Utilities.PROBLEM_GEN_DEBUG)
        //        {
        //            System.Diagnostics.Debug.WriteLine("Forward template node; will generate problems (" + goalNode + "): " + graph.vertices[goalNode].data.ToString());
        //        }

        //        //
        //        // Is this is a forward pebbled node? If so, generate the forward set of problems to this node.
        //        //
        //        if (pebblerGraph.IsNodePebbledForward(goalNode))
        //        {
        //            if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("Forward");
        //            pathGenerator.GenerateForwardProblemsUsingBackwardPathToLeaves(forwardProblems, pebblerGraph, goalNode);
        //        }
        //    }

        //    return FilterForMinimalAndRedundantProblems(forwardProblems.GetAll());
        //}

        ////
        //// Generate all problems using the input template clause as a starting point.
        ////
        //public void GenerateProblems(ProblemHashMap forwardProblems, ProblemHashMap backwardProblems, int goalNode)
        //{
        //    if (Utilities.PROBLEM_GEN_DEBUG)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Found template node; will generate problems (" + goalNode + "): " + graph.vertices[goalNode].data.ToString());
        //    }

        //    //
        //    // Is this is a forward pebbled node? If so, generate the forward set of problems to this node.
        //    //
        //    if (pebblerGraph.IsNodePebbledForward(goalNode))
        //    {
        //        if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("Forward");
        //        pathGenerator.GenerateForwardProblemsUsingBackwardPathToLeaves(forwardProblems, pebblerGraph, goalNode);
        //    }

        //    //
        //    // Is this is a backward pebbled node? If so, generate the backward set of problems to this node.
        //    //
        //    if (pebblerGraph.IsNodePebbledBackward(goalNode))
        //    {
        //        //if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("Backward");
        //        //backwardProblems = pathGenerator.GenerateBackwardProblemsUsingBackwardPathToNonLeaves(pebblerGraph, nodeIndex);
        //    }
        //}

        public List<Problem> FilterForMinimalAndRedundantProblems(List<Problem> problems)
        {
            List<Problem> filtered = new List<Problem>();

            // It is possible for no problems to be generated
            if (!problems.Any()) return problems;

            // For each problem, break the givens into actual vs. suppressed given information
            problems.ForEach(problem => problem.DetermineSuppressedGivens(graph));

            //
            // Filter the problems based on same set of source nodes and goal node
            //   All of these problems have exactly the same goal node.
            //   Now, if we have multiple problems with the exact same (non-suppressed) source nodes, choose the one with shortest path.
            //
            bool[] marked = new bool[problems.Count];
            for (int p1 = 0; p1 < problems.Count - 1; p1++)
            {
                // We may have marked this earlier
                if (!marked[p1])
                {
                    // Save the minimal problem
                    Problem minimalProblem = problems[p1];
                    for (int p2 = p1 + 1; p2 < problems.Count; p2++)
                    {
                        // If we have not yet compared to a problem
                        if (!marked[p2])
                        {
                            // Both problems need the same goal node
                            if (minimalProblem.goal == problems[p2].goal)
                            {
                                // Check if the givens from the minimal problem and this candidate problem equate exactly
                                if (Utilities.EqualSets<int>(minimalProblem.givens, problems[p2].givens))
                                {
                                    // We have now analyzed this problem
                                    marked[p2] = true;

                                    // Choose the shorter problem (fewer edges wins)
                                    if (problems[p2].edges.Count < minimalProblem.edges.Count)
                                    {
                                        if (Utilities.PROBLEM_GEN_DEBUG) Debug.WriteLine("Outer Filtering: " + problems[p2].ToString() + " for " + minimalProblem.ToString());
                                        minimalProblem = problems[p2];
                                    }
                                    else
                                    {
                                        if (Utilities.PROBLEM_GEN_DEBUG) Debug.WriteLine("Outer Filtering: " + minimalProblem.ToString() + " for " + problems[p2].ToString());
                                    }
                                }
                            }
                        }
                    }
                    // Add the minimal problem to the list to be returned
                    filtered.Add(minimalProblem);
                }
            }

            // Pick up last problem in the list
            if (!marked[problems.Count - 1]) filtered.Add(problems[problems.Count - 1]);

            if (Utilities.PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("Generated Problems: " + problems.Count);
                Debug.WriteLine("Filtered Problems: " + (problems.Count - filtered.Count));
                Debug.WriteLine("Problems Remaining: " + filtered.Count);
            }

            if (problems.Count < filtered.Count)
            {
                Debug.WriteLine("Filtered list is larger than original list!");
            }

            return filtered;
        }
    }
}