using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class Subtraction : ArithmeticOperation
    {
        public Subtraction() : base() { }

        public Subtraction(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause Flatten()
        {
            leftExp = leftExp.Flatten();

            rightExp = new Multiplication(new NumericValue(-1), rightExp);

            return this;
        }

        public override List<GroundedClause> CollectTerms()
        {
            List<GroundedClause> list = new List<GroundedClause>();

            list.AddRange(leftExp.CollectTerms());

            foreach (GroundedClause gc in rightExp.CollectTerms())
            {
                gc.multiplier *= -1;
                list.Add(gc);
            }

            return list;
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

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}