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

        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Perpendicular> candPerpendicular = new List<Perpendicular>();
        private static List<Triangle> candidateTriangles = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateIntersection.Clear();
            candidateStrengthened.Clear();
            candPerpendicular.Clear();
            candidateTriangles.Clear();
        }

        //   A
        //   |\
        //   | \
        //   |  \
        //   |   \
        //   |    \
        //   |_____\
        //   B      C
        //
        // Triangle(A, B, C), Perpendicular(B, Segment(A, B), Segment(B, C)) -> RightTriangle(A, B, C)
        //
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Triangle) && !(c is Strengthened) && !(c is Perpendicular)) return newGrounded;


            if (c is Triangle)
            {
                if (c is RightTriangle || c is EquilateralTriangle) return newGrounded;

                Triangle tri = c as Triangle;

                foreach (Perpendicular perp in candPerpendicular)
                {
                    newGrounded.AddRange(StrengthenToRight(tri, perp, tri, perp));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(StrengthenToRight(tri, streng.strengthened as Perpendicular, tri, streng));
                }

                candidateTriangles.Add(tri);
            }
            else if (c is Perpendicular)
            {
                Perpendicular perp = c as Perpendicular;

                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRight(tri, perp, tri, perp));
                }

                candPerpendicular.Add(perp);
            }
            if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is Perpendicular)) return newGrounded;

                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRight(tri, streng.strengthened as Perpendicular, tri, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // DO NOT generate a new clause, instead, report the result and generate all applicable
        // clauses attributed to this strengthening of a triangle from scalene to Right
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> StrengthenToRight(Triangle tri, Perpendicular perp, GroundedClause originalTri, GroundedClause originalPerp)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // This perpendicular intersection must belong to the triangle.
            if (!tri.HasPoint(perp.intersect)) return newGrounded;

            // The intersection must define the angle in the triangle
            if (!perp.InducesNonStraightAngle(tri.GetAngleWithVertex(perp.intersect))) return newGrounded;
            
            // Strengthen the old triangle to a right triangle
            RightTriangle newRight = new RightTriangle(tri, "Strengthened");
            Strengthened newStrengthened = new Strengthened(tri, newRight, NAME);
            tri.SetProvenToBeRight();

            // Create a strengthening of the angle to a right angle
            Strengthened newRightAngle = new Strengthened(newRight.rightAngle, new RightAngle(newRight.rightAngle, "Strengthened"), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(originalTri);
            antecedent.Add(originalPerp);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newStrengthened));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newRightAngle));

            return newGrounded;
        }
    }
}