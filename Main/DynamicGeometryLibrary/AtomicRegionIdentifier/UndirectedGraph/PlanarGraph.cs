using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.AtomicRegionIdentifier.UndirectedGraph
{
    public class PlanarGraph
    {
        public List<PlanarGraphNode> nodes { get; private set; }

        public PlanarGraph()
        {
            nodes = new List<PlanarGraphNode>();
        }

        //
        // Shallow copy constructor
        //
        public PlanarGraph(PlanarGraph thatG) : this()
        {
            foreach (PlanarGraphNode node in thatG.nodes)
            {
                nodes.Add(new PlanarGraphNode(node));
            }
        }

        public void AddNode(Point value, NodePointType type)
        {
            AddNode(new PlanarGraphNode(value, type));
        }

        private void AddNode(PlanarGraphNode node)
        {
            nodes.Add(node);
        }

        //
        // Determine the new, updated edge type.
        //    public enum EdgeType { REAL_ARC, REAL_SEGMENT, REAL_DUAL, EXTENDED_SEGMENT };
        private EdgeType UpdateEdge(EdgeType oldType, EdgeType newType)
        {
            if (oldType == EdgeType.REAL_SEGMENT && newType == EdgeType.REAL_SEGMENT)
            {
                throw new ArgumentException("Cannot have two edges defined by a real segment.");
            }

            if (oldType == EdgeType.EXTENDED_SEGMENT || newType == EdgeType.EXTENDED_SEGMENT)
            {
                throw new ArgumentException("Cannot change an edge to / from an extended segment type.");
            }

            if (newType == EdgeType.REAL_DUAL)
            {
                throw new ArgumentException("Cannot change an edge to be dual.");
            }

            // DUAL + ARC / SEGMENT = DUAL
            if (oldType == EdgeType.REAL_DUAL) return EdgeType.REAL_DUAL;

            // SEGMENT + ARC = DUAL
            if (oldType == EdgeType.REAL_SEGMENT && newType == EdgeType.REAL_ARC) return EdgeType.REAL_DUAL;

            // ARC + SEGMENT = DUAL
            if (oldType == EdgeType.REAL_ARC && newType == EdgeType.REAL_SEGMENT) return EdgeType.REAL_DUAL;

            // ARC + ARC = ARC
            if (oldType == EdgeType.REAL_ARC && newType == EdgeType.REAL_ARC) return EdgeType.REAL_ARC;

            // default should not be reached.
            return EdgeType.REAL_DUAL;
        }

        public void AddUndirectedEdge(Point from, Point to, double cost, EdgeType eType)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from, NodePointType.REAL)); // REAL is arbitrary here.
            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to, NodePointType.REAL)); // REAL is arbitrary here.

            if (fromNodeIndex == -1 || toNodeIndex == -1)
            {
                throw new ArgumentException("Edge uses undefined nodes: " + from + " " + to);
            }

            //
            // Check if the edge already exists
            //
            if (nodes[fromNodeIndex].neighbors.Contains(toNodeIndex))
            {
                nodes[fromNodeIndex].neighborTypes[toNodeIndex] = UpdateEdge(nodes[fromNodeIndex].neighborTypes[toNodeIndex], eType);
                nodes[toNodeIndex].neighborTypes[fromNodeIndex] = nodes[fromNodeIndex].neighborTypes[toNodeIndex];

                // Increment the degree if it is an arc.
                if (eType == EdgeType.REAL_ARC)
                {
                    nodes[fromNodeIndex].degrees[toNodeIndex]++;
                    nodes[toNodeIndex].degrees[fromNodeIndex]++;
                }
            }
            //
            // The edge does not exist.
            //
            else
            {
                nodes[fromNodeIndex].neighbors.Add(toNodeIndex);
                nodes[fromNodeIndex].costs.Add(cost);
                nodes[fromNodeIndex].degrees.Add(eType == EdgeType.REAL_ARC ? 1 : 0);
                nodes[fromNodeIndex].neighborTypes.Add(eType);

                nodes[toNodeIndex].neighbors.Add(fromNodeIndex);
                nodes[toNodeIndex].costs.Add(cost);
                nodes[toNodeIndex].degrees.Add(eType == EdgeType.REAL_ARC ? 1 : 0);
                nodes[toNodeIndex].neighborTypes.Add(eType);
            }
        }

        public bool Contains(Point value)
        {
            return nodes.Contains(new PlanarGraphNode(value, NodePointType.REAL));
        }

        public bool RemoveNode(Point value)
        {
            int thisIndex = nodes.IndexOf(new PlanarGraphNode(value, NodePointType.REAL)); // REAL is arbitrary here.

            // node wasn't found
            if (thisIndex == -1) return false;

            // Remove the node from the node list
            nodes.RemoveAt(thisIndex);

            // enumerate through each node in the nodes, removing edges to this node
            foreach (PlanarGraphNode node in nodes)
            {
                int toEdgeIndex = node.neighbors.IndexOf(thisIndex);
                if (toEdgeIndex != -1)
                {
                    node.RemoveEdge(toEdgeIndex);
                }
            }

            return true;
        }

        public bool RemoveEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from, NodePointType.REAL)); // REAL is arbitrary here.
            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to, NodePointType.REAL)); // REAL is arbitrary here.

            // node wasn't found
            if (fromNodeIndex == -1 || toNodeIndex == -1) return false;

            //
            // Do not remove the edge, just indicate it is removed (both since undirected).
            //
            int fromEdgeIndex = nodes[fromNodeIndex].neighbors.IndexOf(toNodeIndex);
            if (fromEdgeIndex != -1) nodes[fromNodeIndex].RemoveEdge(fromEdgeIndex);

            int toEdgeIndex = nodes[toNodeIndex].neighbors.IndexOf(fromNodeIndex);
            if (toEdgeIndex != -1) nodes[toNodeIndex].RemoveEdge(toEdgeIndex);

            return true;
        }

        public void MarkCycleEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from, NodePointType.REAL)); // REAL is arbitrary here.
            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to, NodePointType.REAL)); // REAL is arbitrary here.

            // node wasn't found
            if (fromNodeIndex == -1 || toNodeIndex == -1) return;

            //
            // Mark the edge as being involved in a cycle.
            //
            int fromEdgeIndex = nodes[fromNodeIndex].neighbors.IndexOf(toNodeIndex);
            if (fromEdgeIndex != -1) nodes[fromNodeIndex].isCycle.Add(fromEdgeIndex);

            int toEdgeIndex = nodes[toNodeIndex].neighbors.IndexOf(fromNodeIndex);
            if (toEdgeIndex != -1) nodes[toNodeIndex].isCycle.Add(toEdgeIndex);
        }

        public bool IsCycleEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from, NodePointType.REAL)); // REAL is arbitrary here.
            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to, NodePointType.REAL)); // REAL is arbitrary here.

            // node wasn't found
            if (fromNodeIndex == -1 || toNodeIndex == -1) return false;

            // Look at this specific edge
            int fromEdgeIndex = nodes[fromNodeIndex].neighbors.IndexOf(toNodeIndex);

            return nodes[fromNodeIndex].isCycle.Contains(fromEdgeIndex);
        }

        //
        // Unmark any marked nodes and edges
        //
        public void Reset()
        {
            foreach (PlanarGraphNode node in nodes)
            {
                node.Clear();
            }
        }

        public int Count
        {
            get { return nodes.Count; }
        }
    }
}
