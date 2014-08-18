using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public partial class RightAngle : Angle
    {
        public RightAngle(Point a, Point b, Point c) : base(a, b, c) { }
        public RightAngle(Angle angle) : base(angle.A, angle.B, angle.C) { }

        // CTA: Be careful with equality; this is object-based equality
        // If we check for angle measure equality that is distinct.
        // If we check to see that a different set of remote vertices describes this angle, that is distinct.
        public override bool Equals(Object obj)
        {
            RightAngle angle = obj as RightAngle;
            if (angle == null) return false;

            // Measures must be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            return base.Equals(obj) && StructurallyEquals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "RightAngle( m" + A.name + B.name + C.name + " = " + measure + ") " + justification;
        }
    }
}
