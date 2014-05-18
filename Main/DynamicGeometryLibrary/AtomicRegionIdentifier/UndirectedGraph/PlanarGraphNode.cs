using System.Collections.Generic;

namespace LiveGeometry.AtomicRegionIdentifier.UndirectedGraph
{
    //
    // For atomic region identification
    //
    public enum NodePointType { REAL, EXTENDED };
    public enum EdgeType { REAL_ARC, REAL_SEGMENT, REAL_DUAL, EXTENDED_SEGMENT };

    public class PlanarGraphNode
    {
        //
        // This node information
        //
        public GeometryTutorLib.ConcreteAST.Point thePoint { get; private set; }
        public NodePointType type { get; private set; } // This node's type

        //
        // Parallel arrays with edge information
        //
        public List<int> neighbors { get; private set; }
        public List<double> costs { get; private set; }
        public List<EdgeType> neighborTypes { get; private set; } // Edge Types
        public List<int> degrees { get; private set; } // The number of connections between two nodes that are ARCS.
        public List<int> isCycle { get; private set; }

        public PlanarGraphNode(GeometryTutorLib.ConcreteAST.Point value, NodePointType t)
        {
            thePoint = value;
            type = t;
            neighbors = new List<int>();
            costs = new List<double>();
            neighborTypes = new List<EdgeType>();
            degrees = new List<int>();
            isCycle = new List<int>();
        }

        //
        // Shallow copy constructor
        //
        public PlanarGraphNode(PlanarGraphNode thatNode)
        {
            thePoint = thatNode.thePoint;
            type = thatNode.type;
            neighbors = new List<int>(thatNode.neighbors);
            costs = new List<double>(thatNode.costs);
            neighborTypes = new List<EdgeType>(thatNode.neighborTypes);
            degrees = new List<int>(thatNode.degrees);
            isCycle = new List<int>(thatNode.isCycle);
        }

        public void RemoveEdge(int toIndex)
        {
            neighbors.RemoveAt(toIndex);
            costs.RemoveAt(toIndex);
            neighborTypes.RemoveAt(toIndex);
            degrees.RemoveAt(toIndex);

            isCycle.Remove(toIndex);
        }

        public int NodeDegree()
        {
            return neighbors.Count;
        }

        public void Clear()
        {
            isCycle.Clear();
        }

        public override int GetHashCode() { return base.GetHashCode(); }
        //
        // Equality is only based on the point in the graph.
        //
        public override bool Equals(object obj)
        {
            PlanarGraphNode node = obj as PlanarGraphNode;
            if (node == null) return false;

            return this.thePoint.Equals(node.thePoint);
        }
    }
}
