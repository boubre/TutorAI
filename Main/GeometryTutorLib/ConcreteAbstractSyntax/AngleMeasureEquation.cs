using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class AngleMeasureEquation : Equation
    {
        public AngleMeasureEquation() : base() { }

        public AngleMeasureEquation(GroundedClause l, GroundedClause r) : base(l, r) { }
        public AngleMeasureEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just) {}

        public override GroundedClause DeepCopy() { return new AngleMeasureEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy()); }

        public override bool Equals(Object obj)
        {
            AngleMeasureEquation eq = obj as AngleMeasureEquation;
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