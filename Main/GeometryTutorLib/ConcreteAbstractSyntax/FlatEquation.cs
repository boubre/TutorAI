using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class FlatEquation : Equation
    {
        public List<GroundedClause> lhsExps { get; private set; }
        public List<GroundedClause> rhsExps {get; private set; }

        public FlatEquation() : base() { }

        public FlatEquation(List<GroundedClause> l, List<GroundedClause> r) : base()
        {
            lhsExps = l;
            rhsExps = r;
        }
//        public FlatEquation(List<GroundedClause> l, List<GroundedClause> r, string just) : base(l, r, just) { }

        public override GroundedClause DeepCopy()
        {
            return new SegmentEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

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

        public override string ToString()
        {
            string retS = "";
            foreach (GroundedClause lc in lhsExps)
            {
                retS += lc.multiplier + " * " + lc.ToString() + " + "; 
            }
            retS = retS.Substring(0, retS.Length - 3) + " = ";
            foreach (GroundedClause rc in rhsExps)
            {
                retS += rc.multiplier + " * " + rc.ToString() + " + ";
            }

            return retS.Substring(0, retS.Length - 3);
        }

    }
}