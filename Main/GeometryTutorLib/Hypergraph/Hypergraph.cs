using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Provides functionality to create a hypergraph: adding nodes and edges
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Hypergraph<T, A>
    {
        // The main graph data structure
        public List<HyperNode<T, A>> vertices { get; private set; }

        public Hypergraph()
        {
            vertices = new List<HyperNode<T, A>>();
        }

        public int Size() { return vertices.Count; }

        //
        // Integer-based representation of the main hypergraph
        //
        public PebblerHypergraph<T, A> GetPebblerHypergraph()
        {
            //
            // Strictly create the nodes
            //
            PebblerHyperNode<T, A>[] pebblerNodes = new PebblerHyperNode<T, A>[vertices.Count];
            for (int v = 0; v < vertices.Count; v++)
            {
                pebblerNodes[v] = vertices[v].CreatePebblerNode();
            }

            //
            // Non-redundantly create all hyperedges
            //
            for (int v = 0; v < vertices.Count; v++)
            {
                foreach (HyperEdge<A> edge in vertices[v].forwardEdges)
                {
                    // Only add once to all nodes when this is the 'minimum' source node
                    if (v == edge.sourceNodes.Min())
                    {
                        PebblerHyperEdge<A> newEdge = new PebblerHyperEdge<A>(edge.sourceNodes, edge.targetNode, edge.annotation);
                        foreach (int src in edge.sourceNodes)
                        {
                            pebblerNodes[src].AddEdge(newEdge);
                        }
                    }
                }
            }

            return new PebblerHypergraph<T, A>(pebblerNodes);
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        private int ConvertToLocalIntegerIndex(T inputData)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].data.Equals(inputData))
                {
                    return i;
                }
            }

            return -1;
        }

        //
        // Return the index of the given node
        //
        public int GetNodeIndex(T inputData)
        {
            return ConvertToLocalIntegerIndex(inputData);
        }

        //
        // Return the stored node in the graph
        //
        public T GetNode(int id)
        {
            if (id < 0 || id > vertices.Count)
            {
                throw new ArgumentException("Unexpected id in hypergraph node access: " + id);
            }

            return vertices[id].data;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public bool HasNode(T inputData)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                if (vertex.data.Equals(inputData)) return true;
            }

            return false;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public T GetNode(T inputData)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                if (vertex.data.Equals(inputData)) return vertex.data;
            }

            return default(T);
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public bool AddNode(T inputData)
        {
            if (!HasNode(inputData))
            {
                vertices.Add(new HyperNode<T, A>(inputData, vertices.Count)); // <data, id>
                return true;
            }

            return false;
        }

        //
        // Is this edge in the graph (using local, integer-based information)
        //
        private bool HasLocalForwardEdge(List<int> antecedent, int consequent)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                foreach (HyperEdge<A> edge in vertex.forwardEdges)
                {
                    if (edge.DefinesEdge(antecedent, consequent)) return true;
                }
            }

            return false;
        }

        //
        // Check if the graph contains an edge defined by a many to one clause mapping
        //
        public bool HasForwardEdge(List<T> antecedent, T consequent)
        {
            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            return HasLocalForwardEdge(local.Key, local.Value);
        }

        //
        // Convert information to local, integer-based representation
        //
        private KeyValuePair<List<int>, int> ConvertToLocal(List<T> antecedent, T consequent)
        {
            List<int> localAnte = new List<int>();

            foreach (T ante in antecedent)
            {
                int index = ConvertToLocalIntegerIndex(ante);

                if (index == -1)
                {
                    throw new ArgumentException("Source node not found as a hypergraph node: " + ante);
                }

                localAnte.Add(index);
            }

            int localConsequent = ConvertToLocalIntegerIndex(consequent);

            if (localConsequent == -1)
            {
                throw new ArgumentException("Target value referenced not found as a hypergraph node: " + consequent);
            }

            return new KeyValuePair<List<int>, int>(localAnte, localConsequent);
        }

        //
        // Adding an edge to the graph
        //
        public void AddForwardEdge(List<T> antecedent, T consequent, A annotation)
        {
            //
            // Add a local representaiton of this edge to each node in which it is applicable
            //
            if (HasForwardEdge(antecedent, consequent)) return;

            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            HyperEdge<A> edge = new HyperEdge<A>(local.Key, local.Value, annotation);

//System.Diagnostics.Debug.WriteLine("Adding edge: " + edge.ToString());

            foreach (int src in local.Key)
            {
                vertices[src].AddForwardEdge(edge);
            }
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("All Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                //if (vertices[v].data is Congruent)
                //{
                    if (vertices[v].backwardEdges.Any())
                    {
                        edgeStr = new StringBuilder();
                        edgeStr.Append("{ ");
                        foreach (int s in vertices[v].backwardEdges[0].sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Remove(edgeStr.Length - 1, 1);
                        edgeStr.Append(" }");
                    }

                    Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
                //}
            }


            Debug.WriteLine("\nEdges: ");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].forwardEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].forwardEdges)
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

        public void DumpNonEquationClauses()
        {
            Debug.WriteLine("Non-Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (!(vertices[v].data is Equation))
                {
                    if (vertices[v].backwardEdges.Any())
                    {
                        edgeStr = new StringBuilder();
                        edgeStr.Append("{ ");
                        foreach (int s in vertices[v].backwardEdges[0].sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Remove(edgeStr.Length - 1, 1);
                        edgeStr.Append(" }");
                    }

                    Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
                }
            }
        }

        public void DumpEquationClauses()
        {
            Debug.WriteLine("Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data is Equation)
                {
                    if (vertices[v].backwardEdges.Any())
                    {
                        edgeStr = new StringBuilder();
                        edgeStr.Append("{ ");
                        foreach (int s in vertices[v].backwardEdges[0].sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Remove(edgeStr.Length - 1, 1);
                        edgeStr.Append(" }");
                    }

                    Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
                }
            }
        }

        public void DumpClauseForwardEdges()
        {
            Debug.WriteLine("\n Forward Edges: ");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].forwardEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].forwardEdges)
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

        public void DumpClauseBackwardEdges()
        {
            Debug.WriteLine("\n Backward Edges: ");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].backwardEdges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].backwardEdges)
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
    }
}