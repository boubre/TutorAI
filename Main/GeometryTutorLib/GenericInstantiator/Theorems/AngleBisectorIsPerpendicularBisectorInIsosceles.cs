using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleBisectorIsPerpendicularBisectorInIsosceles : Theorem
    {
        private readonly static string NAME = "The bisector of the vertex angle of an isosceles triangle is perpendicular to the base at its midpoint.";

        public AngleBisectorIsPerpendicularBisectorInIsosceles() { }

        private static List<IsoscelesTriangle> candidateIsosceles = new List<IsoscelesTriangle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<AngleBisector> candidateBisectors = new List<AngleBisector>();
        private static List<Intersection> candidateIntersections = new List<Intersection>();

        //
        // IsoscelesTriangle(A, B, C),
        // AngleBisector(Segment(M, C), Angle(A, C, B)),
        // Intersection(M, Segment(M, C), Segment(A, B) -> PerpendicularBisector(M, Segment(M, C), Segment(A, B)),
        //                                                 Midpoint(M, Segment(A, B))
        //
        //   A _____M_____B
        //     \    |    /
        //      \   |   /
        //       \  |  / 
        //        \ | /
        //         \|/
        //          C
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Strengthened) && !(c is IsoscelesTriangle) && !(c is AngleBisector) && !(c is Intersection)) return newGrounded;

            if (c is IsoscelesTriangle)
            {
                IsoscelesTriangle isoTri = c as IsoscelesTriangle;

                foreach (AngleBisector ab in candidateBisectors)
                {
                    foreach (Intersection inter in candidateIntersections)
                    {
                        newGrounded.AddRange(GeneratePerpendicularBisector(isoTri, ab, inter));
                    }
                }

                candidateIsosceles.Add(isoTri);
            }
            else if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                // We are not interested if the strengthened clause is not Isosceles (note, this includes equilateral)
                if (!(streng.strengthened is IsoscelesTriangle)) return newGrounded;

                foreach (AngleBisector ab in candidateBisectors)
                {
                    foreach (Intersection inter in candidateIntersections)
                    {
                        newGrounded.AddRange(GeneratePerpendicularBisector(streng.strengthened as IsoscelesTriangle, ab, inter));
                    }
                }

                candidateStrengthened.Add(streng);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                foreach (IsoscelesTriangle isoTri in candidateIsosceles)
                {
                    foreach (AngleBisector ab in candidateBisectors)
                    {
                       newGrounded.AddRange(GeneratePerpendicularBisector(isoTri, ab, newIntersection));
                    }
                }

                candidateIntersections.Add(newIntersection);
            }
            else if (c is AngleBisector)
            {
                AngleBisector newAB = c as AngleBisector;

                foreach (IsoscelesTriangle isoTri in candidateIsosceles)
                {
                    foreach (Intersection inter in candidateIntersections)
                    {
                        newGrounded.AddRange(GeneratePerpendicularBisector(isoTri, newAB, inter));
                    }
                }

                candidateBisectors.Add(newAB);
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GeneratePerpendicularBisector(IsoscelesTriangle tri, AngleBisector ab, Intersection inter)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (tri is EquilateralTriangle)
            {

            }

            // Does the Angle Bisector occur at the vertex angle (non-base angles) of the Isosceles triangle?
            if (!ab.angle.GetVertex().Equals(tri.GetVertexAngle().GetVertex())) return newGrounded;

            // Is the intersection point between the endpoints of the base of the triangle?
            if (!Segment.Between(inter.intersect, tri.baseSegment.Point1, tri.baseSegment.Point2)) return newGrounded;

            // Does this intersection define this angle bisector situation? That is, the bisector and base must align with the intersection
            if (!inter.ImpliesRay(ab.bisector)) return newGrounded;
            if (!inter.HasSegment(tri.baseSegment)) return newGrounded;

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(ab);
            antecedent.Add(inter);

            //
            // PerpendicularBisector(M, Segment(M, C), Segment(A, B))
            //
            PerpendicularBisector perpB = new PerpendicularBisector(inter, NAME);
            Strengthened s = new Strengthened(inter, perpB, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, s));

            //
            // Midpoint(M, Segment(A, B))
            //
            Midpoint midpt = new Midpoint(inter.intersect, tri.baseSegment, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, midpt));

            return newGrounded;
        }
    }
}