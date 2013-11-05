using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // A reduced version of the original hypergraph that provides simple pebbling and exploration
    //
    public class PebblerHypergraph<T>
    {
        private readonly static int UNMARKED_NODE = -1;

        // The main graph data structure
        public PebblerHyperNode<T>[] vertices { get; private set; }

        //private PathGenerator pathGenerator;

        private SharedPebbledNodeList sharedPebbleWorklist;

        // This list must be set before pebbling, otherwise it is empty.
        List<int> pebblingSourceNodes = new List<int>();

        public PebblerHypergraph(PebblerHyperNode<T>[] inputVertices)
        {
            vertices = inputVertices;
            //pathGenerator = new PathGenerator(vertices.Length);
            pebblingSourceNodes = new List<int>();
            sharedPebbleWorklist = null;
        }

        public int NumVertices() { return vertices.Length; }

        // Set the source nodes for pebbling purposes
        public void SetSourceNodes(List<int> src)
        {
            pebblingSourceNodes = new List<int>(src);
        }

        public void SetSharedList(SharedPebbledNodeList sharedData)
        {
            sharedPebbleWorklist = sharedData;
        }

        //
        // Pebble the graph from all the sources
        //
        public void GenerateAllPaths()
        {
            Pebble(pebblingSourceNodes);
        }

        //
        // Use Dowling-Gallier pebbling technique to pebble using all given nodes
        //
        private void Pebble(List<int> src)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // Pebble all start vertices
            foreach (int s in src)
            {
                Traverse(s);
            }

            sharedPebbleWorklist.SetWritingComplete();

            // Stop timing
            stopwatch.Stop();

            Debug.WriteLine("Vertices after pebbling:");
            for (int i = 0; i < vertices.Length; i++)
            {
                Debug.WriteLine(vertices[i].id + ": pebbled(" + vertices[i].pebbled + ")");
            }

            TimeSpan ts = stopwatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                               ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Debug.WriteLine("Length of time to compute all paths: " + elapsedTime);
        }

        //
        // Given a node, pebble the reachable parts of the graph
        //
        private void Traverse(int currentNodeIndex)
        {
            // Has this node already been pebbled?
            if (vertices[currentNodeIndex].pebbled) return;

            // Pebble the node currentnode
            vertices[currentNodeIndex].pebbled = true;

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (PebblerHyperEdge currentEdge in vertices[currentNodeIndex].successorEdges)
            {
                // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
                Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

                // Now, check to see if the target node if available to pebble: number of incoming vertices equates to the number of pebbles
                if (currentEdge.sourceNodes.Count == currentEdge.sourcePebbles.Count)
                {
                    // Percolate this node through the graph recursively; recursion will pebble the target node
                    Traverse(currentEdge.targetNode);

                    // Add to the path generator the fact that we have a successor that has been pebbled.
                    sharedPebbleWorklist.WriteEdge(currentEdge);
                }
            }
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("All Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].predecessorEdges.Any())
                {
                    edgeStr = new StringBuilder();
                    edgeStr.Append("{ ");
                    foreach (int s in vertices[v].predecessorEdges[0].targetNodes)
                    {
                        edgeStr.Append(s + " ");
                    }
                    edgeStr.Remove(edgeStr.Length - 1, 1);
                    edgeStr.Append(" }");
                }

                Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
            }


            Debug.WriteLine("\nEdges: ");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].successorEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (PebblerHyperEdge edge in vertices[v].successorEdges)
                    {
                        edgeStr.Append(" { ");
                        foreach (int s in edge.sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Append("} -> " + edge.targetNode + ", ");
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }
            Debug.WriteLine(edgeStr);
        }

        //
        // Find n interesting goal vertices in the hypergraph
        //
        public MaxHeap FindInterestingGoalNodes()
        {
            // Order the interesting vertices by a scored (numeric) criteria defined by each class
            MaxHeap interestingGoals = new MaxHeap(vertices.Length);

            for (int n = 0, interestingNess = 0; n < vertices.Length; n++, interestingNess = 0)
            {
                // Equations are generally not interesting' congruences are;
                // Or, is this a source node (having no predecessors)?
                if (vertices[n].data is GeometryTutorLib.ConcreteAbstractSyntax.ArithmeticNode || !vertices[n].predecessorEdges.Any())
                {
                    interestingNess = 0;
                }
                else
                {
                    // If this node produces many things and is not a source node
                    if (vertices[n].predecessorEdges.Count > 1)
                    {
                        interestingNess++;
                    }

                    // If this node requires many things to prove, it is interesting
                    if (vertices[n].predecessorEdges[0].targetNodes.Count > 1)
                    {
                        interestingNess += (int)Math.Floor(vertices[n].predecessorEdges[0].targetNodes.Count / 2);
                    }
                }

                interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
            }

            return interestingGoals;
        }
    }
}