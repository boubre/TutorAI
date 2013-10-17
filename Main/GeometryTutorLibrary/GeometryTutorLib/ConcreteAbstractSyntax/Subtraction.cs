using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class Subtraction : ArithmeticOperation
    {
        public Subtraction() { }

        public Subtraction(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause Flatten()
        {
            leftExp = leftExp.Flatten();

            rightExp = new Multiplication(new NumericValue(-1), rightExp);

            return this;
        }
        public override string ToString()
        {
            return "(" + leftExp.ToString() + " - " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            Subtraction s = obj as Subtraction;
            if (s == null) return false;
            return base.Equals(obj);
        }
    }
}