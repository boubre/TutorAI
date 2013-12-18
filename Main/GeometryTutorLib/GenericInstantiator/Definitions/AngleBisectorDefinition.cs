using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleBisectorDefinition : Definition
    {
        private readonly static string NAME = "Definition of Angle Bisector";

        public AngleBisectorDefinition() { }

        //
        // This implements forward and Backward instantiation
        // Forward is Midpoint -> Congruent Clause
        // Backward is Congruent -> Midpoint Clause
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (c is AngleBisector) return InstantiateBisector(c);

            if (c is CongruentAngles || c is Segment) return InstantiateCongruent(c);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        //      V---------------A
        //     / \
        //    /   \
        //   /     \
        //  B       C
        //
        // AngleBisector(Angle(A, V, B), Segment(V, C)) -> Congruent(Angle, A, V, C), Angle(C, V, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateBisector(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is AngleBisector)) return newGrounded;

            AngleBisector ab = c as AngleBisector;

            // Create the two adjacent angles
            Point vertex = ab.angle.GetVertex();
            Angle adj1 = new Angle(ab.angle.ray1.OtherPoint(vertex), vertex, ab.bisector.OtherPoint(vertex));
            Angle adj2 = new Angle(ab.angle.ray2.OtherPoint(vertex), vertex, ab.bisector.OtherPoint(vertex));
            GeometricCongruentAngles cas = new GeometricCongruentAngles(adj1, adj2, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(ab);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cas));

            return newGrounded;
        }

        //      V---------------A
        //     / \
        //    /   \
        //   /     \
        //  B       C
        //
        // Congruent(Angle, A, V, C), Angle(C, V, B)),  Segment(V, C)) -> AngleBisector(Angle(A, V, B)  
        //
        private static List<Segment> candidateSegments = new List<Segment>();
        private static List<CongruentAngles> candidateCongruent = new List<CongruentAngles>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateCongruent(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is CongruentAngles)
            {
                CongruentAngles cas = c as CongruentAngles;

                if (cas.IsReflexive()) return newGrounded;

                // Find the shared segment between the two angles
                Segment candidateBisector = cas.AreAdjacent();

                if (candidateBisector == null) return newGrounded;

                //
                // The candidate must equates to a given segment; that is, find the proper segment
                // The segment must pass through the actual vertex of the adjacent angles
                //
                foreach (Segment segment in candidateSegments)
                {
                    if (candidateBisector.IsCollinearWith(segment))
                    {
                        newGrounded.AddRange(InstantiateToDef(cas, segment));
                        return newGrounded;
                    }
                }

                // Did not unify so add to the candidate list
                candidateCongruent.Add(cas);
            }
            else if (c is Segment)
            {
                Segment segment = c as Segment;
                foreach (CongruentAngles cas in candidateCongruent)
                {
                    newGrounded.AddRange(InstantiateToDef(cas, segment));
                }

                candidateSegments.Add(segment);
            }

            return newGrounded;
        }

        //
        // Take the angle congruence and bisector and create the AngleBisector relation
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToDef(CongruentAngles cas, Segment segment)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!Segment.Between(cas.ca1.GetVertex(), segment.Point1, segment.Point2)) return newGrounded;

            Point vertex = cas.ca1.GetVertex();
            Segment shared = cas.SegmentShared();
            Segment newRay1 = cas.ca1.OtherRay(shared);
            Segment newRay2 = cas.ca2.OtherRay(shared);
            Angle combinedAngle = new Angle(newRay1.OtherPoint(vertex), vertex, newRay2.OtherPoint(vertex));
            AngleBisector newAB = new AngleBisector(combinedAngle, shared, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(segment);
            antecedent.Add(cas);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAB));
            return newGrounded;
        }
    }
}