using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;
using System;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class MinimalCycle : Primitive
    {
        // These points were ordered by the minimal basis algorithm.
        public List<Point> points;

        public MinimalCycle()
        {
            points = new List<Point>();
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public void AddAll(List<Point> pts)
        {
            points.AddRange(pts);
        }

        public bool HasExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph)
        {
            return GetExtendedSegment(graph) != null;
        }

        public GeometryTutorLib.ConcreteAST.Segment GetExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph)
        {
            for (int p = 0; p < points.Count; p++)
            {
                if (graph.GetEdgeType(points[p], points[p + 1 < points.Count ? p + 1 : 0]) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT)
                {
                    return new GeometryTutorLib.ConcreteAST.Segment(points[p], points[p + 1 < points.Count ? p + 1 : 0]);
                }
            }

            return null;
        }

        public bool HasThisExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph, GeometryTutorLib.ConcreteAST.Segment segment)
        {
            if (!points.Contains(segment.Point1)) return false;
            if (!points.Contains(segment.Point2)) return false;

            return graph.GetEdgeType(segment.Point1, segment.Point2) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT;
        }

        private List<Point> GetPointsBookEndedBySegment(GeometryTutorLib.ConcreteAST.Segment segment)
        {
            int index1 = points.IndexOf(segment.Point1);
            int index2 = points.IndexOf(segment.Point2);

            // Are the points book-ended properly already?
            if (index1 == 0 && index2 == points.Count - 1) return new List<Point>(points);

            // The set to be returned.
            List<Point> ordered = new List<Point>();

            // Are the points book-ended in reverse?
            if (index1 == points.Count - 1 && index2 == 0)
            {
                for (int p = points.Count-1; p >= 0; p--)
                {
                    ordered.Add(points[p]);
                }

                return ordered;
            }

            // The order is the same as specified by the segment; just cycle the points.
            if (index1 < index2)
            {
                for (int p = 0; p < points.Count; p++)
                {
                    int tempIndex = (index1 - p) < 0 ? points.Count + (index1 - p) : (index1 - p);
                    ordered.Add(points[(index1 - p) < 0 ? points.Count + (index1 - p) : (index1 - p)]);
                }
            }
            // The order is NOT the same as specified by the segment (it's reversed).
            else
            {
                for (int p = 0; p < points.Count; p++)
                {
                    int tempIndex = (index1 + p) % points.Count;
                    ordered.Add(points[(index1 + p) % points.Count]);
                }
            }

            return ordered;
        }

        public MinimalCycle Compose(MinimalCycle thatCycle, GeometryTutorLib.ConcreteAST.Segment extended)
        {
            MinimalCycle composed = new MinimalCycle();

            List<Point> thisPts = this.GetPointsBookEndedBySegment(extended);
            List<Point> thatPts = thatCycle.GetPointsBookEndedBySegment(extended);

            // Add all points from this;
            composed.AddAll(thisPts);

            // Add all points from that (excluding endpoints)
            for (int p = thatPts.Count - 2; p > 0; p--)
            {
                composed.Add(thatPts[p]);
            }

            return composed;
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("Cycle { ");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].ToString());
                if (p < points.Count - 1) str.Append(", ");
            }
            str.Append(" }");

            return str.ToString();
        }

        //
        //
        // Create the actual set of atomic regions for this cycle.
        //
        //   We need to check to see if any of the cycle segments are based on arcs.
        //   We have to handle the degree of each segment: do many circles intersect at these points?
        //
        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> ConstructAtomicRegions(ImpliedComponentCalculator implied, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> regions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();

            GeometryTutorLib.Area_Based_Analyses.AtomicRegion region = null;

            //
            // Check for a direct polygon (no arcs).
            //
            region = PolygonDefinesRegion(implied, graph);
            if (region != null)
            {
                regions.Add(region);
                return regions;
            }

            //
            // Does this region define a sector? 
            //
            region = SectorDefinesRegion(implied, graph);
            if (region != null)
            {
                regions.Add(region);
                return regions;
            }

            //
            // Do we have a set of regions defined by a polygon in which circle(s) cut out some of that region? 
            //
            regions.AddRange(ChordedRegion(implied, graph));

            return regions;
        }

        private GeometryTutorLib.Area_Based_Analyses.AtomicRegion PolygonDefinesRegion(ImpliedComponentCalculator implied, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> sides = new List<GeometryTutorLib.ConcreteAST.Segment>();

            for (int p = 0; p < points.Count; p++)
            {
                GeometryTutorLib.ConcreteAST.Segment segment = new GeometryTutorLib.ConcreteAST.Segment(points[p], points[(p + 1) % points.Count]);

                sides.Add(segment);

                if (graph.GetEdge(points[p], points[(p + 1) % points.Count]).edgeType != UndirectedPlanarGraph.EdgeType.REAL_SEGMENT) return null;
            }

            //
            // Make the Polygon
            //
            GeometryTutorLib.ConcreteAST.Polygon poly = GeometryTutorLib.ConcreteAST.Polygon.MakePolygon(sides);

            if (poly == null) throw new ArgumentException("Real segments should define a polygon; they did not.");
            
            return new GeometryTutorLib.Area_Based_Analyses.ShapeAtomicRegion(poly);
        }

        private GeometryTutorLib.Area_Based_Analyses.AtomicRegion SectorDefinesRegion(ImpliedComponentCalculator implied, UndirectedPlanarGraph.PlanarGraph graph)
        {
            Dictionary<GeometryTutorLib.ConcreteAST.Segment, UndirectedPlanarGraph.PlanarGraphEdge> edges = new Dictionary<GeometryTutorLib.ConcreteAST.Segment, UndirectedPlanarGraph.PlanarGraphEdge>();

            List<MinorArc> realArcs = new List<MinorArc>();
            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                GeometryTutorLib.ConcreteAST.Segment segment = new GeometryTutorLib.ConcreteAST.Segment(points[p], points[(p + 1) % points.Count]);
                edges.Add(segment, edge);

                //
                // If we have a real arc (no chord associated), create the set of arcs.
                //
                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC)
                {
                    // Find the applicable circle.
                    GeometryTutorLib.ConcreteAST.Circle theCircle = null;
                    foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
                    {
                        if (circle.HasArc(points[p], points[(p + 1) % points.Count]))
                        {
                            theCircle = circle;
                            break;
                        }
                    }

                    realArcs.Add(new MinorArc(theCircle, points[p], points[(p + 1) % points.Count]));
                }
                //
                // Extended segments; if we have several this is an issue; and we omit the region
                //
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT) return null;
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_DUAL) return null;
            }

            // Pacman shape created with a circle results in Sector
            return ConvertToSector(edges, realArcs);
        }

        private List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> ChordedRegion(ImpliedComponentCalculator implied, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> regions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();

            // Every segment may be have a set of circles. (on each side) surrounding it.
            // Keep parallel lists of: (1) segments, (2) left outer circles, and (3) right outer circles
            GeometryTutorLib.ConcreteAST.Segment[] regionsSegments = new GeometryTutorLib.ConcreteAST.Segment[points.Count];
            GeometryTutorLib.ConcreteAST.Circle[] leftOuterCircles = new GeometryTutorLib.ConcreteAST.Circle[points.Count];
            GeometryTutorLib.ConcreteAST.Circle[] rightOuterCircles = new GeometryTutorLib.ConcreteAST.Circle[points.Count];

            //
            // Populate the parallel arrays.
            //
            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                regionsSegments[p] = new GeometryTutorLib.ConcreteAST.Segment(points[p], points[(p + 1) % points.Count]);

                // If a known segment, just add it directly, no circles.
                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_SEGMENT)
                {
                    // No-Op
                }
                // If we have an arc / chord situation, handle it. This method will return the outermost circle.
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_DUAL)
                {
                    //
                    // Get the exact chord and set of circles
                    //
                    GeometryTutorLib.ConcreteAST.Segment chord = null;
                    List<GeometryTutorLib.ConcreteAST.Circle> circles = null;
                    foreach (KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Circle>> chordPair in implied.impliedChords)
                    {
                        if (chordPair.Key.StructurallyEquals(regionsSegments[p]))
                        {
                            chord = chordPair.Key;
                            circles = chordPair.Value;
                            break;
                        }
                    }

                    regions.AddRange(ConvertToCircleCircle(chord, circles, out leftOuterCircles[p], out rightOuterCircles[p]));
                }
                else
                {
                    throw new ArgumentException("Unexpected type in ChordedRegion: " + edge.edgeType);
                }
            }

            // Construct a polygon out of the straight-up segments
            // This might be a polygon that defines a pathological region.
            GeometryTutorLib.ConcreteAST.Polygon poly = GeometryTutorLib.ConcreteAST.Polygon.MakePolygon(new List<GeometryTutorLib.ConcreteAST.Segment>(regionsSegments));

            // Determine which outermost circles apply inside of this polygon.
            GeometryTutorLib.ConcreteAST.Circle[] circlesCutInsidePoly = new GeometryTutorLib.ConcreteAST.Circle[points.Count];
            for (int p = 0; p < points.Count; p++)
            {
                if (leftOuterCircles[p] != null && rightOuterCircles[p] == null)
                {
                    // Is the midpoint on the interior of the polygon?
                    Point midpt = leftOuterCircles[p].Midpoint(points[p], points[(p + 1) % points.Count]);

                    // Is this point in the interior of this polygon?
                    if (poly.IsInPolygon(midpt)) circlesCutInsidePoly[p] = leftOuterCircles[p];
                }
                else if (leftOuterCircles[p] == null && rightOuterCircles[p] != null)
                {
                    // Is the midpoint on the interior of the polygon?
                    Point midpt = rightOuterCircles[p].Midpoint(points[p], points[(p + 1) % points.Count]);

                    // Is this point in the interior of this polygon?
                    if (poly.IsInPolygon(midpt)) circlesCutInsidePoly[p] = rightOuterCircles[p];
                }
                else if (leftOuterCircles[p] != null && rightOuterCircles[p] != null)
                {
                    // Is the midpoint on the interior of the polygon?
                    Point midpt = leftOuterCircles[p].Midpoint(points[p], points[(p + 1) % points.Count]);

                    // Is this point in the interior of this polygon?
                    if (poly.IsInPolygon(midpt)) circlesCutInsidePoly[p] = leftOuterCircles[p];
                    else circlesCutInsidePoly[p] = rightOuterCircles[p];
                }
                else
                {
                    circlesCutInsidePoly[p] = null;
                }
            }

            bool hasNonNull = false;
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circlesCutInsidePoly)
            {
                if (circle != null)
                {
                    hasNonNull = true;
                    break;
                }
            }

            // This is just a normal shape region: polygon.
            if (!hasNonNull)
            {
                regions.Add(new GeometryTutorLib.Area_Based_Analyses.ShapeAtomicRegion(poly));
            }
            // A circle cuts into the polygon.
            else
            {
                //
                // Now that all interior arcs have been identified, construct the atomic (probably pathological) region
                //
                GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion pathological = new GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion();
                for (int p = 0; p < points.Count; p++)
                {
                    if (circlesCutInsidePoly[p] != null) pathological.AddConnection(regionsSegments[p].Point1,
                                                                                    regionsSegments[p].Point2,
                                                                                    GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.ARC,
                                                                                    circlesCutInsidePoly[p]);
                    else pathological.AddConnection(regionsSegments[p].Point1,
                                                    regionsSegments[p].Point2,
                                                    GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.SEGMENT,
                                                    regionsSegments[p]);
                }

                regions.Add(pathological);
            }


            return regions;
        }



        private GeometryTutorLib.Area_Based_Analyses.AtomicRegion ConvertToSector(Dictionary<GeometryTutorLib.ConcreteAST.Segment, UndirectedPlanarGraph.PlanarGraphEdge> edges, List<MinorArc> arcs)
        {
            // Verify that all the arcs belong to the same circle.
            // Add all of the arc measures together for later reference
            double arcMeasure = arcs[0].minorMeasure;
            GeometryTutorLib.ConcreteAST.Circle refCircle = arcs[0].theCircle;
            for (int a = 1; a < arcs.Count; a++)
            {
                if (!refCircle.StructurallyEquals(arcs[a].theCircle))
                {
                    throw new ArgumentException("arc does not match expected circle: " + arcs[a].theCircle);
                }
                arcMeasure += arcs[a].minorMeasure;
            }

            // Identify the 2 radii
            List<GeometryTutorLib.ConcreteAST.Segment> radii = new List<GeometryTutorLib.ConcreteAST.Segment>();
            foreach (KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, UndirectedPlanarGraph.PlanarGraphEdge> edgePair in edges)
            {
                if (edgePair.Value.edgeType != UndirectedPlanarGraph.EdgeType.REAL_ARC) radii.Add(edgePair.Key);
            }

            if (radii.Count != 2) throw new ArgumentException("Expected 2 radii in a sector: " + radii.Count);

            // Find the arc endpoints
            Point endpoint1 = refCircle.PointIsOn(radii[0].Point1) ? radii[0].Point1 : radii[0].Point2;
            Point endpoint2 = refCircle.PointIsOn(radii[1].Point1) ? radii[1].Point1 : radii[1].Point2;

            // Measure the angle between the endpoints and the center to determine if a minor or major arc.
            Arc theArc = null;
            
            if (arcMeasure < 180) theArc = new MinorArc(refCircle, endpoint1, endpoint2);
            theArc = new MajorArc(refCircle, endpoint1, endpoint2);

            return new GeometryTutorLib.Area_Based_Analyses.ShapeAtomicRegion(new Sector(theArc));
        }

        //
        // This is a complex situation because we need to identify situations where circles intersect with the resultant regions:
        //    (|     (|)
        //   ( |    ( | )
        //  (  |   (  |  )
        //   ( |    ( | )
        //    (|     (|)
        //
        // Note: There will always be a chord because of our implied construction.
        // We are interested in only minor arcs of the given circles.
        //
        private List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> ConvertToCircleCircle(GeometryTutorLib.ConcreteAST.Segment chord,
                                                                                              List<GeometryTutorLib.ConcreteAST.Circle> circles,
                                                                                              out GeometryTutorLib.ConcreteAST.Circle leftOuterCircle,
                                                                                              out GeometryTutorLib.ConcreteAST.Circle rightOuterCircle)
        {
            List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> regions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();
            leftOuterCircle = null;
            rightOuterCircle = null;

            //
            // Simple cases that require no special attention.
            //
            if (!circles.Any()) return null;
            if (circles.Count == 1)
            {
                leftOuterCircle = circles[0];

                regions.Add(ConstructBasicLineCircleRegion(chord, circles[0]));

                return regions;
            }

            // All circles that are on each side of the chord 
            List<GeometryTutorLib.ConcreteAST.Circle> leftSide = new List<GeometryTutorLib.ConcreteAST.Circle>();
            List<GeometryTutorLib.ConcreteAST.Circle> rightSide = new List<GeometryTutorLib.ConcreteAST.Circle>();

            // For now, assume max, one circle per side.
            // Construct a collinear list of points that includes all circle centers as well as the single intersection point between the chord and the line passing through all circle centers.
            // This orders the sides and provides implied sizes.

            GeometryTutorLib.ConcreteAST.Segment centerLine = new GeometryTutorLib.ConcreteAST.Segment(circles[0].center, circles[1].center);
            for (int c = 2; c < circles.Count; c++)
            {
                centerLine.AddCollinearPoint(circles[c].center);
            }
            // Find the intersection between the center-line and the chord; add that to the list.
            Point intersection = centerLine.FindIntersection(chord);
            centerLine.AddCollinearPoint(intersection);

            List<Point> collPoints = centerLine.collinear;
            int interIndex = collPoints.IndexOf(intersection);

            for (int i = 0; i < collPoints.Count; i++)
            {
                // find the circle based on center
                int c;
                for (c = 0; c < circles.Count; c++)
                {
                    if (circles[c].center.StructurallyEquals(collPoints[i])) break;
                }

                // Add the circle in order
                if (i < interIndex) leftSide.Add(circles[c]);
                else if (i > interIndex) rightSide.Add(circles[c]);
            }

            // the outermost circle is first in the left list and last in the right list.
            if (leftSide.Any()) leftOuterCircle = leftSide[0];
            if (rightSide.Any()) rightOuterCircle = rightSide[rightSide.Count - 1];

            //
            // Main combining algorithm:
            //     Assume: Increasing Arc sequence A \in A_1, A_2, ..., A_n and the single chord C
            //
            //     Construct region B = (C, A_1)
            //     For the increasing Arc sequence (k subscript)  A_2, A_3, ..., A_n
            //         B = Construct ((C, A_k) \ B)
            //         
            // Alternatively:
            //     Construct(C, A_1)
            //     for each pair Construct (A_k, A_{k+1})
            //
            //
            // Handle each side: left and right.
            //
            if (leftSide.Any()) regions.Add(ConstructBasicLineCircleRegion(chord, leftSide[leftSide.Count - 1]));
            for (int ell = 0; ell < leftSide.Count - 2; ell++)
            {
                regions.Add(ConstructBasicCircleCircleRegion(chord, leftSide[ell], leftSide[ell + 1]));
            }

            if (rightSide.Any()) regions.Add(ConstructBasicLineCircleRegion(chord, rightSide[0]));
            for (int r = 1; r < rightSide.Count - 1; r++)
            {
                regions.Add(ConstructBasicCircleCircleRegion(chord, rightSide[r], rightSide[r+1]));
            }

            return regions;
        }

        // Construct the region between a chord and the circle arc:
        //    (|
        //   ( |
        //  (  |
        //   ( |
        //    (|
        //
        private GeometryTutorLib.Area_Based_Analyses.AtomicRegion ConstructBasicLineCircleRegion(GeometryTutorLib.ConcreteAST.Segment chord, GeometryTutorLib.ConcreteAST.Circle circle)
        {
            GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion region = new GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion();

            region.AddConnection(chord.Point1, chord.Point2, GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.ARC, circle);

            region.AddConnection(chord.Point1, chord.Point2, GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.SEGMENT, chord);

            return region;
        }

        // Construct the region between a circle and circle:
        //     __
        //    ( (
        //   ( ( 
        //  ( (  
        //   ( ( 
        //    ( (
        //     --
        private GeometryTutorLib.Area_Based_Analyses.AtomicRegion ConstructBasicCircleCircleRegion(GeometryTutorLib.ConcreteAST.Segment chord, GeometryTutorLib.ConcreteAST.Circle c1, GeometryTutorLib.ConcreteAST.Circle c2)
        {
            GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion region = new GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion();

            region.AddConnection(chord.Point1, chord.Point2, GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.ARC, c1);

            region.AddConnection(chord.Point1, chord.Point2, GeometryTutorLib.Area_Based_Analyses.NonShapeAtomicRegion.ConnectionType.ARC, c2);

            return region;
        }
    }
}