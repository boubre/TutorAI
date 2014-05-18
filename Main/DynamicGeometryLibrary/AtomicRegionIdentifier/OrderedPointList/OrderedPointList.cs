using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.AtomicRegionIdentifier
{
    //
    // A lexicographic ordering of a list of points.
    //
    public class OrderedPointList
    {
        List<Node<Point>> ordered;

        //
        // Creates the Min-heap array and places the smallest value possible in array position 0
        //
        public OrderedPointList()
        {
            ordered = new List<Node<Point>>();
        }

        public bool IsEmpty() { return ordered.Count == 0; }

        //
        // Inserts an element
        //
        private void Insert(Node<Point> thatNode)
        {
            // Empty list: add to beginning.
            if (!ordered.Any())
            {
                ordered.Add(thatNode);
                return;
            }

            // General insertion
            int n;
            for (n = 0; n < ordered.Count; n++)
            {
                if (Point.LexicographicOrdering(thatNode.data, ordered[n].data) <= 0)
                {
                    break;
                }
            }
            ordered.Insert(n, thatNode);
        }

        public void Add(Point pt, int gIndex)
        {
            Insert(new Node<Point>(pt, gIndex));
        }

        //
        // Removes the node at the first position: O(log n) due to Heapify
        //
        private Node<Point> ExtractTheMin()
        {
            if (!ordered.Any()) return null;

            Node<Point> min = ordered[0];
            ordered.RemoveAt(0);
            return min;
        }

        public KeyValuePair<Point, int> PeekMin()
        {
            return new KeyValuePair<Point, int>(ordered[0].data, ordered[0].graphIndex);
        }

        //
        // public interace to acquire the minimum element
        //
        public KeyValuePair<Point, int> ExtractMin()
        {
            Node<Point> node = ExtractTheMin();

            return new KeyValuePair<Point, int>(node.data, node.graphIndex);
        }

        public void Remove(Point pt)
        {
            ordered.Remove(new Node<Point>(pt, -1));
        }

        //
        // For debugging purposes: traverse the list and dump (key, data) pairs
        //
        public override String ToString()
        {
            String retS = "";

            // Traverse the array and dump the (key, data) pairs
            for (int n = 0; n < ordered.Count; n++)
            {
                retS += "(" + n + ": " + ordered[n].data + ") ";
            }

            return retS + "\n";
        }

    }
}
