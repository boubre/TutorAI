﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DrawingParser
    {
        private const double EPSILON_ANGLE = 0.01;

        private Drawing drawing;

        private Dictionary<IFigure, GroundedClause> parsed;

        public List<Point> Points { get; private set; }
        public List<TempSegment> TempSegs { get; private set; }
        public List<Collinear> Collinear { get; private set; }
        public List<Triangle> Triangles { get; private set; }
        public List<Intersection> Intersections { get; private set; }
        public List<Angle> Angles { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Circle> Circles { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.CircleSegmentIntersection> CircleSegmentIntersections { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.CircleCircleIntersection> CircleCircleIntersections { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.SegmentBisector> SegmentBisectors { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.AngleBisector> AngleBisectors { get; private set; }

        /// <summary>
        /// Create a new Drawing Parser.
        /// </summary>
        /// <param name="drawing">The drawing to parse.</param>
        /// <param name="parseController">The parseController, used to add disambiguation dialogs.</param>
        public DrawingParser(Drawing drawing)
        {
            this.drawing = drawing;

            parsed = new Dictionary<IFigure, GroundedClause>();

            Points = new List<Point>();
            TempSegs = new List<TempSegment>();
            Collinear = new List<Collinear>();
            Triangles = new List<Triangle>();
            Intersections = new List<Intersection>();
            Angles = new List<Angle>();
            Circles = new List<GeometryTutorLib.ConcreteAST.Circle>();
            CircleSegmentIntersections = new List<CircleSegmentIntersection>();
            CircleCircleIntersections = new List<CircleCircleIntersection>();
            SegmentBisectors = new List<GeometryTutorLib.ConcreteAST.SegmentBisector>();
            AngleBisectors = new List<GeometryTutorLib.ConcreteAST.AngleBisector>();
        }

        public void Parse()
        {
            ParseDrawing();
            removeDuplicateSegments();
            calculateCollinear();
            calculateIntersections();
            calculateAngles();
            removeDuplicateAngles();
            calculateSegmentBisectors();
            calculateAngleBisectors();
            //calculateCircleSegmentIntersections();
            //calculateCircleIntersections();
        }

        /// <summary>
        /// Parse the basic Figure in the drawing.
        /// </summary>
        private void ParseDrawing()
        {
            foreach (IFigure figure in drawing.Figures)
                parse(figure);
        }

        /// <summary>
        /// Return clauses that should be passed to the back-end.
        /// </summary>
        /// <returns>Back-end input.</returns>
        public List<GroundedClause> GetIntrinsics()
        {
            List<GroundedClause> intrinsic = new List<GroundedClause>();
            Points.ForEach((Point p) => intrinsic.Add(p));
            Collinear.ForEach((Collinear c) => intrinsic.Add(c));
            Triangles.ForEach((Triangle t) => intrinsic.Add(t));

            return intrinsic;
        }

        /// <summary>
        /// Remove duplicate instances of segments.
        /// </summary>
        private void removeDuplicateSegments()
        {
            TempSegment[] segs = TempSegs.ToArray();
            List<int> duplicates = new List<int>();

            //Search through the segments and mark duplicate indicies.
            for (int i = 0; i < segs.Length - 1; i++)
            {
                if (!duplicates.Contains(i))
                {
                    for (int j = i + 1; j < segs.Length; j++)
                    {
                        if (duplicates.Contains(j)) { }
                        else if ((segs[i].A == segs[j].A && segs[i].B == segs[j].B) || (segs[i].A == segs[j].B && segs[i].B == segs[j].A))
                        {
                            duplicates.Add(j);
                        }
                    }
                }
            }

            //Recreate the segements list, adding only unique instances.
            TempSegs = new List<TempSegment>();
            for (int i = 0; i < segs.Length; i++)
            {
                if (!duplicates.Contains(i))
                {
                    TempSegs.Add(segs[i]);
                }
            }
        }

        /// <summary>
        /// Calculate collinear points.
        /// </summary>
        private void calculateCollinear()
        {
            //See if points lie in the middle of existing segments.
            foreach (Point p in Points)
            {
                foreach (TempSegment s in TempSegs)
                {
                    if (s.A == p || s.B == p) { } //next iteration
                    else if (isInMiddle(s.A, p, s.B))
                    {
                        s.AddCollinear(p);
                    }
                }
            }

            //Create the actual collinear statements.
            foreach (TempSegment s in TempSegs)
            {
                Collinear.Add(new Collinear(s.GetCollinear()));
            }
        }

        /// <summary>
        /// Tests to see if point b is in the middle of points a and c.
        /// </summary>
        /// <param name="a">A point</param>
        /// <param name="b">A point</param>
        /// <param name="c">A point</param>
        /// <returns>TRUE if b is in the middle of b and c</returns>
        private bool isInMiddle(Point a, Point b, Point c)
        {
            double xProd = (c.Y - a.Y) * (b.X - a.X) - (c.X - a.X) * (b.Y - a.Y);
            if (System.Math.Abs(xProd) > 0) return false;

            double dotProd = (c.X - a.X) * (b.X - a.X) + (c.Y - a.Y) * (b.Y - a.Y);
            if (dotProd < 0) return false;

            double squaredLenAB = System.Math.Pow(b.X - a.X, 2) + System.Math.Pow(b.Y - a.Y, 2);
            if (dotProd < squaredLenAB) return false;

            return true;
        }

        /// <summary>
        /// Calculate intersections in order to find angles.
        /// </summary>
        private void calculateIntersections()
        {
            //Each triangle has 3 intersections, 1 at each vertex.
            foreach (Triangle t in Triangles)
            {
                Point vertex = t.SegmentA.SharedVertex(t.SegmentB);
                Intersections.Add(new Intersection(vertex, t.SegmentA, t.SegmentB));

                vertex = t.SegmentB.SharedVertex(t.SegmentC);
                Intersections.Add(new Intersection(vertex, t.SegmentB, t.SegmentC));

                vertex = t.SegmentA.SharedVertex(t.SegmentC);
                Intersections.Add(new Intersection(vertex, t.SegmentA, t.SegmentC));
            }

            //Find the maximal segments.
            List<GeometryTutorLib.ConcreteAST.Segment> maximalSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<GeometryTutorLib.ConcreteAST.Segment> segments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            TempSegs.ForEach((TempSegment s) => segments.Add(new GeometryTutorLib.ConcreteAST.Segment(s.A, s.B)));
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

            //Acquire all intersections from the maximal segment list
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
                            foreach (Point pt in Points)
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
                                Intersections.Add(new Intersection(actualInter, maximalSegments[s1], maximalSegments[s2]));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all angles in the drawing.
        /// </summary>
        private void calculateAngles()
        {
            foreach (Intersection inter in Intersections)
            {
                // 1 angle
                if (inter.StandsOnEndpoint())
                {
                    Angles.Add(new Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect)));
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

                    Angles.Add(new Angle(left, inter.intersect, up));
                    Angles.Add(new Angle(right, inter.intersect, up));
                }
                // 4 angles
                else
                {
                    Angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                    Angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                    Angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                    Angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
                }
            }
        }

        /// <summary>
        /// Remove duplicate instances of angles.
        /// </summary>
        private void removeDuplicateAngles()
        {
            Angle[] angles = this.Angles.ToArray();
            List<int> duplicates = new List<int>();

            //Search through the angles...
            for (int i = 0; i < angles.Length - 1; i++)
            {
                if (!duplicates.Contains(i))
                {
                    //... and compare them to the other angles, marking duplicates.
                    for (int j = i + 1; j < angles.Length; j++)
                    {
                        if (duplicates.Contains(j)) { }
                        else if (angles[i].Equals(angles[j]))
                        {
                            duplicates.Add(j);
                        }
                    }
                }
            }

            //Recreate the angles list, ignoring duplicate entries.
            this.Angles = new List<Angle>();
            for (int i = 0; i < angles.Length; i++)
            {
                if (!duplicates.Contains(i))
                {
                    this.Angles.Add(angles[i]);
                }
            }
        }

        /// <summary>
        /// Calculate segment bisctors.
        /// </summary>
        private void calculateSegmentBisectors()
        {
            //Check each intersetion...
            foreach (Intersection i in Intersections)
            {
                //... and create two new lines for each segment split by the point of intersection.
                var lhs1 = new GeometryTutorLib.ConcreteAST.Segment(i.lhs.Point1, i.intersect);
                var lhs2 = new GeometryTutorLib.ConcreteAST.Segment(i.intersect, i.lhs.Point2);
                var rhs1 = new GeometryTutorLib.ConcreteAST.Segment(i.rhs.Point1, i.intersect);
                var rhs2 = new GeometryTutorLib.ConcreteAST.Segment(i.intersect, i.rhs.Point2);

                //Is the lhs bisected by rhs?
                if (lhs1.Length == lhs2.Length)
                {
                    SegmentBisectors.Add(new GeometryTutorLib.ConcreteAST.SegmentBisector(i, i.rhs));
                }

                //Is the rhs bisected by lhs?
                if (rhs1.Length == rhs2.Length)
                {
                    SegmentBisectors.Add(new GeometryTutorLib.ConcreteAST.SegmentBisector(i, i.lhs));
                }
            }
        }

        /// <summary>
        /// Calculate angle bisectors.
        /// </summary>
        private void calculateAngleBisectors()
        {
            //Check each angle...
            foreach (Angle a in Angles)
            {
                //... and see if a segment passes through point B of the angle.
                foreach (TempSegment ts in TempSegs)
                {
                    GeometryTutorLib.ConcreteAST.Segment s = new GeometryTutorLib.ConcreteAST.Segment(ts.A, ts.B);
                    if (s.PointIsOnAndBetweenEndpoints(a.B))
                    {
                        //Create new angles with this segment and see if they are the same measure
                        Angle a1 = new Angle(a.A, a.B, ts.A);
                        Angle a2 = new Angle(a.C, a.B, ts.A);
                        if (System.Math.Abs(a1.measure - a2.measure) < EPSILON_ANGLE)
                        {
                            //We found an angle bisector!
                            AngleBisectors.Add(new GeometryTutorLib.ConcreteAST.AngleBisector(a, s));
                        }
                    }
                }
            }
        }

        //public int csiPointName = 0;
        ///// <summary>
        ///// Calculate when a circle intersects a segment.
        ///// </summary>
        //private void calculateCircleSegmentIntersections()
        //{
        //    //Check each circle...
        //    foreach (GeometryTutorLib.ConcreteAST.Circle circ in Circles)
        //    {
        //        //... with each segment and see if they intersect.
        //        foreach (TempSegment ts in TempSegs)
        //        {
        //            GeometryTutorLib.ConcreteAST.Segment s = new GeometryTutorLib.ConcreteAST.Segment(ts.A, ts.B);

        //            //SEE: http://stackoverflow.com/questions/1073336/circle-line-collision-detection

        //            //We have line AB, cicle center C, and radius R.
        //            double lengthAB = s.Length;
        //            double[] D = { (ts.B.X - ts.A.X) / lengthAB, (ts.B.Y - ts.A.Y) / lengthAB }; //Direction vector from A to B

        //            //Now the line equation is x = D[0]*t + A.X, y = D[1]*t + A.Y with 0 <= t <= 1.
        //            double t = D[0] * (circ.center.X - ts.A.X) + D[1] * (circ.center.Y - ts.A.Y); //Closest point to circle center
        //            double[] E = { t * D[0] + ts.A.X, t * D[1] + ts.A.Y }; //The point described by t.

        //            double lengthEC = System.Math.Sqrt(System.Math.Pow(E[0] - circ.center.X, 2) + System.Math.Pow(E[1] - circ.center.Y, 2));

        //            if (lengthEC < circ.radius) //Possible Intersection?
        //            {
        //                //Compute distance from t to circle intersection point
        //                double dt = System.Math.Sqrt(System.Math.Pow(circ.radius, 2) - System.Math.Pow(lengthEC, 2));

        //                //First intersection
        //                GeometryTutorLib.ConcreteAST.Point p1 = new GeometryTutorLib.ConcreteAST.Point("csiPt" + csiPointName++, (t - dt) * D[0] + ts.A.X, (t - dt) * D[1] + ts.A.Y);
        //                if (s.PointIsOnAndBetweenEndpoints(p1))
        //                {
        //                    CircleSegmentIntersections.Add(new CircleSegmentIntersection(p1, circ, s));
        //                }

        //                //Second intersection
        //                GeometryTutorLib.ConcreteAST.Point p2 = new GeometryTutorLib.ConcreteAST.Point("csiPt" + csiPointName++, (t + dt) * D[0] + ts.A.X, (t + dt) * D[1] + ts.A.Y);
        //                if (s.PointIsOnAndBetweenEndpoints(p2))
        //                {
        //                    CircleSegmentIntersections.Add(new CircleSegmentIntersection(p2, circ, s));
        //                }
        //            }
        //            else if (lengthEC == circ.radius) //E is tangent point
        //            {
        //                GeometryTutorLib.ConcreteAST.Point p = new GeometryTutorLib.ConcreteAST.Point("csiPt" + csiPointName++, E[0], E[1]);
        //                if (s.PointIsOnAndBetweenEndpoints(p))
        //                {
        //                    CircleSegmentIntersections.Add(new CircleSegmentIntersection(p, circ, s));
        //                }
        //            }
        //        }
        //    }
        //}

        //public int ciPointName = 0;
        ///// <summary>
        ///// Calculate when a circle intersects a segment.
        ///// </summary>
        //private void calculateCircleIntersections()
        //{
        //    GeometryTutorLib.ConcreteAST.Circle[] CircleArray = Circles.ToArray();
        //    //Check each circle...
        //    for (int c1 = 0; c1 < CircleArray.Length - 1; c1++)
        //    {
        //        GeometryTutorLib.ConcreteAST.Circle circ1 = CircleArray[c1];
        //        //... with previously uncompared circles and see if they intersect.
        //        for (int c2 = c1 + 1; c2 < CircleArray.Length; c2++)
        //        {
        //            GeometryTutorLib.ConcreteAST.Circle circ2 = CircleArray[c2];

        //            //SEE: http://stackoverflow.com/questions/3349125/circle-circle-intersection-points

        //            double d = System.Math.Sqrt(System.Math.Pow(circ2.center.X - circ1.center.X, 2) + System.Math.Pow(circ2.center.Y - circ1.center.Y, 2)); //Distance between centers

        //            if (d > circ1.radius + circ2.radius) { } //Separate circles
        //            else if (d < System.Math.Abs(circ1.radius - circ2.radius)) { } //One circle contained in the other
        //            else if (d == 0 && circ1.radius == circ2.radius) { } //Coinciding circles
        //            else //We have intersection(s)!
        //            {
        //                double a = (System.Math.Pow(circ1.radius, 2) - System.Math.Pow(circ2.radius, 2) + System.Math.Pow(d, 2)) / (2 * d); //Distance from center of circ1 to midpt of intersections
        //                double[] midpt = { circ1.center.X + a * (circ2.center.X - circ1.center.X) / d, circ1.center.Y + a * (circ2.center.Y - circ1.center.Y) / d }; //midpt of the intersections
        //                double h = System.Math.Sqrt(System.Math.Pow(circ1.radius, 2) - System.Math.Pow(a, 2)); //Distance from midpt to intersections

        //                if (h == 0) //Only one intersection
        //                {
        //                    GeometryTutorLib.ConcreteAST.Point p = new GeometryTutorLib.ConcreteAST.Point("ciPt" + ciPointName++, midpt[0], midpt[1]);
        //                    CircleIntersections.Add(new CircleIntersection(p, circ1, circ2));
        //                }
        //                else //Two intersections
        //                {
        //                    GeometryTutorLib.ConcreteAST.Point p1 = new GeometryTutorLib.ConcreteAST.Point("ciPt" + ciPointName++,
        //                        midpt[0] + h * (circ2.center.Y - circ1.center.Y) / d, midpt[1] - h * (circ2.center.X - circ1.center.X) / d);
        //                    GeometryTutorLib.ConcreteAST.Point p2 = new GeometryTutorLib.ConcreteAST.Point("ciPt" + ciPointName++,
        //                        midpt[0] - h * (circ2.center.Y - circ1.center.Y) / d, midpt[1] + h * (circ2.center.X - circ1.center.X) / d);
        //                    CircleIntersections.Add(new CircleIntersection(p1, circ1, circ2));
        //                    CircleIntersections.Add(new CircleIntersection(p2, circ1, circ2));
        //                }               
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Parse the given figure.
        /// </summary>
        /// <param name="figure">The figure to parse.</param>
        private void parse(IFigure figure)
        {
            if (parsed.ContainsKey(figure)) return;
            else if (figure is IPoint) parse(figure as IPoint);
            else if (figure is ILine) parse(figure as ILine);
            else if (figure is Polygon) parse(figure as PolygonBase);
            else if (figure is RegularPolygon) parse(figure as RegularPolygon);
            else if (figure is CircleBase) parse(figure as CircleBase);
        }

        /// <summary>
        /// Parse a point.
        /// </summary>
        /// <param name="pt">The point to parse.</param>
        private void parse(IPoint pt)
        {
            Point p = new GeometryTutorLib.ConcreteAST.Point(pt.Name, pt.Coordinates.X, pt.Coordinates.Y);
            Points.Add(p);
            parsed.Add(pt, p);
        }

        /// <summary>
        /// Parse a line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        private void parse(ILine line)
        {
            IPoint p1 = line.Dependencies.FindPoint(line.Coordinates.P1, 0);
            IPoint p2 = line.Dependencies.FindPoint(line.Coordinates.P2, 0);
            parse(p1 as IFigure);
            parse(p2 as IFigure);
            GeometryTutorLib.ConcreteAST.Segment s = new GeometryTutorLib.ConcreteAST.Segment(parsed[p1] as GeometryTutorLib.ConcreteAST.Point, parsed[p2] as GeometryTutorLib.ConcreteAST.Point);
            TempSegs.Add(new TempSegment(s.Point1, s.Point2));
            parsed.Add(line, s);
        }

        /// <summary>
        /// Parse a polygon. (Currently only triangles)
        /// </summary>
        /// <param name="pgon">The polygon to parse.</param>
        private void parse(PolygonBase pgon)
        {
            if (pgon.VertexCoordinates.Length == 3)
            {
                //Find verticies
                System.Windows.Point[] pts = pgon.VertexCoordinates;
                IPoint[] iPts = new IPoint[3];
                for (int i = 0; i < 3; i++) //Parse all points
                {
                    iPts[i] = pgon.Dependencies.FindPoint(pts[i], 0);
                    parse(iPts[i] as IFigure);
                }

                //genereate sides
                TempSegment[] sides = new TempSegment[3];
                GeometryTutorLib.ConcreteAST.Segment[] csegs = new GeometryTutorLib.ConcreteAST.Segment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    sides[i] = new TempSegment(parsed[iPts[i]] as GeometryTutorLib.ConcreteAST.Point, parsed[iPts[j]] as GeometryTutorLib.ConcreteAST.Point);
                    csegs[i] = new GeometryTutorLib.ConcreteAST.Segment(sides[i].A, sides[i].B);
                }
                TempSegs.AddRange(sides);

                Triangle t = new GeometryTutorLib.ConcreteAST.Triangle(csegs[0], csegs[1], csegs[2]);
                parsed.Add(pgon, t);
                Triangles.Add(t);
            }
        }

        /// <summary>
        /// Parse a regular polygon. (Currently only equilateral triangles)
        /// </summary>
        /// <param name="rgon">The regular polygon to parse.</param>
        private void parse(RegularPolygon rgon)
        {
            if (rgon.NumberOfSides == 3)
            {
                IPoint center = rgon.Dependencies.FindPoint(rgon.Center, 0);
                IPoint vertex = rgon.Dependencies.FindPoint(rgon.Vertex, 0);

                //Genereate vertex points knowing that the polygon is regular
                IPoint[] pts = new IPoint[3];
                double radius = Math.Distance(vertex.Coordinates.X, center.Coordinates.X, vertex.Coordinates.Y, center.Coordinates.Y);
                double initAngle = Math.GetAngle(
                    new System.Windows.Point(center.Coordinates.X, center.Coordinates.Y),
                    new System.Windows.Point(vertex.Coordinates.X, vertex.Coordinates.Y));
                double increment = Math.DOUBLEPI / 3.0;
                for (int i = 0; i < 2; i++) //Vertex point generation and parsing.
                {
                    double angle = initAngle + (i + 1) * increment;
                    double X = center.Coordinates.X + radius * System.Math.Cos(angle);
                    double Y = center.Coordinates.Y + radius * System.Math.Sin(angle);
                    System.Windows.Point newPt = new System.Windows.Point(X, Y);
                    pts[i] = rgon.Dependencies.FindPoint(newPt, 0); // CTA drawing.FindPoint(newPt, 0);
                    if (pts[i] == null)
                        pts[i] = Factory.CreateFreePoint(drawing, newPt);
                    parse(pts[i] as IFigure);
                }
                pts[2] = vertex;
                parse(pts[2] as IFigure);

                //generate sides from vertices
                TempSegment[] sides = new TempSegment[3];
                GeometryTutorLib.ConcreteAST.Segment[] csegs = new GeometryTutorLib.ConcreteAST.Segment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    sides[i] = new TempSegment(parsed[pts[i]] as GeometryTutorLib.ConcreteAST.Point, parsed[pts[j]] as GeometryTutorLib.ConcreteAST.Point);
                    csegs[i] = new GeometryTutorLib.ConcreteAST.Segment(sides[i].A, sides[i].B);
                }
                TempSegs.AddRange(sides);

                EquilateralTriangle t = new GeometryTutorLib.ConcreteAST.EquilateralTriangle(csegs[0], csegs[1], csegs[2]);
                parsed.Add(rgon, t);
                Triangles.Add(t);
            }
        }

        /// <summary>
        /// Parse a CircleBase.
        /// </summary>
        /// <param name="c"> The circle to parse.</param>
        private void parse(CircleBase cb)
        {
            IPoint center = cb.Dependencies.FindPoint(cb.Center, 0);
            double radius = cb.Radius;

            parse(center as IFigure);
            GeometryTutorLib.ConcreteAST.Circle c = new GeometryTutorLib.ConcreteAST.Circle(parsed[center] as GeometryTutorLib.ConcreteAST.Point, radius);

            parsed.Add(cb, c);
            Circles.Add(c);
        }

        /// <summary>
        /// A lightweight class to represent segments that are used in other computations.
        /// </summary>
        public class TempSegment
        {
            public Point A { get; set; }
            public Point B { get; set; }
            private List<Point> collinear;

            /// <summary>
            /// Create a new TempSegment.
            /// </summary>
            /// <param name="A">An endpoint.</param>
            /// <param name="B">The other endpoint.</param>
            public TempSegment(Point A, Point B)
            {
                this.A = A;
                this.B = B;
                collinear = new List<Point>();
            }

            /// <summary>
            /// Delcare a point as being on this segment.
            /// </summary>
            /// <param name="p">The collinear point.</param>
            public void AddCollinear(Point p)
            {
                collinear.Add(p);
            }

            /// <summary>
            /// Get collinear points, ordered from point A to B.
            /// </summary>
            /// <returns>The list of points, in order.</returns>
            public List<Point> GetCollinear()
            {
                Point[] collinear = this.collinear.ToArray();
                //These two loops are an insertion sort to order the collinear points based on distance from A.
                for (int i = 1; i < collinear.Length; i++)
                {
                    Point p = collinear[i];
                    for (int j = i; j > 0 && Distance(A, collinear[j - 1]) > Distance(A, p); j--)
                    {
                        Point tmp = collinear[j];
                        collinear[j] = collinear[j - 1];
                        collinear[j - 1] = tmp;
                    }
                }

                List<Point> rv = new List<Point>();
                rv.Add(A);
                rv.AddRange(collinear);
                rv.Add(B);
                return rv;
            }

            /// <returns>The length of the segment</returns>
            public double Length()
            {
                return Distance(A, B);
            }

            /// <summary>
            /// Measure the distance between two points.
            /// </summary>
            /// <param name="p1">A point.</param>
            /// <param name="p2">A point.</param>
            /// <returns>The distance between p1 and p2.</returns>
            private double Distance(Point p1, Point p2)
            {
                return System.Math.Sqrt(System.Math.Pow(p1.X - p2.X, 2) + System.Math.Pow(p1.Y - p2.Y, 2));
            }

            public override string ToString()
            {
                return "[" + A.ToString() + ", " + B.ToString() + "]";
            }
        }
    }
}