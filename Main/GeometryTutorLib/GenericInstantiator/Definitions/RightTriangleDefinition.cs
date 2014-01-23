using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RightTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Right Triangle";

        private RightTriangleDefinition() { }
        private static readonly RightTriangleDefinition thisDescriptor = new RightTriangleDefinition();

        private static List<RightAngle> candidateRightAngles = new List<RightAngle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Triangle> candidateTriangles = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateStrengthened.Clear();
            candidateRightAngles.Clear();
            candidateTriangles.Clear();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            //
            // Instantiating FROM a right triangle
            //
            Strengthened streng = clause as Strengthened;
            if (clause is RightTriangle) return InstantiateFromRightTriangle(clause as RightTriangle, clause);
            if (streng != null && streng.strengthened is RightTriangle) return InstantiateFromRightTriangle(streng.strengthened as RightTriangle, clause);

            //
            // Instantiating TO a right triangle
            //
            if (clause is RightAngle || clause is Strengthened || clause is Triangle)
            {
                return InstantiateToRightTriangle(clause);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromRightTriangle(RightTriangle rightTri, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            
            // Strengthen the old triangle to a right triangle
            Strengthened newStrengthened = new Strengthened(rightTri.rightAngle, new RightAngle(rightTri.rightAngle, NAME), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newStrengthened));

            return newGrounded;
        }

        //   A
        //   |\
        //   | \
        //   |  \
        //   |   \
        //   |_   \
        //   |_|___\
        //   B      C
        //
        // Triangle(A, B, C), RightAngle(A, B, C) -> RightTriangle(A, B, C)
        //
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToRightTriangle(GroundedClause clause)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Triangle)
            {
                Triangle newTri = clause as Triangle;

                foreach (RightAngle ra in candidateRightAngles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(newTri, ra, ra));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(newTri, streng.strengthened as RightAngle, streng));
                }

                candidateTriangles.Add(clause as Triangle);
            }
            else if (clause is RightAngle)
            {
                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(tri, clause as RightAngle, clause));
                }

                candidateRightAngles.Add(clause as RightAngle);
            }
            if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is RightAngle)) return newGrounded;

                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(tri, streng.strengthened as RightAngle, clause));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // DO NOT generate a new clause, instead, report the result and generate all applicable
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> StrengthenToRightTriangle(Triangle tri, RightAngle ra, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // This angle must belong to this triangle.
            if (!tri.HasAngle(ra)) return newGrounded;
            
            // Strengthen the old triangle to a right triangle
            Strengthened newStrengthened = new Strengthened(tri, new RightTriangle(tri, "Strengthened"), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newStrengthened));

            return newGrounded;
        }
    }
}