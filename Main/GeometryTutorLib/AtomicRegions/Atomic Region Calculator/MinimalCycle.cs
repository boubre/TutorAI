using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;
using GeometryTutorLib.ConcreteAST;
using System;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class MinimalCycle : Primitive
    {
        // These points were ordered by the minimal basis algorithm; calculates facets.
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

        public Segment GetExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph)
        {
            for (int p = 0; p < points.Count; p++)
            {
                if (graph.GetEdgeType(points[p], points[(p + 1) % points.Count]) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT)
                {
                    return new Segment(points[p], points[p + 1 < points.Count ? p + 1 : 0]);
                }
            }

            return null;
        }

        public bool HasThisExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph, Segment segment)
        {
            if (!points.Contains(segment.Point1)) return false;
            if (!points.Contains(segment.Point2)) return false;

            return graph.GetEdgeType(segment.Point1, segment.Point2) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT;
        }

        private List<Point> GetPointsBookEndedBySegment(Segment segment)
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

        public MinimalCycle Compose(MinimalCycle thatCycle, Segment extended)
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
        public List<Atomizer.AtomicRegion> ConstructAtomicRegions(List<Circle> circles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Atomizer.AtomicRegion> regions = new List<Atomizer.AtomicRegion>();

            Atomizer.AtomicRegion region = null;

            //
            // Check for a direct polygon (no arcs).
            //
            region = PolygonDefinesRegion(graph);
            if (region != null)
            {
                regions.Add(region);
                return regions;
            }

            //
            // Does this region define a sector? 
            //
            List<AtomicRegion> sectors = SectorDefinesRegion(circles, graph);
            if (sectors != null && sectors.Any())
            {
                regions.AddRange(sectors);
                return regions;
            }

            //
            // Do we have a set of regions defined by a polygon in which circle(s) cut out some of that region? 
            //
            regions.AddRange(MixedArcChordedRegion(circles, graph));

            return regions;
        }

        private Atomizer.AtomicRegion PolygonDefinesRegion(UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Segment> sides = new List<Segment>();

            for (int p = 0; p < points.Count; p++)
            {
                Segment segment = new Segment(points[p], points[(p + 1) % points.Count]);

                sides.Add(segment);

                if (graph.GetEdge(points[p], points[(p + 1) % points.Count]).edgeType != UndirectedPlanarGraph.EdgeType.REAL_SEGMENT) return null;
            }

            //
            // Make the Polygon
            //
            Polygon poly = Polygon.MakePolygon(sides);

            if (poly == null) throw new ArgumentException("Real segments should define a polygon; they did not.");
            
            return new ShapeAtomicRegion(poly);
        }

        private List<Atomizer.AtomicRegion> SectorDefinesRegion(List<Circle> circles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            Dictionary<Segment, UndirectedPlanarGraph.PlanarGraphEdge> edges = new Dictionary<Segment, UndirectedPlanarGraph.PlanarGraphEdge>();

            List<MinorArc> realArcs = new List<MinorArc>();
            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                Segment segment = new Segment(points[p], points[(p + 1) % points.Count]);
                edges.Add(segment, edge);

                //
                // If we have a real arc (no chord associated), create the set of arcs.
                //
                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC)
                {
                    // Find the applicable circle.
                    Circle theCircle = null;
                    foreach (Circle circle in circles)
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

        private List<Atomizer.AtomicRegion> MixedArcChordedRegion(List<Circle> thatCircles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Atomizer.AtomicRegion> regions = new List<Atomizer.AtomicRegion>();

            // Every segment may be have a set of circles. (on each side) surrounding it.
            // Keep parallel lists of: (1) segments, (2) (real) arcs, (3) left outer circles, and (4) right outer circles
            Segment[] regionsSegments = new Segment[points.Count];
            MinorArc[] arcSegments = new MinorArc[points.Count];
            Circle[] leftOuterCircles = new Circle[points.Count];
            Circle[] rightOuterCircles = new Circle[points.Count];

            //
            // Populate the parallel arrays.
            //
            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                // Always put a segment into the collection so we may create a polygon.
                regionsSegments[p] = new Segment(points[p], points[(p + 1) % points.Count]);

                // If a known segment, just add it directly, no circles.
                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_SEGMENT)
                {
                    // No-op
                }
                // If we have an arc / chord situation, handle it. This method will return the outermost circle.
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_DUAL)
                {
                    regionsSegments[p] = new Segment(points[p], points[(p + 1) % points.Count]);

                    //
                    // Get the exact chord and set of circles
                    //
                    Segment chord = regionsSegments[p];
                    List<Circle> circles = new List<Circle>();
                    foreach (Circle circle in thatCircles)
                    {
                        if (circle.PointLiesOn(points[p]) && circle.PointLiesOn(points[(p + 1) % points.Count])) circles.Add(circle);
                    }

                    regions.AddRange(ConvertToCircleCircle(chord, circles, out leftOuterCircles[p], out rightOuterCircles[p]));
                }
                else
                {
                    // Find the unique circle that contains these two points.
                    // (if more than one circle has these points, we would have had more intersections and it would be a direct chorded region)
                    Circle theCircle = null;
                    foreach (Circle circle in thatCircles)
                    {
                        if (circle.PointLiesOn(points[p]) && circle.PointLiesOn(points[(p + 1) % points.Count]))
                        {
                            theCircle = circle;
                            break;
                        }
                    }

                    arcSegments[p] = new MinorArc(theCircle, points[p], points[(p + 1) % points.Count]);
                }
            }

            // Construct a polygon out of the straight-up segments
            // This might be a polygon that defines a pathological region.
            Polygon poly = Polygon.MakePolygon(new List<Segment>(regionsSegments));

            // Determine which outermost circles apply inside of this polygon.
            Circle[] circlesCutInsidePoly = new Circle[points.Count];
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

            bool isStrictPoly = true;
            for (int p = 0; p < points.Count; p++)
            {
                if (circlesCutInsidePoly[p] != null || arcSegments[p] != null)
                {
                    isStrictPoly = false;
                    break;
                }
            }

            // This is just a normal shape region: polygon.
            if (isStrictPoly)
            {
                regions.Add(new ShapeAtomicRegion(poly));
            }
            // A circle cuts into the polygon.
            else
            {
                //
                // Now that all interior arcs have been identified, construct the atomic (probably pathological) region
                //
                AtomicRegion pathological = new AtomicRegion();
                for (int p = 0; p < points.Count; p++)
                {
                    //
                    // A circle cutting inside the polygon
                    //
                    if (circlesCutInsidePoly[p] != null) pathological.AddConnection(regionsSegments[p].Point1,
                                                                                    regionsSegments[p].Point2,
                                                                                    ConnectionType.ARC,
                                                                                    new MinorArc(circlesCutInsidePoly[p], regionsSegments[p].Point1, regionsSegments[p].Point2));
                    
                    //
                    else
                    {
                        // We have a direct arc
                        if (arcSegments[p] != null)
                        {
                            pathological.AddConnection(regionsSegments[p].Point1,
                                                       regionsSegments[p].Point2,
                                                       ConnectionType.ARC,
                                                       arcSegments[p]);
                        }
                        // Use the segment
                        else
                        {
                            pathological.AddConnection(regionsSegments[p].Point1,
                                                       regionsSegments[p].Point2,
                                                       ConnectionType.SEGMENT,
                                                       regionsSegments[p]);
                        }
                    }
                }

                regions.Add(pathological);
            }


            return regions;
        }

        private List<Atomizer.AtomicRegion> ConvertToSector(Dictionary<Segment, UndirectedPlanarGraph.PlanarGraphEdge> edges, List<MinorArc> arcs)
        {
            List<AtomicRegion> newAtoms = new List<AtomicRegion>();

            //
            // Verify that all the arcs belong to the same circle.
            // Add all of the arc measures together for later reference
            //
            double arcMeasure = arcs[0].minorMeasure;
            Circle refCircle = arcs[0].theCircle;
            for (int a = 1; a < arcs.Count; a++)
            {
                if (!refCircle.StructurallyEquals(arcs[a].theCircle))
                {
                    throw new ArgumentException("arc does not match expected circle: " + arcs[a].theCircle);
                }
                arcMeasure += arcs[a].minorMeasure;
            }

            //
            // Identify the segments which will be constructed into two radii; All we care about are the endpoints on the circle and the center.
            //
            List<Point> ptsOnCircle = new List<Point>();
            foreach (KeyValuePair<Segment, UndirectedPlanarGraph.PlanarGraphEdge> edgePair in edges)
            {
                if (edgePair.Value.edgeType != UndirectedPlanarGraph.EdgeType.REAL_ARC)
                {
                    if (refCircle.PointLiesOn(edgePair.Key.Point1)) GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(ptsOnCircle, edgePair.Key.Point1);
                    if (refCircle.PointLiesOn(edgePair.Key.Point2)) GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(ptsOnCircle, edgePair.Key.Point2);
                }
            }

            if (ptsOnCircle.Count != 2) throw new ArgumentException("Expected 2 radii in a sector: " + ptsOnCircle.Count);

            // Measure the angle between the endpoints and the center to determine if a minor or major arc.
            Arc theArc = null;
            
            if (arcMeasure < 180) theArc = new MinorArc(refCircle, ptsOnCircle[0], ptsOnCircle[1]);
            else if (arcMeasure > 180) theArc = new MajorArc(refCircle, ptsOnCircle[0], ptsOnCircle[1]);

            if (theArc != null)
            {
                newAtoms.Add(new ShapeAtomicRegion(new Sector(theArc)));
                return newAtoms;
            }

            //
            // Semicircles
            //
            Point midpt = refCircle.Midpoint(ptsOnCircle[0], ptsOnCircle[1]);
            
            theArc = new Semicircle(refCircle, ptsOnCircle[0], ptsOnCircle[1], midpt, new Segment(ptsOnCircle[0], ptsOnCircle[1]));
            newAtoms.Add(new ShapeAtomicRegion(new Sector(theArc)));

            theArc = new Semicircle(refCircle, ptsOnCircle[0], ptsOnCircle[1], refCircle.OppositePoint(midpt), new Segment(ptsOnCircle[0], ptsOnCircle[1]));
            newAtoms.Add(new ShapeAtomicRegion(new Sector(theArc)));

            return newAtoms;
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
        private List<Atomizer.AtomicRegion> ConvertToCircleCircle(Segment chord,
                                                                                              List<Circle> circles,
                                                                                              out Circle leftOuterCircle,
                                                                                              out Circle rightOuterCircle)
        {
            List<Atomizer.AtomicRegion> regions = new List<Atomizer.AtomicRegion>();
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
            List<Circle> leftSide = new List<Circle>();
            List<Circle> rightSide = new List<Circle>();

            // For now, assume max, one circle per side.
            // Construct a collinear list of points that includes all circle centers as well as the single intersection point between the chord and the line passing through all circle centers.
            // This orders the sides and provides implied sizes.

            Segment centerLine = new Segment(circles[0].center, circles[1].center);
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
        private Atomizer.AtomicRegion ConstructBasicLineCircleRegion(Segment chord, Circle circle)
        {
            AtomicRegion region = new AtomicRegion();

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, new MinorArc(circle, chord.Point1, chord.Point2));

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.SEGMENT, chord);

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
        private Atomizer.AtomicRegion ConstructBasicCircleCircleRegion(Segment chord, Circle c1, Circle c2)
        {
            AtomicRegion region = new AtomicRegion();

            MinorArc arc1 = new MinorArc(c1, chord.Point1, chord.Point2);
            MinorArc arc2 = new MinorArc(c2, chord.Point1, chord.Point2);

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, arc1);

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, arc2);

            return region;
        }
    }
}