using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // Implements a basic directional graph (with no node information)
    //
    public class DiGraph<T>
    {
        //
        // To implement Tajan's Strongly Connected Components (and cycles in the graph)
        //
        // Each node v is assigned a unique integer v.index, which numbers the nodes consecutively in the order
        // in which they are discovered. It also maintains a value v.lowlink that represents (roughly speaking)
        // the smallest index of any node known to be reachable from v, including v itself. Therefore v must be
        // left on the stack if v.lowlink < v.index, whereas v must be removed as the root of a strongly connected
        // component if v.lowlink == v.index. The value v.lowlink is computed during the depth-first search from v,
        // as this finds the nodes that are reachable from v.
        //
        protected class Vertex
        {
            public T node;
            public int lowLink;
            public int index;

            public Vertex(T n)
            {
                node = n;
                lowLink = -1;
                index = -1;
            }

            public Vertex(Vertex v)
            {
                node = v.node;
                lowLink = -1;
                index = -1;
            }

            public override bool Equals(object obj) { return this.node.Equals((obj as Vertex).node); }
            public override int GetHashCode() { return base.GetHashCode(); }
        }

        //
        // The dictionary is a map from: a node index to node indices
        //
        protected Dictionary<int, List<int>> edgeMap;
        protected Dictionary<int, List<int>> transposeEdgeMap;
        protected int numEdges;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> sccs; // Strongly Connected Components

        public DiGraph()
        {
            edgeMap = new Dictionary<int, List<int>>();
            transposeEdgeMap = new Dictionary<int, List<int>>();
            numEdges = 0;
            vertices = new List<Vertex>();
            sccs = new List<List<Vertex>>();
        }

        //
        // Make a shallow copy of this graph (all vertices and edges)
        //
        public DiGraph(DiGraph<T> thatGraph)
        {
            edgeMap = new Dictionary<int, List<int>>();
            transposeEdgeMap = new Dictionary<int, List<int>>();
            numEdges = thatGraph.numEdges;
            vertices = new List<Vertex>();

            // This copy preserves the index values for the vertices
            thatGraph.vertices.ForEach(thatVertex => vertices.Add(new Vertex(thatVertex)));

            // Copy the integer indices
            foreach (KeyValuePair<int, List<int>> pair in thatGraph.edgeMap)
            {
                edgeMap.Add(pair.Key, new List<int>(pair.Value));
            }

            // Copy the integer indices
            foreach (KeyValuePair<int, List<int>> pair in thatGraph.transposeEdgeMap)
            {
                transposeEdgeMap.Add(pair.Key, new List<int>(pair.Value));
            }

            sccs = GetStronglyConnectedComponents();
        }

        //
        // Simple heuristic for which we may use graph minors to acquire isomorphisms
        //
        public int NumEdges()
        {
            return numEdges;
        }

        //
        // The depth of the graph is defined as being the length of the maximal path to the leaf nodes
        // We assume that this graph is a DAG.
        //
        // We use a DFS technique to determine maximality
        //
        // Since we have an implied 1-1 map between vertices and indices, we work on indices
        //
        public int GetLength()
        {
            // passing index 0, depth 1
            return GetLengthHelper(0, 1);
        }
        private int GetLengthHelper(int currentNodeIndex, int currentDepth)
        {
            int maxDepth = -1;

            // Get the edges from this node to traverse
            List<int> backwardEdges;

            // Get the transpose edges from this node (as we are traversing backward from the goal)
            // This node is a leaf if no edges: return the known depth
            if (!transposeEdgeMap.TryGetValue(currentNodeIndex, out backwardEdges)) return currentDepth;

            // Traverse the edges tracking the max depth
            foreach (int edge in backwardEdges)
            {
                int tempDepth = GetLengthHelper(edge, currentDepth + 1);
                if (maxDepth < tempDepth) maxDepth = tempDepth;
            }

            return maxDepth;
        }

        //
        // General graph traversal assuming a DAG; we start at the goal node and walk the transpose edges
        //
        // Since this is a DAG, width is defined as the spread of the graph (like a tree) 
        // Use a BFS technique where we continually update the levelWidth variable to ensure we know where the next level starts
        //
        // Since we have an implied 1-1 map between vertices and indices, we work on indices
        //
        public int GetWidth()
        {
            Queue<int> worklist = new Queue<int>();

            // Add the 'goal' node index as a catalyst
            worklist.Enqueue(0);

            // width for this level
            int currentLevelWidth = 1;

            // max width for the entire graph
            int maxLevelWidth = 1;

            // For verification purposes, we track the total number of nodes visited
            int sumOfAllWidths = 0;

            //
            // Traverse the entire graph
            //
            while (worklist.Any())
            { 
                // Traverse an entire level
                int currentLevelAccumulator = 0;
                for (int ell = 0; ell < currentLevelWidth; ell++)
                {
                    // Get the next node
                    int currentNodeIndex = worklist.Dequeue();

                    // Get the edges from this node to traverse
                    List<int> backwardEdges;
                    if (transposeEdgeMap.TryGetValue(currentNodeIndex, out backwardEdges))
                    {
                        // Add all targets to the worklist
                        backwardEdges.ForEach(edgeIndex => worklist.Enqueue(edgeIndex));
                    }

                    currentLevelAccumulator += backwardEdges.Count;
                }

                // Completed a level; check width values, including if we have a new maxWidth
                if (currentLevelWidth > maxLevelWidth) maxLevelWidth = currentLevelWidth;

                sumOfAllWidths += currentLevelWidth;

                // Next level's number of nodes
                currentLevelWidth = currentLevelAccumulator;
            }

            if (sumOfAllWidths != vertices.Count)
            {
                throw new Exception("Error in width determination: Did not traverse all nodes!");
            }

            return maxLevelWidth;
        }

        //
        // Adds a basic edge to the graph
        //
        public void AddEdge(T from, T to)
        {
            AddEdge(edgeMap, from, to);
            AddEdge(transposeEdgeMap, to, from);

            numEdges++;
        }

        private void AddEdge(Dictionary<int, List<int>> edges, T from, T to)
        {
            // Create the new vertex nodes and add to the vertices
            // This must come first to guarantee the 'start node' is at index 0
            int toVertexIndex = GetVertexIndex(to);
            int fromVertexIndex = GetVertexIndex(from);

            //
            // Acquire the list of target nodes (which imply an edge: from -> to)
            //
            List<int> targetVertexIndices;
            if (edges.TryGetValue(fromVertexIndex, out targetVertexIndices))
            {
                if (!targetVertexIndices.Contains(toVertexIndex))
                //{
                //    throw new ArgumentException("Edge: " + from + " " + to + " already exists in problem graph.");
                //}

                targetVertexIndices.Add(toVertexIndex);
            }
            // No edge exists yet: from -> to
            else
            {
                edges.Add(fromVertexIndex, Utilities.MakeList<int>(toVertexIndex));
            }
        }

        //
        // Adds a many-to-one hyperedge to the graph by adding all the individual edges
        //
        public void AddHyperEdge(List<T> fromList, T to)
        {
            foreach (T from in fromList)
            {
                this.AddEdge(from, to);
            }
        }

        //
        // Acquire the given nodes from the vertices list OR create a new node (and add to the list)
        //
        private int GetVertexIndex(T val)
        {
            Vertex newVertexNode = new Vertex(val);
            int vertexIndex = vertices.IndexOf(newVertexNode);

            // Add as a new vertex
            if (vertexIndex == -1)
            {
                vertices.Add(newVertexNode);
                return vertices.Count;
            }

            return vertexIndex;
        }

        public bool ContainsCycle()
        {
            // Update the SCCs
            sccs = GetStronglyConnectedComponents();

            // Since all strongly connected components should contain one node, there should be the exact same number of SCCs as vertices.
            return sccs.Count != vertices.Count;
        }

        public string GetStronglyConnectedComponentDump()
        {
            if (!sccs.Any())
            {
                sccs = GetStronglyConnectedComponents();
            }

            StringBuilder str = new StringBuilder();
            str.AppendLine("SCCs: ");
            int counter = 0;
            foreach (List<Vertex> scc in sccs)
            {
                str.Append("\t" + (counter++) + ": ");
                foreach (Vertex v in scc)
                {
                    str.Append(v.node + " ");
                }
                str.AppendLine("");
            }

            return str.ToString();
        }

        //
        // Use Tarjan's Algorithm to acquire the Strongly Connected Components of a given directed graph
        //
        private List<List<Vertex>> GetStronglyConnectedComponents()
        {
            List<List<Vertex>> stronglyConnectedComponents = new List<List<Vertex>>();
            Stack<Vertex> workStack = new Stack<Vertex>();
            int overallIndex = 0;

            foreach (Vertex vertex in this.vertices)
            {
                if (vertex.index < 0)
                {
                    StronglyConnectedSub(vertex, overallIndex, workStack, stronglyConnectedComponents);
                }
            }

            //
            // Reset the indices if we update the graph and call this procedure again
            foreach (Vertex vertex in this.vertices)
            {
                vertex.index = -1;
                vertex.lowLink = -1;
            }

            return stronglyConnectedComponents;
        }
        private void StronglyConnectedSub(Vertex vertex, int overallIndex, Stack<Vertex> workStack, List<List<Vertex>> stronglyConnectedComponents)
        {
            //
            // Define the current vertex reachability
            //
            vertex.index = overallIndex;
            vertex.lowLink = overallIndex;
            overallIndex++;

            workStack.Push(vertex);

            //
            // Pursue all dependencies
            //
            List<int> dependencies;
            if (edgeMap.TryGetValue(GetVertexIndex(vertex.node), out dependencies))
            {
                //
                // Follow each edge in depth-first manner
                //
                foreach (int wIndex in dependencies)
                {
                    Vertex w = vertices[wIndex];

                    if (vertices[wIndex].index < 0)
                    {
                        StronglyConnectedSub(w, overallIndex, workStack, stronglyConnectedComponents);
                        vertex.lowLink = Math.Min(vertex.lowLink, w.lowLink);
                    }
                    else if (workStack.Contains(w))
                    {
                        vertex.lowLink = Math.Min(vertex.lowLink, w.index);
                    }
                }
            }

            if (vertex.lowLink == vertex.index)
            {
                List<Vertex> scc = new List<Vertex>();
                Vertex w;
                do
                {
                    w = workStack.Pop();
                    scc.Add(w);
                } while (!vertex.Equals(w));

                stronglyConnectedComponents.Add(scc);
            }
        }
    }
}