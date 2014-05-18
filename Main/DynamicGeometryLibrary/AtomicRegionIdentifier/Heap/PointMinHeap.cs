using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.AtomicRegionIdentifier
{
    //
    // A classic Min-heap implementation done so with an array 
    //
    public class PointMinHeap
    {
        private HeapNode<Point>[] heap;
        public int Size { get; private set; }

        //
        // Creates the Min-heap array and places the smallest value possible in array position 0
        //
        public PointMinHeap(int sz)
        {
            heap = new HeapNode<Point>[sz + 5]; // +5 for safety
            Size = 0;

            // Make index 0 a sentinel value
            heap[0] = new HeapNode<Point>(new Point("", double.MinValue, double.MinValue), -1);
        }

        private int LeftChildIndex(int n) { return 2 * n; }
        private int ParentIndex(int n) { return n / 2; }
        private bool IsLeaf(int n) { return n > Size / 2 && n <= Size; }
        public bool IsEmpty() { return Size == 0; }

        //
        // Swaps two nodes (at the given indices) in the array
        //
        private void Swap(int index1, int index2)
        {
            // Swap the nodes
            HeapNode<Point> temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        //
        // Inserts an element (similar to Insertion; update the heap)
        // O(log n)
        //
        private void Insert(HeapNode<Point> node)
        {
            // Add the node to the last open position
            heap[++Size] = node;

            // Moves the value up the heap until a suitable point is determined
            int current = Size;
            while (Point.LexicographicOrdering(heap[current].data, heap[ParentIndex(current)].data) <= 0)
            {
                Swap(current, ParentIndex(current));
                current = ParentIndex(current);
            }
        }

        public void Add(Point pt, int gIndex)
        {
            Insert(new HeapNode<Point>(pt, gIndex));
        }

        //
        // Removes the node at the first position: O(log n) due to Heapify
        //
        private HeapNode<Point> ExtractTheMin()
        {
            // Switch the first (Min) with the last and re-Heapify
            Swap(1, Size);
            if (--Size != 0) Heapify(1);

            // Return the minimal value
            return heap[Size + 1];
        }

        public KeyValuePair<Point, int> PeekMin()
        {
            return new KeyValuePair<Point, int>(heap[1].data, heap[1].graphIndex);
        }

        //
        // public interace to acquire the minimum element
        //
        public KeyValuePair<Point, int> ExtractMin()
        {
            HeapNode<Point> node = ExtractTheMin();

            return new KeyValuePair<Point, int>(node.data, node.graphIndex);
        }

        //
        // Heapify based on the given index
        //
        private void Heapify(int index)
        {
            while (!IsLeaf(index))
            {
                int MinChild = LeftChildIndex(index);

                // If we're within the bounds of the valid data
                if (MinChild < Size)
                {
                    // left child is greater than the right child
                    if (Point.LexicographicOrdering(heap[MinChild].data, heap[MinChild + 1].data) >= 0) // <=
                    {
                        MinChild++;
                    }
                }

                if (Point.LexicographicOrdering(heap[index].data, heap[MinChild].data) == -1) return; // > 

                Swap(index, MinChild);

                index = MinChild;
            }
        }

        //
        // For debugging purposes: traverse the list and dump (key, data) pairs
        //
        public override String ToString()
        {
            String retS = "";

            // Traverse the array and dump the (key, data) pairs
            for (int i = 1; i <= Size; i++)
            {
                retS += "(" + i + ", " + heap[i].data + ") ";
            }

            return retS + "\n";
        }

    }
}
