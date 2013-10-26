using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.Hypergraph
{
    public class HyperNode<T, A>
    {
        public T data;

        public int id;

        public List<int> successorNodes;
        public List<int> predecessorNodes;

        public List<HyperEdge<A>> successorEdges;
        public List<TransposeHyperEdge<A>> predecessorEdges;

        public void AddSuccessorEdge(HyperEdge<A> edge)
        {
            successorEdges.Add(edge);
        }

        public void AddPredecessorEdge(TransposeHyperEdge<A> tEdge)
        {
            predecessorEdges.Add(tEdge);
        }

        public bool pebbled;

        // For path creation; represents the predecessor set of nodes
        public List<int> pi;

        public HyperNode(T d, int i)
        {
            id = i;
            data = d;
            pebbled = false;
           
            successorNodes = new List<int>();
            successorEdges = new List<HyperEdge<A>>();
            predecessorNodes = new List<int>();
            predecessorEdges = new List<TransposeHyperEdge<A>>();

            pi = new List<int>();
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
            foreach (HyperEdge<A> edge in successorEdges) { retS += edge.ToString() + ", "; }
            if (successorEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }, PredN={";
            foreach (int n in predecessorNodes) retS += n + ",";
            if (predecessorNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += "}, PredE = { ";
            //foreach (TransposeHyperEdge edge in predecessorEdges) { retS += edge.ToString() + ", "; }
            //retS += "}, PI = { ";
            retS += "}, PI = { ";
            foreach (int p in pi) { retS += p + ", "; }
            if (pi.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }";

            return retS;
        }
    }
}
