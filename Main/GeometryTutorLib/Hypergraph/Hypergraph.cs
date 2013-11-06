using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Provides functionality to create a hypergraph: adding nodes and edges
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Hypergraph<T, A>
    {
        private readonly static int UNMARKED_NODE = -1;

        // The main graph data structure
        private List<HyperNode<T, A>> vertices;

        // Clauses that have gone through the instantiation process
        List<GroundedClause> sourceNodes;
        List<GroundedClause> leafNodes;

        public Hypergraph()
        {
            vertices = new List<HyperNode<T, A>>();

            sourceNodes = new List<GroundedClause>();
            leafNodes = new List<GroundedClause>();
        }

        public int Size() { return vertices.Count; }

        public PebblerHypergraph<T> GetPebblerHypergraph()
        {
            //
            // Strictly create the nodes
            //
            PebblerHyperNode<T>[] pebblerNodes = new PebblerHyperNode<T>[vertices.Count];
            for (int v = 0; v < vertices.Count; v++)
            {
                pebblerNodes[v] = vertices[v].CreatePebblerNode();
            }

            //
            // Non-redundantly create all hyperedges
            //
            for (int v = 0; v < vertices.Count; v++)
            {
                foreach (HyperEdge<A> edge in vertices[v].successorEdges)
                {
                    // Only add once to all nodes when this is the 'minimum' source node
                    if (v == edge.sourceNodes.Min())
                    {
                        PebblerHyperEdge newEdge = new PebblerHyperEdge(edge.sourceNodes, edge.targetNode);
                        foreach (int src in edge.sourceNodes)
                        {
                            pebblerNodes[src].AddSuccessorEdge(newEdge);
                        }
                        pebblerNodes[edge.targetNode].AddPredecessorEdge(edge.targetNode, edge.sourceNodes);
                    }
                }
            }

            return new PebblerHypergraph<T>(pebblerNodes);
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        private int ConvertToLocalIntegerIndex(T inputData)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].data.Equals(inputData)) return i;
            }

            return -1;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public bool HasNode(T inputData)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                if (vertex.data.Equals(inputData)) return true;
            }

            return false;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public void AddNode(T inputData)
        {
            if (!HasNode(inputData))
            {
                vertices.Add(new HyperNode<T, A>(inputData, vertices.Count)); // <data, id>
            }
        }

        //
        // Is this edge in the graph (using local, integer-based information)
        //
        private bool HasLocalEdge(List<int> antecedent, int consequent)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                foreach (HyperEdge<A> edge in vertex.successorEdges)
                {
                    if (edge.DefinesEdge(antecedent, consequent)) return true;
                }
            }

            return false;
        }

        //
        // Check if the graph contains an edge defined by a many to one clause mapping
        //
        public bool HasEdge(List<T> antecedent, T consequent)
        {
            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            return HasLocalEdge(local.Key, local.Value);
        }

        //
        // Convert information to local, integer-based representation
        //
        private KeyValuePair<List<int>, int> ConvertToLocal(List<T> antecedent, T consequent)
        {
            List<int> localAnte = new List<int>();

            foreach (T ante in antecedent)
            {
                int index = ConvertToLocalIntegerIndex(ante);

                if (index == -1)
                {
                    throw new ArgumentException("Source node not found as a hypergraph node: " + ante);
                }

                localAnte.Add(index);
            }

            int localConsequent = ConvertToLocalIntegerIndex(consequent);

            if (localConsequent == -1)
            {
                throw new ArgumentException("Target value referenced not found as a hypergraph node: " + consequent);
            }

            return new KeyValuePair<List<int>, int>(localAnte, localConsequent);
        }

        //
        // Adding an edge to the graph
        //
        public void AddEdge(List<T> antecedent, T consequent, A annotation)
        {
            //
            // Add a local representaiton of this edge to each node in which it is applicable
            //
            if (HasEdge(antecedent, consequent)) return;

            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            HyperEdge<A> edge = new HyperEdge<A>(local.Key, local.Value, annotation);

            foreach (int src in local.Key)
            {
                vertices[src].AddSuccessorEdge(edge);
            }

            //
            // Add a predecessor node to the new target
            //
            TransposeHyperEdge<A> tEdge = new TransposeHyperEdge<A>(local.Value, local.Key, annotation);

            vertices[local.Value].AddPredecessorEdge(tEdge);
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("All Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                //if (vertices[v].data is ConcreteCongruent)
                //{
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
                //}
            }


            Debug.WriteLine("\nEdges: ");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].successorEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].successorEdges)
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

        public void DumpNonEquationClauses()
        {
            Debug.WriteLine("Non-Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (!(vertices[v].data is Equation))
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
            }
        }

        public void DumpEquationClauses()
        {
            Debug.WriteLine("Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data is Equation)
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
            }
        }

        public void DumpClauseEdges()
        {
            Debug.WriteLine("\n Clause Edges: ");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].successorEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].successorEdges)
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

        //// Finds all source nodes (nodes that have no predecessor).
        //private void IdentifyAllSourceNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit nodes with no preds and no succs
        //        if (!clause.GetPredecessors().Any() && clause.GetSuccessors().Any()) sourceNodes.Add(clause);
        //    }
        //}

        //// Finds all source nodes (nodes that have no successor).
        //private void IdentifyAllLeafNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit nodes with no preds and no succs
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
                // Construct a list of all possible pairs of source nodes mapped to leaf nodes
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
        //// Given start and end node-pairs, print the required nodes
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
        //    else if (!nodes[target].pi.Any())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        //
        //        // We must add all predecessor nodes to the list and pursue those
        //        // nodes up the graph to nodes without predecessors
        //        //
        //        foreach (int pred in nodes[target].pi)
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
        //// Given start and end node-pairs, print the required nodes
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

        //        // for all predecessors nodes
        //        foreach (int node in pathArray)
        //        {
        //            Debug.WriteLine(node + " " + nodes[node]);
        //        }
        //    }
        //}

        ////
        //// Given start and end node-pairs, print the required nodes
        ////
        //public void PrintAllPathsToInteresting(int source)
        //{
        //    const int NumGoals = 3;
        //    MaxHeap interestingGoals = FindInterestingGoalNodes();

        //    for (int goal = 0; goal < NumGoals; goal++)
        //    {
        //        int goalNode = (int)interestingGoals.ExtractMax().data;

        //        Debug.WriteLine("Interesting Goal (" + goalNode + ")" + nodes[goalNode].ToString());

        //        PrintPath(source, goalNode);
        //    }
        //}

        ////
        //// Find n interesting goal nodes in the hypergraph
        ////
        //public MaxHeap FindInterestingGoalNodes()
        //{
        //    // Order the interesting nodes by a scored (numeric) criteria defined by each class
        //    MaxHeap interestingGoals = new MaxHeap(nodes.Length);

        //    for (int n = 0, interestingNess = 0; n < nodes.Length; n++, interestingNess = 0)
        //    {
        //        // Equations are generally not interesting' congruences are;
        //        // Or, is this a source node (having no predecessors)?
        //        if (nodes[n].clause is ArithmeticNode || !nodes[n].clause.GetPredecessors().Any())
        //        {
        //            interestingNess = 0;
        //        }
        //        else
        //        {
        //            // If this node produces many things and is not a source node
        //            if (nodes[n].clause.GetSuccessors().Count > 1)
        //            {
        //                interestingNess++;
        //            }

        //            // If this node requires many things to prove, it is interesting
        //            if (nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count > 1)
        //            {
        //                interestingNess += (int)Math.Floor(nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count / 2);
        //            }
        //        }

        //        interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
        //    }

        //    return interestingGoals;
        //}
    }
}