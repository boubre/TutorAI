using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class SegmentEquation : Equation
    {
        public SegmentEquation() : base() { }

        public SegmentEquation(GroundedClause l, GroundedClause r) : base(l, r) { }
        public SegmentEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just) { }

        public override GroundedClause DeepCopy() { return new SegmentEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy()); }

        public override bool Equals(Object obj)
        {
            SegmentEquation eq = obj as SegmentEquation;
            if (eq == null) return false;
            return (lhs.Equals(eq.lhs) && rhs.Equals(eq.rhs)) || (lhs.Equals(eq.rhs) && rhs.Equals(eq.lhs));
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}