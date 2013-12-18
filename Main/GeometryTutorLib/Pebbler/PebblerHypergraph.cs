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

        public PebblerHypergraph(PebblerHyperNode<T>[] inputVertices)
        {
            graph = null; // This must be set outside to use
            vertices = inputVertices;
            forwardPebbledEdges = new HyperEdgeMultiMap(vertices.Length);
            backwardPebbledEdges = new HyperEdgeMultiMap(vertices.Length);
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
            List<int> backInformation = PebbleForward(figure, givens, axiomaticNodes);

            Debug.WriteLine("Before backward Pebbling: ");
            DebugDumpClauses();

            // Backward pebble
            PebbleBackward(figure, axiomaticNodes, backInformation);

            Debug.WriteLine("After backward Pebbling: ");
            DebugDumpClauses();
        }
        
        // Returns the set of backward edges and backward reachable nodes
        private List<int> PebbleForward(List<int> figure, List<int> givens, List<int> axiomaticNodes)
        {
            // The list of edges which are blue from the FORWARD analysis
            List<int> backwardReachableNodes = new List<int>();

            //
            // Pebble all axiomatic nodes.
            //
            foreach (int axiomatic in axiomaticNodes)
            {
                Debug.WriteLine("Forward Pebbling Axiomatic: " + axiomatic);
                ForwardNodeTraversal(axiomatic, backwardReachableNodes);
            }

            //
            // Pebble all figure vertices
            //
            foreach (int fNode in figure)
            {
                Debug.WriteLine("Pebbling Figure Node: " + fNode);
                ForwardNodeTraversal(fNode, backwardReachableNodes);
            }

            //
            // Pebble all given vertices
            //
            foreach (int g in givens)
            {
                Debug.WriteLine("Pebbling Given Node: " + g);
                ForwardNodeTraversal(g, backwardReachableNodes);
            }

            return backwardReachableNodes;
        }

        private void PebbleBackward(List<int> figure, List<int> axiomaticNodes, List<int> backwardNodes)
        {
            //
            // Clear all the extracted backward (BLUE) edges determined during forward traversal.
            // This facilitates clean pebbling using the backward nodes as starting points.
            //
            //foreach (PebblerHyperEdge edge in backwardEdges)
            //{
            //    edge.SetColor(PebblerColorType.NO_PEBBLE);
            //    edge.sourcePebbles.Clear();
            //}

            //DebugDumpClauses();

            //
            // Pebble all Figure nodes
            //
            // Should be sorted already
            //figure.Sort();
            //for (int f = figure.Count - 1; f >= 0; f--)
            //{
            //    Debug.WriteLine("Backward Pebbling Figure: " + figure[f]);
            //    BackwardNodeTraversal(figure[f]);
            //}

            List<int> axAndBackwardNodes = new List<int>();
            axAndBackwardNodes.AddRange(axiomaticNodes);
            Utilities.AddUniqueList<int>(axAndBackwardNodes, axiomaticNodes);
            Utilities.AddUniqueList<int>(axAndBackwardNodes, backwardNodes);
            axAndBackwardNodes.Sort();

            for (int n = axAndBackwardNodes.Count - 1; n >= 0; n--)
            {
                BackwardNodeTraversal(axAndBackwardNodes[n]);
            }

            ////
            //// Pebble all axiomatic nodes
            ////
            //axiomaticNodes.Sort();
            //for (int a = axiomaticNodes.Count - 1; a >= 0; a--)
            //{
            //    Debug.WriteLine("Backward Pebbling Axiomatic: " + axiomaticNodes[a]);
            //    BackwardTraversal(axiomaticNodes[a]);
            //}



            //// We do not pebble any given nodes since we are trying to deduce those nodes

            ////
            //// Pebble the backward direction starting at any purple node
            ////
            //// Sort the nodes and pebble in descending order; otherwise, we may acquire more forward-type (uninteresting) edges
            //backwardNodes.Sort();
            //for (int b = backwardNodes.Count - 1; b >= 0; b--)
            //{
            //    BackwardTraversal(backwardNodes[b]);
            //}
        }

        //
        // Given a node, pebble the reachable parts of the graph (in the forward direction)
        //
        // true means a normal node percolation
        // false indicates a backedge 
        //
        private bool ForwardNodeTraversal(int currentNodeIndex, List<int> backwardReachableNodes)
        {
            //
            // Change the color of the node based on previously traversal
            //
            switch(vertices[currentNodeIndex].pebble)
            {
                // Unvisited, mark RED
                case PebblerColorType.NO_PEBBLE:
                    vertices[currentNodeIndex].pebble = PebblerColorType.RED_FORWARD;
                    break;

                // Already visited before, turn it PURPLE and return
                case PebblerColorType.RED_FORWARD:
                    vertices[currentNodeIndex].pebble = PebblerColorType.PURPLE_BOTH;
                    Utilities.AddUnique<int>(backwardReachableNodes, currentNodeIndex);
                    return false;

                // No node should be blue in a forward analysis 
                case PebblerColorType.BLUE_BACKWARD:
                // We have already visited this node twice.
                case PebblerColorType.PURPLE_BOTH:
                    return false;

                case PebblerColorType.BLACK_EDGE:
                    throw new ArgumentException("A node should never be labeled black, specifically during a forward traversal.");
            }

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].edges)
            {
                ForwardEdgeTraversal(currentNodeIndex, currentEdge, backwardReachableNodes);
            }

            return true;
        }

        // Helper function for forward traversal that handles one edge at a time
        private void ForwardEdgeTraversal(int currentNodeIndex, PebblerHyperEdge currentEdge, List<int> backwardReachableNodes)
        {
            //
            // Based on current edge color, 
            //
            switch (currentEdge.pebbleColor)
            {
                // Avoid backward (BLUE) edges which deduced a previously reached node;
                // Note: any blue edge was a fully pebbled edge
                case PebblerColorType.BLUE_BACKWARD:
                    return;

                case PebblerColorType.BLACK_EDGE:
                case PebblerColorType.PURPLE_BOTH:
                    throw new ArgumentException("An edge should not be BLACK or PURPLE");
            }

            // If the node is already completely pebbled, there is no need to visit it again.
            if (currentEdge.IsFullyPebbled()) return;

            // Pebble the edge using this node
            Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

            // If unpebbled, we change the color to forward (RED)
            if (currentEdge.pebbleColor == PebblerColorType.NO_PEBBLE) currentEdge.SetColor(PebblerColorType.RED_FORWARD);

            // With this new node, check if the edge is full pebbled; if so, percolate; if not, leave
            if (!currentEdge.IsFullyPebbled()) return;

            // Percolate this node through the graph recursively; recursion will pebble the target node
            if (ForwardNodeTraversal(currentEdge.targetNode, backwardReachableNodes))
            {
                // Success, we have a forward (RED) edge
                // Construct a static set of pebbled hyperedges for other uses
                forwardPebbledEdges.Put(currentEdge);
            }
            // forward traversal lead to a backward edge; mark this edge as a backward edge (BLUE)
            else
            {
                currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);
                backwardPebbledEdges.Put(currentEdge);
            }
        }

        //
        // Given a node, pebble the reachable parts of the graph (in the forward direction)
        //
        // true means a normal node percolation
        // false indicates a backedge 
        //
        private bool BackwardNodeTraversal(int currentNodeIndex)
        {
            //
            // Change the color of the node based on previous traversal
            //
            switch (vertices[currentNodeIndex].pebble)
            {
                // Unvisited, mark BLUE
                case PebblerColorType.NO_PEBBLE:
                    vertices[currentNodeIndex].pebble = PebblerColorType.BLUE_BACKWARD;
                    break;

                // Already visited as a forward, turn it PURPLE
                case PebblerColorType.RED_FORWARD:
                    vertices[currentNodeIndex].pebble = PebblerColorType.PURPLE_BOTH;
                    break;

                // We have already visited these nodes in the backward direction
                case PebblerColorType.BLUE_BACKWARD:
                case PebblerColorType.PURPLE_BOTH:
                    return false;

                case PebblerColorType.BLACK_EDGE:
                    throw new ArgumentException("A node should never be labeled black.");
            }

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].edges)
            {
                BackwardEdgeTraversal(currentNodeIndex, currentEdge);
            }

            return true;
        }

        // Helper function for forward traversal that handles one edge at a time
        private void BackwardEdgeTraversal(int currentNodeIndex, PebblerHyperEdge currentEdge)
        {
            //
            // Based on current edge color, 
            //
            switch (currentEdge.pebbleColor)
            {
                case PebblerColorType.RED_FORWARD:
                    return;

                case PebblerColorType.BLACK_EDGE:
                case PebblerColorType.PURPLE_BOTH:
                    throw new ArgumentException("An edge should not be BLACK or PURPLE");
            }

            // If the node is already completely pebbled, there is no need to visit it again.
            if (currentEdge.IsFullyPebbled()) return;

            // Pebble the edge using this node
            Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

            // If unpebbled, we change the color to backward (BLUE)
            if (currentEdge.pebbleColor == PebblerColorType.NO_PEBBLE) currentEdge.SetColor(PebblerColorType.BLUE_BACKWARD);

            // With this new node, check if the edge is full pebbled; if so, percolate; if not, leave
            if (!currentEdge.IsFullyPebbled()) return;

            // Percolate this node through the graph recursively; recursion will pebble the target node
            if (BackwardNodeTraversal(currentEdge.targetNode))
            {
                // Success, we have a forward (BLUE) edge
                // Construct a static set of pebbled hyperedges for other uses
                backwardPebbledEdges.Put(currentEdge);
            }
        }




        ////
        //// Given a node, pebble the reachable parts of the graph (in the backward direction)
        ////
        //private bool BackwardNodeTraversal(int currentNodeIndex)
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