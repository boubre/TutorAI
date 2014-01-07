﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;

namespace LiveGeometry
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry figures into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DrawingParser
    {
        private Drawing drawing;
        private ParseController parseController;
        private Dictionary<IFigure, GeometryTutorLib.ConcreteAST.GroundedClause> parsed;

        public DrawingParser(Drawing drawing, ParseController parseController)
        {
            this.drawing = drawing;
            this.parseController = parseController;
        }

        /// <summary>
        /// Parse LiveGeometry concrete figures.
        /// </summary>
        /// <returns>A list of GroundedClases representing figures in the current LiveGeometry drawing.</returns>
        public void ParseDrawing()
        {
            parsed = new Dictionary<IFigure, GeometryTutorLib.ConcreteAST.GroundedClause>();
            foreach (IFigure figure in drawing.Figures)
                parse(figure);
        }

        public List<GeometryTutorLib.ConcreteAST.GroundedClause> GetParsedFigures()
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> rv = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();
            foreach (KeyValuePair<IFigure, GeometryTutorLib.ConcreteAST.GroundedClause> kvP in parsed)
                rv.Add(kvP.Value);

            return rv;
        } 

        /// <summary>
        /// Add points of where two lines intersect.  If a point does not exist, it is created and new segments are produced.
        /// The results of this function are appended to the input list.
        /// </summary>
        /// <param name="figures">A list of GeometryTutorLib.ConcreteAST..GroundedClauses representing LiveGeometry figures.</param>
        public void calculateIntersections(List<GeometryTutorLib.ConcreteAST.GroundedClause> figures)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> segmentList = new List<GeometryTutorLib.ConcreteAST.Segment>();
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in figures)
            {
                if (clause is GeometryTutorLib.ConcreteAST.Segment)
                    segmentList.Add(clause as GeometryTutorLib.ConcreteAST.Segment);
            }

            GeometryTutorLib.ConcreteAST.Segment[] segments = segmentList.ToArray();
            // Iterate through all segments to see if we can find intersections
            for (int i = 0; i < segments.Length - 1; i++)
            {
                // Iterate throught the remeinder of the segments to try to match up endpoints.
                for (int j = i + 1; j < segments.Length; j++)
                {
                    //write lines as ax+by=c. If m is slope, then a=m, b=-1, and c=mx-y.
                    double m_i = segments[i].Slope;
                    double a_i, b_i, c_i;
                    if (!double.IsInfinity(m_i))
                    {
                        a_i = m_i;
                        b_i = -1;
                        c_i = m_i * segments[i].Point1.X - segments[i].Point1.Y;
                    }
                    else //veritcal line!
                    {
                        a_i = 1;
                        b_i = 0;
                        c_i = segments[i].Point1.X;
                    }

                    double m_j = segments[j].Slope;
                    double a_j, b_j, c_j;
                    if (!double.IsInfinity(m_j))
                    {
                        a_j = m_j;
                        b_j = -1;
                        c_j = m_j * segments[j].Point1.X - segments[j].Point1.Y;
                    }
                    else //veritcal line!
                    {
                        a_j = 1;
                        b_j = 0;
                        c_j = segments[j].Point1.X;
                    }

                    //Use Cramer's rule
                    Matrix_2by2 m1 = new Matrix_2by2(a_i, b_i, a_j, b_j);
                    double m1_det = m1.determinant();
                    if (m1_det != 0) //not the same line or parallel lines
                    { 

                        Matrix_2by2 m2 = new Matrix_2by2(c_i, b_i, c_j, b_j);
                        Matrix_2by2 m3 = new Matrix_2by2(a_i, c_i, a_j, c_j);
                        double m2_det = m2.determinant();
                        double m3_det = m3.determinant();

                        //Lookup point if it exists or create it
                        System.Windows.Point newPt = new System.Windows.Point(m2_det / m1_det, m3_det / m1_det);
                        IPoint point = findParsedPoint(newPt);
                        if (point == null)
                        {
                            point = Factory.CreateFreePoint(drawing, newPt);
                            point.Name = "I{" + i + "," + j + "}";
                            parse(point as IFigure);
                        }
                        GeometryTutorLib.ConcreteAST.Point intersection = parsed[point] as GeometryTutorLib.ConcreteAST.Point;

                        //make sure the intersection is not an endpoint
                        if (segments[i].Point1.X == intersection.X || segments[i].Point1.Y == intersection.Y ||
                            segments[i].Point2.X == intersection.X || segments[i].Point2.Y == intersection.Y ||
                            segments[j].Point1.X == intersection.X || segments[j].Point1.Y == intersection.Y ||
                            segments[j].Point2.X == intersection.X || segments[j].Point2.Y == intersection.Y) { } //Do nothing
                        //Make sure it is on the lines
                        else if (!(isInMiddle(segments[i].Point1, intersection, segments[i].Point2)
                            && isInMiddle(segments[j].Point1, intersection, segments[j].Point2))) { } //Do nothing
                        else //Add point and new segments generated by addition of the point
                        {
                            figures.Add(intersection);
                            figures.Add(new GeometryTutorLib.ConcreteAST.Segment(segments[i].Point1, intersection));
                            figures.Add(new GeometryTutorLib.ConcreteAST.Segment(intersection, segments[i].Point2));
                            figures.Add(new GeometryTutorLib.ConcreteAST.Segment(segments[j].Point1, intersection));
                            figures.Add(new GeometryTutorLib.ConcreteAST.Segment(intersection, segments[j].Point2));
                            figures.Add(new GeometryTutorLib.ConcreteAST.InMiddle(intersection, segments[i], "Intrinsic"));
                            figures.Add(new GeometryTutorLib.ConcreteAST.InMiddle(intersection, segments[j], "Intrinsic"));
                            figures.Add(new GeometryTutorLib.ConcreteAST.Intersection(intersection, segments[i], segments[j], "Intrinsic"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Search the parsed dictionary to see if a point with the given coordiantes already exists.
        /// </summary>
        /// <param name="given">The point to search for.</param>
        /// <returns>The point if found, null otherwise.</returns>
        private IPoint findParsedPoint(System.Windows.Point given)
        {
            foreach (IFigure f in parsed.Keys)
            {
                IPoint pt = f as IPoint;
                if (pt != null && pt.Coordinates.X == given.X && pt.Coordinates.Y == given.Y)
                    return pt;
            }
            return null;
        }

        /// <summary>
        /// Find which points lie in the middle of a line
        /// The results of this function are appended to the input list.
        /// </summary>
        /// <param name="clauses">A list of GeometryTutorLib.ConcreteAST..GroundedClauses representing LiveGeometry figures.</param>
        public void calculateInMiddle(List<GeometryTutorLib.ConcreteAST.GroundedClause> clauses)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> segmentList = new List<GeometryTutorLib.ConcreteAST.Segment>(); //Get a list of only segments
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in clauses)
            {
                if (clause is GeometryTutorLib.ConcreteAST.Segment)
                    segmentList.Add(clause as GeometryTutorLib.ConcreteAST.Segment);
            }

            List<GeometryTutorLib.ConcreteAST.Point> pointList = new List<GeometryTutorLib.ConcreteAST.Point>(); //Get a list of only points
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in clauses)
            {
                if (clause is GeometryTutorLib.ConcreteAST.Point)
                    pointList.Add(clause as GeometryTutorLib.ConcreteAST.Point);
            }

            // For every segment and point, see if the point is on the segment.
            foreach (GeometryTutorLib.ConcreteAST.Point point in pointList)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in segmentList)
                {
                    if (segment.Point1.X == point.X || segment.Point1.Y == point.Y ||
                        segment.Point2.X == point.X || segment.Point2.Y == point.Y) { } //Do nothing.
                    else if (isInMiddle(segment.Point1, point, segment.Point2))
                    {
                        if (isMidpoint(segment.Point1, point, segment.Point2))
                        {
                            clauses.Add(new GeometryTutorLib.ConcreteAST.Midpoint(point, segment, "Given"));
                        }
                        else
                        {
                            clauses.Add(new GeometryTutorLib.ConcreteAST.InMiddle(point, segment, "Intrinsic"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests to see if point b is the midpoint of points a and c.
        /// </summary>
        /// <param name="a">A point</param>
        /// <param name="b">A point</param>
        /// <param name="c">A point</param>
        /// <returns>TRUE if b is in the midpoint of b and c</returns>
        private bool isMidpoint(GeometryTutorLib.ConcreteAST.Point a, GeometryTutorLib.ConcreteAST.Point b, GeometryTutorLib.ConcreteAST.Point c)
        {
            const double EPSILON = 0.00000001;

            return (a.X + c.X) / 2 - b.X < EPSILON && (a.Y + c.Y) / 2 - b.Y < EPSILON;
        }

        /// <summary>
        /// Tests to see if point b is in the middle of points a and c.
        /// </summary>
        /// <param name="a">A point</param>
        /// <param name="b">A point</param>
        /// <param name="c">A point</param>
        /// <returns>TRUE if b is in the middle of b and c</returns>
        private bool isInMiddle(GeometryTutorLib.ConcreteAST.Point a, GeometryTutorLib.ConcreteAST.Point b, GeometryTutorLib.ConcreteAST.Point c)
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
        /// Add Eqauls descriptors for all lines of equal length.
        /// The results of this function are appended to the input list.
        /// </summary>
        /// <param name="clauses">A list of GeometryTutorLib.ConcreteAST..GroundedClauses representing LiveGeometry figures.</param>
        public void calculateLineEquality(List<GeometryTutorLib.ConcreteAST.GroundedClause> clauses)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> segmentList = new List<GeometryTutorLib.ConcreteAST.Segment>(); //Get a list of only segments
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in clauses)
                if (clause is GeometryTutorLib.ConcreteAST.Segment)
                    segmentList.Add(clause as GeometryTutorLib.ConcreteAST.Segment);

            GeometryTutorLib.ConcreteAST.Segment[] segments = segmentList.ToArray();
            //If any two segments have equal length, add the equals clause.
            for (int i = 0; i < segments.Length - 1; i++)
                for (int j = i + 1; j < segments.Length; j++)
                    if (segments[i].Length == segments[j].Length)
                        clauses.Add(new GeometryTutorLib.ConcreteAST.GeometricCongruentSegments(segments[i], segments[j], "Given"));
        }

        /// <summary>
        /// Finds midpoints.
        /// The results of this function are appended to the input list.
        /// </summary>
        /// <param name="clauses">A list of GeometryTutorLib.ConcreteAST..GroundedClauses representing LiveGeometry figures.</param>
        public void calculateMidpoints(List<GeometryTutorLib.ConcreteAST.GroundedClause> clauses)
        {
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause gc in clauses) //If a point is in the middle of a line, see if it is a midpoint.
            {
                GeometryTutorLib.ConcreteAST.InMiddle im = gc as GeometryTutorLib.ConcreteAST.InMiddle;
                if (im != null)
                {
                    GeometryTutorLib.ConcreteAST.Point mid = im.point, end1 = im.segment.Point1, end2 = im.segment.Point2;
                    if (GeometryTutorLib.ConcreteAST.Point.calcDistance(end1, mid) == GeometryTutorLib.ConcreteAST.Point.calcDistance(mid, end2))
                    {
                        parseController.addDialog("Is point " + mid + " a midpoint?",
                            "Disambiguate Midpoint",
                            () => clauses.Add(new GeometryTutorLib.ConcreteAST.Midpoint(mid, im.segment, "Given")),
                            () => { }
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Finds new triangles made from intersection points.
        /// The results of this function are appended to the input list.
        /// </summary>
        /// <param name="clauses">A list of GroundedClauses representing LiveGeometry figures.</param>
        public void calculateTriangles(List<GeometryTutorLib.ConcreteAST.GroundedClause> clauses)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> segmentList = new List<GeometryTutorLib.ConcreteAST.Segment>(); //Get a list of only segments
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause gc in clauses)
            {
                if (gc is GeometryTutorLib.ConcreteAST.Segment) segmentList.Add(gc as GeometryTutorLib.ConcreteAST.Segment);
            }

            GeometryTutorLib.ConcreteAST.Segment[] segments = segmentList.ToArray();
            // Iterate through all segments...
            for (int i = 0; i < segments.Length - 2; i++)
            {
                // ... and find another sgement with a matching endpoint ...
                for (int j = i + 1; j < segments.Length - 1; j++)
                {
                    if (segments[i].Equals(segments[j]) || segments[i].Slope == segments[j].Slope) { } //Do nothing
                    else if (segments[i].Point1.Equals(segments[j].Point1))
                        // ... and then see if there is another segment that matches the two free endpoints.
                        for (int k = j + 1; k < segments.Length; k++)
                        {
                            if (segments[k].Equals(segments[i]) || segments[k].Equals(segments[j])) { }
                            else if (segments[k].Slope == segments[i].Slope || segments[k].Slope == segments[j].Slope) { }
                            else if (segments[k].Point1.Equals(segments[i].Point2) && segments[k].Point2.Equals(segments[j].Point2) ||
                                segments[k].Point1.Equals(segments[j].Point2) && segments[k].Point2.Equals(segments[i].Point2))
                            {
                                clauses.Add(new GeometryTutorLib.ConcreteAST.Triangle(segments[i], segments[j], segments[k], "intrinsic"));
                                break;
                            }
                        }
                    else if (segments[i].Point1.Equals(segments[j].Point2))
                        // ... and then see if there is another segment that matches the two free endpoints.
                        for (int k = j + 1; k < segments.Length; k++)
                        {
                            if (segments[k].Equals(segments[i]) || segments[k].Equals(segments[j])) { }
                            else if (segments[k].Slope == segments[i].Slope || segments[k].Slope == segments[j].Slope) { }
                            else if (segments[k].Point1.Equals(segments[i].Point2) && segments[k].Point2.Equals(segments[j].Point1) ||
                                segments[k].Point1.Equals(segments[j].Point1) && segments[k].Point2.Equals(segments[i].Point2))
                            {
                                clauses.Add(new GeometryTutorLib.ConcreteAST.Triangle(segments[i], segments[j], segments[k], "intrinsic"));
                                break;
                            }
                        }
                    else if (segments[i].Point2.Equals(segments[j].Point1))
                        // ... and then see if there is another segment that matches the two free endpoints.
                        for (int k = j + 1; k < segments.Length; k++)
                        {
                            if (segments[k].Equals(segments[i]) || segments[k].Equals(segments[j])) { }
                            else if (segments[k].Slope == segments[i].Slope || segments[k].Slope == segments[j].Slope) { }
                            else if (segments[k].Point1.Equals(segments[i].Point1) && segments[k].Point2.Equals(segments[j].Point2) ||
                                segments[k].Point1.Equals(segments[j].Point2) && segments[k].Point2.Equals(segments[i].Point1))
                            {
                                clauses.Add(new GeometryTutorLib.ConcreteAST.Triangle(segments[i], segments[j], segments[k], "intrinsic"));
                                break;
                            }
                        }
                    else if (segments[i].Point2.Equals(segments[j].Point2))
                        // ... and then see if there is another segment that matches the two free endpoints.
                        for (int k = j + 1; k < segments.Length; k++)
                        {
                            if (segments[k].Equals(segments[i]) || segments[k].Equals(segments[j])) { }
                            else if (segments[k].Slope == segments[i].Slope || segments[k].Slope == segments[j].Slope) { }
                            else if (segments[k].Point1.Equals(segments[i].Point1) && segments[k].Point2.Equals(segments[j].Point1) ||
                                segments[k].Point1.Equals(segments[j].Point1) && segments[k].Point2.Equals(segments[i].Point1))
                            {
                                clauses.Add(new GeometryTutorLib.ConcreteAST.Triangle(segments[i], segments[j], segments[k], "intrinsic"));
                                break;
                            }
                        }
                }
            }
        }

        /// <summary>
        /// Remove duplicate clauses from the input list, if they exist.
        /// </summary>
        /// <param name="clauses">A list of GeometryTutorLib.ConcreteAST..GroundedClauses representing LiveGeometry figures.</param>
        /// <returns>A list of clauses with duplicates removed</returns>
        public List<GeometryTutorLib.ConcreteAST.GroundedClause> removeDuplicates(List<GeometryTutorLib.ConcreteAST.GroundedClause> clauseList)
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> uniqueClauses = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();
            GeometryTutorLib.ConcreteAST.GroundedClause[] clauses = clauseList.ToArray();
            for (int i = 0; i < clauses.Length; i++)
            {
                if (clauses[i] != null)
                {
                    for (int j = i + 1; j < clauses.Length; j++)
                        if (clauses[i].Equals(clauses[j]))
                            clauses[j] = null;
                    uniqueClauses.Add(clauses[i]);
                }
            }
            return uniqueClauses;
        }

        private void parse(IFigure figure)
        {
            if (parsed.ContainsKey(figure)) return;
            else if (figure is IPoint) parse(figure as IPoint);
            else if (figure is ILine) parse(figure as ILine);
            else if (figure is Polygon) parse(figure as PolygonBase);
            else if (figure is RegularPolygon) parse(figure as RegularPolygon);
        }

        private void parse(IPoint pt)
        {
            parsed.Add(pt, new GeometryTutorLib.ConcreteAST.Point(pt.Name, pt.Coordinates.X, pt.Coordinates.Y));
        }

        private void parse(ILine line)
        {
            IPoint p1 = line.Dependencies.FindPoint(line.Coordinates.P1, 0);
            IPoint p2 = line.Dependencies.FindPoint(line.Coordinates.P2, 0);
            parse(p1 as IFigure);
            parse(p2 as IFigure);
            parsed.Add(line, new GeometryTutorLib.ConcreteAST.Segment(parsed[p1] as GeometryTutorLib.ConcreteAST.Point, parsed[p2] as GeometryTutorLib.ConcreteAST.Point));
        }

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
                ILine[] lines = new ILine[3];
                GeometryTutorLib.ConcreteAST.Segment[] csegs = new GeometryTutorLib.ConcreteAST.Segment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    lines[i] = drawing.Figures.FindLine(iPts[i], iPts[j]);
                    if (lines[i] == null) lines[i] = Factory.CreateSegment(drawing, new[] { iPts[i], iPts[j] });
                    parse(lines[i] as IFigure);
                    csegs[i] = parsed[lines[i]] as GeometryTutorLib.ConcreteAST.Segment;
                }

                //is it isosceles?
                bool isosceles = false;
                for (int i = 0; i < 3; i++)
                    isosceles = isosceles || (csegs[i].Length == csegs[(i+1)%3].Length);

                parseController.addDialog("Is triangle " + iPts[0] + ", " + iPts[1] + ", " + iPts[2] + " isosceles?",
                    "Disambiguate Isosceles",
                    () => 
                    {
                        GeometryTutorLib.ConcreteAST.Triangle t = new GeometryTutorLib.ConcreteAST.IsoscelesTriangle(csegs[0], csegs[1], csegs[2], "Given");
                        parsed.Add(pgon, t);
                        if (t.isRightTriangle())
                        {
                            parseController.addDialog("Is triangle " + iPts[0] + ", " + iPts[1] + ", " + iPts[2] + " a right triangle?",
                                "Disambiguate Right",
                                () => t.givenRight = true,
                                () => t.givenRight = false);
                        }
                    },
                    () => 
                    {
                        GeometryTutorLib.ConcreteAST.Triangle t = new GeometryTutorLib.ConcreteAST.Triangle(csegs[0], csegs[1], csegs[2], "Given");
                        parsed.Add(pgon, t);
                        if (t.isRightTriangle())
                        {
                            parseController.addDialog("Is triangle " + iPts[0] + ", " + iPts[1] + ", " + iPts[2] + " a right triangle?",
                                "Disambiguate Right",
                                () => t.givenRight = true,
                                () => t.givenRight = false);
                        }
                    }
                );
            }
        }

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
                ILine[] lines = new ILine[3];
                GeometryTutorLib.ConcreteAST.Segment[] csegs = new GeometryTutorLib.ConcreteAST.Segment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    lines[i] = drawing.Figures.FindLine(pts[i], pts[j]);
                    if (lines[i] == null) lines[i] = Factory.CreateSegment(drawing, new[] { pts[i], pts[j] });
                    parse(lines[i] as IFigure);
                    csegs[i] = parsed[lines[i]] as GeometryTutorLib.ConcreteAST.Segment;
                }

                parsed.Add(rgon, new GeometryTutorLib.ConcreteAST.EquilateralTriangle(csegs[0], csegs[1], csegs[2], "Given"));
            }
        }

        /// <summary>
        /// A utilitly class used to calulcate 2D line intersections.
        /// | a b |
        /// | c d |
        /// </summary>
        private class Matrix_2by2
        {
            public double a { get; private set; }
            public double b { get; private set; }
            public double c { get; private set; }
            public double d { get; private set; }

            /// <summary>
            /// Create a new 2 by 2 matrix:
            /// | a b |
            /// | c d |
            /// </summary>
            /// <param name="a">a value</param>
            /// <param name="b">b value</param>
            /// <param name="c">c value</param>
            /// <param name="d">s value</param>
            public Matrix_2by2(double a, double b, double c, double d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }

            /// <summary>
            /// Find the determinant of this matrix
            /// </summary>
            /// <returns>The determinant of this matrix</returns>
            public double determinant()
            {
                return a * d - b * c;
            }
        }
    }
}
