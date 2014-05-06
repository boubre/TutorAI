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

        public AreaHypergraph(Figure fig, List<AtomicRegion> atoms)
        {
            BuildNodes(atoms);
            BuildEdges(atoms);
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
                // Construct the individual element (set)
                Region region = new Region();
                foreach(int e in set)
                {
                    region.AddAtomicRegion(atoms[e]);
                }

                // Add the new node to the area hypergraph
                graph.AddNode(region);
            }
        }

        //
        // There is one addition edge and two subtraction edges per set of 3 nodes.
        //
        private void BuildEdges(List<AtomicRegion> atoms)
        {
            if (!atoms.Any()) return;

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
                SimpleRegionEquation sumAnnotation = new SimpleRegionEquation(atomsRegion, new AreaAddition(aMinus1Region, atomRegion));
                List<Region> sources = new List<Region>();
                sources.Add(aMinus1Region);
                sources.Add(atomRegion);
                graph.AddForwardEdge(sources, atomsRegion, sumAnnotation);

                //
                // (A \ a) = A - a
                //
                SimpleRegionEquation diff1Annotation = new SimpleRegionEquation(aMinus1Region, new AreaSubtraction(atomsRegion, atomRegion));
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(atomRegion);
                graph.AddForwardEdge(sources, aMinus1Region, diff1Annotation);

                //
                // a = A - (A \ a)
                //
                SimpleRegionEquation diff2Annotation = new SimpleRegionEquation(atomRegion, new AreaSubtraction(atomsRegion, aMinus1Region));
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(aMinus1Region);
                graph.AddForwardEdge(sources, atomRegion, diff2Annotation);

                //
                // Recursive call to construct edges with A \ a
                //
                BuildEdges(atomsMinusAtom);
            }
        }
    }
}