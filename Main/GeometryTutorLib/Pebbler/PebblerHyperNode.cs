using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;

namespace GeometryTutorLib.Pebbler
{
    public class PebblerHyperNode<T, A>
    {
        public T data; // Original Hypergraph representation
        public int id; // index of original hypergraph node

        public List<int> nodes;
        public List<PebblerHyperEdge<A>> edges;

        // Coloration of the edge when pebbled
        public PebblerColorType pebble;

        // Whether the node has been pebbled or not.
        public bool pebbled;

        public PebblerHyperNode(T thatData, int thatId)
        {
            id = thatId;
            data = thatData;
            pebble = PebblerColorType.NO_PEBBLE;

            edges = new List<PebblerHyperEdge<A>>();
        }

        public void AddEdge(PebblerHyperEdge<A> edge)
        {
            edges.Add(edge);
        }

        public void AddEdge(A annotation, List<int> src, int target)
        {
            edges.Add(new PebblerHyperEdge<A>(src, target, annotation));
        }

        public override string ToString()
        {
            string retS = data.ToString() + "\t\t\t\t= { ";

            retS += id + ", Pebbled(";
            if (pebble == PebblerColorType.NO_PEBBLE) retS += "NONE";
            if (pebble == PebblerColorType.RED_FORWARD) retS += "RED";
            if (pebble == PebblerColorType.BLUE_BACKWARD) retS += "BLUE";
            //if (pebble == PebblerColorType.PURPLE_BOTH) retS += "PURPLE";
            retS += "), ";
            retS += "SuccN={";
            foreach (int n in nodes) retS += n + ",";
            if (nodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += "}, SuccE = { ";
            foreach (PebblerHyperEdge<A> edge in edges) { retS += edge.ToString() + ", "; }
            if (edges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }";

            return retS;
        }
    }
}
