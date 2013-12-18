using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AcuteAnglesInRightTriangleComplementary : Theorem
    {
        private readonly static string NAME = "The two acute angles in a right triangle are complementary.";

        //
        // In order for two triangles to be congruent, we require the following:
        //    RightTriangle(A, B, C) -> Complementary(Angle(B, A, C), Angle(A, C, B))
        //     A 
        //    | \
        //    |  \
        //    |   \
        //    |    \
        //    |_____\
        //    B      C
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Triangle) && !(c is Strengthened)) return newGrounded;

            if (c is Triangle)
            {
                Triangle tri = c as Triangle;

                if (!(tri is RightTriangle))
                {
                    if (!tri.provenRight) return newGrounded;
                }
                newGrounded.AddRange(InstantiateRightTriangle(tri, c));
            }

            if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is RightTriangle)) return newGrounded;

                newGrounded.AddRange(InstantiateRightTriangle(streng.strengthened as RightTriangle, c));
            }

            return newGrounded;
        }

        //
        // We know at this point that we have a right triangle
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateRightTriangle(Triangle tri, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            KeyValuePair<Angle, Angle> acuteAngles = tri.GetAcuteAngles();

            Complementary newComp = new Complementary(acuteAngles.Key, acuteAngles.Value, NAME);

            // Hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newComp));

            return newGrounded;
        }
    }
}