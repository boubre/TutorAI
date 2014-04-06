using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TangentPerpendicularToRadius : Theorem
    {
        private readonly static string NAME = "Tangents are Perpendicular To Radii";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, JustificationSwitch.TANGENT_IS_PERPENDICULAR_TO_RADIUS);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidateTangents.Clear();
        }

        private static List<Intersection> candidateIntersections = new List<Intersection>();
        private static List<ArcSegmentIntersection> candidateTangents = new List<ArcSegmentIntersection>();

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // ArcSegmentIntersection(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is ArcSegmentIntersection)
            {
                ArcSegmentIntersection newTangent = clause as ArcSegmentIntersection;

                if (!newTangent.IsTangent()) return newGrounded;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(newTangent, inter));
                }

                candidateTangents.Add(newTangent);
            }
            else if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                foreach (ArcSegmentIntersection oldTangent in candidateTangents)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldTangent, newInter));
                }

                candidateIntersections.Add(newInter);
            }

            return newGrounded;
        }

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // ArcSegmentIntersection(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        private static List<EdgeAggregator> InstantiateTheorem(ArcSegmentIntersection tangent, Intersection inter)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this tangent segment apply to this intersection?
            if (!inter.HasSegment(tangent.segment)) return newGrounded;

            // Get the radius--if it exists
            Segment radius = null;
            Segment garbage = null;
            tangent.GetRadii(out radius, out garbage);

            if (radius == null) return newGrounded;

            // Does this radius apply to this intersection?
            if (!inter.HasSegment(radius)) return newGrounded;

            Strengthened newPerp = new Strengthened(inter, new Perpendicular(inter));
            
            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tangent);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, newPerp, annotation));

            return newGrounded;
        }
    }
}