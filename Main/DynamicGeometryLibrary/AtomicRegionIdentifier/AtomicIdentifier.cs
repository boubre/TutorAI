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
        private UndirectedGraph.PlanarGraph workingGraph; // graph that may be modified by the algorithm.
        private UndirectedGraph.PlanarGraph graphCopy; // original, unaltered graph

        // This provides access to all of the required knowledge of the figure.
        private ImpliedComponentCalculator implied;

        public AtomicIdentifier(ImpliedComponentCalculator impl)
        {
            implied = impl;
            atomicRegions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();
            workingGraph = new UndirectedGraph.PlanarGraph();

            //
            // Populate the planar graph.
            //
            ConstructGraph();

            // Keep an original copy for reference.
            graphCopy = new UndirectedGraph.PlanarGraph(workingGraph);

            //MinimalBasisCalculator mbCalculator = new MinimalBasisCalculator(workingGraph);
            //List<Primitive> primitives = mbCalculator.primitives;

            //ConvertPrimitivesToAtomicRegions(primitives);
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
                workingGraph.AddNode(realPt, UndirectedGraph.NodePointType.REAL);
            }

            foreach (Point extPt in implied.extendedSegmentPoints)
            {
                workingGraph.AddNode(extPt, UndirectedGraph.NodePointType.EXTENDED);
            }

            foreach (Point extPt in implied.extendedCirclePoints)
            {
                workingGraph.AddNode(extPt, UndirectedGraph.NodePointType.EXTENDED);
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
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.impliedChords)
            {
                                                                                                              // CTA: Real, Extended?
                workingGraph.AddUndirectedEdge(segment.Point1, segment.Point2, segment.Length, UndirectedGraph.EdgeType.REAL_SEGMENT);
            }

            // Extended Diameters
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.extendedDiameters)
            {
//                workingGraph.AddUndirectedEdge(arc.endpoint1, arc.endpoint2, arc.length, UndirectedGraph.EdgeType.REAL_ARC);
            }
        }

        //
        // Direct Segments
        //
        // We only want to consider the smallest collinear segments: A---M---B would have 3 segments AM, MB, AB. We only want AM and  MB
        //
        public void AddDirectSegments()
        {
            for (int s1 = 0; s1 < implied.segments.Count; s1++)
            {
                //
                // Does this segment (s1) have any subsegments?
                //
                bool hasSubsegment = false;
                for (int s2 = 0; s2 < implied.segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (implied.segments[s1].HasSubSegment(implied.segments[s2]))
                        {
                            hasSubsegment = true;
                            break;
                        }
                    }
                }

                // Add to the workingGraph if this segment has no other sub-segment.
                if (!hasSubsegment)
                {
                    workingGraph.AddUndirectedEdge(implied.segments[s1].Point1, implied.segments[s1].Point2, implied.segments[s1].Length, UndirectedGraph.EdgeType.REAL_SEGMENT);
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
            for (int a1 = 0; a1 < implied.minorArcs.Count; a1++)
            {
                //
                // Does this segment (s1) have any subsegments?
                //
                bool hasSubArc = false;
                for (int a2 = 0; a2 < implied.minorArcs.Count; a2++)
                {
                    if (a1 != a2)
                    {
                        if (implied.minorArcs[a1].HasMinorSubArc(implied.minorArcs[a2]))
                        {
                            hasSubArc = true;
                            break;
                        }
                    }
                }

                // Add to the workingGraph if this segment has no other sub-segment.
                if (!hasSubArc)
                {
                    workingGraph.AddUndirectedEdge(implied.minorArcs[a1].endpoint1, implied.minorArcs[a1].endpoint2,
                                            Point.calcDistance(implied.minorArcs[a1].endpoint1, implied.minorArcs[a1].endpoint2), UndirectedGraph.EdgeType.REAL_ARC);
                }
            }
        }

        public void ConvertPrimitivesToAtomicRegions(List<Primitive> primitives)
        {
            foreach (Primitive primitive in primitives)
            {
                //primitive.ConvertToAtomicRegion();
            }
        }
    }
}