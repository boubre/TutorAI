using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ArithmeticOperation : ArithmeticNode
    {
        protected GroundedClause leftExp;
        protected GroundedClause rightExp;

        public ArithmeticOperation() { }

        public ArithmeticOperation(GroundedClause l, GroundedClause r)
        {
            leftExp = l;
            rightExp = r;
        }

        public override bool Contains(GroundedClause newG)
        {
            return leftExp.Contains(newG) || rightExp.Contains(newG);
        }

        public override void Substitute(GroundedClause toFind, GroundedClause toSub)
        {
            if (leftExp.Equals(toFind))
            {
                leftExp = toSub;
            }
            if (rightExp.Equals(toFind))
            {
                rightExp = toSub;
            }
        }

        public override GroundedClause Flatten()
        {
            leftExp = leftExp.Flatten();
            rightExp = rightExp.Flatten();

            return this;
        }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " + " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            ArithmeticOperation ao = obj as ArithmeticOperation;
            if (ao == null) return false;
            return leftExp.Equals(ao.leftExp) && rightExp.Equals(ao.rightExp) ||
                   leftExp.Equals(ao.rightExp) && rightExp.Equals(ao.leftExp);
        }
    }
}