using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricSegmentEquation : SegmentEquation
    {
        public GeometricSegmentEquation() : base() { }

        public GeometricSegmentEquation(GroundedClause l, GroundedClause r) : base(l, r) { }
//        public GeometricSegmentEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just) { }

        public override GroundedClause DeepCopy()
        {
            return new GeometricSegmentEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "GeometricEquation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}