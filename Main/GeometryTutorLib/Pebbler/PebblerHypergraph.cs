using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // Coloration of pebbles for forward and backward problem derivation;
    // No edge should EVER be purple, only nodes
    //
    public enum PebblerColorType
    {
        NO_PEBBLE = -1,
        RED_FORWARD = 0,
        BLUE_BACKWARD = 1,
        PURPLE_BOTH = 2,
        BLACK_EDGE = 3
    };

    //
    // A reduced version of the original hypergraph that provides simple pebbling and exploration
    //
    public class PebblerHypergraph<T>
    {
        // The main graph data structure
        public PebblerHyperNode<T>[] vertices { get; private set; }

        // The actual hypergraph for reference purposes only
        private Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, int> graph;
        public void SetOriginalHypergraph(Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, int> g) { graph = g; }

        // A static list of edges that can be processed using means other than a fixpoint analysis.
        public HyperEdgeMultiMap forwardPebbledEdges { get; private set; }
        public HyperEdgeMultiMap backwardPebbledEdges { get; private set; }

        // For backward pebbling only: if we have pebbled an edge completely, the target node is then reachable.
        private List<int> backwardEdgeReachableNodes;

        public PebblerHypergraph(PebblerHyperNode<T>[] inputVertices)
        {
            graph = null; // This must be set outside to use
            vertices = inputVertices;
            forwardPebbledEdges = new HyperEdgeMultiMap(vertices.Length);
            backwardPebbledEdges = new HyperEdgeMultiMap(vertices.Length);
            backwardEdgeReachableNodes = new List<int>();
        }

        //
        // Clear all pebbles from all nodes and edges in the hypergraph
        //
        public void ClearPebbles()
        {
            foreach (PebblerHyperNode<T> node in vertices)
            {
                node.pebble = PebblerColorType.NO_PEBBLE;

                foreach (PebblerHyperEdge edge in node.edges)
                {
                    edge.sourcePebbles.Clear();
                }
            }
        }

        public int NumVertices() { return vertices.Length; }

        //
        // Pebble the graph from all the sources
        //
        public void GenerateAllPaths(List<int> intrinsic, List<int> given)
        {
            Pebble(intrinsic, given);
        }

        //
        // Is the given node pebbled?
        //
        public bool IsNodePebbledForward(int index)
        {
            return vertices[index].pebble == PebblerColorType.RED_FORWARD ||
                   vertices[index].pebble == PebblerColorType.PURPLE_BOTH;
        }

        public bool IsNodePebbledBackward(int index)
        {
            return vertices[index].pebble == PebblerColorType.BLUE_BACKWARD ||
                   vertices[index].pebble == PebblerColorType.PURPLE_BOTH;
        }

        //
        // Use Dowling-Gallier pebbling technique to pebble using all given nodes
        //
        // Pebbling requires TWO phases:
        //    1. Pebble in the forward direction any node which is reached once is RED.
        //       Any node reached twice is PURPLE and identifies a backward node (via an eventual backward edge)
        //       The result of this phase are:
        //           a. marked (RED) nodes and (RED) edges for forward analysis through the graph.
        //           b. marked (PURPLE) nodes (no edges).
        //    2. Using the purple nodes as a starting point, we pebble the graph in a backward direction.
        //       This is the same algorithm as pebbling forward (RED) edges, but this time we color the edges BLUE for backward.
        //       The result of this phase are:
        //           a. All applicable nodes marked BLUE or PURPLE.
        //           b. All applicable backward edges marked BLUE.
        //
        private void Pebble(List<int> figure, List<int> givens)
        {
            // Find all axiomatic nodes.
            List<int> axiomaticNodes = new List<int>();
            for (int v = 0; v < graph.Size(); v++)
            {
                if (graph.GetNode(v).IsAxiomatic())
                {
                    axiomaticNodes.Add(v);
                }
            }

            // Forward pebble; it returns the list of blue edges from the FORWARD analysis
            KeyValuePair<List<PebblerHyperEdge>, List<int>> backInformation = PebbleForward(figure, givens, axiomaticNodes);

            //Debug.WriteLine("Before backward Pebbling: ");
            if (Utilities.DEBUG)
            {
                DebugDumpClauses();
            }
            // Backward pebble
            PebbleBackward(figure, axiomaticNodes, backInformation.Key, backInformation.Value);

            //Debug.WriteLine("After backward Pebbling: ");
            //DebugDumpClauses();
        }

        // Returns the set of backward edges and backward reachable nodes
        private KeyValuePair<List<PebblerHyperEdge>, List<int>> PebbleForward(List<int> figure, List<int> givens, List<int> axiomaticNodes)
        {
            // The list of edges which are blue from the FORWARD analysis
            List<PebblerHyperEdge> backwardEdges = new List<PebblerHyperEdge>();
            List<int> backwardReachableNodes = new List<int>();

            //
            // Pebble all axiomatic nodes.
            //
            foreach (int axiomatic in axiomaticNodes)
            {
                if (Utilities.DEBUG) Debug.WriteLine("Forward Pebbling Axiomatic: " + axiomatic);
                ForwardTraversal(axiomatic, backwardEdges, backwardReachableNodes);
            }

            //
            // Pebble all figure vertices
            //
            foreach (int fNode in figure)
            {
                if (Utilities.DEBUG) Debug.WriteLine("Pebbling Figure Node: " + fNode);
                ForwardTraversal(fNode, backwardEdges, backwardReachableNodes);
            }

            //
            // Pebble all given vertices
            //
            foreach (int g in givens)
            {
                if (Utilities.DEBUG) Debug.WriteLine("Pebbling Given Node: " + g);
                ForwardTraversal(g, backwardEdges, backwardReachableNodes);
            }

            return new KeyValuePair<List<PebblerHyperEdge>, List<int>>(backwardEdges, backwardReachableNodes);
        }

        private void PebbleBackward(List<int> figure, List<int> axiomaticNodes, List<PebblerHyperEdge> backwardEdges, List<int> backwardNodes)
        {
            //
            // Clear all the extracted backward (BLUE) edges determined during forward traversal.
            // This facilitates clean pebbling using the backward nodes as starting points.
            //
            foreach (PebblerHyperEdge edge in backwardEdges)
            {
                edge.SetColor(PebblerColorType.NO_PEBBLE);
                edge.sourcePebbles.Clear();
            }

            //DebugDumpClauses();

            //
            // Pebble all Figure nodes
            //
            // Should be sorted already
            figure.Sort();
            for (int f = figure.Count - 1; f >= 0; f--)
            {
                if (Utilities.DEBUG) Debug.WriteLine("Backward Pebbling Figure: " + figure[f]);
                BackwardTraversal(figure[f]);
            }

            List<int> axAndBackwardNodes = new List<int>();
            axAndBackwardNodes.AddRange(axiomaticNodes);
            Utilities.AddUniqueList<int>(axAndBackwardNodes, backwardNodes);
            axAndBackwardNodes.Sort();

            for (int n = axAndBackwardNodes.Count - 1; n >= 0; n--)
            {
                BackwardTraversal(axAndBackwardNodes[n]);
            }

            // We do not pebble any given nodes since we are trying to deduce those nodes
        }

        //
        // Given a node, pebble the reachable parts of the graph (in the forward direction)
        //
        // true indicates the node was pebbled as being a forward (RED) node
        // false indicates a previously visited node (eventually PURPLE) which means we have a backward (BLUE) edge
        //
        private bool ForwardTraversal(int currentNodeIndex, List<PebblerHyperEdge> backwardEdges, List<int> backwardReachableNodes)
        {
            // Error check since we never turn any node to anything but RED (forward)
            if (IsNodePebbledBackward(currentNodeIndex))
            {
                throw new ArgumentException("Forward pebbling should not result in any backward nodes: " + currentNodeIndex);
            }

            //
            // Has this node already been visited in the forward direction? If so, this is the second time we are visiting it.
            // False indicates that this edge is a back edge and the entirety of the edge should be pebbled in the backward direction
            // We do not need to include the target node in the backward reachable nodes since all source nodes will eventually pebble
            // the target during backward analysis and we want to minimize the number of nodes pebbled (and use percolation of pebbles as much as possible).
            //
            if (IsNodePebbledForward(currentNodeIndex)) return false;

            //
            // Pebble the current node as a forward node and percolate forward
            //
            vertices[currentNodeIndex].pebble = PebblerColorType.RED_FORWARD;

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].edges)
            {
                // Avoid backward (BLUE) edges which deduced a previously reached node
                if (currentEdge.pebbleColor != PebblerColorType.BLUE_BACKWARD)
                {
                    if (!currentEdge.IsFullyPebbled())
                    {
                        // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
                        Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

                        // Set the color of the edge (as a forward edge)
                        currentEdge.SetColor(PebblerColorType.RED_FORWARD);

                        // With this new node, check if the edge is full pebbled; if so, percolate
                        if (currentEdge.IsFullyPebbled())
                        {
                            // Percolate this node through the graph recursively; recursion will pebble the target node
                            if (ForwardTraversal(currentEdge.targetNode, backwardEdges, backwardReachableNodes))
                            {
                                // Success, we have a forward (RED) edge
                                // Construct a static set of pebbled hyperedges for other uses
                                forwardPebbledEdges.Put(currentEdge);
                            }
                            // forward traversal lead to a backward edge; mark this edge as a backward edge (BLUE)
                            else
                            {
                                currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);
                                backwardEdges.Add(currentEdge);
                                Utilities.AddUniqueList<int>(backwardReachableNodes, currentEdge.sourceNodes);
                            }
                        }
                    }
                }
            }

            return true;
        }

        //
        // Given a node, pebble the reachable parts of the graph (in the backward direction)
        //
        // In this algorithm, pebbling of a node may ONLY arise from pebbling an edge completely
        // That is, edge pebbling takes priority over node pebbling.
        private bool BackwardTraversal(int currentNodeIndex)
        {
            // Has this node already been pebbled backward (BLUE or PURPLE)? If so, return indicating a cycle
            if (IsNodePebbledBackward(currentNodeIndex))
            {
                return !backwardEdgeReachableNodes.Contains(currentNodeIndex);
            }

            // Has this node already been pebbled forward (RED); make it purple
            if (IsNodePebbledForward(currentNodeIndex))
            {
                vertices[currentNodeIndex].pebble = PebblerColorType.PURPLE_BOTH;
            }

            // If no pebble, upgrade to a backward (BLUE) node
            if (vertices[currentNodeIndex].pebble == PebblerColorType.NO_PEBBLE)
            {
                vertices[currentNodeIndex].pebble = PebblerColorType.BLUE_BACKWARD;
            }

            //
            // For each non-forward (non-RED) edge leaving this node, pebble accordingly
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].edges)
            {
                if (currentEdge.pebbleColor != PebblerColorType.BLACK_EDGE)
                {
                    //
                    // Handle first the case where we have an edge that was NOT fully pebbled from a forward analysis; this edge could turn into a backward edge.
                    //
                    if (currentEdge.pebbleColor == PebblerColorType.RED_FORWARD && !currentEdge.IsFullyPebbled())
                    {
                        // Clear the pebbles and change the color
                        currentEdge.sourcePebbles.Clear();
                        currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);
                    }

                    //
                    // Pebble the edge, if it is a candidate backward edge
                    //
                    if (currentEdge.pebbleColor != PebblerColorType.RED_FORWARD)
                    {
                        if (!currentEdge.IsFullyPebbled())
                        {
                            // Indicate the edge color
                            currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);

                            // Pebble this part of the edge with this node
                            Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

                            // Now, check to see if the target node is available to pebble: number of incoming vertices equates to the number of pebbles
                            if (currentEdge.IsFullyPebbled())
                            {
                                // Percolate this node through the graph recursively; recursion will pebble the target node
                                if (BackwardTraversal(currentEdge.targetNode))
                                {
                                    //
                                    // Success, we have a backward (BLUE) edge
                                    // Construct a static set of pebbled hyperedges for other uses
                                    //
                                    backwardPebbledEdges.Put(currentEdge);

                                    // Add the target of this edge as a reachable node (from an edge), not just a pebbling
                                    Utilities.AddUnique<int>(backwardEdgeReachableNodes, currentEdge.targetNode);
                                }
                                // check if backward traversal lead to a (cyclic) backward, back-edge; mark this edge as a bad (cyclic backward)
                                else if (backwardEdgeReachableNodes.Contains(currentEdge.targetNode))
                                {
                                    currentEdge.SetColor(PebblerColorType.BLACK_EDGE);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void DebugDumpClauses()
        {
            StringBuilder edgeStr = new StringBuilder();

            int numNonPebbledNodes = 0;
            int numRedNodes = 0;
            int numBlueNodes = 0;
            int numPurpleNodes = 0;

            int numNonPebbledEdges = 0;
            int numRedEdges = 0;
            int numBlueEdges = 0;
            int numPurpleEdges = 0;
            int numBlackEdges = 0;

            Debug.WriteLine("\n Vertices:");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Length; v++)
            {
                edgeStr.Append(v + ": ");
                switch (vertices[v].pebble)
                {
                    case Pebbler.PebblerColorType.NO_PEBBLE:
                        edgeStr.Append("NO PEBBLE");
                        numNonPebbledNodes++;
                        break;
                    case Pebbler.PebblerColorType.RED_FORWARD:
                        edgeStr.Append("RED");
                        numRedNodes++;
                        break;
                    case Pebbler.PebblerColorType.BLUE_BACKWARD:
                        edgeStr.Append("BLUE");
                        numBlueNodes++;
                        break;
                    case Pebbler.PebblerColorType.PURPLE_BOTH:
                        edgeStr.Append("PURPLE");
                        numPurpleNodes++;
                        break;
                }
                edgeStr.AppendLine("");
            }

            Debug.WriteLine("\nPebbled Edges:");
            for (int v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].edges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (PebblerHyperEdge edge in vertices[v].edges)
                    {
                        if (v == edge.sourceNodes.Min())
                        {
                            edgeStr.Append(" { ");

                            if (edge.IsFullyPebbled()) edgeStr.Append("+ ");
                            else edgeStr.Append("- ");
                            if (edge.pebbleColor == Pebbler.PebblerColorType.NO_PEBBLE)
                            {
                                edgeStr.Append("(N) ");
                                numNonPebbledEdges++;
                            }
                            if (edge.pebbleColor == Pebbler.PebblerColorType.RED_FORWARD)
                            {
                                edgeStr.Append("(R) ");
                                numRedEdges++;
                            }
                            if (edge.pebbleColor == Pebbler.PebblerColorType.BLUE_BACKWARD)
                            {
                                edgeStr.Append("(BL) ");
                                numBlueEdges++;
                            }
                            if (edge.pebbleColor == Pebbler.PebblerColorType.PURPLE_BOTH)
                            {
                                edgeStr.Append("(P) ");
                                numPurpleEdges++;
                            }
                            if (edge.pebbleColor == Pebbler.PebblerColorType.BLACK_EDGE)
                            {
                                edgeStr.Append("(BK) ");
                                numBlackEdges++;
                            }
                            foreach (int s in edge.sourceNodes)
                            {
                                edgeStr.Append(s + " ");
                            }
                            edgeStr.Append("} -> " + edge.targetNode + ", ");
                        }
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }

            edgeStr.AppendLine("Nodes: ");
            edgeStr.AppendLine("\tNot Pebbled:\t" + numNonPebbledNodes);
            edgeStr.AppendLine("\tRed:\t\t\t" + numRedNodes);
            edgeStr.AppendLine("\tBlue:\t\t\t" + numBlueNodes);
            edgeStr.AppendLine("\tPurple:\t\t\t" + numPurpleNodes);
            edgeStr.AppendLine("\tTotal:\t\t\t" + (numNonPebbledNodes + numRedNodes + numBlueNodes + numPurpleNodes));

            edgeStr.AppendLine("Edges: ");
            edgeStr.AppendLine("\tNot Pebbled:\t" + numNonPebbledEdges);
            edgeStr.AppendLine("\tRed:\t\t\t" + numRedEdges);
            edgeStr.AppendLine("\tBlue:\t\t\t" + numBlueEdges);
            edgeStr.AppendLine("\tPurple:\t\t\t" + numPurpleEdges);
            edgeStr.AppendLine("\tBlack:\t\t\t" + numBlackEdges);
            edgeStr.AppendLine("\tTotal:\t\t\t" + (numNonPebbledEdges + numRedEdges + numBlueEdges + numBlackEdges + numPurpleEdges));

            Debug.WriteLine(edgeStr);
        }
    }
}


        //private bool BackwardTraversal(int currentNodeIndex)
        //{
        //    // Has this node already been pebbled backward (BLUE or PURPLE)? If so, return indicating a cycle
        //    if (IsNodePebbledBackward(currentNodeIndex)) return false;

        //    // Has this node already been pebbled forward (RED); make it purple
        //    if (IsNodePebbledForward(currentNodeIndex))
        //    {
        //        vertices[currentNodeIndex].pebble = PebblerColorType.PURPLE_BOTH;
        //    }
        //    // If no pebble, upgrade to a backward (BLUE) node
        //    if (vertices[currentNodeIndex].pebble == PebblerColorType.NO_PEBBLE)
        //    {
        //        vertices[currentNodeIndex].pebble = PebblerColorType.BLUE_BACKWARD;
        //    }

        //    //
        //    // For each non-forward (non-RED) edge leaving this node, pebble accordingly
        //    //
        //    foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].edges)
        //    {
        //        if (currentEdge.pebbleColor != PebblerColorType.BLACK_EDGE)
        //        {
        //            //
        //            // Handle first the case where we have an edge that was NOT fully pebbled from a forward analysis; this edge could turn into a backward edge.
        //            //
        //            if (currentEdge.pebbleColor == PebblerColorType.RED_FORWARD && !currentEdge.IsFullyPebbled())
        //            {
        //                // Clear the pebbles and change the color
        //                currentEdge.sourcePebbles.Clear();
        //                currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);
        //            }

        //            //
        //            // Pebble the edge, if it is a candidate backward edge
        //            //
        //            if (currentEdge.pebbleColor != PebblerColorType.RED_FORWARD)
        //            {
        //                if (!currentEdge.IsFullyPebbled())
        //                {
        //                    // Indicate the edge color
        //                    currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);

        //                    // Pebble this part of the edge with this node
        //                    Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

        //                    // Now, check to see if the target node is available to pebble: number of incoming vertices equates to the number of pebbles
        //                    if (currentEdge.IsFullyPebbled())
        //                    {
        //                        // Percolate this node through the graph recursively; recursion will pebble the target node
        //                        if (BackwardTraversal(currentEdge.targetNode))
        //                        {
        //                            //
        //                            // Success, we have a backward (BLUE) edge
        //                            // Construct a static set of pebbled hyperedges for other uses
        //                            //
        //                            backwardPebbledEdges.Put(currentEdge);
        //                        }
        //                        // backward traversal lead to a (cyclic) backward, back-edge; mark this edge as a bad (cyclic backward)
        //                        else
        //                        {
        //                            currentEdge.SetColor(PebblerColorType.BLACK_EDGE);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}