﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A segment defined by two points.
    /// </summary>
    public class Segment : Figure
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }
        public double Length { get; private set; }
        public double Slope { get; private set; }

        /// <summary>
        /// Create a new ConcreteSegment. 
        /// </summary>
        /// <param name="p1">A point defining the segment.</param>
        /// <param name="p2">Another point defining the segment.</param>
        public Segment(Point p1, Point p2) : base()
        {
            Point1 = p1;
            Point2 = p2;
            Length = Point.calcDistance(p1, p2);
            Slope = (p2.Y - p1.Y) / (p2.X - p1.X);

            Utilities.AddUniqueStructurally(Point1.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point2.getSuperFigures(), this);
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteSegment [l=");
            sb.Append(Length);
            sb.Append(']');
            sb.AppendLine();
            Point1.BuildUnparse(sb, tabDepth + 1);
            Point2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Segment segment = obj as Segment;
            if (segment == null) return false;

            return ((segment.Point1.StructurallyEquals(Point1) && segment.Point2.StructurallyEquals(Point2)) ||
                    (segment.Point1.StructurallyEquals(Point2) && segment.Point2.StructurallyEquals(Point1)));
        }

        public override bool Equals(Object obj)
        {
            Segment segment = obj as Segment;
            if (segment == null) return false;

            return base.Equals(obj) && ((segment.Point1.Equals(Point1) && segment.Point2.Equals(Point2)) ||
                                        (segment.Point1.Equals(Point2) && segment.Point2.Equals(Point1)));
        }

        //
        // Use point-slope form to determine if the given point is on the line
        //
        public bool PointIsOn(Point thatPoint)
        {
            // If the segments are vertical, just compare the X values of one point of each
            if (this.IsVertical())
            {
                return Utilities.CompareValues(this.Point1.X, thatPoint.X);
            }

            // If the segments are horizontal, just compare the Y values of one point of each; this is redundant
            if (this.IsHorizontal())
            {
                return Utilities.CompareValues(this.Point1.Y, thatPoint.Y);
            }

            return Utilities.CompareValues(this.Point1.Y - thatPoint.Y, this.Slope * (this.Point1.X - thatPoint.X));
        }

        //
        // Use point-slope form to determine if the given point is on the line
        //
        public bool PointIsOnAndBetweenEndpoints(Point thatPoint)
        {
            return Segment.Between(thatPoint, Point1, Point2);
        }

        public bool PointIsOnAndExactlyBetweenEndpoints(Point thatPoint)
        {
            if (Point1.Equals(thatPoint) || Point2.Equals(thatPoint)) return false;

            return Segment.Between(thatPoint, Point1, Point2);
        }

        // Does this segment contain a subsegment:
        // A-------B-------C------D
        // A subsegment is: AB, AC, AD, BC, BD, CD
        public bool HasSubSegment(Segment possSubSegment)
        {
            return this.PointIsOnAndBetweenEndpoints(possSubSegment.Point1) && this.PointIsOnAndBetweenEndpoints(possSubSegment.Point2);
        }

        public bool HasStrictSubSegment(Segment possSubSegment)
        {
            return (this.PointIsOnAndBetweenEndpoints(possSubSegment.Point1) && this.PointIsOnAndExactlyBetweenEndpoints(possSubSegment.Point2)) ||
                   (this.PointIsOnAndBetweenEndpoints(possSubSegment.Point2) && this.PointIsOnAndExactlyBetweenEndpoints(possSubSegment.Point1));
        }

        public bool IsVertical()
        {
            return Utilities.CompareValues(this.Point1.X, this.Point2.X);
        }

        public bool IsHorizontal()
        {
            return Utilities.CompareValues(this.Point1.Y, this.Point2.Y);
        }

        //
        // Determine if the given segment is collinear with this segment (same slope and they share a point)
        //  
        public bool IsCollinearWith(Segment otherSegment)
        {
            // If the segments are vertical, just compare the X values of one point of each
            if (this.IsVertical() && otherSegment.IsVertical())
            {
                return Utilities.CompareValues(this.Point1.X, otherSegment.Point1.X);
            }

            // If the segments are horizontal, just compare the Y values of one point of each; this is redundant
            if (this.IsHorizontal() && otherSegment.IsHorizontal())
            {
                return Utilities.CompareValues(this.Point1.Y, otherSegment.Point1.Y);
            }

            return Utilities.CompareValues(this.Slope, otherSegment.Slope) &&
                   this.PointIsOn(otherSegment.Point1) && this.PointIsOn(otherSegment.Point2); // Check both endpoints just to be sure
        }

        public Point SharedVertex(Segment s)
        {
            if (Point1.Equals(s.Point1)) return Point1;
            if (Point1.Equals(s.Point2)) return Point1;
            if (Point2.Equals(s.Point1)) return Point2;
            if (Point2.Equals(s.Point2)) return Point2;
            return null;
        }

        public Point OtherPoint(Point p)
        {
            if (p.Equals(Point1)) return Point2;
            if (p.Equals(Point2)) return Point1;

            return null;
        }

        // Is M between A and B; uses segment addition
        public static bool Between(Point M, Point A, Point B)
        {
            return Utilities.CompareValues(Point.calcDistance(A, M) + Point.calcDistance(M, B),
                                           Point.calcDistance(A, B));
        }

        // Does the given segment overlay this segment; we are looking at both as a RAY only.
        // We assume both rays share the same start vertex
        public bool RayOverlays(Segment thatRay)
        {
            if (this.Equals(thatRay)) return true;

            if (!this.IsCollinearWith(thatRay)) return false;

            // Do they share a vertex?
            Point shared = this.SharedVertex(thatRay);

            if (shared == null) return false;

            Point thatOtherPoint = thatRay.OtherPoint(shared);
            Point thisOtherPoint = this.OtherPoint(shared);

            // Is thatRay smaller than the this ray
            if (Between(thatOtherPoint, shared, thisOtherPoint)) return true;

            // Or if that Ray extends this Ray
            if (Between(thisOtherPoint, shared, thatOtherPoint)) return true;

            return false;
        }

        public bool HasPoint(Point p)
        {
            return Point1.Equals(p) || Point2.Equals(p);
        }

        public override bool Contains(GroundedClause target)
        {
            return this.Equals(target);
        }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            Segment other = (Segment)(this.MemberwiseClone());
            other.Point1 = (Point)Point1.DeepCopy();
            other.Point2 = (Point)Point2.DeepCopy();

            return other;
        }

        //
        // Is this segment congruent to the given segment in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Segment s)
        {
            return Utilities.CompareValues(s.Length, this.Length);
        }

        //
        // Is this segment proportional to the given segment in terms of the coordinatization from the UI?
        // We should not report proportional if the ratio between segments is 1
        //
        public KeyValuePair<int, int> CoordinateProportional(Segment s)
        {
            return Utilities.RationalRatio(s.Length, this.Length);
        }

        //
        // Parallel and not Coinciding
        //
        public bool IsParallelWith(Segment s)
        {
            if (IsCollinearWith(s)) return false;

            if (IsVertical() && s.IsVertical()) return true;

            if (IsHorizontal() && s.IsHorizontal()) return true;

            return Utilities.CompareValues(s.Slope, this.Slope);
        }

        //
        // Is this segment parallel to the given segment in terms of the coordinatization from the UI?
        //
        public bool CoordinateParallel(Segment s)
        {
            return IsParallelWith(s);
        }

        public static bool IntersectAtSamePoint(Segment seg1, Segment seg2, Segment seg3)
        {
            Point intersection1 = seg1.FindIntersection(seg3);
            Point intersection2 = seg2.FindIntersection(seg3);

            return intersection1.Equals(intersection2);
        }

        //
        // Is this segment perpendicular to the given segment in terms of the coordinatization from the UI?
        //
        public Point CoordinatePerpendicular(Segment thatSegment)
        {
            //
            // Do these segments intersect within both sets of stated endpoints?
            //
            Point intersection = this.FindIntersection(thatSegment);

            if (!this.PointIsOnAndBetweenEndpoints(intersection)) return null;
            if (!thatSegment.PointIsOnAndBetweenEndpoints(intersection)) return null;

            //
            // Special Case
            //
            if ((IsVertical() && thatSegment.IsHorizontal()) || (thatSegment.IsVertical() && IsHorizontal())) return intersection;

            // Does m1 * m2 = -1 (opposite reciprocal slopes)
            return Utilities.CompareValues(thatSegment.Slope * this.Slope, -1) ? intersection : null;
        }

        //
        // Is thatSegment a bisector of this segment in terms of the coordinatization from the UI?
        //
        public Point CoordinateBisector(Segment thatSegment)
        {
            // Do these segments intersect within both sets of stated endpoints?
            Point intersection = this.FindIntersection(thatSegment);

            if (!this.PointIsOnAndExactlyBetweenEndpoints(intersection)) return null;
            if (!thatSegment.PointIsOnAndBetweenEndpoints(intersection)) return null;

            // Do they intersect in the middle of this segment
            return Utilities.CompareValues(Point.calcDistance(this.Point1, intersection), Point.calcDistance(this.Point2, intersection)) ? intersection : null;
        }

        //
        // Each segment is congruent to itself; only generate if it is a shared segment
        //
        private static readonly string REFLEXIVE_SEGMENT_NAME = "Reflexive Segments";
        private static Hypergraph.EdgeAnnotation reflexAnnotation = new Hypergraph.EdgeAnnotation(REFLEXIVE_SEGMENT_NAME, JustificationSwitch.REFLEXIVE);

        public static List<GenericInstantiator.EdgeAggregator> Instantiate(GroundedClause gc)
        {
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            Segment segment = gc as Segment;
            if (segment == null) return newGrounded;

            // Only generate reflexive if this segment is shared
            if (!segment.isShared()) return newGrounded;

            GeometricCongruentSegments ccss = new GeometricCongruentSegments(segment, segment);
            ccss.MakeIntrinsic(); // This is an 'obvious' notion so it should be intrinsic to any figure

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(segment);
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, ccss, reflexAnnotation));

            return newGrounded;
        }

        //
        //     PointA
        //     |
        //     |             X (pt)
        //     |_____________________ otherSegment
        //     |
        //     |
        //     PointB
        //
        public Point SameSidePoint(Segment otherSegment, Point pt)
        {
            // Is the given point on other? If so, we cannot make a determination.
            if (otherSegment.PointIsOn(pt)) return null;

            // Make a vector out of this vector as well as the vector connecting one of the points to the given pt
            Vector thisVector = new Vector(Point1, Point2);
            Vector thatVector = new Vector(Point1, pt);

            Vector projectionOfOtherOntoThis = thisVector.Projection(thatVector);

            // We are interested most in the endpoint of the projection (which is not the 
            Point projectedEndpoint = projectionOfOtherOntoThis.NonOriginEndpoint();

            // Find the intersection between the two lines
            Point intersection = FindIntersection(otherSegment);

            if (this.PointIsOn(projectedEndpoint))
            {
                System.Diagnostics.Debug.WriteLine("Unexpected: Projection does not lie on this line. " + this + " " + projectedEndpoint);
            }

            // The endpoint of the projection is on this vector. Therefore, we can judge which side of the given segment the given pt lies on.
            if (Segment.Between(projectedEndpoint, Point1, intersection)) return Point1;
            if (Segment.Between(projectedEndpoint, Point2, intersection)) return Point2;

            return null;
        }

        public Point Midpoint()
        {
            return new Point(null, (Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        }

        public override string ToString() { return "Segment(" + Point1.ToString() + ", " + Point2.ToString() + ")"; }

        //
        // Do these angles share this segment overlay this angle?
        //
        public bool IsIncludedSegment(Angle ang1, Angle ang2)
        {
            return this.Equals(ang1.SharedRay(ang2));
        }

        // Is the given clause an intrinsic component of this Segment?
        public override bool Covers(GroundedClause gc)
        {
            // immeidate hierarchy: a segment covers a point
            if (gc is Point) return this.PointIsOnAndBetweenEndpoints(gc as Point);

            // A triangle is covered if at least one of the sides is covered
            if (gc is Triangle) return (gc as Triangle).HasSegment(this);

            // If the segments are coinciding and have a point in between this segment, we say this segment is covered.
            Segment thatSegment = gc as Segment;
            if (thatSegment == null) return false;

            if (!this.IsCollinearWith(thatSegment)) return false;

            return this.PointIsOnAndBetweenEndpoints(thatSegment.Point1) || this.PointIsOnAndBetweenEndpoints(thatSegment.Point2);
        }

        //
        // Determine the intersection point of the two segments
        //
        //
        // | a b |
        // | c d |
        //
        private double determinant(double a, double b, double c, double d)
        {
            return a * d - b * c;
        }
        private void MakeLine(double x_1, double y_1, double x_2, double y_2, out double a, out double b, out double c)
        {
            double slope = (y_2 - y_1) / (x_2 - x_1);
            a = - slope;
            b = 1;
            c = y_2 - slope * x_2;
        }
        private double EvaluateYGivenX(double a, double b, double e, double x)
        {
            // ax + by = e
            return (e - a * x) / b;
        }
        private double EvaluateXGivenY(double a, double b, double e, double y)
        {
            // ax + by = e
            return (e - b * y) / a;
        }
        public Point FindIntersection(Segment thatSegment)
        {
            double a, b, c, d, e, f;

            if (this.IsVertical() && thatSegment.IsHorizontal()) return new Point(null, this.Point1.X, thatSegment.Point1.Y);

            if (thatSegment.IsVertical() && this.IsHorizontal()) return new Point(null, thatSegment.Point1.X, this.Point1.Y);

            if (this.IsVertical())
            {
                MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out a, out b, out e);
                return new Point(null, this.Point1.X, EvaluateYGivenX(a, b, e, this.Point1.X));
            }
            if (thatSegment.IsVertical())
            {
                MakeLine(this.Point1.X, this.Point1.Y, this.Point2.X, this.Point2.Y, out a, out b, out e);
                return new Point(null, thatSegment.Point1.X, EvaluateYGivenX(a, b, e, thatSegment.Point1.X));
            }
            if (this.IsHorizontal())
            {
                MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out a, out b, out e);
                return new Point(null, EvaluateXGivenY(a, b, e, this.Point1.Y), this.Point1.Y);
            }
            if (thatSegment.IsHorizontal())
            {
                MakeLine(this.Point1.X, this.Point1.Y, this.Point2.X, this.Point2.Y, out a, out b, out e);
                return new Point(null, EvaluateXGivenY(a, b, e, thatSegment.Point1.Y), thatSegment.Point1.Y);
            }

            //
            // ax + by = e
            // cx + dy = f
            // 

            MakeLine(Point1.X, Point1.Y, Point2.X, Point2.Y, out a, out b, out e);
            MakeLine(thatSegment.Point1.X, thatSegment.Point1.Y, thatSegment.Point2.X, thatSegment.Point2.Y, out c, out d, out f);

            double overallDeterminant = a * d - b * c;
            double x = determinant(e, b, f, d) / overallDeterminant;
            double y = determinant(a, e, c, f) / overallDeterminant;

            return new Point("Intersection", x, y);
        }

        private class Vector
        {
            private double originX;
            private double originY;
            private double otherX;
            private double otherY;

            public Vector(Point origin, Point other)
            {
                originX = origin.X;
                originY = origin.Y;
                otherX = other.X;
                otherY = other.Y;
            }

            public Vector(double x1, double y1, double x2, double y2)
            {
                originX = x1;
                originY = y1;
                otherX = x2;
                otherY = y2;
            }

            public Point NonOriginEndpoint() { return new Point("ProjectedEndpoint", otherX, otherY); }

            private double DotProduct() { return originX * otherX + originY * otherY; }
            private static double EuclideanDistance(double x1, double y1, double x2, double y2)
            {
                return System.Math.Sqrt(System.Math.Pow(x1 - x2, 2) + System.Math.Pow(y1 - y2, 2));
            }

            //
            // Projects the given vector onto this vector using standard vector projection
            //
            public Vector Projection(Vector thatVector)
            {
                double magnitude = EuclideanDistance(thatVector.originX, thatVector.originY, thatVector.otherX, thatVector.otherY);
                double cosIncluded = CosineOfIncludedAngle(thatVector);

                double projectionDistance = magnitude * cosIncluded;

                return new Vector(originX, originY, otherX / projectionDistance, otherY / projectionDistance);
            }

            //
            // Use Law of Cosines to determine cos(\theta)
            //      ^
            //      / \
            //   a /   \ c
            //    /\    \
            //   /__\____\__>
            //       b 
            //
            private double CosineOfIncludedAngle(Vector thatVector)
            {
                if (HasSameOriginPoint(thatVector)) return -2;

                double a = EuclideanDistance(originX, originY, otherX, otherY);
                double b = EuclideanDistance(originX, originY, thatVector.otherX, thatVector.otherY);
                double c = EuclideanDistance(otherX, otherY, thatVector.otherX, thatVector.otherY);

                // Law of Cosines
                return (Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b);
            }

            private bool HasSameOriginPoint(Vector thatVector)
            {
                return Utilities.CompareValues(originX, thatVector.originX) && Utilities.CompareValues(originY, thatVector.originY);
            }

            public override string ToString()
            {
                return "(" + originX + ", " + originY + ") -> (" + otherX + ", " + otherY + ")";
            }
        }
    }
}