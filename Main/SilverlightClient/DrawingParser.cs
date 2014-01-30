﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry figures into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DrawingParser
    {
        private const double EPSILON_SEGMENT_LENGTH = 0.001;
        private const double EPSILON_ANGLE_DEGREES = 0.01;

        private Drawing drawing;
        private ParseController parseController;

        private Dictionary<IFigure, GroundedClause> parsed;

        private List<Point> points;
        private List<TempSegment> tempSegs;
        private List<Collinear> collinear;
        private List<Triangle> triangles;
        private List<GeometricCongruentSegments> congSegs;
        List<Intersection> intersections;
        private List<Angle> angles;
        private List<GeometricCongruentAngles> congAngles;
        private List<RightAngle> rightAngles;

        /// <summary>
        /// Create a new Drawing Parser.
        /// </summary>
        /// <param name="drawing">The drawing to parse.</param>
        /// <param name="parseController">The parseController, used to add disambiguation dialogs.</param>
        public DrawingParser(Drawing drawing, ParseController parseController)
        {
            this.drawing = drawing;
            this.parseController = parseController;

            parsed = new Dictionary<IFigure, GroundedClause>();

            points = new List<Point>();
            tempSegs = new List<TempSegment>();
            collinear = new List<Collinear>();
            triangles = new List<Triangle>();
            congSegs = new List<GeometricCongruentSegments>();
            intersections = new List<Intersection>();
            angles = new List<Angle>();
            congAngles = new List<GeometricCongruentAngles>();
            rightAngles = new List<RightAngle>();
        }

        /// <summary>
        /// Parse the basic figures in the drawing.
        /// </summary>
        public void ParseDrawing()
        {
            foreach (IFigure figure in drawing.Figures)
                parse(figure);
        }

        /// <summary>
        /// Return clauses that should be passed to the back-end.
        /// </summary>
        /// <returns>Back-end input.</returns>
        public List<GroundedClause> getClauses()
        {
            List<GroundedClause> rv = new List<GroundedClause>();
            points.ForEach((Point p) => rv.Add(p));
            collinear.ForEach((Collinear c) => rv.Add(c));
            triangles.ForEach((Triangle t) => rv.Add(t));
            congSegs.ForEach((GeometricCongruentSegments gcs) => rv.Add(gcs));
            congAngles.ForEach((GeometricCongruentAngles cga) => rv.Add(cga));
            rightAngles.ForEach((RightAngle r) => rv.Add(r));
            return rv;
        }

        /// <summary>
        /// Remove duplicate instances of segments.
        /// </summary>
        public void removeDuplicateSegments()
        {
            TempSegment[] segs = tempSegs.ToArray();
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
            tempSegs = new List<TempSegment>();
            for (int i = 0; i < segs.Length; i++)
            {
                if (!duplicates.Contains(i))
                {
                    tempSegs.Add(segs[i]);
                }
            }
        }

        /// <summary>
        /// Calculate collinear points.
        /// </summary>
        public void calculateCollinear()
        {
            //See if points lie in the middle of existing segments.
            foreach (Point p in points)
            {
                foreach (TempSegment s in tempSegs)
                {
                    if (s.A == p || s.B == p) { } //next iteration
                    else if (isInMiddle(s.A, p, s.B))
                    {
                        s.AddCollinear(p);
                    }
                }
            }

            //Create the actual collinear statements.
            foreach (TempSegment s in tempSegs)
            {
                collinear.Add(new Collinear(s.GetCollinear()));
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
        /// Calculate which segments have the same length and create a disambiguation prompt for them.
        /// </summary>
        public void calculateCongruentSegments()
        {
            TempSegment[] segs = tempSegs.ToArray();
            //Search through all segments...
            for (int i = 0; i < (segs.Length - 1); i++)
            {
                //... and compare to other segments to see if their length macthes.
                for (int j = i + 1; j < segs.Length; j++)
                {
                    if (System.Math.Abs(segs[i].Length() - segs[j].Length()) < EPSILON_SEGMENT_LENGTH)
                    {
                        //Generate disambiguation prompt and action
                        object[] param = new object[2];
                        param[0] = segs[i];
                        param[1] = segs[j];
                        parseController.addDialog("Are segments " + segs[i].ToString() + " and " + segs[j].ToString() + " congruent?",
                            "Disambiguate Congruent Segments", param,
                             (object[] args) =>
                             {
                                 TempSegment s1 = args[0] as TempSegment, s2 = args[1] as TempSegment;
                                 GeometryTutorLib.ConcreteAST.Segment a = new GeometryTutorLib.ConcreteAST.Segment(s1.A, s1.B);
                                 GeometryTutorLib.ConcreteAST.Segment b = new GeometryTutorLib.ConcreteAST.Segment(s2.A, s2.B);
                                 congSegs.Add(new GeometricCongruentSegments(a, b));
                             },
                             (object[] args) => { }
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Calculate intersections in order to find angles.
        /// </summary>
        public void calculateIntersections()
        {
            //Each triangle has 3 intersections, 1 at each vertex.
            foreach (Triangle t in triangles)
            {
                Point vertex = t.SegmentA.SharedVertex(t.SegmentB);
                intersections.Add(new Intersection(vertex, t.SegmentA, t.SegmentB));

                vertex = t.SegmentB.SharedVertex(t.SegmentC);
                intersections.Add(new Intersection(vertex, t.SegmentB, t.SegmentC));

                vertex = t.SegmentA.SharedVertex(t.SegmentC);
                intersections.Add(new Intersection(vertex, t.SegmentA, t.SegmentC));
            }

            //Find the maximal segments.
            List<GeometryTutorLib.ConcreteAST.Segment> maximalSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<GeometryTutorLib.ConcreteAST.Segment> segments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            tempSegs.ForEach((TempSegment s) => segments.Add(new GeometryTutorLib.ConcreteAST.Segment(s.A, s.B)));
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
                                intersections.Add(new Intersection(actualInter, maximalSegments[s1], maximalSegments[s2]));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all angles in the drawing.
        /// </summary>
        public void calculateAngles()
        {
            foreach (Intersection inter in intersections)
            {
                // 1 angle
                if (inter.StandsOnEndpoint())
                {
                    angles.Add(new Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect)));
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

                    angles.Add(new Angle(left, inter.intersect, up));
                    angles.Add(new Angle(right, inter.intersect, up));
                }
                // 4 angles
                else
                {
                    angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                    angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                    angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                    angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
                }
            }
        }

        /// <summary>
        /// Remove duplicate instances of angles.
        /// </summary>
        public void removeDuplicateAngles()
        {
            Angle[] angles = this.angles.ToArray();
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
            this.angles = new List<Angle>();
            for (int i = 0; i < angles.Length; i++)
            {
                if (!duplicates.Contains(i))
                {
                    this.angles.Add(angles[i]);
                }
            }
        }

        /// <summary>
        /// Calculate which angles have the same measure.
        /// </summary>
        public void calculateCongruentAngles()
        {
            //Search through the angles...
            for (int i = 0; i < (angles.Count - 1); i++)
            {
                //... and compare them to the other angles.
                for (int j = i + 1; j < angles.Count; j++)
                {
                    if (System.Math.Abs(angles[i].measure - angles[j].measure) < EPSILON_ANGLE_DEGREES)
                    {
                        //Generate disambiguation dialog and action.
                        object[] param = new object[2];
                        param[0] = angles[i];
                        param[1] = angles[j];
                        parseController.addDialog("Are angles " + angles[i].ToString() + " and " + angles[j].ToString() + " congruent?",
                            "Disambiguate Congruent Angles", param,
                             (object[] args) =>
                             {
                                 Angle a1 = args[0] as Angle, a2 = args[1] as Angle;
                                 congAngles.Add(new GeometricCongruentAngles(a1, a2));
                             },
                             (object[] args) => { }
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Calculate which angles measure 90 degrees.
        /// </summary>
        public void calculateRightAngles()
        {
            foreach (Angle a in angles)
            {
                if (System.Math.Abs(90 - a.measure) <= EPSILON_ANGLE_DEGREES)
                {
                    //Generate disambiguation dialog and action.
                    object[] param = new object[1];
                    param[0] = a;
                    parseController.addDialog("Is angle " + a.ToString() + " a right angle?",
                        "Disambiguate Right Angle", param,
                         (object[] args) =>
                         {
                             Angle angle = args[0] as Angle;
                             rightAngles.Add(new RightAngle(angle));
                         },
                         (object[] args) => { }
                    );
                }
            }
        }

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
        }

        /// <summary>
        /// Parse a point.
        /// </summary>
        /// <param name="pt">The point to parse.</param>
        private void parse(IPoint pt)
        {
            Point p = new GeometryTutorLib.ConcreteAST.Point(pt.Name, pt.Coordinates.X, pt.Coordinates.Y);
            points.Add(p);
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
            tempSegs.Add(new TempSegment(s.Point1, s.Point2));
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
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    sides[i] = new TempSegment(parsed[iPts[i]] as GeometryTutorLib.ConcreteAST.Point, parsed[iPts[j]] as GeometryTutorLib.ConcreteAST.Point);
                }
                tempSegs.AddRange(sides);

                //is it isosceles?
                bool isosceles = false;
                for (int i = 0; i < 3; i++)
                    isosceles = isosceles || (System.Math.Abs(sides[i].Length() - sides[(i + 1) % 3].Length()) <= EPSILON_SEGMENT_LENGTH);

                if (isosceles)
                {
                    //Generate disambiguation dialog and actions.
                    object[] param = sides;
                    parseController.addDialog("Is triangle " + iPts[0] + ", " + iPts[1] + ", " + iPts[2] + " isosceles?",
                        "Disambiguate Isosceles", param,
                        (object[] args) =>
                        {
                            TempSegment[] s = args as TempSegment[];
                            GeometryTutorLib.ConcreteAST.Segment[] segs = new GeometryTutorLib.ConcreteAST.Segment[3];
                            for (int i = 0; i < 3; i++)
                            {
                                segs[i] = new GeometryTutorLib.ConcreteAST.Segment(s[i].A, s[i].B);
                            }
                            GeometryTutorLib.ConcreteAST.Triangle t = new GeometryTutorLib.ConcreteAST.IsoscelesTriangle(segs[0], segs[1], segs[2]);
                            parsed.Add(pgon, t);
                            triangles.Add(t);
                        },
                        (object[] args) =>
                        {
                            TempSegment[] s = args as TempSegment[];
                            GeometryTutorLib.ConcreteAST.Segment[] segs = new GeometryTutorLib.ConcreteAST.Segment[3];
                            for (int i = 0; i < 3; i++)
                            {
                                segs[i] = new GeometryTutorLib.ConcreteAST.Segment(s[i].A, s[i].B);
                            }
                            GeometryTutorLib.ConcreteAST.Triangle t = new GeometryTutorLib.ConcreteAST.Triangle(segs[0], segs[1], segs[2]);
                            parsed.Add(pgon, t);
                            triangles.Add(t);
                        }
                    );
                }
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
                    pts[i] = drawing.Figures.FindPoint(newPt, 0);
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
                }
                tempSegs.AddRange(sides);

                Triangle t = new GeometryTutorLib.ConcreteAST.EquilateralTriangle(csegs[0], csegs[1], csegs[2]);
                parsed.Add(rgon, t);
                triangles.Add(t);
            }
        }

        /// <summary>
        /// A lightweight class to represent segments that are used in other computations.
        /// </summary>
        private class TempSegment
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