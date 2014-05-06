using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using System;

namespace GeometryTutorLib.Precomputer
{
    public static class ClauseConstructor
    {
        //
        // Given a series of points, generate all objects associated with segments and InMiddles
        //
        public static List<GroundedClause> GenerateSegmentClauses(Collinear collinear)
        {
            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Generate all Segment and InMiddle objects
            //
            for (int p1 = 0; p1 < collinear.points.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < collinear.points.Count; p2++)
                {
                    Segment newSegment = new Segment(collinear.points[p1], collinear.points[p2]);
                    newClauses.Add(newSegment);
                    for (int imIndex = p1 + 1; imIndex < p2; imIndex++)
                    {
                        newClauses.Add(new InMiddle(collinear.points[imIndex], newSegment));
                    }
                }
            }

            return newClauses;
        }

        //
        // Given a series of points, generate all objects associated with segments and InMiddles
        //
        public static List<GroundedClause> GenerateAngleIntersectionPolygonClauses(List<GroundedClause> clauses, bool problemIsOn)
        {
            List<GroundedClause> newClauses = new List<GroundedClause>();

            // Find all the Segment and Point objects
            List<Segment> segments = new List<Segment>();
            List<Point> points = new List<Point>();
            foreach (GroundedClause clause in clauses)
            {
                if (clause is Segment) segments.Add(clause as Segment);
                if (clause is Point) points.Add(clause as Point);
            }

            // If points are unnamed, find them and add them to the list
            //points.AddRange(GenerateAllImpliedSegmentSegmentIntersectionPoints(segments, points));

            //
            // Generate all polygons, angles, and intersections created by segments
            //
            List<Quadrilateral> newQuadrilaterals = new List<Quadrilateral>(); // GenerateQuadrilateralClauses(clauses, segments);
            List<Triangle> newTriangles = GenerateTriangleClauses(clauses, segments);
            List<Intersection> newIntersections = GenerateIntersectionClauses(newQuadrilaterals, newTriangles, segments, points);
            List<Angle> newAngles = GenerateAngleClauses(newIntersections);

            newAngles.ForEach(angle => newClauses.Add(angle));
            newIntersections.ForEach(inter => newClauses.Add(inter));
            newTriangles.ForEach(tri => newClauses.Add(tri));
            newQuadrilaterals.ForEach(quad => newClauses.Add(quad));

            if (problemIsOn && GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("----------------------------------------");
                foreach (GroundedClause gc in newClauses)
                {
                    System.Diagnostics.Debug.WriteLine(gc.ToString());
                }
            }

            return newClauses;
        }

        //
        // Add a point (uniquely) to the given list
        //
        private static Point AddImpliedPoint(List<Point> implied, Point p)
        {
            Point figPoint = Point.GetFigurePoint(p);

            // If we have the point, don't generate it.
            if (figPoint != null) return figPoint;

            Point newPoint = PointFactory.GeneratePoint(p.X, p.Y);

            // Add to this uniquely (based on structural equivalence)
            bool found = false;
            foreach (Point impPt in implied)
            {
                if (impPt.StructurallyEquals(newPoint))
                {
                    found = true;
                    break;
                }
            }
            if (!found) implied.Add(newPoint);

            return newPoint;
        }

