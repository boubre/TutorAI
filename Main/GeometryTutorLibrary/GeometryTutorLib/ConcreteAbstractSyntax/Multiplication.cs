using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class Multiplication : ArithmeticOperation
    {
        public Multiplication() { }

        public Multiplication(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " * " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            Multiplication m = obj as Multiplication;
            if (m == null) return false;
            return base.Equals(obj);
        }
    }
}