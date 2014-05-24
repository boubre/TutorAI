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
        //
        private UndirectedPlanarGraph.PlanarGraph workingGraph; // graph that may be modified by the algorithm.
        private UndirectedPlanarGraph.PlanarGraph originalGraph; // original, unaltered graph

        // This provides access to all of the required knowledge of the figure.
        private ImpliedComponentCalculator implied;

        public AtomicIdentifier(ImpliedComponentCalculator impl)
        {
            implied = impl;
            atomicRegions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();
            workingGraph = new UndirectedPlanarGraph.PlanarGraph();

            //
            // Populate the planar graph.
            //
            ConstructGraph();

            // Keep an original copy for reference.
            originalGraph = new UndirectedPlanarGraph.PlanarGraph(workingGraph);

            Debug.WriteLine(workingGraph.ToString());

            MinimalBasisCalculator mbCalculator = new MinimalBasisCalculator(workingGraph);
            List<Primitive> primitives = mbCalculator.primitives;

            if (GeometryTutorLib.Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine("Primitives:");
                foreach (Primitive prim in primitives)
                {
                    Debug.WriteLine("\t" + prim.ToString());
                }
            }

            atomicRegions = ConvertPrimitivesToAtomicRegions(primitives);
        }

        //
        // Make the graph.
        //   (1) The nodes of the graph are all evident points in the graph and all extended points.
        //   (2) Edges refer to the physical, planar connections between nodes.   
        //
        //
        private void ConstructGraph()
        {
            ConstructGraphNodes();
            ConstructGraphEdges();
        }

        //
        // Add all real and extended points as nodes
        //
        //  Label all nodes as (REAL, EXTENDED)
        //
        private void ConstructGraphNodes()
        {
            foreach (Point realPt in implied.allEvidentPoints)
            {
                workingGraph.AddNode(realPt, UndirectedPlanarGraph.NodePointType.REAL);
            }

            //foreach (Point extPt in implied.extendedSegmentPoints)
            //{
            //    workingGraph.AddNode(extPt, UndirectedPlanarGraph.NodePointType.EXTENDED);
            //}

            foreach (Point extPt in implied.extendedCirclePoints)
            {
                workingGraph.AddNode(extPt, UndirectedPlanarGraph.NodePointType.EXTENDED);
            }
        }

        //
        // Construct all edges:
        //
        //  Real:
        //    (1) Direct segments
        //    (2) Circle arcs
        //  Extended:
        //    (1) Extended diameters (and implied radii)
        //    (2) Extended chords
        //    (3) Extensions of segments (which result in extended segment-segment points)
        private void ConstructGraphEdges()
        {
            AddDirectSegments();
            AddDirectArcs();

            // Implied Chords
            foreach (KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Circle>> chordPair in implied.impliedChords)
            {
                                                                                                              // CTA: Real, Extended?
                workingGraph.AddUndirectedEdge(chordPair.Key.Point1, chordPair.Key.Point2, chordPair.Key.Length, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT); //.EXTENDED_SEGMENT);
            }

            // Extended Diameters (in the form of Radii)
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.extendedRadii)
            {
                workingGraph.AddUndirectedEdge(segment.Point1, segment.Point2, segment.Length, UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT);
            }
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.extendedRealRadii)
            {
                workingGraph.AddUndirectedEdge(segment.Point1, segment.Point2, segment.Length, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
            }
        }

        //
        // Direct Segments
        //
        // We only want to consider the smallest collinear segments: A---M---B would have 3 segments AM, MB, AB. We only want AM and  MB
        //
        public void AddDirectSegments()
        {
            // Determine the actual set of minimal segments; check if any circle implied points are in the middle of the minimal segments.
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.minimalSegments)
            {
                // Find the subset of applicable points; ensure they are ordered.
                foreach (Point impliedCircPt in implied.impliedCirclePoints)
                {
                    if (segment.PointIsOnAndExactlyBetweenEndpoints(impliedCircPt)) segment.AddCollinearPoint(impliedCircPt);
                }

                // Generate the actual minimal segments.
                for (int p = 0; p < segment.collinear.Count-1; p++)
                {
                    GeometryTutorLib.ConcreteAST.Segment temp = new GeometryTutorLib.ConcreteAST.Segment(segment.collinear[p], segment.collinear[p + 1]);
                    if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.Segment>(implied.extendedRealRadii, temp))
                    {
                        workingGraph.AddUndirectedEdge(temp.Point1, temp.Point2, temp.Length, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                    }
                }
            }
        }

        //
        // Direct Arcs
        //
        // Same as Direct Segments, but with DirectArcs:
        // We only want to consider the smallest collinear Arcs: A---M---B would have 3 arcs AM, MB, AB. We only want AM and  MB
        //
        public void AddDirectArcs()
        {
            //
            // For each circle:
            //   (1) Make a copy of the circle.
            //   (2) Add the applicable extended points to the circle
            //   (3) Traverse the set of circle points to add edges to the graph
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                GeometryTutorLib.ConcreteAST.Circle copy = new GeometryTutorLib.ConcreteAST.Circle(circle.center, circle.radius);
                List<Point> pts = new List<Point>(circle.pointsOnCircle);

                foreach (Point extendedPt in implied.extendedCirclePoints)
                {
                    if (circle.PointIsOn(extendedPt)) pts.Add(extendedPt);
                }

                copy.SetPointsOnCircle(pts);

                for (int p = 0; p < copy.pointsOnCircle.Count; p++)
                {
                    GeometryTutorLib.ConcreteAST.Segment temp = new GeometryTutorLib.ConcreteAST.Segment(copy.pointsOnCircle[p], copy.pointsOnCircle[p+1 < copy.pointsOnCircle.Count ? p+1 : 0]);
                    workingGraph.AddUndirectedEdge(temp.Point1, temp.Point2, temp.Length, UndirectedPlanarGraph.EdgeType.REAL_ARC); 
                }
            }
        }

        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> ConvertPrimitivesToAtomicRegions(List<Primitive> primitives)
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