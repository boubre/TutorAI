using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a quadrilateral (defined by 4 segments)
    /// </summary>
    public class Quadrilateral : Polygon
    {
        public Point topLeft { get; private set; }
        public Point topRight { get; private set; }
        public Point bottomLeft { get; private set; }
        public Point bottomRight { get; private set; }

        public Segment left { get; private set; }
        public Segment right { get; private set; }
        public Segment top { get; private set; }
        public Segment bottom { get; private set; }

        public Angle topLeftAngle { get; private set; }
        public Angle topRightAngle { get; private set; }
        public Angle bottomLeftAngle { get; private set; }
        public Angle bottomRightAngle { get; private set; }

        //
        // Diagonals
        //
        public Intersection diagonalIntersection { get; private set; }
        public void SetIntersection(Intersection diag) { diagonalIntersection = diag; }

        public Segment topLeftBottomRightDiagonal { get; private set; }
        private bool tlbrDiagonalValid = true;
        public bool TopLeftDiagonalIsValid() { return tlbrDiagonalValid; }
        public void SetTopLeftDiagonalInValid() { tlbrDiagonalValid = false; }

        public Segment bottomLeftTopRightDiagonal { get; private set; }
        private bool bltrDiagonalValid = true;
        public bool BottomRightDiagonalIsValid() { return bltrDiagonalValid; }
        public void SetBottomRightDiagonalInValid() { bltrDiagonalValid = false; }

        public Quadrilateral(Segment left, Segment right, Segment top, Segment bottom) : base()
        {
            //
            // Segments
            //
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;

            orderedSides = new List<Segment>();
            orderedSides.Add(left);
            orderedSides.Add(right);
            orderedSides.Add(top);
            orderedSides.Add(bottom);

            //
            // Points
            //
            this.topLeft = left.SharedVertex(top);
            if (topLeft == null) throw new ArgumentException("Top left point is invalid: " + top + " " + left);

            this.topRight = right.SharedVertex(top);
            if (topRight == null) throw new ArgumentException("Top left point is invalid: " + top + " " + right);

            this.bottomLeft = left.SharedVertex(bottom);
            if (bottomLeft == null) throw new ArgumentException("Bottom left point is invalid: " + bottom + " " + left);

            this.bottomRight = right.SharedVertex(bottom);
            if (bottomRight == null) throw new ArgumentException("Bottom right point is invalid: " + bottom + " " + right);

            points = new List<Point>();
            points.Add(topLeft);
            points.Add(topRight);
            points.Add(bottomLeft);
            points.Add(bottomRight);

            // Verify that we have 4 unique points
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].StructurallyEquals(points[j]))
                    {
                        throw new ArgumentException("Points of quadrilateral are not distinct: " + points[i] + " " + points[j]);
                    }
                }
            }

            //
            // Diagonals
            //
            this.topLeftBottomRightDiagonal = new Segment(topLeft, bottomRight);
            this.bottomLeftTopRightDiagonal = new Segment(bottomLeft, topRight);
            this.diagonalIntersection = null;

            //
            // Angles
            //
            this.topLeftAngle = new Angle(bottomLeft, topLeft, topRight);
            this.topRightAngle = new Angle(topLeft, topRight, bottomRight);
            this.bottomRightAngle = new Angle(topRight, bottomRight, bottomLeft);
            this.bottomLeftAngle = new Angle(bottomRight, bottomLeft, topLeft);

            angles = new List<Angle>();
            angles.Add(topLeftAngle);
            angles.Add(topRightAngle);
            angles.Add(bottomLeftAngle);
            angles.Add(bottomRightAngle);

            addSuperFigureToDependencies();
        }

        public Quadrilateral(List<Segment> segs) : this(segs[0], segs[1], segs[2], segs[3])
        {
            if (segs.Count != 4) throw new ArgumentException("Quadrilateral constructed with " + segs.Count + " segments.");
        }

        protected void addSuperFigureToDependencies()
        {
            Utilities.AddUniqueStructurally(topLeft.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topRight.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomLeft.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomRight.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(left.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(right.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottom.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(top.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topLeftAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topRightAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomLeftAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomRightAngle.getSuperFigures(), this);
        }

        public bool IsStrictQuadrilateral()
        {
            if (this is Parallelogram) return false;
            if (this is Trapezoid) return false;
            if (this is Kite) return false;

            return true;
        }

        //
        // Acquire one of the quadrilateral angles.
        //
        public Angle GetAngle(Angle thatAngle)
        {
            foreach (Angle angle in angles)
            {
                if (angle.Equates(thatAngle)) return angle;
            }

            return null;
        }

        //
        // Does this quadrilateral have the given side (exactly, no extension)?
        //
        public bool HasAngle(Angle thatAngle)
        {
            foreach (Angle angle in angles)
            {
                if (angle.Equates(thatAngle)) return true;
            }

            return false;
        }

        //
        // Does this quadrilateral have the given side (exactly, no extension)?
        //
        public bool HasSide(Segment segment)
        {
            foreach(Segment side in orderedSides)
            {
                if (side.StructurallyEquals(segment)) return true;
            }

            return false;
        }

        //
        // Are the two given segments on the opposite sides of this quadrilateral?
        //
        public bool AreOppositeSides(Segment segment1, Segment segment2)
        {
            if (!this.HasSide(segment1) || !this.HasSide(segment2)) return false;

            if (top.StructurallyEquals(segment1) && bottom.StructurallyEquals(segment2)) return true;
            if (top.StructurallyEquals(segment2) && bottom.StructurallyEquals(segment1)) return true;

            if (left.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;
            if (left.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;

            return false;
        }

        //
        // Are the two given angles on the opposite sides of this quadrilateral?
        //
        public bool AreOppositeAngles(Angle angle1, Angle angle2)
        {
            if (!this.HasAngle(angle1) || !this.HasAngle(angle2)) return false;

            if (topLeftAngle.Equates(angle1) && bottomRightAngle.Equates(angle2)) return true;
            if (topLeftAngle.Equates(angle2) && bottomRightAngle.Equates(angle1)) return true;

            if (topRightAngle.Equates(angle1) && bottomLeftAngle.Equates(angle2)) return true;
            if (topRightAngle.Equates(angle2) && bottomLeftAngle.Equates(angle1)) return true;

            return false;
        }

        //
        // Are the two given segments adjacent with this quadrilateral?
        //
        public bool AreAdjacentSides(Segment segment1, Segment segment2)
        {
            if (!this.HasSide(segment1) || !this.HasSide(segment2)) return false;

            if (top.StructurallyEquals(segment1) && left.StructurallyEquals(segment2)) return true;
            if (top.StructurallyEquals(segment2) && left.StructurallyEquals(segment1)) return true;

            if (top.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;
            if (top.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;

            if (bottom.StructurallyEquals(segment1) && left.StructurallyEquals(segment2)) return true;
            if (bottom.StructurallyEquals(segment2) && left.StructurallyEquals(segment1)) return true;

            if (bottom.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;
            if (bottom.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;

            return false;
        }

        //
        // Does this parallel set apply to this quadrilateral?
        //
        public bool HasOppositeParallelSides(Parallel parallel)
        {
            return AreOppositeSides(parallel.segment1, parallel.segment2);
        }

        //
        // Does this congruent pair apply to this quadrilateral?
        //
        public bool HasOppositeCongruentSides(CongruentSegments cs)
        {
            return AreOppositeSides(cs.cs1, cs.cs2);
        }

        //
        // Does this congruent pair of angles apply to this quadrilateral?
        //
        public bool HasOppositeCongruentAngles(CongruentAngles cas)
        {
            return AreOppositeAngles(cas.ca1, cas.ca2);
        }

        //
        // Does this parallel set apply to this quadrilateral?
        //
        public bool HasAdjacentCongruentSides(CongruentSegments cs)
        {
            return AreAdjacentSides(cs.cs1, cs.cs2);
        }

        //
        // Acquire the other 2 sides not in this parallel relationship; works for a n-gon (polygon) as well.
        //
        public List<Segment> GetOtherSides(List<Segment> inSegments)
        {
            List<Segment> outSegments = new List<Segment>();

            // This quadrilateral must have these given segments to return valid data.
            foreach (Segment inSeg in inSegments)
            {
                if (!this.HasSide(inSeg)) return outSegments;
            }

            //
            // Traverse given segments partitioning this quad into in / out.
            //
            foreach (Segment side in orderedSides)
            {
                if (!inSegments.Contains(side))
                {
                    outSegments.Add(side);
                }
            }

            return outSegments;
        }

        //
        // Acquire the other 2 sides not in this parallel relationship.
        //
        public List<Segment> GetOtherSides(Parallel parallel)
        {
            List<Segment> segs = new List<Segment>();
            segs.Add(parallel.segment1);
            segs.Add(parallel.segment2);

            return GetOtherSides(segs);
        }

        //
        // Coordinate-based determination if we have a parallelogram.
        //
        protected bool VerifyParallelogram()
        {
            if (!left.IsParallelWith(right)) return false;
            if (!top.IsParallelWith(bottom)) return false;

            if (!Utilities.CompareValues(left.Length, right.Length)) return false;
            if (!Utilities.CompareValues(top.Length, bottom.Length)) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, bottomRightAngle.measure)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, bottomLeftAngle.measure)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a rhombus.
        //
        protected bool VerifyRhombus()
        {
            if (!VerifyParallelogram()) return false;

            if (!Utilities.CompareValues(left.Length, top.Length)) return false;
            if (!Utilities.CompareValues(left.Length, bottom.Length)) return false;

            // Redundant
            if (!Utilities.CompareValues(right.Length, top.Length)) return false;
            if (!Utilities.CompareValues(right.Length, bottom.Length)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Square
        //
        protected bool VerifySquare()
        {
            if (!VerifyRhombus()) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomRightAngle.measure, 90)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Rectangle
        //
        protected bool VerifyRectangle()
        {
            if (!VerifyParallelogram()) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomRightAngle.measure, 90)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Trapezoid
        //
        protected bool VerifyTrapezoid()
        {
            bool lrParallel = left.IsParallelWith(right);
            bool tbParallel = top.IsParallelWith(bottom);

            // XOR of parallel opposing sides
            if (lrParallel && tbParallel) return false;
            if (!lrParallel && !tbParallel) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have an Isosceles Trapezoid
        //
        protected bool VerifyIsoscelesTrapezoid()
        {
            if (!VerifyTrapezoid()) return false;

            //
            // Determine parallel sides, then compare lengths of other sides
            //
            if (left.IsParallelWith(right))
            {
                if (!Utilities.CompareValues(top.Length, bottom.Length)) return false;
            }
            else if (top.IsParallelWith(bottom))
            {
                if (!Utilities.CompareValues(left.Length, right.Length)) return false;
            }

            return true;
        }

        //
        // Coordinate-based determination if we have an Isosceles Trapezoid
        //
        protected bool VerifyKite()
        {
            //
            // Adjacent sides must equate in length
            //
            if (Utilities.CompareValues(left.Length, top.Length) && Utilities.CompareValues(right.Length, bottom.Length)) return true;

            if (Utilities.CompareValues(left.Length, bottom.Length) && Utilities.CompareValues(right.Length, top.Length)) return true;

            return false;
        }

        //
        // Can this Quadrilateral be strengthened to any of the specific quadrilaterals (parallelogram, kite, square, etc)?
        //
        public static List<Strengthened> CanBeStrengthened(Quadrilateral thatQuad)
        {
            List<Strengthened> strengthened = new List<Strengthened>();

            if (thatQuad.VerifyParallelogram())
            {
                strengthened.Add(new Strengthened(thatQuad, new Parallelogram(thatQuad)));
            }

            if (thatQuad.VerifyRectangle())
            {
                strengthened.Add(new Strengthened(thatQuad, new Rectangle(thatQuad)));
            }

            if (thatQuad.VerifySquare())
            {
                strengthened.Add(new Strengthened(thatQuad, new Square(thatQuad)));
            }

            if (thatQuad.VerifyRhombus())
            {
                strengthened.Add(new Strengthened(thatQuad, new Rhombus(thatQuad)));
            }

            if (thatQuad.VerifyKite())
            {
                strengthened.Add(new Strengthened(thatQuad, new Kite(thatQuad)));
            }

            if (thatQuad.VerifyTrapezoid())
            {
                Trapezoid newTrap = new Trapezoid(thatQuad);
                strengthened.Add(new Strengthened(thatQuad, newTrap));

                if (thatQuad.VerifyIsoscelesTrapezoid())
                {
                    strengthened.Add(new Strengthened(newTrap, new IsoscelesTrapezoid(thatQuad)));
                }
            }

            return strengthened;
        }

        protected bool HasSamePoints(Quadrilateral quad)
        {
            foreach (Point p in quad.points)
            {
                if (!this.points.Contains(p)) return false;
            }

            return true;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Quadrilateral thatQuad = obj as Quadrilateral;
            if (thatQuad == null) return false;

            return this.HasSamePoints(thatQuad);
        }

        public override bool Equals(Object obj)
        {
            Quadrilateral thatQuad = obj as Quadrilateral;
            if (thatQuad == null) return false;

            if (!thatQuad.IsStrictQuadrilateral()) return false;

            return this.HasSamePoints(thatQuad);
        }

        //
        // Maintain a public repository of all triangles objects in the figure.
        //
        public static void Clear()
        {
            figureQuadrilaterals.Clear();
        }
        public static List<Quadrilateral> figureQuadrilaterals = new List<Quadrilateral>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Quadrilateral) figureQuadrilaterals.Add(clause as Quadrilateral);
        }
        public static Quadrilateral GetFigureQuadrilateral(Quadrilateral q)
        {
            // Search for exact segment first
            foreach (Quadrilateral quad in figureQuadrilaterals)
            {
                if (quad.StructurallyEquals(q)) return quad;
            }

            return null;
        }

        public override string ToString()
        {
            return "Quadrilateral(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                      bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }


        //
        // generate a Quadrilateral object, if the 4 segments construct a valid quadrilateral.
        //
        public static Quadrilateral GenerateQuadrilateral(List<Segment> segments)
        {
            if (segments.Count < 4) return null;

            return GenerateQuadrilateral(segments[0], segments[1], segments[2], segments[3]);
        }
        public static Quadrilateral GenerateQuadrilateral(Segment s1, Segment s2, Segment s3, Segment s4)
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


        //public bool HasSegment(Segment segment)
        //{
        //    return left.Equals(segment) || right.Equals(segment) || top.Equals(segment) || bottom.Equals(segment);
        //}

        ////
        //// Check directly if this angle is in the triangle
        //// Also check indirectly that the given angle is an extension (subangle) of this angle
        //// That is, all the rays are shared and the vertex is shared, but the endpoint of rays may be different
        ////
        //public bool HasAngle(Angle ca)
        //{
        //    return HasThisSpecificAngle(ca) || ExtendsAnAngle(ca);
        //}

        ////
        //// Acquire the particular angle which belongs to this triangle (of a congruence)
        ////
        //public Angle AngleBelongs(CongruentAngles ccas)
        //{
        //    if (HasAngle(ccas.ca1)) return ccas.ca1;
        //    if (HasAngle(ccas.ca2)) return ccas.ca2;
        //    return null;
        //}

        //// Of the congruent pair, return the segment that applies to this triangle
        //public Segment GetSegment(CongruentSegments ccss)
        //{
        //    if (HasSegment(ccss.cs1)) return ccss.cs1;
        //    if (HasSegment(ccss.cs2)) return ccss.cs2;

        //    return null;
        //}

        //// Of the propportional pair, return the segment that applies to this triangle
        //public Segment GetSegment(ProportionalSegments prop)
        //{
        //    if (HasSegment(prop.smallerSegment)) return prop.smallerSegment;
        //    if (HasSegment(prop.largerSegment)) return prop.largerSegment;

        //    return null;
        //}

        //public bool HasPoint(Point p)
        //{
        //    if (Point1.Equals(p)) return true;
        //    if (Point2.Equals(p)) return true;
        //    if (Point3.Equals(p)) return true;

        //    return false;
        //}

        //public Point OtherPoint(Segment cs)
        //{
        //    if (!cs.HasPoint(Point1)) return Point1;
        //    if (!cs.HasPoint(Point2)) return Point2;
        //    if (!cs.HasPoint(Point3)) return Point3;

        //    return null;
        //}

        //public Point OtherPoint(Point p1, Point p2)
        //{
        //    if (SegmentA.HasPoint(p1) && SegmentA.HasPoint(p2)) return OtherPoint(SegmentA);
        //    if (SegmentB.HasPoint(p1) && SegmentB.HasPoint(p2)) return OtherPoint(SegmentB);
        //    if (SegmentC.HasPoint(p1) && SegmentC.HasPoint(p2)) return OtherPoint(SegmentC);

        //    return null;
        //}


        //public List<Angle> GetAngles()
        //{
        //    List<Angle> angles = new List<Angle>();
        //    angles.Add(AngleA);
        //    angles.Add(AngleB);
        //    angles.Add(AngleC);
        //    return angles;
        //}

        //public List<Point> GetPoints()
        //{
        //    List<Point> pts = new List<Point>();
        //    pts.Add(Point1);
        //    pts.Add(Point2);
        //    pts.Add(Point3);
        //    return pts;
        //}

        //public List<Segment> GetSegments()
        //{
        //    List<Segment> segments = new List<Segment>();
        //    segments.Add(SegmentA);
        //    segments.Add(SegmentB);
        //    segments.Add(SegmentC);
        //    return segments;
        //}

        ///// <summary>
        ///// Determines if this is a right traingle.
        ///// </summary>
        ///// <returns>TRUE if this is a right triangle.</returns>
        //public bool isRightTriangle()
        //{
        //    return Utilities.CompareValues(AngleA.measure, 90) ||
        //           Utilities.CompareValues(AngleB.measure, 90) ||
        //           Utilities.CompareValues(AngleC.measure, 90);

        //    //bool right = false;
        //    //Segment[] segments = new Segment[3];
        //    //segments[0] = SegmentA;
        //    //segments[1] = SegmentB;
        //    //segments[2] = SegmentC;

        //    ////Compare vector representations of lines to see if dot product is 0.
        //    //for (int i = 0; i < 3; i++)
        //    //{
        //    //    int j = (i + 1) % 3;
        //    //    double v1x = segments[i].Point1.X - segments[i].Point2.X;
        //    //    double v1y = segments[i].Point1.Y - segments[i].Point2.Y;
        //    //    double v2x = segments[j].Point1.X - segments[j].Point2.X;
        //    //    double v2y = segments[j].Point1.Y - segments[j].Point2.Y;
        //    //    right = right || (v1x * v2x + v1y * v2y) == 0;
        //    //    if ((v1x * v2x + v1y * v2y) < EPSILON) // == 0
        //    //    {
        //    //        Point vertex = segments[i].SharedVertex(segments[j]);
        //    //        Point other1 = segments[i].OtherPoint(vertex);
        //    //        Point other2 = segments[j].OtherPoint(vertex);
        //    //        rightAngle = new Angle(other1, vertex, other2);
        //    //        return true;
        //    //    }
        //    //}
        //    //return false;
        //}

        ///// <summary>
        ///// Determines if this is an isosceles traingle.
        ///// </summary>
        ///// <returns>TRUE if this is an isosceles triangle.</returns>
        //private bool IsIsosceles()
        //{
        //    Segment[] segments = new Segment[3];
        //    segments[0] = SegmentA;
        //    segments[1] = SegmentB;
        //    segments[2] = SegmentC;

        //    for (int i = 0; i < segments.Length; i++)
        //    {
        //        if (segments[i].Length == segments[i + 1 < segments.Length ? i + 1 : 0].Length)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Determines if this is an equilateral traingle.
        ///// </summary>
        ///// <returns>TRUE if this is an equilateral triangle.</returns>
        //private bool IsEquilateral()
        //{
        //    return Utilities.CompareValues(SegmentA.Length, SegmentB.Length) &&
        //           Utilities.CompareValues(SegmentB.Length, SegmentC.Length);
        //}

        //public Angle GetOppositeAngle(Segment s)
        //{
        //    Point oppVertex = this.OtherPoint(s);

        //    if (oppVertex.Equals(AngleA.GetVertex())) return AngleA;
        //    if (oppVertex.Equals(AngleB.GetVertex())) return AngleB;
        //    if (oppVertex.Equals(AngleC.GetVertex())) return AngleC;

        //    return null;
        //}

        //public Segment GetOppositeSide(Angle angle)
        //{
        //    Point vertex = angle.GetVertex();

        //    if (!SegmentA.HasPoint(vertex)) return SegmentA;
        //    if (!SegmentB.HasPoint(vertex)) return SegmentB;
        //    if (!SegmentC.HasPoint(vertex)) return SegmentC;

        //    return null;
        //}

        //public Segment GetOppositeSide(Point vertex)
        //{
        //    if (!SegmentA.HasPoint(vertex)) return SegmentA;
        //    if (!SegmentB.HasPoint(vertex)) return SegmentB;
        //    if (!SegmentC.HasPoint(vertex)) return SegmentC;

        //    return null;
        //}

        //public Segment OtherSide(Angle a)
        //{
        //    Point vertex = a.GetVertex();

        //    if (!SegmentA.HasPoint(vertex)) return SegmentA;
        //    if (!SegmentB.HasPoint(vertex)) return SegmentB;
        //    if (!SegmentC.HasPoint(vertex)) return SegmentC;

        //    return null;
        //}

        //public Segment GetSegmentWithPointOnOrExtends(Point pt)
        //{
        //    if (SegmentA.PointIsOn(pt)) return SegmentA;
        //    if (SegmentB.PointIsOn(pt)) return SegmentB;
        //    if (SegmentC.PointIsOn(pt)) return SegmentC;

        //    return null;
        //}

        //public Segment GetSegmentWithPointDirectlyOn(Point pt)
        //{
        //    if (Segment.Between(pt, SegmentA.Point1, SegmentA.Point2)) return SegmentA;
        //    if (Segment.Between(pt, SegmentB.Point1, SegmentB.Point2)) return SegmentB;
        //    if (Segment.Between(pt, SegmentC.Point1, SegmentC.Point2)) return SegmentC;

        //    return null;
        //}

        //public void MakeIsosceles()
        //{
        //    if (!IsIsosceles())
        //    {
        //        Console.WriteLine("Deduced fact that this triangle is isosceles does not match empirical information for " + this.ToString());
        //    }

        //    provenIsosceles = true;
        //}


        //public bool LiesOn(Segment cs)
        //{
        //    return SegmentA.IsCollinearWith(cs) || SegmentB.IsCollinearWith(cs) || SegmentC.IsCollinearWith(cs);
        //}


        //// Does this triangle have this specific angle with these vertices
        //private bool HasThisSpecificAngle(Angle ca)
        //{
        //    return (HasSegment(ca.ray1) && HasSegment(ca.ray2)); // Could call SharedVertex
        //}

        //// Does the given angle correspond to any of the angles?
        //public bool ExtendsAnAngle(Angle ca)
        //{
        //    return ExtendsSpecificAngle(ca) != null;
        //}

        //// Does the given angle correspond to any of the angles?
        //public Angle ExtendsSpecificAngle(Angle ca)
        //{
        //    if (AngleA.Equates(ca)) return AngleA;
        //    if (AngleB.Equates(ca)) return AngleB;
        //    if (AngleC.Equates(ca)) return AngleC;

        //    return null;
        //}

        //// Does the given angle correspond to any of the angles?
        //public Angle GetAngleWithVertex(Point pt)
        //{
        //    if (AngleA.GetVertex().Equals(pt)) return AngleA;
        //    if (AngleB.GetVertex().Equals(pt)) return AngleB;
        //    if (AngleC.GetVertex().Equals(pt)) return AngleC;

        //    return null;
        //}


        //public Segment SharesSide(Quadrilateral cs)
        //{
        //    if (SegmentA.Equals(cs.SegmentA) || SegmentA.Equals(cs.SegmentB) || SegmentA.Equals(cs.SegmentC))
        //    {
        //        return SegmentA;
        //    }
        //    if (SegmentB.Equals(cs.SegmentA) || SegmentB.Equals(cs.SegmentB) || SegmentB.Equals(cs.SegmentC))
        //    {
        //        return SegmentB;
        //    }
        //    if (SegmentC.Equals(cs.SegmentA) || SegmentC.Equals(cs.SegmentB) || SegmentC.Equals(cs.SegmentC))
        //    {
        //        return SegmentC;
        //    }

        //    return null;
        //}



        ////
        //// Acquire the particular angle which belongs to this triangle (of a congruence)
        ////
        //public Angle OtherAngle(Angle thatAngle1, Angle thatAngle2)
        //{
        //    if (AngleA.Equates(thatAngle1) && AngleB.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleB.Equates(thatAngle1)) return AngleC;
        //    if (AngleB.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleB.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleA;
        //    if (AngleA.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleB;

        //    return null;
        //}

        //public bool IsIncludedAngle(Segment s1, Segment s2, Angle a)
        //{
        //    if (!HasSegment(s1) || !HasSegment(s2) && !HasAngle(a)) return false;

        //    // If the shared vertex between the segments is the vertex of this given angle, then
        //    // the angle is the included angle as desired
        //    return s1.SharedVertex(s2).Equals(a.GetVertex());
        //}



        //public KeyValuePair<Angle, Angle> GetAcuteAngles()
        //{
        //    if (AngleA.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleB, AngleC);
        //    if (AngleB.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleC);
        //    if (AngleC.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleB);

        //    return new KeyValuePair<Angle,Angle>(null, null);
        //}

        ////
        //// Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
        //// the angle, using only the triangles vertices to represent the angle
        ////
        //public Angle NormalizeAngle(Angle extendedAngle)
        //{
        //    return ExtendsSpecificAngle(extendedAngle);
        //}

        ////
        //// Returns the longest side of the triangle; arbitary choice if equal and longest
        ////
        //public Segment GetLongestSide()
        //{
        //    if (SegmentA.Length > SegmentB.Length)
        //    {
        //        if (SegmentA.Length > SegmentC.Length)
        //        {
        //            return SegmentA;
        //        }
        //    }
        //    else if (SegmentB.Length > SegmentC.Length)
        //    {
        //        return SegmentB;
        //    }

        //    return SegmentC;
        //}

        ////
        //// return the hypotenuse if we know we have a right triangle
        ////
        //public Segment GetHypotenuse()
        //{
        //    if (!isRight) return null;

        //    return GetLongestSide();
        //}

        ////
        //// Two sides known, return the third side
        ////
        //public Segment OtherSide(Segment s1, Segment s2)
        //{
        //    if (!HasSegment(s1) || !HasSegment(s2)) return null;
        //    if (!SegmentA.Equals(s1) && !SegmentA.Equals(s2)) return SegmentA;
        //    if (!SegmentB.Equals(s1) && !SegmentB.Equals(s2)) return SegmentB;
        //    if (!SegmentC.Equals(s1) && !SegmentC.Equals(s2)) return SegmentC;
        //    return null;
        //}

        //// 
        //// Quadrilateral(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C),
        ////                      Angle(A, B, C), Angle(B, C, A), Angle(C, A, B)
        ////
        //// RightTriangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C), m\angle ABC = 90^o
        ////
        //private static readonly string INTRINSIC_NAME = "Intrinsic";
        //private static Hypergraph.EdgeAnnotation intrinsicAnnotation = new Hypergraph.EdgeAnnotation(INTRINSIC_NAME, true);

        //public static List<GenericInstantiator.EdgeAggregator> Instantiate(GroundedClause c)
        //{
        //    List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

        //    Quadrilateral tri = c as Quadrilateral;
        //    if (tri == null) return newGrounded;

        //    // Generate the FOL for segments
        //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
        //    //tri.SegmentA.SetJustification("Intrinsic");
        //    //tri.SegmentB.SetJustification("Intrinsic");
        //    //tri.SegmentC.SetJustification("Intrinsic");
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentA, intrinsicAnnotation));
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentB, intrinsicAnnotation));
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentC, intrinsicAnnotation));

        //    //tri.AngleA.SetJustification("Intrinsic");
        //    //tri.AngleB.SetJustification("Intrinsic");
        //    //tri.AngleC.SetJustification("Intrinsic");
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleA, intrinsicAnnotation));
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleB, intrinsicAnnotation));
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleC, intrinsicAnnotation));

        //    // If this is a right triangle, generate the FOL equation
        //    if (tri.provenRight)
        //    {
        //        newGrounded.AddRange(Angle.Instantiate(tri, tri.rightAngle));
        //    }

        //    return newGrounded;
        //}

        ////
        //// Is the given angle directly exterior to this triangle?
        ////
        //// The triangle must share a side with the angle, the non-shared side must extend the adjacent side
        //public bool HasExteriorAngle(Angle extAngle)
        //{
        //    // Disallow any angle in this triangle (since it technically satisfies the requirements)
        //    if (HasAngle(extAngle)) return false;

        //    // Determine which angle in the triangle has the same vertex as the input angle
        //    Angle triangleAngle = GetAngleWithVertex(extAngle.GetVertex());

        //    if (triangleAngle == null) return false;

        //    // Acquire the ray that is shared between the angle and the triangle
        //    Segment sharedSegment = triangleAngle.SharedRay(extAngle);

        //    if (sharedSegment == null) return false;

        //    // Acquire the other side of the triangle
        //    Segment otherTriangleSegment = triangleAngle.OtherRay(sharedSegment);

        //    if (otherTriangleSegment == null) return false;

        //    // Acquire the ray that is not shared
        //    Segment exteriorSegment = extAngle.OtherRay(sharedSegment);

        //    if (exteriorSegment == null) return false;

        //    //           DISALLOW                                     ALLOW
        //    //              /                                           /
        //    //             / \                                         / \
        //    //            /TRI\                                       /TRI\
        //    //           /-----\                                     /-----\
        //    //                 /                                            \
        //    //                /                                              \
        //    //               /                                                \
        //    return otherTriangleSegment.IsCollinearWith(exteriorSegment);
        //}

        //// Determine if the given segment is coinciding with one of the triangle sides; return that 
        //public KeyValuePair<Segment, Segment> OtherSides(Segment candidate)
        //{
        //    if (SegmentA.Equals(candidate)) return new KeyValuePair<Segment,Segment>(SegmentB, SegmentC);
        //    if (SegmentB.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentC);
        //    if (SegmentC.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentB);

        //    return new KeyValuePair<Segment, Segment>(null, null);
        //}

        //// Determine if the given segment is coinciding with one of the triangle sides; return that 
        //public Segment CoincidesWithASide(Segment candidate)
        //{
        //    if (SegmentA.IsCollinearWith(candidate)) return SegmentA;
        //    if (SegmentB.IsCollinearWith(candidate)) return SegmentB;
        //    if (SegmentC.IsCollinearWith(candidate)) return SegmentC;

        //    return null;
        //}

        //// Determine if the given segment is coinciding with one of the triangle sides; return that 
        //public Segment DoesParallelCoincideWith(Parallel p)
        //{
        //    if (CoincidesWithASide(p.segment1) != null) return p.segment1;
        //    if (CoincidesWithASide(p.segment2) != null) return p.segment2;

        //    return null;
        //}

        ////
        //// Given a point on the triangle, return the two angles not at that vertex
        ////
        //public void AcquireRemoteAngles(Point givenVertex, out Angle remote1, out Angle remote2)
        //{
        //    remote1 = null;
        //    remote2 = null;

        //    if (!HasPoint(givenVertex)) return;

        //    if (AngleA.GetVertex().Equals(givenVertex))
        //    {
        //        remote1 = AngleB;
        //        remote2 = AngleC;
        //    }
        //    else if (AngleB.GetVertex().Equals(givenVertex))
        //    {
        //        remote1 = AngleA;
        //        remote2 = AngleC;
        //    }
        //    else
        //    {
        //        remote1 = AngleA;
        //        remote2 = AngleB;
        //    }
        //}

        ////
        //// Have we deduced a congrunence between this triangle and t already?
        ////
        //public bool HasEstablishedCongruence(Quadrilateral t)
        //{
        //    return congruencePairs.Contains(t);
        //}

        //// Add to the list of congruent triangles
        //public void AddCongruentTriangle(Quadrilateral t)
        //{
        //    congruencePairs.Add(t);
        //}

        ////
        //// Have we deduced a similarity between this triangle and t already?
        ////
        //public bool HasEstablishedSimilarity(Quadrilateral t)
        //{
        //    return similarPairs.Contains(t);
        //}

        //// Add to the list of similar triangles
        //public void AddSimilarTriangle(Quadrilateral t)
        //{
        //    similarPairs.Add(t);
        //}

        //public Point GetVertexOn(Segment thatSegment)
        //{
        //    if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentB, thatSegment)) return SegmentA.SharedVertex(SegmentB);
        //    if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentC, thatSegment)) return SegmentA.SharedVertex(SegmentC);
        //    if (Segment.IntersectAtSamePoint(this.SegmentB, this.SegmentC, thatSegment)) return SegmentB.SharedVertex(SegmentC);

        //    return null;
        //}

        //////
        ////// Is this triangle congruent to the given triangle in terms of the coordinatization from the UI?
        //////
        ////public KeyValuePair<Quadrilateral, Quadrilateral> CoordinateCongruent(Quadrilateral thatTriangle)
        ////{
        ////    bool[] marked = new bool[3];
        ////    List<Segment> thisSegments = GetSegments();
        ////    List<Segment> thatSegments = thatTriangle.GetSegments();

        ////    List<Segment> corrSegmentsThis = new List<Segment>();
        ////    List<Segment> corrSegmentsThat = new List<Segment>();

        ////    for (int thisS = 0; thisS < thisSegments.Count; thisS++)
        ////    {
        ////        bool found = false;
        ////        for (int thatS = 0; thatS < thatSegments.Count; thatS++)
        ////        {
        ////            if (!marked[thatS])
        ////            {
        ////                if (thisSegments[thisS].CoordinateCongruent(thatSegments[thatS]))
        ////                {
        ////                    corrSegmentsThat.Add(thatSegments[thatS]);
        ////                    corrSegmentsThis.Add(thisSegments[thisS]);
        ////                    marked[thatS] = true;
        ////                    found = true;
        ////                    break;
        ////                }
        ////            }
        ////        }
        ////        if (!found) return new KeyValuePair<Quadrilateral,Quadrilateral>(null, null);
        ////    }

        ////    //
        ////    // Find the exact corresponding points
        ////    //
        ////    List<Point> triThis = new List<Point>();
        ////    List<Point> triThat = new List<Point>();

        ////    for (int i = 0; i < 3; i++)
        ////    {
        ////        triThis.Add(corrSegmentsThis[i].SharedVertex(corrSegmentsThis[i + 1 < 3 ? i + 1 : 0]));
        ////        triThat.Add(corrSegmentsThat[i].SharedVertex(corrSegmentsThat[i + 1 < 3 ? i + 1 : 0]));
        ////    }

        ////    return new KeyValuePair<Quadrilateral, Quadrilateral>(new Quadrilateral(triThis), new Quadrilateral(triThat));
        ////}

        //////
        ////// Is this triangle similar to the given triangle in terms of the coordinatization from the UI?
        //////
        ////public bool CoordinateSimilar(Quadrilateral thatTriangle)
        ////{
        ////    bool[] congruentAngle = new bool[3];
        ////    List<Angle> thisAngles = GetAngles();
        ////    List<Angle> thatAngles = thatTriangle.GetAngles();

        ////    for (int thisS = 0; thisS < thisAngles.Count; thisS++)
        ////    {
        ////        int thatS = 0;
        ////        for (; thatS < thatAngles.Count; thatS++)
        ////        {
        ////            if (thisAngles[thisS].CoordinateCongruent(thatAngles[thatS]))
        ////            {
        ////                congruentAngle[thisS] = true;
        ////                break;
        ////            }
        ////        }

        ////        if (thatS == thatAngles.Count) return false;
        ////    }

        ////    KeyValuePair<Quadrilateral, Quadrilateral> corresponding = CoordinateCongruent(thatTriangle);
        ////    return !congruentAngle.Contains(false) && (corresponding.Key == null && corresponding.Value == null); // CTA: Congruence is stronger than Similarity
        ////}



        //public override bool CanBeStrengthenedTo(GroundedClause gc)
        //{
        //    Quadrilateral tri = gc as Quadrilateral;
        //    if (gc == null) return false;

        //    // Handles isosceles, right, or equilateral
        //    if (!this.StructurallyEquals(gc)) return false;

        //    // Ensure we know the original has been 'proven' (given) to be a particular type of triangle
        //    if (tri.provenIsosceles) this.provenIsosceles = true;
        //    if (tri.provenEquilateral) this.provenEquilateral = true;
        //    if (tri.provenRight) this.provenRight = true;

        //    return true;
        //}

        //public bool CoordinateMedian(Segment thatSegment)
        //{
        //    //
        //    // Two sides must intersect the median at a single point
        //    //
        //    Point midptIntersection = null;
        //    Point coincidingIntersection = null;
        //    Segment oppSide = null;
        //    if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
        //    {
        //        coincidingIntersection = SegmentA.FindIntersection(SegmentB);
        //        midptIntersection = SegmentC.FindIntersection(thatSegment);
        //        oppSide = SegmentC;
        //    }
        //    else if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
        //    {
        //        coincidingIntersection = SegmentA.FindIntersection(SegmentC);
        //        midptIntersection = SegmentB.FindIntersection(thatSegment);
        //        oppSide = SegmentB;
        //    }
        //    else if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
        //    {
        //        coincidingIntersection = SegmentB.FindIntersection(SegmentC);
        //        midptIntersection = SegmentA.FindIntersection(thatSegment);
        //        oppSide = SegmentA;
        //    }

        //    if (midptIntersection == null || oppSide == null) return false;

        //    // It is possible for the segment to be parallel to the opposite side; results in NaN.
        //    if (midptIntersection.X == double.NaN || midptIntersection.Y == double.NaN) return false;

        //    // The intersection must be on the potential median
        //    if (!thatSegment.PointIsOnAndBetweenEndpoints(coincidingIntersection)) return false;

        //    // The midpoint intersection must be on the potential median
        //    if (!thatSegment.PointIsOnAndBetweenEndpoints(midptIntersection)) return false;

        //    if (!Segment.Between(coincidingIntersection, thatSegment.Point1, thatSegment.Point2)) return false;

        //    if (!oppSide.PointIsOnAndBetweenEndpoints(midptIntersection)) return false;

        //    // Midpoint of the remaining side needs to align
        //    return midptIntersection.Equals(oppSide.Midpoint());
        //}

        ////
        //// Is this segment an altitude based on the coordinates (precomputation)
        ////
        //public bool CoordinateAltitude(Segment thatSegment)
        //{
        //    //
        //    // Check to see if the altitude is actually one of the sides of the triangle
        //    //
        //    if (this.HasSegment(thatSegment) && this.isRight)
        //    {
        //        // Find the right angle; the altitude must be one of those segments
        //        if (Utilities.CompareValues(this.AngleA.measure, 90)) return AngleA.HasSegment(thatSegment);
        //        if (Utilities.CompareValues(this.AngleB.measure, 90)) return AngleB.HasSegment(thatSegment);
        //        if (Utilities.CompareValues(this.AngleC.measure, 90)) return AngleC.HasSegment(thatSegment);
        //    }

        //    //
        //    // Two sides must intersect the given segment at a single point
        //    //
        //    Point otherIntersection = null;
        //    Point thisIntersection = null;
        //    Segment oppSide = null;
        //    if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
        //    {
        //        thisIntersection = SegmentA.FindIntersection(SegmentB);
        //        otherIntersection = SegmentC.FindIntersection(thatSegment);
        //        oppSide = SegmentC;
        //    }
        //    if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
        //    {
        //        thisIntersection = SegmentA.FindIntersection(SegmentC);
        //        otherIntersection = SegmentB.FindIntersection(thatSegment);
        //        oppSide = SegmentB;
        //    }
        //    if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
        //    {
        //        thisIntersection = SegmentB.FindIntersection(SegmentC);
        //        otherIntersection = SegmentA.FindIntersection(thatSegment);
        //        oppSide = SegmentA;
        //    }

        //    if (otherIntersection == null || oppSide == null) return false;

        //    // Avoid a dangling altitude:
        //    //
        //    // |\
        //    // | \
        //    // |  \
        //    //     \
        //    // Need to make sure 'this' and the the 'other' intersection is actually on the potential altitude segment
        //    if (!thatSegment.PointIsOnAndBetweenEndpoints(thisIntersection)) return false;
        //    if (!thatSegment.PointIsOnAndBetweenEndpoints(otherIntersection)) return false;

        //    // We require a perpendicular intersection
        //    return Utilities.CompareValues((new Angle(thisIntersection, otherIntersection, oppSide.Point1)).measure, 90);
        //}

        //// Returns the exact correspondence between the triangles; <this, that>
        //public Dictionary<Point, Point> PointsCorrespond(Quadrilateral thatTriangle)
        //{
        //    if (!this.StructurallyEquals(thatTriangle)) return null;

        //    List<Point> thatTrianglePts = thatTriangle.points;
        //    List<Point> thisTrianglePts = this.points;

        //    // Find the index of the first point (in this Quadrilateral) 
        //    int i = 0;
        //    for (; i < 3; i++)
        //    {
        //        if (thisTrianglePts[0].StructurallyEquals(thatTrianglePts[i])) break;
        //    }

        //    // Sanity check; something bad happened
        //    if (i == 3) return null;

        //    Dictionary<Point, Point> correspondence = new Dictionary<Point, Point>();
        //    for (int j = 0; j < 3; j++)
        //    {
        //        if (!thisTrianglePts[j].StructurallyEquals(thatTrianglePts[(j + i) % 3])) return null;
        //        correspondence.Add(thisTrianglePts[j], thatTrianglePts[(j + i) % 3]);
        //    }

        //    return correspondence;
        //}

    }
}
