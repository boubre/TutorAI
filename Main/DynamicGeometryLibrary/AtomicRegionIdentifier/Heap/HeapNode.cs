using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class HeapNode<T>
    {
        public T data;
        public HeapNode<T> child;
        public HeapNode<T> left;
        public HeapNode<T> parent;
        public HeapNode<T> right;
        public int graphIndex;

        public HeapNode(T data, int gIndex)
        {
            right = this;
            left = this;
            this.data = data;
            graphIndex = gIndex;
        }

        public override string ToString() { return data.ToString() + ", " + graphIndex; }
    }
}
