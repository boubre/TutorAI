using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class PerpendicularBisector : Perpendicular
    {
        public PerpendicularBisector(Point i, Segment l, Segment r, string just) : base(i, l, r, just) { }
        public PerpendicularBisector(Intersection inter, String just) : base(inter.intersect, inter.lhs, inter.rhs, just) { }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            PerpendicularBisector p = obj as PerpendicularBisector;
            if (p == null) return false;
            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            PerpendicularBisector p = obj as PerpendicularBisector;
            if (p == null) return false;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "PerpendicularBisector(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }
    }
}
