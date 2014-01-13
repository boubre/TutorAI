using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class RightAngle : Angle
    {
        public RightAngle(Point a, Point b, Point c, string just) : base(a, b, c) { justification = just; }
        public RightAngle(Angle angle, string just) : base(angle.A, angle.B, angle.C) { justification = just; }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            RightAngle ra = null;
            if (clause is Strengthened)
            {
                ra = ((clause as Strengthened).strengthened) as RightAngle;
            }
            else if (clause is RightAngle)
            {
                ra = clause as RightAngle;
            }
            else return newGrounded;

            // Strengthening may be something else
            if (ra == null) return newGrounded;

            GeometricAngleEquation angEq = new GeometricAngleEquation(ra, new NumericValue(90), "Definition of Right Angle");
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(clause);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, angEq));

            return newGrounded;
        }

        // CTA: Be careful with equality; this is object-based equality
        // If we check for angle measure equality that is distinct.
        // If we check to see that a different set of remote vertices describes this angle, that is distinct.
        public override bool Equals(Object obj)
        {
            RightAngle angle = obj as RightAngle;
            if (angle == null) return false;

            // Measures must be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            return base.Equals(obj) && StructurallyEquals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "RightAngle( m" + A.name + B.name + C.name + " = " + measure + ") " + justification;
        }
    }
}