        //
        // We need to know all points of intersections of all segments; if points are unnamed, add them to the list
        //
        private static List<Point> GenerateAllImpliedSegmentSegmentIntersectionPoints(List<Segment> segments, List<Point> points)
        {
            List<Point> impliedPoints = new List<Point>();

            //
            // Check all combinations of segments.
            //
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    // Get the intersection and see if we have that point.
                    Point intersection = segments[s1].FindIntersection(segments[s2]);

                    if (intersection != null) AddImpliedPoint(impliedPoints, intersection);
                }
            }

            return impliedPoints;
        }

        //
        // We need to know all points of intersections between circles and segments; if points are unnamed, add them to the list.
        //
        private static List<GroundedClause> GenerateAllCircleSegmentIntersectionAndPoints(List<Segment> segments, List<Circle> circles, List<Point> points)
        {
            List<Point> impliedPoints = new List<Point>();
            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Check all combinations of circles / segments.
            //
            foreach (Circle circle in circles)
            {
                foreach (Segment segment in segments)
                {
                    // Get the intersection and see if we have an actual intersection
                    Point pt1 = null;
                    Point pt2 = null;

                    circle.FindIntersection(segment, out pt1, out pt2);

                    if (pt1 != null) pt1 = AddImpliedPoint(impliedPoints, pt1);
                    if (pt2 != null) pt2 = AddImpliedPoint(impliedPoints, pt2);

                    // Check tangent
                    if (pt1 != null && pt2 == null)
                    {
                        if (pt2 != null) newClauses.Add(new ArcSegmentIntersection(pt2, newArc, segment));
                    }
                    else
                    {
                        // Generate the arc
                        MinorArc newArc = new MinorArc(circle, pt1, pt2);
                        newClauses.Add(newArc);

                        // There may be two new intersections between segment and circle
                        if (pt1 != null) newClauses.Add(new ArcSegmentIntersection(pt1, newArc, segment));
                        if (pt2 != null) newClauses.Add(new ArcSegmentIntersection(pt2, newArc, segment));
                    }
                }
            }

            impliedPoints.ForEach(point => newClauses.Add(point));

            return newClauses;
        }

        //
        // We need to know all points of intersections between circles and segments; if points are unnamed, add them to the list.
        //
        private static List<Point> GenerateAllImpliedCircleCircleIntersectionPoints(List<Circle> circles, List<Point> points)
        {
            List<Point> impliedPoints = new List<Point>();

            //
            // Check all combinations of segments.
            //
            for (int c1 = 0; c1 < circles.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < circles.Count; c2++)
                {
                    // Get the intersection and see if we have an actual intersection
                    Point pt1 = null;
                    Point pt2 = null;

                    circles[c1].FindIntersection(circles[c2], out pt1, out pt2);

                    if (pt1 != null) pt1 = AddImpliedPoint(impliedPoints, pt1);
                    if (pt2 != null) pt2 = AddImpliedPoint(impliedPoints, pt2);

                    // Check tangent
                    if (pt1 != null && pt2 == null)
                    {
                    }
                    else
                    {
                        // Generate the arc
                        MinorArc newArc1 = new MinorArc(circles[c1], pt1, pt2);
                        MinorArc newArc2 = new MinorArc(circles[c2], pt1, pt2);
                        newClauses.Add(newArc1);
                        newClauses.Add(newArc2);

                        // There may be two new intersections between segment and circle
                        if (pt1 != null) newClauses.Add(new ArcSegmentIntersection(pt1, newArc, segment));
                        if (pt2 != null) newClauses.Add(new ArcSegmentIntersection(pt2, newArc, segment));
                    }
                }
            }

            return impliedPoints;
        }

        public List<GroundedClause> GenerateAllImpliedCircleClauses(List<GroundedClause> clauses, bool problemIsOn)
        {
            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Find all the Segment, Circle, and Point objects
            //
            List<Segment> segments = new List<Segment>();
            List<Point> points = new List<Point>();
            List<Circle> circles = new List<Circle>();
            foreach (GroundedClause clause in clauses)
            {
                if (clause is Segment) segments.Add(clause as Segment);
                else if (clause is Point) points.Add(clause as Point);
                else if (clause is Circle) circles.Add(clause as Circle);
            }

            //
            // Find all the implied points due to intersectons as well as 
            //


            // Generate all clauses among a circle and a segment


            return newClauses;
        }

        //
        // Generate all Triangle clauses based on segments
        //
        public static List<Triangle> GenerateTriangleClauses(List<GroundedClause> clauses, List<Segment> segments)
        {
            List<Triangle> newTriangles = new List<Triangle>();
            for (int s1 = 0; s1 < segments.Count - 2; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count - 1; s2++)
                {
                    Point vertex1 = segments[s1].SharedVertex(segments[s2]);
                    if (vertex1 != null)
                    {
                        for (int s3 = s2 + 1; s3 < segments.Count; s3++)
                        {
                            Point vertex2 = segments[s3].SharedVertex(segments[s1]);
                            Point vertex3 = segments[s3].SharedVertex(segments[s2]);
                            if (vertex2 != null && vertex3 != null)
                            {
                                // Vertices must be distinct
                                if (!vertex1.Equals(vertex2) && !vertex1.Equals(vertex3) && !vertex2.Equals(vertex3))
                                {
                                    // Vertices must be non-collinear
                                    Segment side1 = new Segment(vertex1, vertex2);
                                    Segment side2 = new Segment(vertex2, vertex3);
                                    Segment side3 = new Segment(vertex1, vertex3);
                                    if (!side1.IsCollinearWith(side2))
                                    {
                                        // Construct the triangle based on the sides to ensure reflexivity clauses are generated
                                        newTriangles.Add(new Triangle(ClauseConstructor.GetProblemSegment(clauses, side1), ClauseConstructor.GetProblemSegment(clauses, side2), ClauseConstructor.GetProblemSegment(clauses, side3)));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return newTriangles;
        }

        //
        // Generate all Quadrilateral clauses based on segments
        //
        public static List<Quadrilateral> GenerateQuadrilateralClauses(List<GroundedClause> clauses, List<Segment> segments)
        {
            List<Quadrilateral> newQuads = new List<Quadrilateral>();

            if (segments.Count < 4) return newQuads;

            for (int s1 = 0; s1 < segments.Count - 3; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count - 2; s2++)
                {
                    for (int s3 = s2 + 1; s3 < segments.Count - 1; s3++)
                    {
                        for (int s4 = s3 + 1; s4 < segments.Count; s4++)
                        {
                            Quadrilateral quad = GenerateQuadrilateral(segments[s1], segments[s2], segments[s3], segments[s4]);
                            if (quad != null) Utilities.AddUnique<Quadrilateral>(newQuads, quad);
                        }
                    }
                }
            }

            return newQuads;
        }

        //
        // generate a Quadrilateral object, if the 4 segments construct a valid quadrilateral.
        //
        private static Quadrilateral GenerateQuadrilateral(Segment s1, Segment s2, Segment s3, Segment s4)
        {
            //    ____
            //   |
            //   |____
            // Check a C shape of 3 segments; the 4th needs to be opposite 
            Segment top;
            Segment bottom;
            Segment left = AcquireMiddleSegment(s1, s2, s3, out top, out bottom);

            // Check C for the top, bottom, and right sides
            if (left == null) return null;

            Segment right = s4;

            Segment tempOut1, tempOut2;
            Segment rightMid = AcquireMiddleSegment(top, bottom, right, out tempOut1, out tempOut2);

            // The middle segment we acquired must match the 4th segment
            if (!right.StructurallyEquals(rightMid)) return null;

            //
            // The top / bottom cannot cross; bowtie or hourglass shape
            // A valid quadrilateral will have the intersections outside of the quad, that is defined
            // by the order of the three points: intersection and two endpts of the side
            //
            Point intersection = top.FindIntersection(bottom);

            // Check for parallel lines, then in-betweenness
            if (intersection != null && !double.IsNaN(intersection.X) && !double.IsNaN(intersection.Y))
            {
                if (Segment.Between(intersection, top.Point1, top.Point2)) return null;
                if (Segment.Between(intersection, bottom.Point1, bottom.Point2)) return null;
            }

            // The left / right cannot cross; bowtie or hourglass shape
            intersection = left.FindIntersection(right);

            // Check for parallel lines, then in-betweenness
            if (intersection != null && !double.IsNaN(intersection.X) && !double.IsNaN(intersection.Y))
            {
                if (Segment.Between(intersection, left.Point1, left.Point2)) return null;
                if (Segment.Between(intersection, right.Point1, right.Point2)) return null;
            }

            //
            // Verify that we have 4 unique points; And not different shapes (like a star, or triangle with another segment)
            //
            List<Point> pts = new List<Point>();
            pts.Add(left.SharedVertex(top));
            pts.Add(left.SharedVertex(bottom));
            pts.Add(right.SharedVertex(top));
            pts.Add(right.SharedVertex(bottom));
            for (int i = 0; i < pts.Count - 1; i++)
            {
                for (int j = i + 1; j < pts.Count; j++)
                {
                    if (pts[i].StructurallyEquals(pts[j]))
                    {
                        return null;
                    }
                }
            }

            return new Quadrilateral(left, right, top, bottom);
        }

        //            top
        // shared1  _______   off1
        //         |
        //   mid   |
        //         |_________   off2
        //            bottom
        private static Segment AcquireMiddleSegment(Segment seg1, Segment seg2, Segment seg3, out Segment top, out Segment bottom)
        {
            if (seg1.SharedVertex(seg2) != null && seg1.SharedVertex(seg3) != null)
            {
                top = seg2;
                bottom = seg3;
                return seg1;
            }

            if (seg2.SharedVertex(seg1) != null && seg2.SharedVertex(seg3) != null)
            {
                top = seg1;
                bottom = seg3;
                return seg2;
            }

            if (seg3.SharedVertex(seg1) != null && seg3.SharedVertex(seg2) != null)
            {
                top = seg1;
                bottom = seg2;
                return seg3;
            }

            top = null;
            bottom = null;

            return null;
        }

        //
        // Generate all covering intersection clauses; that is, generate maximal intersections (a subset of all intersections)
        //
        public static List<Intersection> GenerateIntersectionClauses(List<Quadrilateral> quadrilaterals, List<Triangle> triangles, List<Segment> segments, List<Point> points)
        {
            List<Intersection> newIntersections = new List<Intersection>();

            //
            // Each quad has 4 valid intersections + 1 diagonal intersections ONLY if the diagonals exist + 1 median (for trapezoids)
            //
            foreach (Quadrilateral quad in quadrilaterals)
            {
                AddIntersection(newIntersections, new Intersection(quad.topLeft, quad.left, quad.top));
                AddIntersection(newIntersections, new Intersection(quad.topRight, quad.right, quad.top));
                AddIntersection(newIntersections, new Intersection(quad.bottomLeft, quad.left, quad.bottom));
                AddIntersection(newIntersections, new Intersection(quad.bottomRight, quad.bottom, quad.right));

                if (GetMaximalProblemSegment(segments, quad.bottomLeftTopRightDiagonal) != null)
                {
                    quad.SetBottomRightDiagonalInValid();
                }

                if (GetMaximalProblemSegment(segments, quad.topLeftBottomRightDiagonal) != null)
                {
                    quad.SetTopLeftDiagonalInValid();
                }

                // If both diagonals exist in the figure, create an intersection and provide the quadrilateral with the intersection.
                if (quad.TopLeftDiagonalIsValid() && quad.BottomRightDiagonalIsValid())
                {
                    int actInterIndex = points.IndexOf(quad.bottomLeftTopRightDiagonal.FindIntersection(quad.topLeftBottomRightDiagonal));

                    if (actInterIndex == -1)
                    {
                        System.Diagnostics.Debug.WriteLine("CTA: Cannot findintersection point of diagonals..." + quad);
                    }
                    else
                    {
                        Intersection diagInter = new Intersection(points[actInterIndex], quad.bottomLeftTopRightDiagonal, quad.topLeftBottomRightDiagonal);

                        AddIntersection(newIntersections, diagInter);

                        quad.SetIntersection(diagInter);
                    }
                }
            }

            //
            // Each triangle has 3 valid intersections
            //
            foreach (Triangle triangle in triangles)
            {
                Point vertex = triangle.SegmentA.SharedVertex(triangle.SegmentB);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentA, triangle.SegmentB));

                vertex = triangle.SegmentB.SharedVertex(triangle.SegmentC);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentB, triangle.SegmentC));

                vertex = triangle.SegmentA.SharedVertex(triangle.SegmentC);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentA, triangle.SegmentC));
            }

            //
            // Find the maximal segments (remove all sub-segments from the list)
            //
            List<Segment> maximalSegments = new List<Segment>();
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool isSubsegment = false;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s2].HasSubSegment(segments[s1]))
                        {
                            isSubsegment = true;
                            break;
                        }
                    }
                }
                if (!isSubsegment) maximalSegments.Add(segments[s1]);
            }

            //
            // Acquire all intersections from the maximal segment list
            //
            for (int s1 = 0; s1 < maximalSegments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < maximalSegments.Count; s2++)
                {
                    // An intersection should not be between collinear segments
                    if (!maximalSegments[s1].IsCollinearWith(maximalSegments[s2]))
                    {
                        // The point must be 'between' both segment endpoints
                        Point numericInter = maximalSegments[s1].FindIntersection(maximalSegments[s2]);
                        if (maximalSegments[s1].PointIsOnAndBetweenEndpoints(numericInter) &&
                            maximalSegments[s2].PointIsOnAndBetweenEndpoints(numericInter))
                        {
                            // Find the actual point for which there is an intersection between the segments
                            Point actualInter = null;
                            foreach (Point pt in points)
                            {
                                if (numericInter.StructurallyEquals(pt))
                                {
                                    actualInter = pt;
                                    break;
                                }
                            }

                            // Create the intersection
                            if (actualInter != null)
                            {
                                AddIntersection(newIntersections, new Intersection(actualInter, maximalSegments[s1], maximalSegments[s2]));
                            }
                        }
                    }
                }
            }

            return newIntersections;
        }

        //
        // Generate all angles based on the intersections
        //
        public static List<Angle> GenerateAngleClauses(List<Intersection> intersections)
        {
            List<Angle> newAngles = new List<Angle>();

            foreach (Intersection inter in intersections)
            {
                // 1 angle
                if (inter.StandsOnEndpoint())
                {
                    AddAngle(newAngles, (new Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect))));
                }
                // 2 angles
                else if (inter.StandsOn())
                {
                    Point up = null;
                    Point left = null;
                    Point right = null;
                    if (inter.lhs.HasPoint(inter.intersect))
                    {
                        up = inter.lhs.OtherPoint(inter.intersect);
                        left = inter.rhs.Point1;
                        right = inter.rhs.Point2;
                    }
                    else
                    {
                        up = inter.rhs.OtherPoint(inter.intersect);
                        left = inter.lhs.Point1;
                        right = inter.lhs.Point2;
                    }

                    AddAngle(newAngles, new Angle(left, inter.intersect, up));
                    AddAngle(newAngles, new Angle(right, inter.intersect, up));
                }
                // 4 angles
                else
                {
                    AddAngle(newAngles, new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                    AddAngle(newAngles, new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                    AddAngle(newAngles, new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                    AddAngle(newAngles, new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
                }
            }

            return newAngles;
        }

        //
        // Generate all arcs based on circles and points (like collinearity with lines)
        //
        public static List<GroundedClause> GenerateArcClauses(List<GroundedClause> clauses)
        {
            List<Point> points = new List<Point>();
            List<Circle> circles = new List<Circle>();

            //
            // Acquire the points and circles
            //
            foreach (GroundedClause clause in clauses)
            {
                if (clause is Point) points.Add(clause as Point);
                if (clause is Circle) circles.Add(clause as Circle);
            }

            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Find the points that are on the given circle; generate the arcs
            //
            foreach (Circle circle in circles)
            {
                List<Point> pointsOnCircle = new List<Point>();
                foreach (Point point in points)
                {
                    if (circle.PointIsOn(point)) pointsOnCircle.Add(point);
                }
                newClauses.AddRange(GenerateArcClauses(circle, pointsOnCircle));
            }

            return newClauses;
        }

        //
        // Generate all of the Arc and ArcInMiddle clauses; similar to generating for collinear points on segments.
        //
        public static List<GroundedClause> GenerateArcClauses(Circle circle, List<Point> pts)
        {
            List<Point> ordered = OrderPoints(circle, pts);
            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Generate all Arc objects with their minor / major arc points; also generate ArcInMiddle clauses.
            //
            for (int p1 = 0; p1 < ordered.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < ordered.Count; p2++)
                {
                    // All points x, from p1 < x < p2, are arc minor points
                    // All points x, from x < p1 or x > p2, are arc major points
                    List<Point> minorArcPoints;
                    List<Point> majorArcPoints;
                    PartitionArcPoints(ordered, p1, p2, out minorArcPoints, out majorArcPoints);

                    MinorArc newArc = new MinorArc(circle, ordered[p1], ordered[p2], minorArcPoints, majorArcPoints);
                    newClauses.Add(newArc);

                    // Generate ArcInMiddle clauses.
                    for (int imIndex = p1 + 1; imIndex < p2; imIndex++)
                    {
                        newClauses.Add(new ArcInMiddle(ordered[imIndex], newArc));
                    }
                }
            }

            return newClauses;
        }

        //
        // All points x, from p1 < x < p2, are arc minor points
        // All points x, from x < p1 or x > p2, are arc major points
        //
        private static void PartitionArcPoints(List<Point> points, int endpt1, int endpt2, out List<Point> minorArcPoints, out List<Point> majorArcPoints)
        {
            minorArcPoints = new List<Point>();
            majorArcPoints = new List<Point>();

            // Traverse list and add to the appropriate list
            for (int i = 0; i < points.Count; i++)
            {
                if (endpt1 < i && i < endpt2) minorArcPoints.Add(points[i]);
                else if (i < endpt1 || i > endpt2) majorArcPoints.Add(points[i]);
                // else i == enpt1 || i == endpt2
            }
        }

        //
        // For arcs, order the points so that there is a consistency: A, B, C, D-> B between AC, B between AD, etc.
        // Only need to order the points if there are more than three points
        //
        private static List<Point> OrderPoints(Circle circle, List<Point> points)
        {
            List<KeyValuePair<double, Point>> pointAngleMap = new List<KeyValuePair<double, Point>>();

            // Issues with ordering arise with 4 or more points (due to symmetry).
            if (points.Count < 4) return points;

            foreach (Point point in points)
            {
                double deltaX = point.X - circle.center.X;
                double deltaY = point.Y - circle.center.Y;

                double radianAngle = Math.Atan2(deltaY, deltaX);

                // Find the correct quadrant the point lies in to find the exact angle (w.r.t. unit circle)
                // fourth quadrant
                if (deltaX > 0 && deltaY < 0) radianAngle += 2 * Math.PI;
                // second  or third quadrant
                if (deltaX < 0) radianAngle += Math.PI;
                // on the Y-axis (below x-axis)
                if (Utilities.CompareValues(deltaX, 0) && deltaY < 0) radianAngle += Math.PI;

                // Angles are between 0 and 2pi
                // insert the point into the correct position (starting from the back); insertion sort-style
                int index;
                for (index = 0; index < pointAngleMap.Count; index++)
                {
                    if (radianAngle > pointAngleMap[index].Key) break;
                }
                pointAngleMap.Insert(index, new KeyValuePair<double, Point>(radianAngle, point));
            }

            // Put all the points in the ordered list
            List<Point> ordered = new List<Point>();
            foreach (KeyValuePair<double, Point> pair in pointAngleMap)
            {
                ordered.Add(pair.Value);
            }

            return ordered;
        }

        // Add an angle to the list uniquely
        public static void AddAngle(List<Angle> angles, Angle thatAngle)
        {
            if (thatAngle.measure == 0 || thatAngle.measure == 180) return;

            foreach (Angle thisAngle in angles)
            {
                if (thisAngle.Equates(thatAngle)) return;
            }

            angles.Add(thatAngle);
        }

        // Add an intersection to the list uniquely
        public static void AddIntersection(List<Intersection> intersections, Intersection thatInter)
        {
            foreach (Intersection inter in intersections)
            {
                if (inter.StructurallyEquals(thatInter)) return;
            }

            intersections.Add(thatInter);
        }

        public static Segment GetProblemSegment(List<GroundedClause> clauses, Segment thatSegment)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatSegment)) return clause as Segment;
            }

            return null;
        }

        //
        // Acquire the exact segment if it exists...otherwise return the maximal segment
        //
        public static Segment GetMaximalProblemSegment(List<Segment> segments, Segment thatSegment)
        {
            // Exact segment
            foreach (Segment segment in segments)
            {
                if (segment.StructurallyEquals(thatSegment)) return segment;
            }

            // Maximal Segment
            foreach (Segment segment in segments)
            {
                if (segment.HasSubSegment(thatSegment)) return segment;
            }

            return null;
        }

        // Acquire an established angle
        public static Angle GetProblemAngle(List<GroundedClause> clauses, Angle thatAngle)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause is Angle)
                {
                    if (clause.StructurallyEquals(thatAngle)) return clause as Angle;
                }
            }

            return null;
        }

        // Acquire an established triangle
        public static Triangle GetProblemTriangle(List<GroundedClause> clauses, Triangle thatTriangle)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatTriangle)) return clause as Triangle;
            }

            return null;
        }

        // Acquire an established intersection
        public static Intersection GetProblemIntersection(List<GroundedClause> clauses, Segment segment1, Segment segment2)
        {
            foreach (GroundedClause clause in clauses)
            {
                Intersection inter = clause as Intersection;
                if (inter != null)
                {
                    if (inter.HasSegment(segment1) && inter.HasSegment(segment2)) return inter;
                }
            }

            return null;
        }

        // Acquire an established InMiddle
        public static InMiddle GetProblemInMiddle(List<GroundedClause> clauses, Point p, Segment segment)
        {
            foreach (GroundedClause clause in clauses)
            {
                InMiddle im = clause as InMiddle;
                if (im != null)
                {
                    if (im.point.StructurallyEquals(p) && im.segment.StructurallyEquals(segment)) return im;
                }
            }

            return null;
        }
    }
}