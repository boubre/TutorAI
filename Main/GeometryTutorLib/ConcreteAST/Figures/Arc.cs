using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Arc : Figure
    {
        public Circle theCircle { get; private set; }
        public Point endpoint1 { get; private set; }
        public Point endpoint2 { get; private set; }
        public List<Point> arcMinorPoints { get; private set; }
        public List<Point> arcMajorPoints { get; private set; }
        public double minorMeasure { get; private set; }
        public double length { get; private set; }

        public Arc(Circle circle, Point e1, Point e2) : this(circle, e1, e2, new List<Point>(), new List<Point>()) { }

        public Arc(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts) : base()
        {
            theCircle = circle;
            e1 = endpoint1;
            e2 = endpoint2;
            arcMinorPoints = new List<Point>(minorPts);
            arcMajorPoints = new List<Point>(majorPts);

            Utilities.AddUniqueStructurally(e1.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(e2.getSuperFigures(), this);

            minorMeasure = CalculateArcMinorMeasureDegrees();
            length = CalculateArcMinorLength();
        }

        //
        // Calculate the length of the arc: s = r * theta (radius * central angle)
        //
        private double CalculateArcMinorLength() { return GetMinorArcMeasureRadians() * theCircle.radius; }

        //
        // The measure of the minor arc is equal to the measure of the central angle it cuts out.
        // This is calculated in degrees.
        //
        private double CalculateArcMinorMeasureDegrees()
        {
            return new Angle(new Segment(theCircle.center, endpoint1), new Segment(theCircle.center, endpoint2)).measure;
        }
        private double CalculateArcMinorMeasureRadians()
        {
            return Angle.toRadians(new Angle(new Segment(theCircle.center, endpoint1), new Segment(theCircle.center, endpoint2)).measure);
        }

        public double GetMinorArcMeasureDegrees() { return minorMeasure; }
        public double GetMinorArcMeasureRadians() { return Angle.toRadians(GetMinorArcMeasureDegrees()); }
        public double GetMajorArcMeasureDegrees() { return 360 - minorMeasure; }
        public double GetMajorArcMeasureRadians() { return Angle.toRadians(GetMajorArcMeasureDegrees()); }

        //
        // Maintain a public repository of all segment objects in the figure.
        //
        public static void Clear()
        {
            figureArcs.Clear();
        }
        public static List<Arc> figureArcs = new List<Arc>();
        public static void Record(GroundedClause clause)
        {
            if (clause is Arc) figureArcs.Add(clause as Arc);
        }
        public static Arc GetFigureArc(Circle circle, Point pt1, Point pt2)
        {
            Arc candArc = new Arc(circle, pt1, pt2);

            // Search for exact segment first
            foreach (Arc arc in figureArcs)
            {
                if (arc.StructurallyEquals(candArc)) return arc;
            }

            return null;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Arc arc = obj as Arc;
            if (arc == null) return false;

            return this.theCircle.StructurallyEquals(arc.theCircle) && this.endpoint1.StructurallyEquals(arc.endpoint1)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint2);
        }

        public override bool Equals(Object obj)
        {
            Arc arc = obj as Arc;
            if (arc == null) return false;

            // Check equality of arc minor / major points?

            return this.theCircle.Equals(arc.theCircle) && this.endpoint1.Equals(arc.endpoint1)
                                                        && this.endpoint2.Equals(arc.endpoint2);
        }

        public override string ToString() { return "Arc(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "))"; }

        // Does this arc contain a sub-arc:
        // A-------B-------C------D
        // A subarc is: AB, AC, AD, BC, BD, CD
        public bool HasMinorSubArc(Arc arc)
        {
            return Arc.BetweenMinor(arc.endpoint1, this) && Arc.BetweenMinor(arc.endpoint2, this);
        }

        public bool HasStrictMinorSubArc(Arc arc)
        {
            return Arc.StrictlyBetweenMinor(arc.endpoint1, this) && Arc.StrictlyBetweenMinor(arc.endpoint2, this);
        }

        public bool HasMajorSubArc(Arc arc)
        {
            return Arc.BetweenMajor(arc.endpoint1, this) && Arc.BetweenMajor(arc.endpoint2, this);
        }

        public bool HasStrictMajorSubArc(Arc arc)
        {
            return Arc.StrictlyBetweenMajor(arc.endpoint1, this) && Arc.StrictlyBetweenMajor(arc.endpoint2, this);
        }

        //
        // Is M between A and B in the minor arc
        //
        public static bool BetweenMinor(Point m, Arc originalArc)
        {
            // Is the point on this circle?
            if (!originalArc.theCircle.PointIsOn(m)) return false;

            // Create two arcs from this new point to the endpoints; just like with segments,
            // the sum of the arc measures must equate to the overall arc measure.
            Arc arc1 = new Arc(originalArc.theCircle, m, originalArc.endpoint1);
            Arc arc2 = new Arc(originalArc.theCircle, m, originalArc.endpoint2);
 
            return Utilities.CompareValues(arc1.minorMeasure + arc2.minorMeasure, originalArc.minorMeasure);
        }

        public static bool StrictlyBetweenMinor(Point m, Arc originalArc)
        {
            if (originalArc.HasEndpoint(m)) return false;

            return BetweenMinor(m, originalArc);
        }

        //
        // If it's on the circle and not in the minor arc, it's in the major arc.
        //
        public static bool BetweenMajor(Point m, Arc originalArc)
        {
            // Is the point on this circle?
            if (!originalArc.theCircle.PointIsOn(m)) return false;

            // Is it on the arc minor?
            if (BetweenMinor(m, originalArc)) return false;

            return true;
        }

        public static bool StrictlyBetweenMajor(Point m, Arc originalArc)
        {
            if (originalArc.HasEndpoint(m)) return false;

            return BetweenMajor(m, originalArc);
        }

        public bool HasEndpoint(Point p)
        {
            return endpoint1.Equals(p) || endpoint2.Equals(p);
        }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            Arc other = (Arc)(this.MemberwiseClone());
            other.endpoint1 = (Point)endpoint1.DeepCopy();
            other.endpoint2 = (Point)endpoint2.DeepCopy();

            return other;
        }

        //
        // Is this arc congruent to the given arc in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Arc a) { return Utilities.CompareValues(this.length, a.length); }

        //
        // Is this segment proportional to the given segment in terms of the coordinatization from the UI?
        // We should not report proportional if the ratio between segments is 1
        //
        public KeyValuePair<int, int> CoordinateProportional(Arc a) { return Utilities.RationalRatio(this.length, a.length); }

        //
        // Concentric
        //
        public bool IsConcentricWith(Arc thatArc) { return this.theCircle.AreConcentric(thatArc.theCircle); }
        //
        // Orthogonal
        //
        //
        // Orthogonal arcs intersect at 90^0: radii connecting to intersection point are perpendicular.
        //
        public bool AreOrthognal(Arc thatArc)
        {
            if (!this.theCircle.AreOrthognal(thatArc.theCircle)) return false;
            
            // Find the intersection points
            Point inter1;
            Point inter2;
            this.theCircle.Intersection(thatArc.theCircle, out inter1, out inter2);

            // Is the intersection between the endpoints of both arcs? Check both.
            if (Arc.BetweenMinor(inter1, this) && Arc.BetweenMinor(inter1, thatArc)) return true;
            if (Arc.BetweenMinor(inter2, this) && Arc.BetweenMinor(inter2, thatArc)) return true;

            return false;
        }

        //
        // Tangent circle have 1 intersection point
        //
        public Point AreTangent(Arc thatArc)
        {
            Point intersection = this.theCircle.AreTangent(thatArc.theCircle);

            // Is the intersection between the endpoints of both arcs? Check both.
            if (Arc.BetweenMinor(intersection, this) && Arc.BetweenMinor(intersection, thatArc)) return intersection;
            if (Arc.BetweenMinor(intersection, this) && Arc.BetweenMinor(intersection, thatArc)) return intersection;

            return null;
        }



        //
        // Is thatArc a bisector of this segment in terms of the coordinatization from the UI?
        //
        //public Point CoordinateBisector(Segment thatSegment)
        //{
        //    return Utilities.CompareValues()
        //    // Do these segments intersect within both sets of stated endpoints?
        //    Point intersection = this.FindIntersection(thatSegment);

        //    if (!this.PointIsOnAndExactlyBetweenEndpoints(intersection)) return null;
        //    if (!thatSegment.PointIsOnAndBetweenEndpoints(intersection)) return null;

        //    // Do they intersect in the middle of this segment
        //    return Utilities.CompareValues(Point.calcDistance(this.Point1, intersection), Point.calcDistance(this.Point2, intersection)) ? intersection : null;
        //}

        ////
        //// Each segment is congruent to itself; only generate if it is a shared segment
        ////
        //private static readonly string REFLEXIVE_SEGMENT_NAME = "Reflexive Segments";
        //private static Hypergraph.EdgeAnnotation reflexAnnotation = new Hypergraph.EdgeAnnotation(REFLEXIVE_SEGMENT_NAME, JustificationSwitch.REFLEXIVE);

        //public static List<GenericInstantiator.EdgeAggregator> Instantiate(GroundedClause gc)
        //{
        //    List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

        //    Segment segment = gc as Segment;
        //    if (segment == null) return newGrounded;

        //    // Only generate reflexive if this segment is shared
        //    if (!segment.isShared()) return newGrounded;

        //    GeometricCongruentSegments ccss = new GeometricCongruentSegments(segment, segment);
        //    ccss.MakeIntrinsic(); // This is an 'obvious' notion so it should be intrinsic to any figure

        //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(segment);
        //    newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, ccss, reflexAnnotation));

        //    return newGrounded;
        //}

        ////
        ////     PointA
        ////     |
        ////     |             X (pt)
        ////     |_____________________ otherSegment
        ////     |
        ////     |
        ////     PointB
        ////
        //public Point SameSidePoint(Segment otherSegment, Point pt)
        //{
        //    // Is the given point on other? If so, we cannot make a determination.
        //    if (otherSegment.PointIsOn(pt)) return null;

        //    // Make a vector out of this vector as well as the vector connecting one of the points to the given pt
        //    Vector thisVector = new Vector(Point1, Point2);
        //    Vector thatVector = new Vector(Point1, pt);

        //    Vector projectionOfOtherOntoThis = thisVector.Projection(thatVector);

        //    // We are interested most in the endpoint of the projection (which is not the 
        //    Point projectedEndpoint = projectionOfOtherOntoThis.NonOriginEndpoint();

        //    // Find the intersection between the two lines
        //    Point intersection = FindIntersection(otherSegment);

        //    if (this.PointIsOn(projectedEndpoint))
        //    {
        //        System.Diagnostics.Debug.WriteLine("Unexpected: Projection does not lie on this line. " + this + " " + projectedEndpoint);
        //    }

        //    // The endpoint of the projection is on this vector. Therefore, we can judge which side of the given segment the given pt lies on.
        //    if (Segment.Between(projectedEndpoint, Point1, intersection)) return Point1;
        //    if (Segment.Between(projectedEndpoint, Point2, intersection)) return Point2;

        //    return null;
        //}

        //public Point Midpoint()
        //{
        //    return new Point(null, (Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        //}

        ////
        //// Do these angles share this segment overlay this angle?
        ////
        //public bool IsIncludedSegment(Angle ang1, Angle ang2)
        //{
        //    return this.Equals(ang1.SharedRay(ang2));
        //}

        //// Is the given clause an intrinsic component of this Segment?
        //public override bool Covers(GroundedClause gc)
        //{
        //    // immeidate hierarchy: a segment covers a point
        //    if (gc is Point) return this.PointIsOnAndBetweenEndpoints(gc as Point);

        //    // A triangle is covered if at least one of the sides is covered
        //    if (gc is Triangle) return (gc as Triangle).HasSegment(this);

        //    // If the segments are coinciding and have a point in between this segment, we say this segment is covered.
        //    Segment thatSegment = gc as Segment;
        //    if (thatSegment == null) return false;

        //    if (!this.IsCollinearWith(thatSegment)) return false;

        //    return this.PointIsOnAndBetweenEndpoints(thatSegment.Point1) || this.PointIsOnAndBetweenEndpoints(thatSegment.Point2);
        //}

        ////
        //// Determine the intersection point of the two segments
        ////
        ////
        //// | a b |
        //// | c d |
        ////
        //private double determinant(double a, double b, double c, double d)
        //{
        //    return a * d - b * c;
        //}
        //private void MakeLine(double x_1, double y_1, double x_2, double y_2, out double a, out double b, out double c)
        //{
        //    double slope = (y_2 - y_1) / (x_2 - x_1);
        //    a = - slope;
        //    b = 1;
        //    c = y_2 - slope * x_2;
        //}
        //private double EvaluateYGivenX(double a, double b, double e, double x)
        //{
        //    // ax + by = e
        //    return (e - a * x) / b;
        //}
        //private double EvaluateXGivenY(double a, double b, double e, double y)
        //{
        //    // ax + by = e
        //    return (e - b * y) / a;
        //}
        //public Point FindIntersection(Segment thatSegment)
        //{
        //    double a, b, c, d, e, f;

        //    if (this.IsVertical() && thatSegment.IsHorizontal()) return new Point(null, this.Point1.X, thatSegment.Point1.Y);

        //    if (thatSegment.IsVertical() && this.IsHorizontal()) return new Point(null, thatSegment.Point1.X, this.Point1.Y);

        //    if (this.IsVertical())
        //    {
        //        MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out a, out b, out e);
        //        return new Point(null, this.Point1.X, EvaluateYGivenX(a, b, e, this.Point1.X));
        //    }
        //    if (thatSegment.IsVertical())
        //    {
        //        MakeLine(this.Point1.X, this.Point1.Y, this.Point2.X, this.Point2.Y, out a, out b, out e);
        //        return new Point(null, thatSegment.Point1.X, EvaluateYGivenX(a, b, e, thatSegment.Point1.X));
        //    }
        //    if (this.IsHorizontal())
        //    {
        //        MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out a, out b, out e);
        //        return new Point(null, EvaluateXGivenY(a, b, e, this.Point1.Y), this.Point1.Y);
        //    }
        //    if (thatSegment.IsHorizontal())
        //    {
        //        MakeLine(this.Point1.X, this.Point1.Y, this.Point2.X, this.Point2.Y, out a, out b, out e);
        //        return new Point(null, EvaluateXGivenY(a, b, e, thatSegment.Point1.Y), thatSegment.Point1.Y);
        //    }

        //    //
        //    // ax + by = e
        //    // cx + dy = f
        //    // 

        //    MakeLine(Point1.X, Point1.Y, Point2.X, Point2.Y, out a, out b, out e);
        //    MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out c, out d, out f);

        //    double overallDeterminant = a * d - b * c;
        //    double x = determinant(e, b, f, d) / overallDeterminant;
        //    double y = determinant(a, e, c, f) / overallDeterminant;

        //    return new Point("Intersection", x, y);
        //}

        //private class Vector
        //{
        //    private double originX;
        //    private double originY;
        //    private double otherX;
        //    private double otherY;

        //    public Vector(Point origin, Point other)
        //    {
        //        originX = origin.X;
        //        originY = origin.Y;
        //        otherX = other.X;
        //        otherY = other.Y;
        //    }

        //    public Vector(double x1, double y1, double x2, double y2)
        //    {
        //        originX = x1;
        //        originY = y1;
        //        otherX = x2;
        //        otherY = y2;
        //    }

        //    public Point NonOriginEndpoint() { return new Point("ProjectedEndpoint", otherX, otherY); }

        //    private double DotProduct() { return originX * otherX + originY * otherY; }
        //    private static double EuclideanDistance(double x1, double y1, double x2, double y2)
        //    {
        //        return System.Math.Sqrt(System.Math.Pow(x1 - x2, 2) + System.Math.Pow(y1 - y2, 2));
        //    }

        //    //
        //    // Projects the given vector onto this vector using standard vector projection
        //    //
        //    public Vector Projection(Vector thatVector)
        //    {
        //        double magnitude = EuclideanDistance(thatVector.originX, thatVector.originY, thatVector.otherX, thatVector.otherY);
        //        double cosIncluded = CosineOfIncludedAngle(thatVector);

        //        double projectionDistance = magnitude * cosIncluded;

        //        return new Vector(originX, originY, otherX / projectionDistance, otherY / projectionDistance);
        //    }

        //    //
        //    // Use Law of Cosines to determine cos(\theta)
        //    //      ^
        //    //      / \
        //    //   a /   \ c
        //    //    /\    \
        //    //   /__\____\__>
        //    //       b 
        //    //
        //    private double CosineOfIncludedAngle(Vector thatVector)
        //    {
        //        if (HasSameOriginPoint(thatVector)) return -2;

        //        double a = EuclideanDistance(originX, originY, otherX, otherY);
        //        double b = EuclideanDistance(originX, originY, thatVector.otherX, thatVector.otherY);
        //        double c = EuclideanDistance(otherX, otherY, thatVector.otherX, thatVector.otherY);

        //        // Law of Cosines
        //        return (Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b);
        //    }

        //    private bool HasSameOriginPoint(Vector thatVector)
        //    {
        //        return Utilities.CompareValues(originX, thatVector.originX) && Utilities.CompareValues(originY, thatVector.originY);
        //    }

        //    public override string ToString()
        //    {
        //        return "(" + originX + ", " + originY + ") -> (" + otherX + ", " + otherY + ")";
        //    }
        //}

        ////
        //// Return the line passing through the given point which is perpendicular to this segment. 
        ////
        //public Point ProjectOnto(Point pt)
        //{
        //    //
        //    // Special Cases
        //    //
        //    if (this.IsVertical())
        //    {
        //        Point newPoint = Point.GetFigurePoint(new Point("", this.Point1.X, pt.Y));

        //        return newPoint != null ? newPoint : new Point("", this.Point1.X, pt.Y);
        //    }

        //    if (this.IsHorizontal())
        //    {
        //        Point newPoint = Point.GetFigurePoint(new Point("", pt.X, this.Point1.Y));

        //        return newPoint != null ? newPoint : new Point("", pt.X, this.Point1.Y);
        //    }

        //    //
        //    // General Cases
        //    //

        //    // Find the line perpendicular; specifically, a point on that line
        //    double perpSlope = -1 / Slope;

        //    // We will choose a random value for x (to acquire y); we choose 1.
        //    double newX = pt.X == 0 ? 1 : 0;

        //    double newY = pt.Y + perpSlope * (newX - pt.X);

        //    // The new perpendicular segment is defined by (newX, newY) and pt
        //    return new Point("", newX, newY);
        //}

        ////
        //// Return the line passing through the given point which is perpendicular to this segment. 
        ////
        //public Segment GetPerpendicular(Point pt)
        //{
        //    // If the given point is already on the line, projection does not create new information.
        //    if (this.PointIsOnAndBetweenEndpoints(pt)) return this;

        //    Point projection = ProjectOnto(pt);

        //    // The new perpendicular segment is defined by the projection and the point
        //    return new Segment(projection, pt);
        //}
    }
}