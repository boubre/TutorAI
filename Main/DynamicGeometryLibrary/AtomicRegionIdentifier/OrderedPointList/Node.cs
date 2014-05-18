using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class Node<T>
    {
        public T data;
        public int graphIndex;

        public Node(T data, int gIndex)
        {
            this.data = data;
            graphIndex = gIndex;
        }

        public override bool Equals(object obj)
        {
            Node<T> thatObj = obj as Node<T>;
            if (thatObj == null) return false;

            return this.data.Equals(thatObj.data);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "< " + data.ToString() + ", " + graphIndex + " >"; }
    }
}
