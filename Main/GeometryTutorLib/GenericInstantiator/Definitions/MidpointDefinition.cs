using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MidpointDefinition : Definition
    {
        private readonly static string NAME = "Definition of Midpoint";

        public MidpointDefinition() { }

        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateSegments.Clear();
        }

        //
        // This implements forward and Backward instantiation
        // Forward is Midpoint -> Congruent Clause
        // Backward is Congruent -> Midpoint Clause
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (c is Midpoint || c is Strengthened) return InstantiateMidpoint(c);

            if (c is CongruentSegments || c is Segment) return InstantiateCongruent(c);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        //
        // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
        // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B)); This implies: AM = MB
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateMidpoint(GroundedClause clause)
        {
            if (clause is Midpoint) return InstantiateMidpoint(clause, clause as Midpoint);

            if ((clause as Strengthened).strengthened is Midpoint) return InstantiateMidpoint(clause, (clause as Strengthened).strengthened as Midpoint);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateMidpoint(GroundedClause original, Midpoint midpt)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
            InMiddle im = new InMiddle(midpt.midpoint, midpt.segment, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, im));

            //
            // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B))
            //
            Segment left = new Segment(midpt.segment.Point1, midpt.midpoint);
            Segment right = new Segment(midpt.midpoint, midpt.segment.Point2);
            GeometricCongruentSegments ccss = new GeometricCongruentSegments(left, right, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccss));

            return newGrounded;
        }

        //
        // Congruent(Segment(A, M), Segment(M, B)) -> Midpoint(M, Segment(A, B))
        //
        private static List<Segment> candidateSegments = new List<Segment>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateCongruent(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is CongruentSegments)
            {
                CongruentSegments cs = c as CongruentSegments;

                if (cs.IsReflexive()) return newGrounded;

                if (!cs.cs1.IsCollinearWith(cs.cs2)) return newGrounded;

                Point midpt = cs.cs1.SharedVertex(cs.cs2);

                // If the segments are collinear, but disconnected
                if (midpt == null) return newGrounded;

                for (int s = 0; s < candidateSegments.Count; s++)
                {
                    Segment seg = candidateSegments[s];
                    if (seg.HasPoint(cs.cs1.OtherPoint(midpt)) && seg.HasPoint(cs.cs2.OtherPoint(midpt)))
                    {
                        Midpoint newMidpoint = new Midpoint(midpt, seg, NAME);

                        // No need to consider this segment anymore
                        candidateSegments.RemoveAt(s);

                        // For hypergraph
                        List<GroundedClause> antecedent = new List<GroundedClause>();
                        antecedent.Add(seg);
                        antecedent.Add(cs);

                        newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newMidpoint));
                        return newGrounded;
                    }
                }
                // Did not unify so add to the candidate list
                candidateCongruent.Add(cs);
            }
            else if (c is Segment)
            {
                Segment segment = c as Segment;
                for (int cs = 0; cs < candidateCongruent.Count; cs++)
                {
                    CongruentSegments conSegs = candidateCongruent[cs];
                    Point midpt = conSegs.cs1.SharedVertex(conSegs.cs2);

                    if (segment.HasPoint(conSegs.cs1.OtherPoint(midpt)) && segment.HasPoint(conSegs.cs2.OtherPoint(midpt)))
                    {
                        Midpoint newMidpoint = new Midpoint(midpt, segment, NAME);

                        // No need to consider this segment anymore
                        candidateCongruent.RemoveAt(cs);

                        // For hypergraph
                        List<GroundedClause> antecedent = new List<GroundedClause>();
                        antecedent.Add(segment);
                        antecedent.Add(conSegs);

                        newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newMidpoint));
                        return newGrounded;
                    }
                }

                // Did not unify so add to the candidate list
                candidateSegments.Add(segment);
            }

            return newGrounded;
        }
    }
}