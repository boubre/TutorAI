using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleAdditionAxiom : Axiom
    {
        private readonly static string NAME = "Angle Addition Axiom";

        // Candidate angles
        private static List<ConcreteAngle> unifyCandAngles = new List<ConcreteAngle>();

        //
        // Angle(A, B, C), Angle(C, B, D) -> Angle(A, B, C) + Angle(C, B, D) = Angle(A, B, D)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is ConcreteAngle)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            ConcreteAngle newAngle = (ConcreteAngle)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!unifyCandAngles.Any())
            {
                unifyCandAngles.Add(newAngle);
                return newGrounded;
            }

            //
            // Determine if another angle in the candidate unify list can be combined with this new angle
            //
            foreach (ConcreteAngle ang in unifyCandAngles)
            {
                ConcreteSegment shared = newAngle.SharesOneRayAndHasSameVertex(ang);
                if (shared != null)
                {
                    ConcretePoint vertex = ang.GetVertex();
                    ConcretePoint exteriorPt1 = ang.OtherPoint(shared);
                    ConcretePoint exteriorPt2 = newAngle.OtherPoint(shared);
                    ConcreteAngle angle = new ConcreteAngle(exteriorPt1, vertex, exteriorPt2);
                    Addition sum = new Addition(newAngle, ang);
                    AngleMeasureEquation eq = new AngleMeasureEquation(sum, angle, NAME);

                    // For hypergraph construction
                    List<GroundedClause> antecedent = new List<GroundedClause>();
                    antecedent.Add(newAngle);
                    antecedent.Add(ang);

                    newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq));
                }
            }

            // Add this angle to the unifying candidates
            unifyCandAngles.Add(newAngle);

            return newGrounded;
        }
    }
}