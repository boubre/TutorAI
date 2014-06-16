using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A 2D imaginary point.
    /// </summary>
    public class ImaginaryPoint : Point
    {
        /// <summary>
        /// A point that is the result of two constructed segments.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public ImaginaryPoint(double x, double y) : base("", x, y) { }

        public ImaginaryPoint(Point pt) : base("", pt.X, pt.Y) { }

        public override bool StructurallyEquals(Object obj)
        {
            ImaginaryPoint pt = obj as ImaginaryPoint;
            if (pt == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            ImaginaryPoint pt = obj as ImaginaryPoint;
            if (pt == null) return false;

            return base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Make a deep copy of this object; this is actually shallow, but is all that is required.
        public override GroundedClause DeepCopy() { return (ImaginaryPoint)(this.MemberwiseClone()); }

        public override string ToString() { return "(" + string.Format("{0:N3}", X) + ", " + string.Format("{0:N3}", Y) + ")"; }
    }
}
