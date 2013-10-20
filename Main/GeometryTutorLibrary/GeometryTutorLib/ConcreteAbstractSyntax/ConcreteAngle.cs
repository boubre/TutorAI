using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class ConcreteAngle : GroundedClause
    {
        public ConcretePoint A { get; private set; }
        public ConcretePoint B { get; private set; }
        public ConcretePoint C { get; private set; }
        public ConcreteSegment ray1 { get; private set; }
        public ConcreteSegment ray2 { get; private set; }
        public double measure { get; private set; }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            ConcreteAngle other = (ConcreteAngle)(this.MemberwiseClone());
            other.A = (ConcretePoint)this.A.DeepCopy();
            other.B = (ConcretePoint)this.B.DeepCopy();
            other.C = (ConcretePoint)this.C.DeepCopy();
            other.ray1 = (ConcreteSegment)this.ray1.DeepCopy();
            other.ray2 = (ConcreteSegment)this.ray2.DeepCopy();

            return other;
        }

        /// <summary>
        /// Create a new angle.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        public ConcreteAngle(ConcretePoint a, ConcretePoint b, ConcretePoint c) : base()
        {
            this.A = a;
            this.B = b;
            this.C = c;
            ray1 = new ConcreteSegment(a, b);
            ray2 = new ConcreteSegment(b, c);
            this.measure = toDegrees(findAngle(A, B, C));
        }

        public ConcreteAngle(List<ConcretePoint> pts) : base()
        {
            if (pts.Count != 3)
            {
                //Console.WriteLine("ERROR: Construction of an angle with " + pts.Count + ", not 3.");
                return;
            }

            this.A = pts.ElementAt(0);
            this.B = pts.ElementAt(1);
            this.C = pts.ElementAt(2);
            this.measure = toDegrees(findAngle(A, B, C));
        }

        /// <summary>
        /// Find the measure of the angle (in radians) specified by the three points.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        /// <returns>The measure of the angle (in radians) specified by the three points.</returns>
        public static double findAngle(ConcretePoint a, ConcretePoint b, ConcretePoint c)
        {
            double v1x = a.X - b.X;
            double v1y = a.Y - b.Y;
            double v2x = c.X - b.X;
            double v2y = c.Y - b.Y;
            double dotProd = v1x * v2x + v1y * v2y;
            double cosAngle = dotProd / (ConcretePoint.calcDistance(a, b) * ConcretePoint.calcDistance(b, c));
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

        public ConcretePoint GetVertex()
        {
            return B;
        }

        public ConcretePoint SameVertex(ConcreteAngle ang)
        {
            return GetVertex().Equals(ang.GetVertex()) ? GetVertex() : null;
        }

        //
        // Looks for a single shared ray
        //
        public ConcreteSegment SharedRay(ConcreteAngle ang)
        {
            if (ray1.Equals(ang.ray1) || ray1.Equals(ang.ray2)) return ray1;

            if (ray2.Equals(ang.ray1) || ray2.Equals(ang.ray2)) return ray2;

            return null;
        }

        public ConcreteSegment SharesOneRayAndHasSameVertex(ConcreteAngle ang)
        {
            if (SameVertex(ang) == null) return null;

            return SharedRay(ang);
        }

        public ConcretePoint OtherPoint(ConcreteSegment seg)
        {
            if (seg.HasPoint(A) && seg.HasPoint(B)) return C;
            if (seg.HasPoint(A) && seg.HasPoint(C)) return B;
            if (seg.HasPoint(B) && seg.HasPoint(C)) return A;

            return null;
        }

        public bool IsIncludedAngle(ConcreteSegment seg1, ConcreteSegment seg2)
        {
            return seg1.Equals(ray1) && seg2.Equals(ray2) || seg1.Equals(ray2) && seg2.Equals(ray1);
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

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause pred, GroundedClause c)
        {
            ConcreteAngle angle = c as ConcreteAngle;
            if (angle == null) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            if (IsSpecialAngle(angle.measure))
            {
                AngleMeasureEquation angEq = new AngleMeasureEquation(angle, new NumericValue((int)angle.measure), "Given:tbd");
                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(pred);
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, angEq));
                GroundedClause.ConstructClauseLinks(antecedent, angEq);
            }

            return newClauses;
        }

        // CTA: Be careful with equality; this is object-based equality
        // If we check for angle measure equality that is distinct.
        // If we check to see that a different set of remote vertices describes this angle, that is distinct.
        public override bool Equals(Object obj)
        {
            ConcreteAngle angle = obj as ConcreteAngle;
            if (angle == null) return false;
            return angle.A.Equals(A) && angle.B.Equals(B) && angle.C.Equals(C) || angle.A.Equals(C) && angle.B.Equals(B) && angle.C.Equals(A);
        }

        public override string ToString()
        {
            return "Angle( m" + A.name + B.name + C.name + " = " + measure + ")";
        }
    }
}
