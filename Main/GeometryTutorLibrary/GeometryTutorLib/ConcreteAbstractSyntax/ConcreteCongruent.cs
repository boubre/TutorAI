using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public abstract class ConcreteCongruent : Descriptor
    {
        public ConcreteCongruent() : base() { }

        //
        // Given an equation, if both sides are atomic, then they are equal; generate the appropriate congruence clause
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause gc)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Equation eq = gc as Equation;

            if (eq == null) return newGrounded;

            // Both sides are single, atomic clauses
            if (eq.GetAtomicity() != Equation.BOTH_ATOMIC) return newGrounded;

            // Make sure the multiplier (constant coefficient) of each term is 1 for a valid congruence
            if (!(eq.lhs.multiplier == 1 && eq.rhs.multiplier == 1)) return newGrounded;

            //
            // Atomic expressions on both sides need to be segments or angles, respectively
            //
            ConcreteCongruent cc = null;
            if (eq is SegmentEquation && eq.lhs is ConcreteSegment && eq.rhs is ConcreteSegment)
            {
                cc = new ConcreteCongruentSegments((ConcreteSegment)eq.lhs, (ConcreteSegment)eq.rhs, "Segments of Equal Length are Congruent");
            }
            else if (eq is AngleMeasureEquation && eq.lhs is ConcreteAngle && eq.rhs is ConcreteAngle)
            {
                cc = new ConcreteCongruentAngles((ConcreteAngle)eq.lhs, (ConcreteAngle)eq.rhs, "Angles of Equal Measure are Congruent");
            }
            else
            {
                return newGrounded;
            }

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(eq);
            GroundedClause.ConstructClauseLinks(antecedent, cc);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cc));

            return newGrounded;
        }
    }
}
