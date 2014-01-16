using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // This class represents a general hyperedge; both forward and backward edges in the hypergraph.
    //
    public class PebblerHyperEdge
    {
        public List<int> sourceNodes;
        public int targetNode;

        // Contains all source nodes that have been pebbled: for each source node,
        // there is a 'standard edge' that must be pebbled
        public List<int> sourcePebbles;
        public PebblerColorType pebbleColor;

        // Number of false atoms in a clause; in this case, init to number of source nodes
        //public int numNegArgs;

        public PebblerHyperEdge(List<int> src, int target)
        {
            sourceNodes = src;
            sourcePebbles = new List<int>(); // If empty, we assume all false (not pebbled)
            targetNode = target;
            //numNegArgs = src.Count;
            pebbleColor = PebblerColorType.NO_PEBBLE;
        }

        public bool IsEdgePebbledForward()
        {
            // An edge should NEVER be purple
            return pebbleColor == PebblerColorType.RED_FORWARD || pebbleColor == PebblerColorType.PURPLE_BOTH;
        }

        public bool IsEdgePebbledBackward()
        {
            // An edge should NEVER be purple
            return pebbleColor == PebblerColorType.BLUE_BACKWARD || pebbleColor == PebblerColorType.PURPLE_BOTH;
        }

        public bool IsFullyPebbled()
        {
            foreach (int srcNode in sourceNodes)
            {
                if (!sourcePebbles.Contains(srcNode)) return false;
            }

            return sourceNodes.Count == sourcePebbles.Count;
        }

        public void SetColor(PebblerColorType color)
        {
            if (color == PebblerColorType.PURPLE_BOTH)
            {
                throw new ArgumentException("Attempt to color an edge PURPLE; this is not possible.");
            }

            pebbleColor = color;
        }

        public bool HasNotBeenVisited()
        {
            return pebbleColor == PebblerColorType.NO_PEBBLE || !IsFullyPebbled();
        }

        // The source nodes and target must be the same for equality.
        public override bool Equals(object obj)
        {
            PebblerHyperEdge thatEdge = obj as PebblerHyperEdge;
            if (thatEdge == null) return false;
            foreach (int src in sourceNodes)
            {
                if (!thatEdge.sourceNodes.Contains(src)) return false;
            }
            return targetNode == thatEdge.targetNode;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            String retS = " { ";
            foreach (int node in sourceNodes)
            {
                retS += node + ", ";
            }
            if (sourceNodes.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } -> " + targetNode;
            return retS;
        }
    }
}
