﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class Angle : Figure
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public Point C { get; private set; }
        public Segment ray1 { get; private set; }
        public Segment ray2 { get; private set; }
        public double measure { get; private set; }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            Angle other = (Angle)(this.MemberwiseClone());
            other.A = (Point)this.A.DeepCopy();
            other.B = (Point)this.B.DeepCopy();
            other.C = (Point)this.C.DeepCopy();
            other.ray1 = (Segment)this.ray1.DeepCopy();
            other.ray2 = (Segment)this.ray2.DeepCopy();

            return other;
        }

        /// <summary>
        /// Create a new angle.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        public Angle(Point a, Point b, Point c) : base()
        {
            this.A = a;
            this.B = b;
            this.C = c;
            ray1 = new Segment(a, b);
            ray2 = new Segment(b, c);
            this.measure = toDegrees(findAngle(A, B, C));
        }

        public Angle(List<Point> pts) : base()
        {
            if (pts.Count != 3)
            {
                //Console.WriteLine("ERROR: Construction of an angle with " + pts.Count + ", not 3.");
                return;
            }

            this.A = pts.ElementAt(0);
            this.B = pts.ElementAt(1);
            this.C = pts.ElementAt(2);
            ray1 = new Segment(A, B);
            ray2 = new Segment(B, C);
            this.measure = toDegrees(findAngle(A, B, C));
        }

        /// <summary>
        /// Find the measure of the angle (in radians) specified by the three points.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        /// <returns>The measure of the angle (in radians) specified by the three points.</returns>
        public static double findAngle(Point a, Point b, Point c)
        {
            double v1x = a.X - b.X;
            double v1y = a.Y - b.Y;
            double v2x = c.X - b.X;
            double v2y = c.Y - b.Y;
            double dotProd = v1x * v2x + v1y * v2y;
            double cosAngle = dotProd / (Point.calcDistance(a, b) * Point.calcDistance(b, c));

            // Avoid minor calculation issues and retarget the given value to specific angles. 
            // 0 or 180 degrees
            if (Utilities.CompareValues(Math.Abs(cosAngle), 1))
            {
                cosAngle = cosAngle < 0 ? -1 : 1;
            }

            // 90 degrees
            if (Utilities.CompareValues(cosAngle, 0)) cosAngle = 0;

            return Math.Acos(cosAngle);
        }

        /// <summary>
        /// Converts radians into degrees.
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>An angle in degrees</returns>
        public static double toDegrees(double radians)
        {
            return radians * 180 / System.Math.PI;
        }

        /// <summary>
        /// Converts degrees into radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>An angle in radians</returns>
        public static double toRadians(double degrees)
        {
            return degrees * System.Math.PI / 180;
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteAngle [angle=");
            sb.Append(measure);
            sb.Append("deg");
            sb.AppendLine();
            A.BuildUnparse(sb, tabDepth + 1);
            B.BuildUnparse(sb, tabDepth + 1);
            C.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public Point GetVertex()
        {
            return B;
        }

        public Point SameVertex(Angle ang)
        {
            return GetVertex().Equals(ang.GetVertex()) ? GetVertex() : null;
        }

        //
        // Looks for a single shared ray
        //
        public Segment SharedRay(Angle ang)
        {
            //if (ray1.Equals(ang.ray1) || ray1.Equals(ang.ray2)) return ray1;

            //if (ray2.Equals(ang.ray1) || ray2.Equals(ang.ray2)) return ray2;

            if (ray1.RayOverlays(ang.ray1) || ray1.RayOverlays(ang.ray2)) return ray1;

            if (ray2.RayOverlays(ang.ray1) || ray2.RayOverlays(ang.ray2)) return ray2;

            return null;
        }

        public Segment SharesOneRayAndHasSameVertex(Angle ang)
        {
            if (SameVertex(ang) == null) return null;

            return SharedRay(ang);
        }


        // Return the shared angle in both congruences
        public Segment IsAdjacentTo(Angle thatAngle)
        {
            if (thatAngle.IsOnInterior(this)) return null;
            if (this.IsOnInterior(thatAngle)) return null;

            //Segment shared =  SharesOneRayAndHasSameVertex(thatAngle);
            //if (shared == null) return null;

            //// Is this a scenario where one angle encompasses completely the other angle?
            //Segment otherThat = thatAngle.OtherRayEquates(shared);
            //Angle tempAngle = new Angle(shared.OtherPoint(GetVertex()), GetVertex(), this.OtherRayEquates(shared).OtherPoint(GetVertex()));

            //if (tempAngle.IsOnInterior(otherThat.OtherPoint(GetVertex())) return null; 

            return SharesOneRayAndHasSameVertex(thatAngle);
        }

        //
        // Is this point in the interior of the angle?
        //
        public bool IsOnInterior(Point pt)
        {
            //     |
            //     |
            //  x  |_____
            // Is the point on either ray such that it is outside the angle? (x in the image above)
            if (ray1.PointIsOn(pt) && Segment.Between(pt, GetVertex(), ray1.OtherPoint(GetVertex()))) return true;
            if (ray2.PointIsOn(pt) && Segment.Between(pt, GetVertex(), ray2.OtherPoint(GetVertex()))) return true;

            Angle newAngle1 = new Angle(A, GetVertex(), pt);
            Angle newAngle2 = new Angle(C, GetVertex(), pt);

            // This is an angle addition scenario, BUT not with these two angles; that is, one is contained in the other.
            if (Utilities.CompareValues(newAngle1.measure + newAngle2.measure, this.measure)) return true;

            return newAngle1.measure + newAngle2.measure <= this.measure;
        }

        //
        // Is this point in the interior of the angle?
        //
        public bool IsOnInteriorExplicitly(Point pt)
        {
            if (ray1.PointIsOn(pt)) return false;
            if (ray2.PointIsOn(pt)) return false;

            Angle newAngle1 = new Angle(A, GetVertex(), pt);
            Angle newAngle2 = new Angle(C, GetVertex(), pt);

            // This is an angle addition scenario, BUT not with these two angles; that is, one is contained in the other.
            if (Utilities.CompareValues(newAngle1.measure + newAngle2.measure, this.measure)) return true;

            return newAngle1.measure + newAngle2.measure <= this.measure;
        }

        //
        // Is this angle on the interior of the other?
        //
        public bool IsOnInterior(Angle thatAngle)
        {
            if (this.measure < thatAngle.measure) return false;

            return this.IsOnInterior(thatAngle.A) && this.IsOnInterior(thatAngle.B) && this.IsOnInterior(thatAngle.C);
        }

        public Point OtherPoint(Segment seg)
        {
            if (seg.HasPoint(A) && seg.HasPoint(B)) return C;
            if (seg.HasPoint(A) && seg.HasPoint(C)) return B;
            if (seg.HasPoint(B) && seg.HasPoint(C)) return A;

            if (seg.PointIsOn(A) && seg.PointIsOn(B)) return C;
            if (seg.PointIsOn(A) && seg.PointIsOn(C)) return B;
            if (seg.PointIsOn(B) && seg.PointIsOn(C)) return A;

            return null;
        }

        //
        // Given one ray of the angle, return the other ray
        //
        public Segment OtherRay(Segment seg)
        {
            if (ray1.Equals(seg)) return ray2;
            if (ray2.Equals(seg)) return ray1;

            return null;
        }

        //
        // Given one ray of the angle, return the other ray
        //
        public Segment OtherRayEquates(Segment seg)
        {
            if (ray1.RayOverlays(seg)) return ray2;
            if (ray2.RayOverlays(seg)) return ray1;

            if (ray1.IsCollinearWith(seg)) return ray2;
            if (ray2.IsCollinearWith(seg)) return ray1;

            return null;
        }

        //
        // Do these segments overlay this angle?
        //
        public bool IsIncludedAngle(Segment seg1, Segment seg2)
        {
            // Check direct inclusion
            if (seg1.Equals(ray1) && seg2.Equals(ray2) || seg1.Equals(ray2) && seg2.Equals(ray1)) return true;

            // Check overlaying angle
            Point shared = seg1.SharedVertex(seg2);

            if (shared == null) return false;

            Angle thatAngle = new Angle(seg1.OtherPoint(shared), shared, seg2.OtherPoint(shared));

            return this.Equates(thatAngle);
        }

        private static readonly int[] VALID_CONCRETE_SPECIAL_ANGLES = { 30, 45 }; // 0 , 60, 90, 120, 135, 150, 180, 210, 225, 240, 270, 300, 315, 330 }; // 15, 22.5, ...


        private static bool IsSpecialAngle(double measure)
        {
            foreach (int d in VALID_CONCRETE_SPECIAL_ANGLES)
            {
                if (Utilities.GCD((int)measure, d) == d) return true;
            }

            return false;
        }

        public override bool Contains(GroundedClause target)
        {
            return this.Equals(target);
        }

        //
        // Is the given angle the same as this angle? that is, the vertex is the same and the rays coincide
        // (not necessarily with the same endpoints)
        // Can't just be collinear, must be collinear and on same side of an angle
        //
        public bool Equates(Angle thatAngle)
        {
            //if (this.Equals(thatAngle)) return true;

            // Vertices must equate
            if (!this.GetVertex().Equals(thatAngle.GetVertex())) return false;

            // Rays must originate at the vertex and emanate outward
            return (ray1.RayOverlays(thatAngle.ray1) && ray2.RayOverlays(thatAngle.ray2)) ||
                   (ray2.RayOverlays(thatAngle.ray1) && ray1.RayOverlays(thatAngle.ray2));
        }

        // Does this angle lie between the two lines? This is mainly for a parallelism check
        public bool OnInteriorOf(Intersection inter1, Intersection inter2)
        {
            Intersection angleBelongs = null;
            Intersection angleDoesNotBelong = null;

            // Determine the intersection to which the angle belongs
            if (inter1.InducesNonStraightAngle(this))
            {
                angleBelongs = inter1;
                angleDoesNotBelong = inter2;
            }
            else if (inter2.InducesNonStraightAngle(this))
            {
                angleBelongs = inter2;
                angleDoesNotBelong = inter1;
            }

            if (angleBelongs == null || angleDoesNotBelong == null) return false;

            // Make the transversal out of the points of intersection
            Segment transversal = new Segment(angleBelongs.intersect, angleDoesNotBelong.intersect);
            Segment angleRayOnTraversal = this.ray1.IsCollinearWith(transversal) ? ray1 : ray2;

            // Is the endpoint of the angle (on the transversal) between the two intersection points?
            // Or, is that same endpoint on the far end beyond the other line: the other intersection point lies between the other points
            return Segment.Between(angleRayOnTraversal.OtherPoint(this.GetVertex()), angleBelongs.intersect, angleDoesNotBelong.intersect) ||
                   Segment.Between(angleDoesNotBelong.intersect, angleBelongs.intersect, angleRayOnTraversal.OtherPoint(this.GetVertex())); 
        }

        //
        // Maintain a public repository of all angle objects in the figure
        //
        public static void Clear() { figureAngles.Clear(); }
        public static List<Angle> figureAngles = new List<Angle>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Angle) figureAngles.Add(clause as Angle);
        }
        public static Angle AcquireFigureAngle(Angle thatAngle)
        {
            foreach (Angle angle in figureAngles)
            {
                if (angle.Equates(thatAngle)) return angle;
            }
            return null;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause pred, GroundedClause c)
        {
            if (c is Angle) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //Angle angle = c as Angle;

            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            //if (IsSpecialAngle(angle.measure))
            //{
            //    GeometricAngleEquation angEq = new GeometricAngleEquation(angle, new NumericValue((int)angle.measure), "Given:tbd");
            //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(pred);
            //    newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, angEq));
            //}

            return newClauses;
        }

        //
        // Each angle is congruent to itself; only generate if both rays are shared
        //
        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<Angle> knownSharedAngles = new List<Angle>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateReflexiveAngles(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Triangle newTriangle = clause as Triangle;

            if (newTriangle == null) return newGrounded;

            //
            // Compare all angles in this new triangle to all the angles in the old triangles
            //
            foreach (Triangle oldTriangle in candidateTriangles)
            {
                if (newTriangle.HasAngle(oldTriangle.AngleA))
                {
                    KeyValuePair<List<GroundedClause>, GroundedClause> newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleA);
                    if (newClause.Key != null) newGrounded.Add(newClause);
                }

                if (newTriangle.HasAngle(oldTriangle.AngleB))
                {
                    KeyValuePair<List<GroundedClause>, GroundedClause> newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleB);
                    if (newClause.Key != null) newGrounded.Add(newClause);
                }

                if (newTriangle.HasAngle(oldTriangle.AngleC))
                {
                    KeyValuePair<List<GroundedClause>, GroundedClause> newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleC);
                    if (newClause.Key != null) newGrounded.Add(newClause);
                }
            }

            candidateTriangles.Add(newTriangle);

            return newGrounded;
        }

        //
        // Generate the actual angle congruence
        //
        public static KeyValuePair<List<GroundedClause>, GroundedClause> GenerateAngleCongruence(Triangle tri, Angle angle)
        {
            //
            // If we have already generated a reflexive congruence, avoid regenerating
            //
            foreach (Angle oldSharedAngle in knownSharedAngles)
            {
                if (oldSharedAngle.Equates(angle)) return new KeyValuePair<List<GroundedClause>, GroundedClause>(null, null);
            }

            // Generate
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(angle, angle, "Reflexive");

            // This is an 'obvious' notion so it should be intrinsic to any figure
            gcas.MakeIntrinsic();

            return new KeyValuePair<List<GroundedClause>, GroundedClause>(Utilities.MakeList<GroundedClause>(Angle.AcquireFigureAngle(angle)), gcas);
        }


        public bool IsComplementaryTo(Angle thatAngle)
        {
            return Utilities.CompareValues(this.measure + thatAngle.measure, 90);
        }

        public bool IsSupplementaryTo(Angle thatAngle)
        {
            return Utilities.CompareValues(this.measure + thatAngle.measure, 180);
        }

        public bool IsStraightAngle()
        {
            return ray1.IsCollinearWith(ray2);
        }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        // This is either a direct comparison of the angle based on vertices or 
        public override bool StructurallyEquals(object obj)
        {
            Angle angle = obj as Angle;
            if (angle == null) return false;

            // Measures better be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            if (Equates(angle)) return true;

            return (angle.A.StructurallyEquals(A) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(C)) ||
                   (angle.A.StructurallyEquals(C) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(A));
        }

        //
        // Is this angle congruent to the given angle in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Angle a)
        {
            return Utilities.CompareValues(a.measure, this.measure);
        }

        public bool CoordinateAngleBisector(Segment thatSegment)
        {
            if (!thatSegment.PointIsOnAndBetweenEndpoints(this.GetVertex())) return false;

            if (thatSegment.IsCollinearWith(this.ray1) || thatSegment.IsCollinearWith(this.ray2)) return false;

            Point interiorPoint = this.IsOnInteriorExplicitly(thatSegment.Point1) ? thatSegment.Point1 : thatSegment.Point2;

            Angle angle1 = new Angle(A, GetVertex(), interiorPoint);
            Angle angle2 = new Angle(C, GetVertex(), interiorPoint);

            return Utilities.CompareValues(angle1.measure, angle2.measure);
        }

        //
        // Is this angle proportional to the given segment in terms of the coordinatization from the UI?
        // We should not report proportional if the ratio between segments is 1
        //
        public KeyValuePair<int, int> CoordinateProportional(Angle a)
        {
            return Utilities.RationalRatio(a.measure, this.measure);
        }

        public bool HasPoint(Point p)
        {
            if (A.Equals(p)) return true;
            if (B.Equals(p)) return true;
            if (C.Equals(p)) return true;

            return false;
        }

        public bool HasSegment(Segment seg)
        {
            return ray1.RayOverlays(seg) || ray2.RayOverlays(seg);
        }

        // Is the given clause an intrinsic component of this angle?
        public override bool Covers(GroundedClause gc)
        {
            if (gc is Point) return this.HasPoint(gc as Point);
            else if (gc is Segment) return this.HasSegment(gc as Segment);
            else if (gc is Triangle) return (gc as Triangle).Covers(this);

            return false;
        }

        // CTA: Be careful with equality; this is object-based equality
        // If we check for angle measure equality that is distinct.
        // If we check to see that a different set of remote vertices describes this angle, that is distinct.
        public override bool Equals(Object obj)
        {
            Angle angle = obj as Angle;
            if (angle == null) return false;

            // Measures must be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            return base.Equals(obj) && StructurallyEquals(obj);
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            RightAngle ra = gc as RightAngle;

            if (ra == null) return false;

            return this.StructurallyEquals(ra);
        }

        public override string ToString()
        {
            return "Angle( m" + A.name + B.name + C.name + " = " + measure + ")";
        }
    }
}