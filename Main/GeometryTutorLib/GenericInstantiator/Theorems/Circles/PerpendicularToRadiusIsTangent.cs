using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularToRadiusIsTangent : Theorem
    {
        private readonly static string NAME = "Radii perpendicular to a segment is a tangent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, JustificationSwitch.PERPENDICULAR_TO_RADIUS_IS_TANGENT);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidatePerpendicular.Clear();
            candidateStrengthened.Clear();
        }

        private static List<ArcSegmentIntersection> candidateIntersections = new List<ArcSegmentIntersection>();
        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // Tangent(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is ArcSegmentIntersection)
            {
                ArcSegmentIntersection newInter = clause as ArcSegmentIntersection;

                foreach (Perpendicular oldPerp in candidatePerpendicular)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, oldPerp, oldPerp));
                }

                foreach (Strengthened oldStreng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, oldStreng.strengthened as Perpendicular, oldStreng));
                }

                candidateIntersections.Add(newInter);
            }
            else if (clause is Perpendicular)
            {
                Perpendicular newPerp = clause as Perpendicular;

                foreach (ArcSegmentIntersection oldInter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, newPerp, newPerp));
                }

                candidatePerpendicular.Add(newPerp);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is Perpendicular)) return newGrounded;

                foreach (ArcSegmentIntersection oldInter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, newStreng.strengthened as Perpendicular, newStreng));
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // Tangent(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        private static List<EdgeAggregator> InstantiateTheorem(ArcSegmentIntersection inter, Perpendicular perp, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Get the radius
            Segment radius = null;
            Segment garbage = null;
            inter.GetRadii(out radius, out garbage);

            // Two intersections, not a tangent situation.
            if (garbage != null) return newGrounded;

            // Does this perpendicular apply to this Arc intersection?
            if (!perp.HasSegment(radius) || !perp.HasSegment(inter.segment)) return newGrounded;

            Strengthened newTangent = new Strengthened(inter, new Tangent(inter));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, newTangent, annotation));

            return newGrounded;
        }
    }
}