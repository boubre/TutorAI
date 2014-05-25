using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaHypergraph
    {
        // The area hypergraph has nodes which are regions and annotated edges which relate with \pm (+ / -)
        public Hypergraph.Hypergraph<Region, SimpleRegionEquation> graph { get; private set; }

        //
        private bool[] visited;

        public AreaHypergraph(List<AtomicRegion> atoms)
        {
            graph = new Hypergraph.Hypergraph<Region, SimpleRegionEquation>();

            BuildNodes(atoms);
            BuildEdges(atoms);

            visited = new bool[graph.Size()];
        }

        //
        // Reset the fact we have not visited any nodes.
        //
        public void Clear()
        {
            visited = new bool[graph.Size()];
        }


        //
        // The graph nodes are the powerset of atomic nodes
        //
        private void BuildNodes(List<AtomicRegion> atoms)
        {
            // Acquire an integer representation of the powerset of atomic nodes
            List<List<int>> powerset = Utilities.ConstructPowerSetWithNoEmpty(atoms.Count);

            // Construct each element of the powerset
            foreach (List<int> set in powerset)
            {
                List<AtomicRegion> theseAtoms = new List<AtomicRegion>();

                // Construct the individual element (set)
                foreach(int e in set)
                {
                    theseAtoms.Add(atoms[e]);
                }

                // Add the new node to the area hypergraph
                graph.AddNode(new Region(theseAtoms));
            }
        }

        //
        // There is one addition edge and two subtraction edges per set of 3 nodes.
        // Build the edges top-down from complete set of atoms down to singletons.
        //
        private void BuildEdges(List<AtomicRegion> atoms)
        {
            // We don't want edges connecting a singleton region to an 'empty' region.
            if (atoms.Count == 1) return;

            Region atomsRegion = new Region(atoms);
            foreach (AtomicRegion atom in atoms)
            {
                List<AtomicRegion> atomsMinusAtom = new List<AtomicRegion>(atoms);
                atomsMinusAtom.Remove(atom);

                Region aMinus1Region = new Region(atomsMinusAtom);
                Region atomRegion = new Region(atom);

                //
                // A = (A \ a) + a
                //
                SimpleRegionEquation sumAnnotation = new SimpleRegionEquation(atomsRegion, aMinus1Region, OperationT.ADDITION, atomRegion);
                List<Region> sources = new List<Region>();
                sources.Add(aMinus1Region);
                sources.Add(atomRegion);
                graph.AddEdge(sources, atomsRegion, sumAnnotation);

                //
                // (A \ a) = A - a
                //
                SimpleRegionEquation diff1Annotation = new SimpleRegionEquation(aMinus1Region, atomsRegion, OperationT.SUBTRACTION, atomRegion);
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(atomRegion);
                graph.AddEdge(sources, aMinus1Region, diff1Annotation);

                //
                // a = A - (A \ a)
                //
                SimpleRegionEquation diff2Annotation = new SimpleRegionEquation(atomRegion,atomsRegion, OperationT.SUBTRACTION, aMinus1Region);
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(aMinus1Region);
                graph.AddEdge(sources, atomRegion, diff2Annotation);

                //
                // Recursive call to construct edges with A \ a
                //
                BuildEdges(atomsMinusAtom);
            }
        }

        //public ComplexRegionEquation TraceRegionArea(Region thatRegion)
        //{
        //    // Find this region in the hypergraph.
        //    int startIndex = graph.GetNodeIndex(thatRegion);

        //    if (startIndex == -1) throw new ArgumentException("Desired region not found in area hypergraph: " + thatRegion);

        //    return SimpleVisit(startIndex);
        //}

        ////
        //// Graph traversal to find shapes and thus the resulting equation (solution).
        ////
        //// Breadth first.
        //private ComplexRegionEquation SimpleVisit(int startIndex)
        //{
        //    Queue<int> worklist = new Queue<int>();
        //    worklist.Enqueue(startIndex);

        //    while (worklist.Any())
        //    {
        //        // Acquire the next value to consider
        //        int currentNodeIndex = worklist.Dequeue();

        //        // We have now visited this node.
        //        visited[currentNodeIndex] = true;

        //        // For all hyperedges leaving this node, add the sources to the worklist.
        //        foreach (Hypergraph.HyperEdge<SimpleRegionEquation> edge in graph.vertices[currentNodeIndex].edges)
        //        {
        //            edge.sourceNodes.ForEach(s => worklist.Enqueue(s));
        //        }

        //        if ()





        //        foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> currentEdge in pebblerGraph.vertices[currentNodeIndex].edges)
        //        {
        //            if (!Utilities.RESTRICTING_AXS_DEFINITIONS_THEOREMS || (Utilities.RESTRICTING_AXS_DEFINITIONS_THEOREMS && currentEdge.annotation.IsActive()))
        //            {
        //                if (!currentEdge.IsFullyPebbled())
        //                {
        //                    // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
        //                    Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

        //                    // With this new node, check if the edge is full pebbled; if so, percolate
        //                    if (currentEdge.IsFullyPebbled())
        //                    {
        //                        // Has the target of this edge been pebbled previously? Pebbled -> Pebbled means we have a backward edge
        //                        if (!IsNodePebbled(currentEdge.targetNode))
        //                        {
        //                            // Success, we have an edge
        //                            // Construct a static set of pebbled hyperedges for problem construction
        //                            edgeDatabase.Put(currentEdge);

        //                            // Add this node to the worklist to percolate further
        //                            if (!worklist.Contains(currentEdge.targetNode))
        //                            {
        //                                worklist.Add(currentEdge.targetNode);
        //                                worklist.Sort();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public SimpleRegionEquation TraceRegionArea(List<AtomicRegion> atoms)
        //{
        //    return TraceRegionArea(new Region(atoms));
        //}
    }
}