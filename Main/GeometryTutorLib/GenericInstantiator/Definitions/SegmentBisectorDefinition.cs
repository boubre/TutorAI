using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SegmentBisectorDefinition : Definition
    {
        private readonly static string NAME = "Definition of Segment Bisector";

        // Resets all saved data.
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateIntersection.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (c is SegmentBisector || c is Strengthened) return InstantiateFromSegmentBisector(c);

            if (c is Intersection || c is CongruentSegments) return InstantiateToSegmentBisector(c);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // SegmentBisector(Segment(V, C), Segment(B, A)) -> Midpoint(V, Segment(B, A))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromSegmentBisector(GroundedClause clause)
        {
            if (clause is SegmentBisector) return InstantiateFromSegmentBisector(clause, clause as SegmentBisector);

            if ((clause as Strengthened).strengthened is SegmentBisector)
            {
                return InstantiateFromSegmentBisector(clause, (clause as Strengthened).strengthened as SegmentBisector);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromSegmentBisector(GroundedClause original, SegmentBisector sb)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Create the midpoint
            Midpoint midpt = new Midpoint(sb.bisected.intersect, sb.bisected.OtherSegment(sb.bisector), NAME);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, midpt));

            return newGrounded;
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // Congruent(Segment(B, V), Segment(V, A)), Intersection(V, Segment(B, A), Segment(V, C)) -> SegmentBisector(Segment(V, C), Segment(B, A))
        //        
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToSegmentBisector(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is CongruentSegments)
            {
                CongruentSegments conSegs = c as CongruentSegments;

                // We are not interested in a reflexive relationship
                if (conSegs.IsReflexive()) return newGrounded;

                // The congruent segments need to share an endpoint (adjacent congruent segments)
                Point shared = conSegs.cs1.SharedVertex(conSegs.cs2);
                if (shared == null) return newGrounded;

                // The segments need to be collinear
                if (!conSegs.cs1.IsCollinearWith(conSegs.cs2)) return newGrounded;

                // Match the congruences with the intersection
                foreach (Intersection inter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateToDef(shared, inter, conSegs));
                }

                // Did not unify so add to the candidate list
                candidateCongruent.Add(conSegs);
            }
            else if (c is Intersection)
            {
                Intersection inter = c as Intersection;

                //    /
                //   /
                //  /____
                // If we have a corner situation, we are not interested
                if (inter.StandsOnEndpoint()) return newGrounded;

                foreach (CongruentSegments cs in candidateCongruent)
                {
                    newGrounded.AddRange(InstantiateToDef(cs.cs1.SharedVertex(cs.cs2), inter, cs));
                }

                // Did not unify so add to the candidate list
                candidateIntersection.Add(inter);
            }

            return newGrounded;
        }

        //
        // Take the angle congruence and bisector and create the AngleBisector relation
        //              \
        //               \
        //     B ---------V---------A
        //                 \
        //                  \
        //                   C
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToDef(Point intersectionPoint, Intersection inter, CongruentSegments cs)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Does the given point of intersection apply to this actual intersection object
            if (!intersectionPoint.Equals(inter.intersect)) return newGrounded;

            // The entire segment AB
            Segment overallSegment = new Segment(cs.cs1.OtherPoint(intersectionPoint), cs.cs2.OtherPoint(intersectionPoint));

            // The segment must align completely with one of the intersection segments
            Segment interCollinearSegment = inter.GetCollinearSegment(overallSegment);
            if (interCollinearSegment == null) return newGrounded;

            // Does this intersection have the entire segment AB
            if (!inter.HasSegment(overallSegment)) return newGrounded;

            Segment bisector = inter.OtherSegment(overallSegment);
            Segment bisectedSegment = inter.GetCollinearSegment(overallSegment);

            // Check if the bisected segment extends is the exact same segment as the overall segment AB
            if (!bisectedSegment.StructurallyEquals(overallSegment))
            {
                if (overallSegment.PointIsOnAndBetweenEndpoints(bisectedSegment.Point1) &&
                    overallSegment.PointIsOnAndBetweenEndpoints(bisectedSegment.Point2)) return newGrounded;
            }

            SegmentBisector newSB = new SegmentBisector(inter, bisector, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(cs);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newSB));
            return newGrounded;
        }
    }
}