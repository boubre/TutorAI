using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Hypergraph
{
    public class HyperEdge<A>
    {
        // Allows us to note how the edge was derived
        public A annotation;

        public List<int> sourceNodes;
        public List<int> pebbles; // Contains all source nodes that have been pebbled: for each source node, there is a 'standard edge' that must be pebbled

        public int targetNode;
        public bool visited;
        public int numNegArgs; // Number of false atoms in a clause; in this case, init to number of source nodes

        public HyperEdge(List<int> src, int target, A annot)
        {
            sourceNodes = src;
            pebbles = new List<int>(); // If empty, we assume all false (not pebbled)
            targetNode = target;
            visited = false;
            numNegArgs = src.Count;
            annotation = annot;
        }

        // The source nodes and target must be the same for equality.
        public override bool Equals(object obj)
        {
            HyperEdge<A> thatEdge = obj as HyperEdge<A>;
            if (thatEdge == null) return false;
            foreach (int src in sourceNodes)
            {
                if (!thatEdge.sourceNodes.Contains(src)) return false;
            }
            return targetNode == thatEdge.targetNode;
        }

        //
        // This is an equals method by only providing the many-to-one mapping that defines an edge.
        //
        public bool DefinesEdge(List<int> antecedent, int consequent)
        {
            foreach (int ante in antecedent)
            {
                if (!sourceNodes.Contains(ante)) return false;
            }

            return targetNode == consequent;
        }

        public override string ToString()
        {
            String retS = " { ";
            foreach (int node in sourceNodes)
            {
                retS += node + ",";
            }
            if (sourceNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += " } -> " + targetNode;
            return retS;
        }
    }
}
