using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    public class PebblerHyperEdge
    {
        public List<int> sourceNodes;
        public int targetNode;

        // Contains all source nodes that have been pebbled: for each source node,
        // there is a 'standard edge' that must be pebbled
        public List<int> sourcePebbles;

        public bool visited;

        // Number of false atoms in a clause; in this case, init to number of source nodes
        public int numNegArgs;

        public PebblerHyperEdge(List<int> src, int target)
        {
            sourceNodes = src;
            sourcePebbles = new List<int>(); // If empty, we assume all false (not pebbled)
            targetNode = target;
            visited = false;
            numNegArgs = src.Count;
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
