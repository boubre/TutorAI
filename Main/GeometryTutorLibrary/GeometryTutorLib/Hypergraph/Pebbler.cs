using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericInstantiator;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Provides functionality to create a hypergraph: adding nodes and edges
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Pebbler
    {
        private readonly static int UNMARKED_NODE = -1;

        private Hypergraph<GroundedClause, int> graph;

        public Pebbler(Hypergraph<GroundedClause, int> g)
        {
            graph = g;
        }

        //public void ConstructGraphRepresentation()
        //{
        //    IdentifyAllSourceNodes();
        //    IdentifyAllLeafNodes();

        //    //
        //    // Construct the nodes of the graph
        //    //
        //    nodes = new HyperNode[groundChecked.Count];

        //    for (int i = 0; i < nodes.Length; i++)
        //    {
        //        // (clause                   ; local id)
        //        nodes[i] = new HyperNode(groundChecked.ElementAt(i), i);
        //    }

        //    //
        //    // Construct the hyperedges of the graph
        //    //
        //    for (int n = 0; n < nodes.Length; n++)
        //    {
        //        List<KeyValuePair<List<GroundedClause>, GroundedClause>> succs = nodes[n].clause.GetSuccessors();

        //        // Convert all edge representations to integers
        //        foreach (KeyValuePair<List<GroundedClause>, GroundedClause> edge in succs)
        //        {
        //            // Convert the nodes > ints
        //            List<int> srcNodes = new List<int>();
        //            foreach (GroundedClause src in edge.Key)
        //            {
        //                srcNodes.Add(src.graphId);
        //                Utilities.AddUnique<int>(nodes[n].successorNodes, src.graphId);
        //            }

        //            //
        //            // Create ONE hyperedge object and add to all appropriate nodes
        //            //
        //            // If this is the smallest valued source node, create edge and add to all source nodes
        //            // Note: this is ok due to the creation of the node list first
        //            if (n == srcNodes.Min())
        //            {
        //                HyperEdge edgeObj = new HyperEdge(srcNodes, edge.Value.graphId);

        //                foreach (int srcNode in srcNodes)
        //                {
        //                        nodes[srcNode].successorEdges.Add(edgeObj);
        //                }

        //                // Add to the overall list of hyperedges; unique addition should not be needed
        //                Utilities.AddUnique<HyperEdge>(graphHyperedges, edgeObj);
        //            }
        //        }

        //        List<KeyValuePair<GroundedClause, List<GroundedClause>>> preds = nodes[n].clause.GetPredecessors();

        //        // Convert all transpose edge representations to integers
        //        foreach (KeyValuePair<GroundedClause, List<GroundedClause>> transposeEdge in preds)
        //        {
        //            List<int> targetNodes = new List<int>();
        //            foreach (GroundedClause target in transposeEdge.Value)
        //            {
        //                targetNodes.Add(target.graphId);
        //                Utilities.AddUnique<int>(nodes[n].predecessorNodes, target.graphId);
        //            }

        //            nodes[n].predecessorEdges.Add(new TransposeHyperEdge(transposeEdge.Key.graphId, targetNodes));
        //        }
        //    }
        //}

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

        //
        // Use Dowling-Gallier pebbling technique
        //
        public bool Pebble(List<int> src, List<int> goals)
        {
            // Pebble all figure-based clauses
            foreach (GroundedClause figureClause in grounds)
            {
                if (groundChecked.Contains(figureClause))
                {
                    Traverse(figureClause.graphId);
                }
            }


            // Pebble all start nodes
            foreach (int s in src)
            {
                Traverse(s);
            }

            foreach (int goal in goals)
            {
                if (!nodes[goal].pebbled)
                {
                    Debug.WriteLine("Did not successfully pebble: " + groundChecked.ElementAt(goal).ToString());
                }
                else
                {
                    Debug.WriteLine("Pebbled: " + groundChecked.ElementAt(goal).ToString());
                }
            }

            Debug.WriteLine("Nodes after pebbling.");
            for (int i = 0; i < nodes.Length; i++)
            {
                StringBuilder str = new StringBuilder();

                str.Append(nodes[i].id + ": pebbled(" + nodes[i].pebbled + ")");

                str.Append(i + ": preds(pi)(");
                foreach (int pred in nodes[i].pi)
                {
                    str.Append(pred + " ");
                }
                str.Append(")");
                Debug.WriteLine(str);
            }

            Debug.WriteLine("Edges after pebbling.");
            for (int i = 0; i < graphHyperedges.Count; i++)
            {
                StringBuilder str = new StringBuilder();
                str.Append(i + ": sources(");
                foreach (int pred in graphHyperedges.ElementAt(i).sourceNodes)
                {
                    str.Append(pred + " ");
                }
                str.Append(")");
                
                str.Append(i + ": pebbling("); 
                foreach (int pebbled in graphHyperedges.ElementAt(i).pebbles)
                {
                    str.Append(pebbled + " ");
                }
                str.Append( ")");
                Debug.WriteLine(str);
            }


            return true;
        }

        //
        // Given a node, pebble the reachable parts of the graph
        //
        public void Traverse(int currentNodeIndex)
        {
            // Has this node already been pebbled?
            if (nodes[currentNodeIndex].pebbled) return;

            // Pebble the node currentnode
            nodes[currentNodeIndex].pebbled = true;

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (HyperEdge currentEdge in nodes[currentNodeIndex].successorEdges)
            {
                // Indicate the node has been pebbled by adding to the list of pebbled nodes; should not have to be a unique addition
                Utilities.AddUnique<int>(currentEdge.pebbles, currentNodeIndex);

                // Now, check to see if the target node if available to pebble: number of incoming nodes equates to the number of pebbles
                if (currentEdge.sourceNodes.Count == currentEdge.pebbles.Count)
                {
                    // Percolate this node through the graph recursively; recursion will pebble the target node
                    Traverse(currentEdge.targetNode);

                    // For paths, indicate that we have a predecessor
                    nodes[currentEdge.targetNode].pi.AddRange(currentEdge.sourceNodes);     // it is possible to have more than one edge pebble into the target; need to handle for ALL paths
                }
            }
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public bool AcquirePath(List<int> path, int source, int target)
        {
            bool pathFound = false;

            // Did we find our goal?
            if (source == target)
            {
                Utilities.AddUnique<int>(path, source);
                //Debug.WriteLine("Added: " + source);
                return true;
            }
            // Are there any predecessors to this node?
            else if (!nodes[target].pi.Any())
            {
                return false;
            }
            else
            {
                //
                // We must add all predecessor nodes to the list and pursue those
                // nodes up the graph to nodes without predecessors
                //
                foreach (int pred in nodes[target].pi)
                {
                    if (!path.Contains(pred))
                    {
                        Utilities.AddUnique<int>(path, pred);
                        //Debug.WriteLine("Added: " + pred);
                        pathFound |= AcquirePath(path, source, pred);
                    }
                }
                Utilities.AddUnique<int>(path, target);
                //Debug.WriteLine("Added: " + target);
            }

            return pathFound;
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public void PrintPath(int source, int target)
        {
            List<int> path = new List<int>();

            if (!AcquirePath(path, source, target))
            {
                Debug.WriteLine("No path found from " + source + " to " + target);
            }
            else
            {

                int[] pathArray = path.ToArray();
                Array.Sort<int>(pathArray);

                // for all predecessors nodes
                foreach (int node in pathArray)
                {
                    Debug.WriteLine(node + " " + nodes[node]);
                }
            }
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public void PrintAllPathsToInteresting(int source)
        {
            const int NumGoals = 3;
            MaxHeap interestingGoals = FindInterestingGoalNodes();

            for (int goal = 0; goal < NumGoals; goal++)
            {
                int goalNode = (int)interestingGoals.ExtractMax().data;

                Debug.WriteLine("Interesting Goal (" + goalNode + ")" + nodes[goalNode].ToString());

                PrintPath(source, goalNode);
            }
        }

        //
        // Find n interesting goal nodes in the hypergraph
        //
        public MaxHeap FindInterestingGoalNodes()
        {
            // Order the interesting nodes by a scored (numeric) criteria defined by each class
            MaxHeap interestingGoals = new MaxHeap(nodes.Length);

            for (int n = 0, interestingNess = 0; n < nodes.Length; n++, interestingNess = 0)
            {
                // Equations are generally not interesting' congruences are;
                // Or, is this a source node (having no predecessors)?
                if (nodes[n].clause is ArithmeticNode || !nodes[n].clause.GetPredecessors().Any())
                {
                    interestingNess = 0;
                }
                else
                {
                    // If this node produces many things and is not a source node
                    if (nodes[n].clause.GetSuccessors().Count > 1)
                    {
                        interestingNess++;
                    }

                    // If this node requires many things to prove, it is interesting
                    if (nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count > 1)
                    {
                        interestingNess += (int)Math.Floor(nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count / 2);
                    }
                }

                interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
            }

            return interestingGoals;
        }

        ////
        //// According to the paper, this procedure updates numNegArgs for every clause in the 
        //// clauselist corresponding to the positive literal current
        ////
        //private void Update(int nodeId)
        //{
        //    foreach (HyperEdge edge in graphHyperedges)
        //    {
        //        if (edge.sourceNodes.Contains(nodeId))
        //        {
        //            edge.numNegArgs--;
        //        }
        //    }
        //}

        ////
        //// Given a node, traverse the graph
        ////
        //public void OriginalTraverse(int currentNodeIndex)
        //{
        //    // If val of current is not already computed call traverse recursively
        //    if (nodes[currentNodeIndex].computed) return;

        //    // Take care of nodes initialized to true
        //    if (nodes[currentNodeIndex].pebbled)
        //    {
        //        nodes[currentNodeIndex].computed = true;

        //        // Update clause to have one fewer negative value
        //        Update(currentNodeIndex);

        //        return;
        //    }

        //    // For every clause number j, compute the value of the targets of all edges with source
        //    // current labeled j, as long as current.pebbled is not true
        //    //
        //    // Each hypernode has independent outgoing edges:
        //    // For all outgoing hyperedge 'groups' from this node,
        //    //   traverse depth-first to other nodes. 
        //    //
        //    List<HyperEdge> tagset = nodes[currentNodeIndex].successorEdges;

        //    for (int j = 0; j < tagset.Count && !nodes[currentNodeIndex].pebbled; j++)
        //    {
        //        HyperEdge arc = tagset[j];

        //        // Traverse Recursively for every arc labeled j
        //        // That is, for all nodes which are sources of this edge, traverse
        //        foreach (int arcSourceNode in arc.sourceNodes)
        //        {
        //            Debug.WriteLine("Considering Edge <" + currentNodeIndex + " : " + arcSourceNode + ", " + arc.targetNode + ">");

        //            // If arc not visited then call traverse on the target of the hyperedge
        //            if (!arc.visited)
        //            {
        //                nodes[currentNodeIndex].marked--;
        //                arc.visited = true;
        //                Traverse(arc.targetNode);
        //            }
        //            // If all arcs have been visited and target node has some unmarked outgoing edge then call traverse 
        //            else if (nodes[arc.targetNode].marked != 0 && nodes[currentNodeIndex].marked == 0)
        //            {
        //                Traverse(arc.targetNode);
        //            }
        //        }

        //        // If not already computed and all arguments for clause j are available, compute the truth
        //        // values of current
        //        if (!nodes[currentNodeIndex].computed)
        //        {
        //            if (arc.numNegArgs == 0)
        //            {
        //                // Update counter for every clause in the clauselist corresponding to current and set to true
        //                Update(currentNodeIndex);
        //                nodes[currentNodeIndex].pebbled = true;
        //            }
        //        }
        //    }

        //    nodes[currentNodeIndex].computed = true;
        //}

        //public bool AcquirePath(List<int> path, int source, int target)
        //{
        //    bool pathFound = false;

        //    if (source == target)
        //    {
        //        Utilities.AddUnique<int>(path, source);
        //        return true;
        //    }
        //    else if (!nodes[target].pi.Any())
        //    {
        //        Debug.WriteLine("No path exists from node " + source + " to " + target);
        //    }
        //    else
        //    {
        //        // for all predecessors nodes
        //        for (int i = 0; i < nodes[target].pi.Count && !pathFound; i++)
        //        {
        //            int pred = nodes[target].pi[i];
        //            if (!path.Contains(pred))
        //            {
        //                pathFound = AcquirePath(path, source, pred);
        //                Utilities.AddUnique<int>(path, pred);
        //            }
        //        }
        //        Utilities.AddUnique<int>(path, target);
        //    }
        //    return pathFound;
        //}
    }
}