﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class PathGenerator
    {
        // The original graph in order to have information about node types; right now just axiomatic
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;
        private readonly int GRAPH_DIAMETER;
        public PathGenerator(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> hypergraph)
        {
            this.graph = hypergraph;
            GRAPH_DIAMETER = hypergraph.vertices.Count;
        }

        //
        // Given a start node, construct a path using reachability analysis to construct a path from start template node back to leaves.
        // Forward problems are generated by following forward edges in reverse
        //
        //                                      A multimap of all pebbled edges (from pebbling)
        public void GenerateProblemsUsingBackwardPathToLeaves(ProblemHashMap memoizedProblems, HyperEdgeMultiMap edgeDatabase, int goalNode)
        {
            if (goalNode == 115)
            {
                //Debug.WriteLine("NO-OP");
            }

            // Although this returns the set of problems, we can still acquire them through the ProblemHashMap
            GenerateAllMaximalProblemsFrom(memoizedProblems, edgeDatabase, goalNode);
        }
        
        //
        // Generate ALL maximal problems from the given start node (note, each incoming basic edge results in a maximal problem).
        // This gives soundness of generating problems from given nodes (current def of interesting problems), but lacks
        // completeness by not generating all of the subproblems along the way
        //

        //
        // From this node, generate all problems backward.
        // Do so by generating simple problems first (in a depth-first manner)
        //
        private List<Problem> GenerateAllMaximalProblemsFrom(ProblemHashMap memoizedProblems, HyperEdgeMultiMap edgeDatabase, int node)
        {
            if (node == 131)
            {
                //Debug.WriteLine("NO-OP");
            }

            // If we have already generated problems, no need to regenerate
            if (memoizedProblems.HasNodeBeenGenerated(node)) return memoizedProblems.Get(node);

            // For all edges, create base problems and add them all to the database
            List<PebblerHyperEdge> edges = edgeDatabase.GetBasedOnGoal(node);

            // If there are no edges, this is a 'root' node (has no predecessors)
            if (edges == null || !edges.Any())
            {
                // We've successfully generated all the problems for this node: zero problems
                memoizedProblems.SetGenerated(node);
                return new List<Problem>();
            }

            //
            // Create all the base problems consisting of only one edge and add to the database
            List<Problem> baseProblems = new List<Problem>();
            foreach (PebblerHyperEdge edge in edges)
            {
                baseProblems.Add(new Problem(edge));
            }
            // memoizedProblems.PutUnchecked(baseProblems);

            // For all of the base problems, generate backward
            foreach (Problem baseProblem in baseProblems)
            {
                GenerateMaximalProblemFromSingleEdge(memoizedProblems, edgeDatabase, baseProblem);
            }

            // We've successfully generated all the problems for this node
            memoizedProblems.SetGenerated(node);

            // Return all the generated problems
            return memoizedProblems.Get(node);
        }

        //
        // For all given nodes in this problem, generate the single maximal problem
        //
        // This is done is a convluted way to speed up execution since we need to combine in the size of the powerset of cardinality of givens
        //
        private void GenerateMaximalProblemFromSingleEdge(ProblemHashMap memoizedProblems, HyperEdgeMultiMap edgeDatabase, Problem baseProblem)
        {
            // Create all simple problems by pursuing only a single path backward in the givens: append
            // all problems from a given to the base problem
            //
            //           |
            //           v  
            //         given    __
            //           |        |
            //           v        |  baseProblem
            //          goal    __|
            //
            // Acquire all of the problems for the singleton sets of the powerset for all of the givens of this problem
            List<Problem>[] singletonMapToNewProblems = new List<Problem>[baseProblem.givens.Count];
            List<Problem>[] singletonMapToOriginalProblems = new List<Problem>[baseProblem.givens.Count];
            bool generatedNewProblems = false;
            for (int g = 0; g < baseProblem.givens.Count; g++)
            {
                // Acquire the original problems and save them
                singletonMapToOriginalProblems[g] = GenerateAllMaximalProblemsFrom(memoizedProblems, edgeDatabase, baseProblem.givens[g]);
                if (singletonMapToOriginalProblems[g] == null) singletonMapToOriginalProblems[g] = new List<Problem>();

                // Using the original problems, append them to the base problem
                singletonMapToNewProblems[g] = new List<Problem>();

                // Append all of these given problems to the base problem
                foreach (Problem problem in singletonMapToOriginalProblems[g])
                {
                    Problem baseProblemCopy = new Problem(baseProblem);
                    baseProblemCopy.Append(graph, edgeDatabase, problem);
                    //memoizedProblems.Put(baseProblemCopy);
                    singletonMapToNewProblems[g].Add(baseProblemCopy);
                    generatedNewProblems = true;
                }
            }

            if (baseProblem.goal == 98)
            {
                Debug.WriteLine("98NO-OP");
            }

            // If we did not perform any appending, we have reached a maximal situation
            // Add the maximal problem to the database
            if (!generatedNewProblems)
            {
                // Determine suppression now to mitigate number of problems added
                baseProblem.DetermineSuppressedGivens(graph);
                memoizedProblems.Put(baseProblem);
                return;
            }

            //
            // Stitch together all of the possible combinations of maximal problems
            //
            //    |  |        |
            //    |  |        |
            //    v  v        v
            //   g_1 g_2 ... g_n   __
            //           |           |  baseProblem
            //           v           |
            //         target      __|
            //
            //
            // We are looking for the maximal set of problems; therefore, we don't need all combinations.
            // What we do require is the combining of all generated problems with the base problem.
            //
            // Find all the sets of populated new problems
            List<int> populatedIndices = new List<int>();
            for (int index = 0; index < singletonMapToNewProblems.Length; index++)
            {
                if (singletonMapToNewProblems[index].Any()) populatedIndices.Add(index);
            }

            List<Problem> maximalProblems = new List<Problem>(singletonMapToNewProblems[populatedIndices[0]]);
            populatedIndices.RemoveAt(0);
            foreach (int index in populatedIndices)
            {
                int count = 0;
                List<Problem> tmpMaximalProbs = new List<Problem>();
                foreach (Problem singleton in singletonMapToOriginalProblems[index])
                {
                    foreach (Problem problem in maximalProblems)
                    {
                        //if (baseProblem.goal == 98)
                        //{
                        //    Debug.WriteLine(count++);
                        //}

                        Problem problemCopy = new Problem(problem);
                        // It is possible for a problem to have been created which deduced further information with an additional edge;
                        // that is, a given node was pushed into the path of the problem. Hence, no need to append in this situation
                        if (problem.givens.Contains(singleton.goal))
                        {
                            problemCopy.Append(graph, edgeDatabase, singleton);
                        }
                        tmpMaximalProbs.Add(problemCopy);
                    }
                }
                maximalProblems = tmpMaximalProbs;
            }

            // Add all the maximal problems to the database
            foreach (Problem problem in maximalProblems)
            {
                // Determine suppression now to mitigate number of problems added
                problem.DetermineSuppressedGivens(graph);
                memoizedProblems.Put(problem);
            }

        }

        //
        // COMPELTE POWERSET implementation
        //
        // Time consuming, but complete.
        //

        //
        // From this node, generate all problems backward.
        // Do so by generating simple problems first (in a depth-first manner)
        //
        private List<Problem> GenerateAllProblemsFrom(ProblemHashMap memoizedProblems, HyperEdgeMultiMap edgeDatabase, int node)
        {
            if (node == 162)
            {
                //Debug.WriteLine("NO-OP");
            }

            // If we have already generated problems, no need to regenerate
            if (memoizedProblems.HasNodeBeenGenerated(node)) return memoizedProblems.Get(node);

            // For all edges, create base problems and add them all to the database
            List<PebblerHyperEdge> forwardEdges = edgeDatabase.GetBasedOnGoal(node);

            // If there are no edges, this is a 'root' node (has no predecessors)
            if (forwardEdges == null || !forwardEdges.Any())
            {
                // We've successfully generated all the problems for this node: zero problems
                memoizedProblems.SetGenerated(node);
                return new List<Problem>();
            }

            //
            // Create all the base problems consisting of only one edge and add to the database
            List<Problem> baseProblems = new List<Problem>();
            foreach (PebblerHyperEdge edge in forwardEdges)
            {
                baseProblems.Add(new Problem(edge));
            }
            memoizedProblems.PutUnchecked(baseProblems);

            // For all of the base problems, generate backward
            foreach (Problem baseProblem in baseProblems)
            {
                GenerateAllProblemsFromSingleEdge(memoizedProblems, edgeDatabase, baseProblem);
            }

            // We've successfully generated all the problems for this node
            memoizedProblems.SetGenerated(node);

            // Return all the generated problems
            return memoizedProblems.Get(node);
        }

        //
        // For all given nodes in this problem, generate all problems
        //
        // This is done is a convluted way to speed up execution since we need to combine in the size of the powerset of cardinality of givens
        //
        private void GenerateAllProblemsFromSingleEdge(ProblemHashMap memoizedProblems, HyperEdgeMultiMap edgeDatabase, Problem baseProblem)
        {
            // Create all simple problems by pursuing only a single path backward in the givens: append
            // all problems from a given to the base problem
            //
            //           |
            //           v  
            //         given    __
            //           |        |
            //           v        |  baseProblem
            //          goal    __|
            //
            // Acquire all of the problems for the singleton sets of the powerset for all of the givens of this problem
            List<Problem>[] singletonMapToNewProblems = new List<Problem>[baseProblem.givens.Count];
            List<Problem>[] singletonMapToOriginalProblems = new List<Problem>[baseProblem.givens.Count];
            for (int g = 0; g < baseProblem.givens.Count; g++)
            {
                // Acquire the original problems and save them
                singletonMapToOriginalProblems[g] = GenerateAllProblemsFrom(memoizedProblems, edgeDatabase, baseProblem.givens[g]);
                if (singletonMapToOriginalProblems[g] == null) singletonMapToOriginalProblems[g] = new List<Problem>();

                // Using the original problems, append them to the base problem
                singletonMapToNewProblems[g] = new List<Problem>();

                // Append all of these given problems to the base problem
                foreach (Problem problem in singletonMapToOriginalProblems[g])
                {
                    Problem baseProblemCopy = new Problem(baseProblem);
                    baseProblemCopy.Append(graph, edgeDatabase, problem);
                    memoizedProblems.Put(baseProblemCopy);
                    singletonMapToNewProblems[g].Add(baseProblemCopy);
                }
            }

            // If there was only 1 given, we need to perform a powerset combining
            if (baseProblem.givens.Count == 1) return;

            //
            // Stitch together all of the possible combinations of problems that will result;
            // note this is a POWERSET construction of all possible problems; hence, note exponential explosion in number of problems
            //
            //    |   |
            //    |   |
            //    v   v
            //   g_1 g_2 ... g_n   __
            //           |           |  baseProblem
            //           v           |
            //         target      __|
            //
            //    |           |
            //    |           |
            //    v           v
            //   g_1 g_2 ... g_n   __
            //           |           |  baseProblem
            //           v           |
            //         target      __|
            //
            //    |  |        |
            //    |  |        |
            //    v  v        v
            //   g_1 g_2 ... g_n   __
            //           |           |  baseProblem
            //           v           |
            //         target      __|
            //
            // Acquire an encoded list of subsets: the powerset for the number of givens in this problem
            List<string> powerset = Utilities.ConstructPowerSetStringsWithNoEmpty(baseProblem.givens.Count, baseProblem.givens.Count); // <-- We avoid generating singleton sets

            // Maps a subset string representation (from the powerset) to the list of problems that it results in.
            Dictionary<string, List<Problem>> powersetMapToProblems = new Dictionary<string, List<Problem>>(powerset.Count);

            //
            // For every combination of possible problems (a subset of the powerset), we combine problems
            //
            // Note, the Utilities generated the powerset in the specific sorted ordering of lower cardinality sets first (also values in subset are increasing)
            // So a typical subset string is given by 012 or 023
            foreach (string subset in powerset)
            {
                // Break the subset string into its constituent elements: <Arbitrary subset, singleton>
                KeyValuePair<string, int> splitSubset = Utilities.SplitStringIntoKnownToProcess(subset);

                // Subset Problems; given the subset {012}: {01}
                List<Problem> subsetProblems;
                if (!powersetMapToProblems.TryGetValue(splitSubset.Key, out subsetProblems))
                {
                    // Use the singleton list; ASCII conversion of char to int
                    subsetProblems = singletonMapToNewProblems[Convert.ToInt32(splitSubset.Key)];
                }

                // Singleton tails; given the subset {012}: {2}
                List<Problem> singletonProblems = singletonMapToOriginalProblems[splitSubset.Value];

                // Combine the precomputed powerset subset (.Key) guiding the appending of the singleton (.Value) problems 
                List<Problem> combinedSubsetProblems = new List<Problem>();
                foreach (Problem subsetProblem in subsetProblems)
                {
                    foreach (Problem singletonProb in singletonProblems)
                    {
                        // It is possible for a problem to have been created which deduced further information with an additional edge;
                        // that is, a given node was pushed into the path of the problem. Hence, no need to append in this situation
                        if (subsetProblem.givens.Contains(singletonProb.goal))
                        {
                            // Combine the two problems and record
                            Problem subsetProblemCopy = new Problem(subsetProblem);
                            subsetProblemCopy.Append(graph, edgeDatabase, singletonProb);
                            memoizedProblems.Put(subsetProblemCopy);
                            combinedSubsetProblems.Add(subsetProblemCopy);
                        }
                    }
                }

                // We have calculated the problems for this subset, keep track of them
                powersetMapToProblems.Add(subset, combinedSubsetProblems);
            }
        }












        //
        // Given a start node, construct a path using reachability analysis to construct a path from start template node back to leaves.
        //
        //                                      A multimap of all pebbled edges (from pebbling)
        //public List<Problem> GenerateBackwardProblemsUsingBackwardPathToNonLeaves(PebblerHypergraph<ConcreteAST.GroundedClause> pebbler, int startNode)
        //{
        //    return TraverseBackwardPathToNonLeaves(pebbler, startNode, new Problem(), 0);
        //}
        ////
        //// Main backward reachability code
        ////
        //public List<Problem> TraverseBackwardPathToNonLeaves(PebblerHypergraph<ConcreteAST.GroundedClause> pebbler, int startNode, Problem problem, int numNodesVisited)
        //{
        //    List<Problem> newProblems = new List<Problem>();

        //    // Stop analysis if we have visited all nodes in the graph.
        //    if (numNodesVisited > GRAPH_DIAMETER - 1) return newProblems;

        //    // Acquire all possible edges which lead to the start node
        //    List<PebblerHyperEdge> backwardEdges = pebbler.backwardPebbledEdges.GetBasedOnGoal(startNode);

        //    //
        //    // Is this a direct non-leaf node with no more backward edges?
        //    //
        //    if (backwardEdges == null) return newProblems;
        //    if (!backwardEdges.Any()) return newProblems;

        //    // Spin (recursively) until a fixpoint where we find all non-leaf nodes
        //    foreach (PebblerHyperEdge edge in backwardEdges)
        //    {
        //        // The outgoing edge needs to be a backward edge
        //        if (edge.IsEdgePebbledBackward() && !edge.sourceNodes.Contains(problem.goal) && !problem.edges.Contains(edge) && !problem.ContainsGoalEdge(edge.targetNode))
        //        {
        //            // Each distinct edge represents a new Problem (path in the graph); but, it is not an interesting problem
        //            // Problem newEdgeProblem = new Problem(edge.sourceNodes, edge.targetNode);
        //            // An interesting problem, by definition has path length > 0
        //            // newProblems.Add(newEdgeProblem);
        //            Problem copyProblem = new Problem(problem);
        //            //copyProblem.CombineWithEdge(edge.sourceNodes, edge.targetNode);
        //            //copyProblem.AddEdge(edge);

        //            //
        //            // Avoid any problem with a cycle
        //            //
        //            if (!copyProblem.ContainsCycle())
        //            //{
        //            //    Debug.WriteLine(copyProblem.EdgeAndSCCDump());

        //            //    throw new Exception("Problem has a cycle: " + copyProblem);
        //            //}
        //            //else
        //            {
        //                //
        //                // We have a new problem; recur on it
        //                //
        //                newProblems.Add(copyProblem);

        //                //
        //                // Check if we have all red or purple nodes from this hyperedge 
        //                // These are considered 'leaf' nodes in this backward analysis
        //                //
        //                bool LeafSet = true;
        //                foreach (int src in edge.sourceNodes)
        //                {
        //                    if (pebbler.vertices[src].pebble != PebblerColorType.RED_FORWARD)
        //                    {
        //                        LeafSet = false;
        //                        break;
        //                    }
        //                }

        //                // If we have not hit the 'leaves', recur
        //                if (!LeafSet)
        //                {
        //                    foreach (int src in edge.sourceNodes)
        //                    {
        //                        newProblems.AddRange(TraverseBackwardPathToNonLeaves(pebbler, src, copyProblem, numNodesVisited + 1));
        //                    }
        //                }
        //                //bool intrinsicSet = true;
        //                //foreach (int src in edge.sourceNodes)
        //                //{
        //                //    if (!graph.GetNode(src).IsIntrinsic())
        //                //    {
        //                //        intrinsicSet = false;
        //                //        break;
        //                //    }
        //                //}

        //                //// If we have not hit the leaves, recur
        //                //if (!intrinsicSet)
        //                //{
        //                //    foreach (int src in edge.sourceNodes)
        //                //    {
        //                //        newProblems.AddRange(TraverseBackwardPathToLeaves(pebbler, src, combined));
        //                //    }
        //                //}
        //            }
        //        }
        //    }

        //    return newProblems;
        //}

        ////
        //// Given all the previously generated problems, combine ALL of those problems with this new problem to create
        //// another list of problems appending 
        ////
        //private bool MemoizedGeneration(ProblemHashMap allMemoizedProblems, HyperEdgeMultiMap forwardEdges, Problem currentProblem, int startNode)
        //{
        //    List<Problem> preGenProblems = allMemoizedProblems.Get(startNode);

        //    // If this node has not been explored, no previously generated problems exist; memoizing was not used
        //    if (preGenProblems == null) return false;

        //    // Combine all of the old problems with the new problem
        //    foreach (Problem oldProblem in preGenProblems)
        //    {
        //        // Make a copy of the incoming original problem
        //        Problem copyOfCurrentProblem = new Problem(currentProblem);

        //        // Append the old problem to the current new problem
        //        copyOfCurrentProblem.Append(graph, forwardEdges, oldProblem);

        //        // Add to the overall set of deduced problems
        //        allMemoizedProblems.Put(copyOfCurrentProblem);
        //    }

        //    // We memoized successfully
        //    return true;
        //}

        ////
        //// Acquire the next problem if we consider the number of source nodes.
        ////
        //private const int MAX_SOURCE_NODES = 6;
        //private Problem GetNextProblem(List<Problem> worklist)
        //{
        //    if (!worklist.Any()) return null;

        //    // Seek the next viable problem
        //    int w = 0;
        //    for ( ; w < worklist.Count; w++)
        //    {
        //        if(worklist[w].givens.Count <= MAX_SOURCE_NODES &&            worklist[w].goal <= 70)
        //        {
        //            break;
        //        }
        //    }

        //    // If there are no valid problems
        //    if (w == worklist.Count)
        //    {
        //        // Remove all problems from the worklist returning an invalid next problem
        //        worklist.Clear();
        //        return null;
        //    }

        //    // Take that one valid problem and remove all elements from the list up to that problem (including that problem)
        //    Problem next = worklist[w];
        //    if (w == 0) worklist.RemoveAt(0);
        //    else if (w == worklist.Count) worklist.Clear();  // These statements are overkill I am sure.
        //    else worklist.RemoveRange(0, w+1);

        //    return next;
        //}

        ////
        //// If U |- n is a problem:
        ////   let v \in U such that V |- v is a problem then
        ////   generate a new problem: (U \ v) \bigcup V |- n provided: (U \ v) \bigcup V != { n } AND 
        ////
        //private int probCount = 1;
        //public void GeneratePaths(Problem firstNewProblem)
        //{
        //    List<Problem> worklist = new List<Problem>();
        //    worklist.Add(firstNewProblem);

        //    while (worklist.Any())
        //    {
        //        // Acquire a new element to work on
        //        Problem thisNewProblem = GetNextProblem(worklist);
        //        if (thisNewProblem == null) return;

        //        Debug.WriteLine("Considering (" + (probCount++) + "): " + thisNewProblem);

        //        // Acquire the old problems in which we can combine with the problem under consideration
        //        List<Problem> oldApplicableProblems = pathHashMap.Get(thisNewProblem.goal);

        //        if (oldApplicableProblems != null)
        //        {
        //            foreach (Problem oldProblem in oldApplicableProblems)
        //            {
        //                // Can we combine? That is, is the new goal in the old source; even more specifically,
        //                // avoid a cycle by checking that the old goal is not in the thisNew source
        //                if (!thisNewProblem.InSource(oldProblem.goal) && !thisNewProblem.InPath(oldProblem.goal))
        //                {
        //                    // To avoid a cycle, avoid deducing this same node
        //                    if (!oldProblem.HasGoal(thisNewProblem.goal))
        //                    {
        //                        Utilities.AddUnique(worklist, oldProblem.CombineAndCreateNewProblem(thisNewProblem));
        //                    }
        //                }
        //            }
        //        }

        //        // Add this new edge as a problem, but avoid redundancy
        //        if (!problems.Contains(thisNewProblem))
        //        {
        //            pathHashMap.Put(thisNewProblem);
        //            problems.Put(thisNewProblem);
        //        }
        //        //else
        //        //{
        //        //    Debug.WriteLine("Deduced already: " + thisNewProblem.ToString());
        //        //}
        //    }
        //}

        ////
        //// Adds a hyperedge to the reachability matrix as well as the predecessor matrix
        ////
        //public void AddEdge(PebblerHyperEdge edge)
        //{
        //    Debug.WriteLine("Considering Edge: " + edge.ToString());
        //    AddToReachability(edge);

        //    edges.Add(edge);
        //    GeneratePaths(new Problem(edge.sourceNodes, edge.targetNode, edges.Count));
        //}

        ////
        //// Consumer thread executes this method
        ////
        //public void GenerateAllPaths()
        //{
        //    //while (!sharedEdgeList.IsReadingAndWritingComplete()) // busy waiting?
        //    //{
        //    //    // Acquire the new edge
        //    //    PebblerHyperEdge edge = sharedEdgeList.ReadEdge();

        //    //    // Prohibit generating any problem in which an axiomatic node is a goal.
        //    //    if (!graph.GetNode(edge.targetNode).IsAxiomatic())
        //    //    {
        //    //        // Create the corresponding problem
        //    //        Problem newProblem = new Problem(edge.sourceNodes, edge.targetNode, problems.size);

        //    //        // Create the problem paths / solutions
        //    //        GeneratePaths(newProblem);
        //    //    }
        //    //}

        //    Debug.WriteLine("-------------------");
        //    foreach (Problem problem in problems.GetProblems())
        //    {
        //        Debug.WriteLine(problem.ToString());
        //    }

        //    //Debug.WriteLine("Path Map (Size: " + pathHashMap.size + "): \n" + pathHashMap.ToString());
        // }

        ////
        //// Adds a hyperedge to the reachability matrix; [r, c] indicates is there exists a path from r -> c
        ////
        //private void AddToReachability(PebblerHyperEdge edge)
        //{
        //    //
        //    // for each node in the source list, indicate that they can reach the target.
        //    //
        //    // Create a single edge path
        //    Path newP = new Path(Utilities.MakeList<PebblerHyperEdge>(edge));
        //    foreach (int src in edge.sourceNodes)
        //    {
        //        reachable[src, edge.targetNode] = true;
        //        Debug.WriteLine("Setting <" + src + ", " + edge.targetNode + ">");

        //        //paths[src, edge.targetNode].Add(newP); // What about target?
        //    }

        //    //
        //    // Any predecessor of a source node can now reach the target.
        //    //
        //    foreach (int src in edge.sourceNodes)
        //    {
        //        for (int r = 0; r < reachable.GetLength(0); r++)
        //        {
        //            if (reachable[r, src])
        //            {
        //                reachable[r, edge.targetNode] = true;
        //            }
        //        }
        //    }

        //    //
        //    // Any successor of the target can now be reached by the source nodes
        //    //
        //    foreach (int src in edge.sourceNodes)
        //    {
        //        for (int r = 0; r < reachable.GetLength(0); r++)
        //        {
        //            if (reachable[edge.targetNode, r])
        //            {
        //                reachable[src, r] = true;
        //            }
        //        }
        //    }
        //}

        ////
        //// For demonstration purposes of all the generated problems.
        ////
        //public void PrintAllProblemsAndSolutions()
        //{
        //    Debug.WriteLine("\n---------- Problems / Path / Goal ---------\n");
        //    List<Problem> probs = problems.GetProblems();
        //    for (int p = 0; p < probs.Count; p++)
        //    {
        //        Debug.WriteLine(p + " " + probs[p].ConstructProblemAndSolution(graph) + "\n");
        //    }
        //}

        //public override string ToString()
        //{
        //    StringBuilder s = new StringBuilder();

        //    // Print the reachability matrix
        //    s.Append("   ");
        //    for (int c = 0; c < reachable.GetLength(1); c++)
        //    {
        //        s.Append((c % 10) + " ");
        //    }
        //    s.AppendLine();
        //    for (int r = 0; r < reachable.GetLength(0); r++)
        //    {
        //        s.Append(r + ": ");
        //        for (int c = 0; c < reachable.GetLength(1); c++)
        //        {
        //            s.Append((reachable[r, c] ? "T" : "F") + " ");
        //        }
        //        s.AppendLine();
        //    }


        //    //
        //    // Print all paths
        //    //
        //    //s.AppendLine();
        //    //for (int r = 0; r < reachable.GetLength(0); r++)
        //    //{
        //    //    for (int c = 0; c < reachable.GetLength(1); c++)
        //    //    {
        //    //        if (paths[r, c].Any())
        //    //        {
        //    //            s.AppendLine("<" + r + ", " + c + ">: ");
        //    //            foreach (Path path in paths[r, c])
        //    //            {
        //    //                s.AppendLine("\t" + path.ToString());
        //    //            }
        //    //        }
        //    //    }
        //    //    s.AppendLine();
        //    //}

        //    return s.ToString();
        //}
    }
}





        ////
        //// From this node, generate all problems backward.
        //// Do so by generating simple problems first (in a depth-first manner)
        ////
        //private List<Problem> GenerateAllProblemsFrom(ProblemHashMap memoizedProblems, HyperEdgeMultiMap forwardPebbledEdges, int node)
        //{
        //    if (node == 162)
        //    {
        //        Debug.WriteLine("NO-OP");
        //    }

        //    // If we have already generated problems, no need to regenerate
        //    if (memoizedProblems.HasNodeBeenGenerated(node)) return memoizedProblems.Get(node);

        //    // For all edges, create base problems and add them all to the database
        //    List<PebblerHyperEdge> forwardEdges = forwardPebbledEdges.GetBasedOnGoal(node);

        //    // If there are no edges, this is a 'root' node (has no predecessors)
        //    if (forwardEdges == null || !forwardEdges.Any())
        //    {
        //        // We've successfully generated all the problems for this node: zero problems
        //        memoizedProblems.SetGenerated(node);
        //        return new List<Problem>();
        //    }
            
        //    //
        //    // Create all the base problems consisting of only one edge and add to the database
        //    List<Problem> baseProblems = new List<Problem>();
        //    foreach (PebblerHyperEdge edge in forwardEdges)
        //    {
        //        baseProblems.Add(new Problem(edge));
        //    }
        //    memoizedProblems.PutUnchecked(baseProblems);

        //    // For all of the base problems, generate backward
        //    foreach (Problem baseProblem in baseProblems)
        //    {
        //        // For all Acquire all problems generated from each of the givens (and append to this problem) 
        //        List<int> tempGivens = new List<int>(baseProblem.givens);
        //        foreach (int given in tempGivens)
        //        {
        //            // While we append, givens might be pushed (deduced) into the path; so no need to use / generate with some givens
        //            if (baseProblem.givens.Contains(given))
        //            {
        //                // For all problems from the given node, append it to a copy of the base problem
        //                List<Problem> thisGivenProblems = GenerateAllProblemsFrom(memoizedProblems, forwardPebbledEdges, given);
        //                foreach (Problem givenProblem in thisGivenProblems)
        //                {
        //                    // Create a copy of the simple base problem
        //                    Problem copyOfBaseProblem = new Problem(baseProblem);
                            
        //                    // Append the recursively generated (potentially more complicated) problem
        //                    copyOfBaseProblem.Append(graph, forwardPebbledEdges, givenProblem);

        //                    //// Avoid any simple cycles in the problem; this should not happen since cycles are avoided during pebbling
        //                    //if (copyOfBaseProblem.ContainsCycle())
        //                    //{
        //                    //    Debug.WriteLine("Problem has a cycle: " + copyOfBaseProblem);
        //                    //    //throw new Exception("Problem has a cycle: " + copyOfBaseProblem);
        //                    //}
        //                    //else
        //                    //{

        //                    // Add this problem to the database
        //                    memoizedProblems.Put(copyOfBaseProblem);
        //                }
        //            }
        //        }
        //    }

        //    // We've successfully generated all the problems for this node
        //    memoizedProblems.SetGenerated(node);

        //    // Return all the generated problems
        //    return memoizedProblems.Get(node);
        //}