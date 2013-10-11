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
        public double angle { get; private set; }

        /// <summary>
        /// Create a new angle.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        public ConcreteAngle(ConcretePoint a, ConcretePoint b, ConcretePoint c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.angle = toDegrees(findAngle(a, b, c));
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

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteAngle [angle=");
            sb.Append(angle);
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

        public override bool Equals(GroundedClause obj)
        {
            ConcreteAngle angle = obj as ConcreteAngle;
            if (angle == null) return false;
            return angle.A.Equals(A) && angle.B.Equals(B) && angle.C.Equals(C);
        }
    }
}
