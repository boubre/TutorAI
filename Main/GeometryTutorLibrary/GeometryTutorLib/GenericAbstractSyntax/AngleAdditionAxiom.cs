using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class AngleAdditionAxiom : Axiom
    {
        private readonly static string NAME = "Angle Addition Axiom";

        public AngleAdditionAxiom() { }

        public static bool MayUnifyWith(GroundedClause c)
        {
            return c is ConcreteAngle;
        }

        private static List<ConcreteAngle> unifyCandAngles = new List<ConcreteAngle>();
        //
        // Angle(A, B, C), Angle(C, B, D) -> Angle(A, B, C) + Angle(C, B, D) = Angle(A, B, D)
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            if (!MayUnifyWith(c)) return new List<GroundedClause>();

            ConcreteAngle newAngle = (ConcreteAngle)c;
            List<GroundedClause> newGrounded = new List<GroundedClause>();

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

                    newGrounded.Add(eq);

                    // For hypergraph construction
                    List<GroundedClause> preds = new List<GroundedClause>();
                    preds.Add(newAngle);
                    preds.Add(ang);
                    eq.AddPredecessor(preds);
                    ang.AddSuccessor(eq);
                    newAngle.AddSuccessor(eq);
                }
            }
            // Add this angle to the unifying candidates
            unifyCandAngles.Add(newAngle);

            return newGrounded;
        }
    }
}