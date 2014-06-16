using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;

namespace LiveGeometry.AtomicRegionIdentifier
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public class AtomicIdentifier
    {
        private List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> atomicRegions;
        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> GetAtomicRegions() { return atomicRegions; }

        //
        // The graph we use as the basis for region identification.
        // will be modified by the algorithm
        //
        private UndirectedPlanarGraph.PlanarGraph workingGraph;
        private UndirectedPlanarGraph.PlanarGraph originalGraph;

        public AtomicIdentifier(ImpliedComponentCalculator implied)
        {
            //
            // Construct the Planar graph
            //
            PlanarGraphConstructor graphConstructor = new PlanarGraphConstructor(implied);
            originalGraph = graphConstructor.GetGraph();
            workingGraph = new UndirectedPlanarGraph.PlanarGraph(originalGraph);

            if (GeometryTutorLib.Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine(originalGraph.ToString());
            }

            MinimalBasisCalculator mbCalculator = new MinimalBasisCalculator(workingGraph);
            List<Primitive> primitives = mbCalculator.GetPrimitives();

            if (GeometryTutorLib.Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine("Primitives:");
                foreach (Primitive prim in primitives)
                {
                    Debug.WriteLine("\t" + prim.ToString());
                }
            }

            atomicRegions = ConvertPrimitivesToAtomicRegions(primitives, implied);
        }

        //
        // Take the cycle-based representation and convert in into AtomicRegion objects.
        //
        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> ConvertPrimitivesToAtomicRegions(List<Primitive> primitives, ImpliedComponentCalculator implied)
        {
            List<MinimalCycle> cycles = new List<MinimalCycle>();
            List<Filament> filaments = new List<Filament>();

            foreach (Primitive primitive in primitives)
            {
                if (primitive is MinimalCycle) cycles.Add(primitive as MinimalCycle);
                if (primitive is Filament) filaments.Add(primitive as Filament);
            }

            // TBC: HandleFilaments();

            ComposeCycles(cycles);

            if (GeometryTutorLib.Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine("Composed:");
                foreach (MinimalCycle cycle in cycles)
                {
                    Debug.WriteLine("\t" + cycle.ToString());
                }
            }

            //
            // Convert all cycles (perimeters) to atomic regions
            //
            List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> regions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();
            foreach (MinimalCycle cycle in cycles)
            {
                List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> temp = cycle.ConstructAtomicRegions(implied, originalGraph);
                foreach (GeometryTutorLib.Area_Based_Analyses.AtomicRegion atom in temp)
                {
                    if (!regions.Contains(atom)) regions.Add(atom);
                }
            }

            return regions;
        }

        //
        // If a cycle has an edge that is EXTENDED, there exist two regions, one on each side of the segment; compose the two segments.
        //
        // Fixed point algorithm: while there exists a cycle with an extended segment, compose.
        private void ComposeCycles(List<MinimalCycle> cycles)
        {
            for (int cycleIndex = HasComposableCycle(cycles); cycleIndex != -1; cycleIndex = HasComposableCycle(cycles))
            {
                // Get the cycle and remove it from the list.
                MinimalCycle thisCycle = cycles[cycleIndex];

                cycles.RemoveAt(cycleIndex);

                // Get the extended segment which is the focal segment of composition.
                GeometryTutorLib.ConcreteAST.Segment extendedSeg = thisCycle.GetExtendedSegment(originalGraph);

                // Find the matching cycle that has the same Extended segment
                int otherIndex = GetComposableCycleWithSegment(cycles, extendedSeg);
                MinimalCycle otherCycle = cycles[otherIndex];
                cycles.RemoveAt(otherIndex);

                // Compose the two cycles into a single cycle.
                MinimalCycle composed = thisCycle.Compose(otherCycle, extendedSeg);

                // Add the new, composed cycle
                cycles.Add(composed);
            }
        }

        private int HasComposableCycle(List<MinimalCycle> cycles)
        {
            for (int c = 0; c < cycles.Count; c++)
            {
                if (cycles[c].HasExtendedSegment(originalGraph)) return c;
            }
            return -1;
        }

        private int GetComposableCycleWithSegment(List<MinimalCycle> cycles, GeometryTutorLib.ConcreteAST.Segment segment)
        {
            for (int c = 0; c < cycles.Count; c++)
            {
                if (cycles[c].HasThisExtendedSegment(originalGraph, segment)) return c;
            }

            return -1;
        }
    }
}