using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // A reduced version of the original hypergraph that provides simple pebbling and exploration
    //
    public class PebblerHypergraph<T>
    {
        private readonly static int UNMARKED_NODE = -1;

        // The main graph data structure
        private PebblerHyperNode<T>[] vertices;

        private PathGenerator pathGenerator;

        public PebblerHypergraph(PebblerHyperNode<T>[] inputVertices)
        {
            vertices = inputVertices;
            pathGenerator = new PathGenerator(vertices.Length);
        }

        public void GenerateAllPaths(List<int> src)
        {
            Pebble(src);

            Debug.WriteLine(pathGenerator.ToString());
        }

        //
        // Use Dowling-Gallier pebbling technique to pebble using all given nodes
        //
        public void Pebble(List<int> src)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // Pebble all start vertices
            foreach (int s in src)
            {
                Traverse(s);
            }

            // Stop timing
            stopwatch.Stop();

            Debug.WriteLine("Vertices after pebbling:");
            for (int i = 0; i < vertices.Length; i++)
            {
                Debug.WriteLine(vertices[i].id + ": pebbled(" + vertices[i].pebbled + ")");
            }

            Debug.WriteLine("All Paths:");
            pathGenerator.GenerateAllPaths();

            TimeSpan ts = stopwatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                               ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Debug.WriteLine("Length of time to compute all paths: " + elapsedTime);
            Debug.WriteLine("Number of Unique Paths / Problems: " + pathGenerator.GetPaths().Count);
        }

        //
        // Given a node, pebble the reachable parts of the graph
        //
        public void Traverse(int currentNodeIndex)
        {
            // Has this node already been pebbled?
            if (vertices[currentNodeIndex].pebbled) return;

            // Pebble the node currentnode
            vertices[currentNodeIndex].pebbled = true;

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].successorEdges)
            {
                // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
                Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

                // Now, check to see if the target node if available to pebble: number of incoming vertices equates to the number of pebbles
                if (currentEdge.sourceNodes.Count == currentEdge.sourcePebbles.Count)
                {
                    // Percolate this node through the graph recursively; recursion will pebble the target node
                    Traverse(currentEdge.targetNode);

                    // Add to the path generator the fact that we have a successor that has been pebbled.
                    pathGenerator.AddEdge(currentEdge);
                }
            }
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("All Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].predecessorEdges.Any())
                {
                    edgeStr = new StringBuilder();
                    edgeStr.Append("{ ");
                    foreach (int s in vertices[v].predecessorEdges[0].targetNodes)
                    {
                        edgeStr.Append(s + " ");
                    }
                    edgeStr.Remove(edgeStr.Length - 1, 1);
                    edgeStr.Append(" }");
                }

                Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
            }


            Debug.WriteLine("\nEdges: ");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].successorEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (PebblerHyperEdge edge in vertices[v].successorEdges)
                    {
                        edgeStr.Append(" { ");
                        foreach (int s in edge.sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Append("} -> " + edge.targetNode + ", ");
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }
            Debug.WriteLine(edgeStr);
        }


        //
        // Use Dowling-Gallier pebbling technique
        //
        public bool Pebble(List<int> src, List<int> goals)
        {
            // Pebble all figure-based clauses
            //foreach (GroundedClause figureClause in grounds)
            //{
            //    if (groundChecked.Contains(figureClause))
            //    {
            //        Traverse(figureClause.graphId);
            //    }
            //}

            // Pebble all start vertices
            foreach (int s in src)
            {
                Traverse(s);
            }

            foreach (int goal in goals)
            {
                if (!vertices[goal].pebbled)
                {
                    Debug.WriteLine("Did not successfully pebble: " + goal);
                }
                else
                {
                    Debug.WriteLine("Pebbled: " + goal);
                }
            }

            Debug.WriteLine("vertices after pebbling.");
            for (int i = 0; i < vertices.Length; i++)
            {
                StringBuilder str = new StringBuilder();

                str.Append(vertices[i].id + ": pebbled(" + vertices[i].pebbled + ")");

                //str.Append(i + ": preds(pi)(");
                //foreach (int pred in vertices[i].pi)
                //{
                //    str.Append(pred + " ");
                //}
                //str.Append(")");
                Debug.WriteLine(str);
            }

            //Debug.WriteLine("Edges after pebbling.");
            //for (int i = 0; i < graphHyperedges.Count; i++)
            //{
            //    StringBuilder str = new StringBuilder();
            //    str.Append(i + ": sources(");
            //    foreach (int pred in graphHyperedges.ElementAt(i).sourceNodes)
            //    {
            //        str.Append(pred + " ");
            //    }
            //    str.Append(")");

            //    str.Append(i + ": pebbling(");
            //    foreach (int pebbled in graphHyperedges.ElementAt(i).pebbles)
            //    {
            //        str.Append(pebbled + " ");
            //    }
            //    str.Append(")");
            //    Debug.WriteLine(str);
            //}


            return true;
        }



        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public bool AcquirePath(List<int> path, int source, int target)
        //{
        //    bool pathFound = false;

        //    // Did we find our goal?
        //    if (source == target)
        //    {
        //        Utilities.AddUnique<int>(path, source);
        //        //Debug.WriteLine("Added: " + source);
        //        return true;
        //    }
        //    // Are there any predecessors to this node?
        //    else if (!vertices[target].pi.Any())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        //
        //        // We must add all predecessor vertices to the list and pursue those
        //        // vertices up the graph to vertices without predecessors
        //        //
        //        foreach (int pred in vertices[target].pi)
        //        {
        //            if (!path.Contains(pred))
        //            {
        //                Utilities.AddUnique<int>(path, pred);
        //                //Debug.WriteLine("Added: " + pred);
        //                pathFound |= AcquirePath(path, source, pred);
        //            }
        //        }
        //        Utilities.AddUnique<int>(path, target);
        //        //Debug.WriteLine("Added: " + target);
        //    }

        //    return pathFound;
        //}

        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public void PrintPath(int source, int target)
        //{
        //    List<int> path = new List<int>();

        //    if (!AcquirePath(path, source, target))
        //    {
        //        Debug.WriteLine("No path found from " + source + " to " + target);
        //    }
        //    else
        //    {

        //        int[] pathArray = path.ToArray();
        //        Array.Sort<int>(pathArray);

        //        // for all predecessors vertices
        //        foreach (int node in pathArray)
        //        {
        //            Debug.WriteLine(node + " " + vertices[node]);
        //        }
        //    }
        //}

        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public void PrintAllPathsToInteresting(int source)
        //{
        //    const int NumGoals = 3;
        //    MaxHeap interestingGoals = FindInterestingGoalNodes();

        //    for (int goal = 0; goal < NumGoals; goal++)
        //    {
        //        int goalNode = (int)interestingGoals.ExtractMax().data;

        //        Debug.WriteLine("Interesting Goal (" + goalNode + ")" + vertices[goalNode].ToString());

        //        PrintPath(source, goalNode);
        //    }
        //}

        //
        // Find n interesting goal vertices in the hypergraph
        //
        public MaxHeap FindInterestingGoalNodes()
        {
            // Order the interesting vertices by a scored (numeric) criteria defined by each class
            MaxHeap interestingGoals = new MaxHeap(vertices.Length);

            for (int n = 0, interestingNess = 0; n < vertices.Length; n++, interestingNess = 0)
            {
                // Equations are generally not interesting' congruences are;
                // Or, is this a source node (having no predecessors)?
                if (vertices[n].data is GeometryTutorLib.ConcreteAbstractSyntax.ArithmeticNode || !vertices[n].predecessorEdges.Any())
                {
                    interestingNess = 0;
                }
                else
                {
                    // If this node produces many things and is not a source node
                    if (vertices[n].predecessorEdges.Count > 1)
                    {
                        interestingNess++;
                    }

                    // If this node requires many things to prove, it is interesting
                    if (vertices[n].predecessorEdges[0].targetNodes.Count > 1)
                    {
                        interestingNess += (int)Math.Floor(vertices[n].predecessorEdges[0].targetNodes.Count / 2);
                    }
                }

                interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
            }

            return interestingGoals;
        }


        //public int Size() { return vertices.Count; }

        ////
        //// Check if the graph contains this specific grounded clause
        ////
        //private int ConvertToLocalIntegerIndex(T inputData)
        //{
        //    for (int i = 0; i < vertices.Count; i++)
        //    {
        //        if (vertices[i].data.Equals(inputData)) return i;
        //    }

        //    return -1;
        //}

        ////
        //// Check if the graph contains this specific grounded clause
        ////
        //public bool HasNode(T inputData)
        //{
        //    foreach (HyperNode<T, A> vertex in vertices)
        //    {
        //        if (vertex.data.Equals(inputData)) return true;
        //    }

        //    return false;
        //}

        ////
        //// Check if the graph contains this specific grounded clause
        ////
        //public void AddNode(T inputData)
        //{
        //    if (!HasNode(inputData))
        //    {
        //        vertices.Add(new HyperNode<T, A>(inputData, vertices.Count)); // <data, id>
        //    }
        //}

        ////
        //// Is this edge in the graph (using local, integer-based information)
        ////
        //private bool HasLocalEdge(List<int> antecedent, int consequent)
        //{
        //    foreach (HyperNode<T, A> vertex in vertices)
        //    {
        //        foreach (HyperEdge<A> edge in vertex.successorEdges)
        //        {
        //            if (edge.DefinesEdge(antecedent, consequent)) return true;
        //        }
        //    }

        //    return false;
        //}

        ////
        //// Check if the graph contains an edge defined by a many to one clause mapping
        ////
        //public bool HasEdge(List<T> antecedent, T consequent)
        //{
        //    KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

        //    return HasLocalEdge(local.Key, local.Value);
        //}

        ////
        //// Convert information to local, integer-based representation
        ////
        //private KeyValuePair<List<int>, int> ConvertToLocal(List<T> antecedent, T consequent)
        //{
        //    List<int> localAnte = new List<int>();

        //    foreach (T ante in antecedent)
        //    {
        //        int index = ConvertToLocalIntegerIndex(ante);

        //        if (index == -1)
        //        {
        //            throw new ArgumentException("Source node not found as a hypergraph node: " + ante);
        //        }

        //        localAnte.Add(index);
        //    }

        //    int localConsequent = ConvertToLocalIntegerIndex(consequent);

        //    if (localConsequent == -1)
        //    {
        //        throw new ArgumentException("Target value referenced not found as a hypergraph node: " + consequent);
        //    }

        //    return new KeyValuePair<List<int>, int>(localAnte, localConsequent);
        //}

        ////
        //// Adding an edge to the graph
        ////
        //public void AddEdge(List<T> antecedent, T consequent, A annotation)
        //{
        //    //
        //    // Add a local representaiton of this edge to each node in which it is applicable
        //    //
        //    if (HasEdge(antecedent, consequent)) return;

        //    KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

        //    HyperEdge<A> edge = new HyperEdge<A>(local.Key, local.Value, annotation);

        //    foreach (int src in local.Key)
        //    {
        //        vertices[src].AddSuccessorEdge(edge);
        //    }

        //    //
        //    // Add a predecessor node to the new target
        //    //
        //    TransposeHyperEdge<A> tEdge = new TransposeHyperEdge<A>(local.Value, local.Key, annotation);

        //    vertices[local.Value].AddPredecessorEdge(tEdge);
        //}

        //public void DebugDumpClauses()
        //{
        //    Debug.WriteLine("All Clauses:\n");

        //    StringBuilder edgeStr = new StringBuilder();
        //    for (int v = 0; v < vertices.Count; v++)
        //    {
        //        if (vertices[v].data is ConcreteCongruent)
        //        {
        //            if (vertices[v].predecessorEdges.Any())
        //            {
        //                edgeStr = new StringBuilder();
        //                edgeStr.Append("{ ");
        //                foreach (int s in vertices[v].predecessorEdges[0].targetNodes)
        //                {
        //                    edgeStr.Append(s + " ");
        //                }
        //                edgeStr.Remove(edgeStr.Length - 1, 1);
        //                edgeStr.Append(" }");
        //            }

        //            Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
        //        }
        //    }


        //    Debug.WriteLine("\nEdges: ");
        //    edgeStr = new StringBuilder();
        //    for (int v = 0; v < vertices.Count; v++)
        //    {
        //        if (vertices[v].successorEdges.Any())
        //        {
        //            edgeStr.Append(v + ": {");
        //            foreach (HyperEdge<A> edge in vertices[v].successorEdges)
        //            {
        //                edgeStr.Append(" { ");
        //                foreach (int s in edge.sourceNodes)
        //                {
        //                    edgeStr.Append(s + " ");
        //                }
        //                edgeStr.Append("} -> " + edge.targetNode + ", ");
        //            }
        //            edgeStr.Remove(edgeStr.Length - 2, 2);
        //            edgeStr.Append(" }\n");
        //        }
        //    }
        //    Debug.WriteLine(edgeStr);
        //}

        //// Finds all source vertices (vertices that have no predecessor).
        //private void IdentifyAllSourceNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit vertices with no preds and no succs
        //        if (!clause.GetPredecessors().Any() && clause.GetSuccessors().Any()) sourceNodes.Add(clause);
        //    }
        //}

        //// Finds all source vertices (vertices that have no successor).
        //private void IdentifyAllLeafNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit vertices with no preds and no succs
        //        if (!clause.GetSuccessors().Any() && clause.GetPredecessors().Any()) leafNodes.Add(clause);
        //    }
        //}

        /*
                //
                // Constructs an integer representation of the powerset based on input value integer n
                // e.g. 2 -> { {}, {0}, {1}, {0, 1} }
                //
                private List<List<int>> ConstructPowerSetWithNoEmpty(int n)
                {
                    if (n <= 0) return Utilities.MakeList<List<int>>(new List<int>());

                    List<List<int>> powerset = ConstructPowerSetWithNoEmpty(n - 1);
                    List<List<int>> newCopies = new List<List<int>>();

                    foreach (List<int> intlist in powerset)
                    {
                        // Make a copy, add to copy, add to overall list
                        List<int> copy = new List<int>(intlist);
                        copy.Add(n - 1); // We are dealing with indices, subtract 1
                        newCopies.Add(copy);
                    }

                    powerset.AddRange(newCopies);

                    return powerset;
                }

                //
                // Construct a list of all possible pairs of source vertices mapped to leaf vertices
                //
                private List<KeyValuePair<List<int>, List<int>>> ConstructSourceAndGoalNodes()
                {
                    List<List<int>> powersetSrc = ConstructPowerSetWithNoEmpty(sourceNodes.Count);
                    List<List<int>> powersetLeaf = ConstructPowerSetWithNoEmpty(leafNodes.Count);
                    List<KeyValuePair<List<int>, List<int>>> pairs = new List<KeyValuePair<List<int>, List<int>>>();

                    foreach (List<int> srcNodeIndices in powersetSrc)
                    {
                        foreach (List<int> leafNodeIndices in powersetLeaf)
                        {

                            pairs.Add(new KeyValuePair<List<int>, List<int>>(srcNodeIndices, leafNodeIndices));
                        }
                    }

                    StringBuilder debugStr = new StringBuilder("{");
                    foreach (List<int> srcNodeIndices in powersetSrc)
                    {
                        debugStr.Append(" { ");
                        foreach (int i in srcNodeIndices)
                        {
                            debugStr.Append(i + " ");
                        }
                        debugStr.AppendLine(" } ");
                    }
                    debugStr.AppendLine(" } ");
                    Debug.WriteLine(debugStr.ToString());

                    return pairs;
                }
        */

        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public bool AcquirePath(List<int> path, int source, int target)
        //{
        //    bool pathFound = false;

        //    // Did we find our goal?
        //    if (source == target)
        //    {
        //        Utilities.AddUnique<int>(path, source);
        //        //Debug.WriteLine("Added: " + source);
        //        return true;
        //    }
        //    // Are there any predecessors to this node?
        //    else if (!vertices[target].pi.Any())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        //
        //        // We must add all predecessor vertices to the list and pursue those
        //        // vertices up the graph to vertices without predecessors
        //        //
        //        foreach (int pred in vertices[target].pi)
        //        {
        //            if (!path.Contains(pred))
        //            {
        //                Utilities.AddUnique<int>(path, pred);
        //                //Debug.WriteLine("Added: " + pred);
        //                pathFound |= AcquirePath(path, source, pred);
        //            }
        //        }
        //        Utilities.AddUnique<int>(path, target);
        //        //Debug.WriteLine("Added: " + target);
        //    }

        //    return pathFound;
        //}

        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public void PrintPath(int source, int target)
        //{
        //    List<int> path = new List<int>();

        //    if (!AcquirePath(path, source, target))
        //    {
        //        Debug.WriteLine("No path found from " + source + " to " + target);
        //    }
        //    else
        //    {

        //        int[] pathArray = path.ToArray();
        //        Array.Sort<int>(pathArray);

        //        // for all predecessors vertices
        //        foreach (int node in pathArray)
        //        {
        //            Debug.WriteLine(node + " " + vertices[node]);
        //        }
        //    }
        //}

        ////
        //// Given start and end node-pairs, print the required vertices
        ////
        //public void PrintAllPathsToInteresting(int source)
        //{
        //    const int NumGoals = 3;
        //    MaxHeap interestingGoals = FindInterestingGoalNodes();

        //    for (int goal = 0; goal < NumGoals; goal++)
        //    {
        //        int goalNode = (int)interestingGoals.ExtractMax().data;

        //        Debug.WriteLine("Interesting Goal (" + goalNode + ")" + vertices[goalNode].ToString());

        //        PrintPath(source, goalNode);
        //    }
        //}

        ////
        //// Find n interesting goal vertices in the hypergraph
        ////
        //public MaxHeap FindInterestingGoalNodes()
        //{
        //    // Order the interesting vertices by a scored (numeric) criteria defined by each class
        //    MaxHeap interestingGoals = new MaxHeap(vertices.Length);

        //    for (int n = 0, interestingNess = 0; n < vertices.Length; n++, interestingNess = 0)
        //    {
        //        // Equations are generally not interesting' congruences are;
        //        // Or, is this a source node (having no predecessors)?
        //        if (vertices[n].clause is ArithmeticNode || !vertices[n].clause.GetPredecessors().Any())
        //        {
        //            interestingNess = 0;
        //        }
        //        else
        //        {
        //            // If this node produces many things and is not a source node
        //            if (vertices[n].clause.GetSuccessors().Count > 1)
        //            {
        //                interestingNess++;
        //            }

        //            // If this node requires many things to prove, it is interesting
        //            if (vertices[n].clause.GetPredecessors().ElementAt(0).Value.Count > 1)
        //            {
        //                interestingNess += (int)Math.Floor(vertices[n].clause.GetPredecessors().ElementAt(0).Value.Count / 2);
        //            }
        //        }

        //        interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
        //    }

        //    return interestingGoals;
        //}
    }
}