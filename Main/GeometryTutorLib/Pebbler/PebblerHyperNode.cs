using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;

namespace GeometryTutorLib.Pebbler
{
    public class PebblerHyperNode<T>
    {
        public T data;

        public int id;

        public List<int> successorNodes;
        public List<int> predecessorNodes;

        public List<PebblerHyperEdge> successorEdges;
        public List<PebblerTransposeHyperEdge> predecessorEdges;

        public bool pebbled;

        public PebblerHyperNode(T thatData, int thatId)
        {
            id = thatId;
            data = thatData;
            pebbled = false;

            successorEdges = new List<PebblerHyperEdge>();
            predecessorEdges = new List<PebblerTransposeHyperEdge>();
        }

        public void AddSuccessorEdge(PebblerHyperEdge edge)
        {
            successorEdges.Add(edge);
        }

        public void AddSuccessorEdge(List<int> src, int target)
        {
            successorEdges.Add(new PebblerHyperEdge(src, target));
        }

        public void AddPredecessorEdge(int source, List<int> target)
        {
            predecessorEdges.Add(new PebblerTransposeHyperEdge(source, target));
        }

        public override string ToString()
        {
            string retS = data.ToString() + "\t\t\t\t= { ";

            retS += id + ", Pebbled(";
            retS += pebbled + "), ";
            retS += "SuccN={";
            foreach (int n in successorNodes) retS += n + ",";
            if (successorNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += "}, SuccE = { ";
            foreach (PebblerHyperEdge edge in successorEdges) { retS += edge.ToString() + ", "; }
            if (successorEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }, PredN={";
            foreach (int n in predecessorNodes) retS += n + ",";
            if (predecessorNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += " } }";

            return retS;
        }
    }
}
