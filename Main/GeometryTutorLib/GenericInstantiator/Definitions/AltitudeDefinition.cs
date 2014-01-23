using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AltitudeDefinition : Definition
    {
        private readonly static string NAME = "Definition of Altitude";

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateAltitude.Clear();
            candidateIntersection.Clear();
            candidatePerpendicular.Clear();
            candidateTriangle.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is PerpendicularBisector || c is Strengthened)
            {
                newGrounded.AddRange(InstantiatePerpendicularBisector(c));
            }

            if (c is Triangle || c is Perpendicular)
            {
                newGrounded.AddRange(InstantiatePerpendicular(c));
            }

            else if (c is Altitude || c is Intersection)
            {
                newGrounded.AddRange(InstantiateAltitude(c));
            }

            return newGrounded;
        }

        //
        //       A
        //      /|\
        //     / | \
        //    /  |  \
        //   /   |_  \
        //  /____|_|__\
        // B     M     C
        //
        // Altitude(Segment(A, M), Triangle(A, B, C)), Intersection(M, Segment(A, M), Segment(B, C)) -> Perpendicular(M, Segment(A, M), Segment(B, C))
        //
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Altitude> candidateAltitude = new List<Altitude>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateAltitude(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Intersection)
            {
                Intersection inter = c as Intersection;

                // We are only interested in straight-angle type intersections
                if (inter.StandsOnEndpoint()) return newGrounded;

                foreach (Altitude altitude in candidateAltitude)
                {
                    newGrounded.AddRange(InstantiateAltitude(inter, altitude));
                }

                candidateIntersection.Add(inter);
            }
            else if (c is Altitude)
            {
                Altitude altitude  = c as Altitude;

                foreach (Intersection inter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateAltitude(inter, altitude));
                }

                candidateAltitude.Add(altitude);
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateAltitude(Intersection inter, Altitude altitude)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The intersection should contain the altitude segment
            if (!inter.HasSegment(altitude.segment)) return newGrounded;

            // The triangle should contain the other segment in the intersection
            Segment triangleSide = altitude.triangle.CoincidesWithASide(inter.OtherSegment(altitude.segment));
            if (triangleSide == null) return newGrounded;
            if (!inter.OtherSegment(altitude.segment).HasSubSegment(triangleSide)) return newGrounded;

            //
            // Create the Perpendicular relationship
            //
            Strengthened streng = new Strengthened(inter, new Perpendicular(inter, NAME), NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(altitude);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, streng));

            return newGrounded;
        }

        //
        //       A
        //      /|\
        //     / | \
        //    /  |  \
        //   /   |_  \
        //  /____|_|__\
        // B     M     C
        //
        // Triangle(A, B, C), Perpendicular(M, Segment(A, M), Segment(B, C)) -> Altitude(Segment(A, M), Triangle(A, B, C))
        //
        private static List<Triangle> candidateTriangle = new List<Triangle>();
        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiatePerpendicular(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Triangle)
            {
                Triangle tri = c as Triangle;

                foreach (Perpendicular perp in candidatePerpendicular)
                {
                    newGrounded.AddRange(InstantiatePerpendicular(tri, perp));
                }

                candidateTriangle.Add(tri);
            }
            else if (c is Perpendicular)
            {
                Perpendicular perp = c as Perpendicular;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiatePerpendicular(tri, perp));
                }

                candidatePerpendicular.Add(perp);
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiatePerpendicular(Triangle triangle, Perpendicular perp)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Acquire the side of the triangle containing the intersection point
            // This point may or may not be directly on the triangle side
            Segment baseSegment = triangle.GetSegmentWithPointOnOrExtends(perp.intersect);
            if (baseSegment == null) return newGrounded;

            // The altitude must pass through the intersection point as well as the opposing vertex
            Point oppositeVertex = triangle.OtherPoint(baseSegment);

            Segment altitude = new Segment(perp.intersect, oppositeVertex);

            // The alitude must alig with the intersection
            if (!perp.ImpliesRay(altitude)) return newGrounded;

            // The opposing side must align with the intersection
            if (!perp.OtherSegment(altitude).IsCollinearWith(baseSegment)) return newGrounded;

            Altitude newAltitude = new Altitude(triangle, altitude, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(triangle);
            antecedent.Add(perp);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAltitude));
            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiatePerpendicularBisector(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is PerpendicularBisector)
            {
                PerpendicularBisector perpBis = c as PerpendicularBisector;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiatePerpendicularBisector(tri, perpBis, perpBis));
                }
            }
            else if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is PerpendicularBisector)) return newGrounded;

                PerpendicularBisector perpBis = streng.strengthened as PerpendicularBisector;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiatePerpendicularBisector(tri, perpBis, streng));
                }
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiatePerpendicularBisector(Triangle triangle, PerpendicularBisector perpBis, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Acquire the side of the triangle containing the intersection point
            // This point may or may not be directly on the triangle side
            Segment baseSegment = triangle.GetSegmentWithPointOnOrExtends(perpBis.intersect);
            if (baseSegment == null) return newGrounded;

            // The altitude must pass through the intersection point as well as the opposing vertex
            Point oppositeVertex = triangle.OtherPoint(baseSegment);

            Segment altitude = new Segment(perpBis.intersect, oppositeVertex);

            // The alitude must alig with the intersection
            if (!perpBis.ImpliesRay(altitude)) return newGrounded;

            // The opposing side must align with the intersection
            if (!perpBis.OtherSegment(altitude).IsCollinearWith(baseSegment)) return newGrounded;

            Altitude newAltitude = new Altitude(triangle, altitude, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(triangle);
            antecedent.Add(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAltitude));
            return newGrounded;
        }
    }
}