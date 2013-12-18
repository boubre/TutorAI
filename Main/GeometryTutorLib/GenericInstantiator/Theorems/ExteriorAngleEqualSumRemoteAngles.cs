using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ExteriorAngleEqualSumRemoteAngles : Theorem
    {
        private readonly static string NAME = "Exterior Angle is Equal to the Sum of Remote Interior Angles";

        private static List<Triangle> unifyCandTris = new List<Triangle>();
        private static List<Angle> unifyCandAngles = new List<Angle>();

        //
        // Triangle(A, B, C), Angle(D, A, B) -> Equation(m\angle DAB = m\angle ABC + m\angle BCA)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Triangle) && !(c is Angle)) return newGrounded;

            if (c is Triangle)
            {
                Triangle tri = c as Triangle;

                // Given the new triangle, are any of the old angles exterior angles of this triangle?
                foreach (Angle extAngle in unifyCandAngles)
                {
                    if (tri.HasExteriorAngle(extAngle))
                    {
                        newGrounded.Add(ConstructExteriorRelationship(tri, extAngle));
                    }
                }

                // Add to the list of candidate triangles
                unifyCandTris.Add(tri);
            }
            else if (c is Angle)
            {
                Angle extAngle = c as Angle;

                // Given the new angle, do any of the old triangles have this exterior angle?
                foreach (Triangle tri in unifyCandTris)
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

        private static KeyValuePair<List<GroundedClause>, GroundedClause> ConstructExteriorRelationship(Triangle tri, Angle extAngle)
        {
            //
            // Acquire the remote angles
            //
            Angle remote1 = null;
            Angle remote2 = null;

            tri.AcquireRemoteAngles(extAngle.GetVertex(), out remote1, out remote2);

            //
            // Construct the new equation
            //
            Addition sum = new Addition(remote1, remote2);
            GeometricAngleEquation eq = new GeometricAngleEquation(extAngle, sum, NAME);

            //
            // For the hypergraph
            //
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
            antecedent.Add(extAngle);

            return new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq);
        }
    }
}