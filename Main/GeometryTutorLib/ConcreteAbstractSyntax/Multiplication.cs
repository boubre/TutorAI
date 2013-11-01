using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class Multiplication : ArithmeticOperation
    {
        public Multiplication() : base() { }

        public Multiplication(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " * " + rightExp.ToString() + ")";
        }

        public override List<GroundedClause> CollectTerms()
        {
            List<GroundedClause> list = new List<GroundedClause>();

            if (leftExp is NumericValue && rightExp is NumericValue)
            {
                list.AddRange(leftExp.CollectTerms());
                list.AddRange(rightExp.CollectTerms());
                return list;
            }
            
            if (leftExp is NumericValue)
            {
                foreach (GroundedClause gc in rightExp.CollectTerms())
                {
                    gc.multiplier *= ((NumericValue)leftExp).value;
                    list.Add(gc);
                }
            }

            if (rightExp is NumericValue)
            {
                foreach (GroundedClause gc in leftExp.CollectTerms())
                {
                    gc.multiplier *= ((NumericValue)rightExp).value;
                    list.Add(gc);
                }
            }

            return list;
        }

        public override bool Equals(Object obj)
        {
            Multiplication m = obj as Multiplication;
            if (m == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}