using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Perpendicular : Intersection
    {
        public Perpendicular(Point i, Segment l, Segment r, string just) : base(i, l, r, just) { }
        public Perpendicular(Intersection inter, String just) : base(inter.intersect, inter.lhs, inter.rhs, just) { }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Perpendicular perp = obj as Perpendicular;
            if (perp == null) return false;
            return intersect.Equals(perp.intersect) && ((lhs.StructurallyEquals(perp.lhs) && rhs.StructurallyEquals(perp.rhs)) ||
                                                        (lhs.StructurallyEquals(perp.rhs) && rhs.StructurallyEquals(perp.lhs)));
        }

        public override bool Equals(Object obj)
        {
            if (obj is PerpendicularBisector) return (obj as PerpendicularBisector).Equals(this);

            Perpendicular p = obj as Perpendicular;
            if (p == null) return false;

            return intersect.Equals(p.intersect) && lhs.Equals(p.lhs) && rhs.Equals(p.rhs);
        }

        public override string ToString()
        {
            return "Perpendicular(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }

        // 
        // Equation(m\angle ABC = 90) -> Perpendicular(B, Segment(A, B), Segment(B, C))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is AngleEquation)) return newGrounded;

            AngleEquation angEq = c as AngleEquation;

            // We need a basic angle equation containing the constant 90
            if (angEq.GetAtomicity() != Equation.BOTH_ATOMIC) return newGrounded;
            if (!angEq.Contains(new NumericValue(90))) return newGrounded;

            Angle angle = (Angle)(angEq.lhs is NumericValue ? angEq.rhs : angEq.lhs);

            // The multiplier must be one for a perpendicular situation
            if (angle.multiplier != 1) return newGrounded;

            Perpendicular rightPerp = new Perpendicular(angle.GetVertex(), angle.ray1, angle.ray2, "Definition of Perpendicular");

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(angEq);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, rightPerp));

            return newGrounded;
        }

    }
}
