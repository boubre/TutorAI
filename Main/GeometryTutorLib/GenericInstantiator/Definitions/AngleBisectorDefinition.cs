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
            if (c is AngleBisector) return InstantiateBisector(c as AngleBisector);

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
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateBisector(AngleBisector ab)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Create the two adjacent angles
            Point vertex = ab.angle.GetVertex();
            Point interiorPt = ab.angle.IsOnInteriorExplicitly(ab.bisector.Point1) ? ab.bisector.Point1 : ab.bisector.Point2;
            Angle adj1 = new Angle(ab.angle.ray1.OtherPoint(vertex), vertex, interiorPt);
            Angle adj2 = new Angle(ab.angle.ray2.OtherPoint(vertex), vertex, interiorPt);
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

                // We are interested in adjacent angles, not reflexive
                if (cas.IsReflexive()) return newGrounded;
                if (cas.AreAdjacent() == null) return newGrounded;

                //
                // The candidate must equates to a given segment; that is, find the proper segment
                // The segment must pass through the actual vertex of the adjacent angles
                //
                foreach (Segment segment in candidateSegments)
                {
                    newGrounded.AddRange(InstantiateToDef(cas, segment));
                }

                // Did not unify so add to the candidate list
                if (!newGrounded.Any()) candidateCongruent.Add(cas);
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
        // Construct the AngleBisector if we have
        //
        //      V---------------A
        //     / \
        //    /   \
        //   /     \
        //  B       C
        //
        // Congruent(Angle, A, V, C), Angle(C, V, B)),  Segment(V, C)) -> AngleBisector(Angle(A, V, B)  
        //
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToDef(CongruentAngles cas, Segment segment)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Find the shared segment between the two angles; we know it is valid if we reach this point
            Segment shared = cas.AreAdjacent();

            // The bisector must align with the given segment
            if (!segment.IsCollinearWith(shared)) return newGrounded;

            // We need a true bisector in which the shared vertex of the angles in between the endpoints of this segment
            if (!segment.PointIsOnAndBetweenEndpoints(cas.ca1.GetVertex())) return newGrounded;

            //
            // Create the overall angle which is being bisected
            //
            Point vertex = cas.ca1.GetVertex();
            Segment newRay1 = cas.ca1.OtherRayEquates(shared);
            Segment newRay2 = cas.ca2.OtherRayEquates(shared);
            Angle combinedAngle = new Angle(newRay1.OtherPoint(vertex), vertex, newRay2.OtherPoint(vertex));

            // Determine if the segment is a straight angle (we don't want an angle bisector here, we would want a segment bisector)
            if (newRay1.IsCollinearWith(newRay2)) return newGrounded;

            // The bisector cannot be of the form:
            //    \
            //     \
            //      V---------------A
            //     /
            //    /
            //   B
            if (!combinedAngle.IsOnInteriorExplicitly(segment.Point1) && !combinedAngle.IsOnInteriorExplicitly(segment.Point2)) return newGrounded;            
            
            AngleBisector newAB = new AngleBisector(combinedAngle, segment, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(segment);
            antecedent.Add(cas);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAB));
            return newGrounded;
        }
    }
}