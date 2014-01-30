using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    public class HyperNode<T, A>
    {
        public T data;
        public int id;

        public List<HyperEdge<A>> forwardEdges;
        public List<HyperEdge<A>> backwardEdges;

        public void AddForwardEdge(HyperEdge<A> edge)
        {
            forwardEdges.Add(edge);
        }

        public void AddBackwardEdge(HyperEdge<A> edge)
        {
            backwardEdges.Add(edge);
        }


        public HyperNode(T d, int i)
        {
            id = i;
            data = d;
           
            forwardEdges = new List<HyperEdge<A>>();
            backwardEdges = new List<HyperEdge<A>>();
        }

        // Creating a shallow copy of this node as a pebbler node
        public PebblerHyperNode<T, A> CreatePebblerNode()
        {
            return new PebblerHyperNode<T, A>(data, id);
        }

        public override string ToString()
        {
            string retS = data.ToString() + "\t\t\t\t= { ";

            retS += id + "SuccE = { ";
            foreach (HyperEdge<A> edge in forwardEdges) { retS += edge.ToString() + ", "; }
            if (forwardEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }, BackE={";
            foreach (HyperEdge<A> edge in backwardEdges) { retS += edge.ToString() + ", "; }
            if (backwardEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }";

            return retS;
        }
    }
}
