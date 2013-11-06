using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ExteriorAngleEqualSumRemoteAngles : Theorem
    {
        private readonly static string NAME = "Exterior Angle is Equal to the Sum of Remote Interior Angles";

        private static List<ConcreteTriangle> unifyCandTris = new List<ConcreteTriangle>();
        private static List<ConcreteAngle> unifyCandAngles = new List<ConcreteAngle>();
        //        private static List<ConcreteCongruentSegments> unifyCandSegments = new List<ConcreteCongruentSegments>();

        //
        // Triangle(A, B, C), Angle(D, A, B) -> Equation(m\angle DAB = m\angle ABC + m\angle BCA)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is ConcreteTriangle) && !(c is ConcreteAngle)) return newGrounded;

            if (c is ConcreteTriangle)
            {
                ConcreteTriangle tri = c as ConcreteTriangle;

                // Given the new triangle, are any of the old angles exterior angles of this triangle?
                foreach (ConcreteAngle extAngle in unifyCandAngles)
                {
                    if (tri.HasExteriorAngle(extAngle))
                    {
                        newGrounded.Add(ConstructExteriorRelationship(tri, extAngle));
                    }
                }

                // Add to the list of candidate triangles
                unifyCandTris.Add(tri);
            }
            else if (c is ConcreteAngle)
            {
                ConcreteAngle extAngle = c as ConcreteAngle;

                // Given the new angle, do any of the old triangles have this exterior angle?
                foreach (ConcreteTriangle tri in unifyCandTris)
                {
                    if (tri.HasExteriorAngle(extAngle))
                    {
                        newGrounded.Add(ConstructExteriorRelationship(tri, extAngle));
                    }
                }

                // Add to the list of candidate triangles
                unifyCandAngles.Add(extAngle);
            }

            return newGrounded;
        }

        private static KeyValuePair<List<GroundedClause>, GroundedClause> ConstructExteriorRelationship(ConcreteTriangle tri, ConcreteAngle extAngle)
        {
            //
            // Acquire the remote angles
            //
            ConcreteAngle remote1 = null;
            ConcreteAngle remote2 = null;

            tri.AcquireRemoteAngles(extAngle.GetVertex(), out remote1, out remote2);

            //
            // Construct the new equation
            //
            Addition sum = new Addition(remote1, remote2);
            AngleMeasureEquation eq = new AngleMeasureEquation(extAngle, sum, NAME);

            //
            // For the hypergraph
            //
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
            antecedent.Add(extAngle);

            return new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq);
        }
    }
}