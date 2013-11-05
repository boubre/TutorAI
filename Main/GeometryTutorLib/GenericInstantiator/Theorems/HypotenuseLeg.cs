using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class HypotenuseLeg : Theorem
    {

        private readonly static string NAME = "Hypotenuse Leg";

        private static List<ConcreteTriangle> unifyCandTris = new List<ConcreteTriangle>();
        private static List<ConcreteCongruentSegments> unifyCandSegments = new List<ConcreteCongruentSegments>();

        //
        // In order for two right triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Segment(A, C), Angle(D, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            //
            // The list of new grounded clauses if they are deduced
            //
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Do we have a segment or triangle?
            if (!(c is ConcreteCongruentSegments) && !(c is ConcreteTriangle))
            {
                return newGrounded;
            }

            //
            // Do we have enough information for unification?
            //
            if (c is ConcreteCongruentSegments && (!unifyCandSegments.Any() || unifyCandTris.Count <= 1))
            {
                unifyCandSegments.Add((ConcreteCongruentSegments)c);
                return newGrounded;
            }
            else if (c is ConcreteTriangle && (!unifyCandTris.Any() || unifyCandSegments.Count < 2))
            {
                unifyCandTris.Add((ConcreteTriangle)c);
                return newGrounded;
            }

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (c is ConcreteCongruentSegments)
            {
                ConcreteCongruentSegments newCs = (ConcreteCongruentSegments)c;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < unifyCandTris.Count; i++)
                {
                    for (int j = i + 1; j < unifyCandTris.Count; j++)
                    {
                        // Do not compare a triangle to itself
                        if (i != j)
                        {
                            ConcreteTriangle ct1 = unifyCandTris.ElementAt(i);
                            ConcreteTriangle ct2 = unifyCandTris.ElementAt(j);

                            // We must know both candidate triangles are right triangles
                            if (ct1.provenRight && ct2.provenRight)
                            {
                                // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                                if (newCs.LinksTriangles(ct1, ct2))
                                {
                                    newGrounded.AddRange(CollectAndCheckHL(unifyCandTris.ElementAt(i), unifyCandTris.ElementAt(j), newCs));
                                }
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandSegments.Add(newCs);
            }

            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (c is ConcreteTriangle)
            {
                ConcreteTriangle candidateTri = (ConcreteTriangle)c;
                foreach (ConcreteTriangle ct in unifyCandTris)
                {
                    // Only consider pairs of right triangles
                    if (((ConcreteTriangle)c).provenRight && ct.provenRight)
                    {
                        List<ConcreteCongruentSegments> applicSegments = new List<ConcreteCongruentSegments>();

                        newGrounded.AddRange(CollectAndCheckHL(candidateTri, ct, applicSegments));
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // Is this a true HL situation? We know at this point both triangles are right triangles
        //
        private static List<KeyValuePair<ConcretePoint, ConcretePoint>> IsHLsituation(ConcreteTriangle ct1, ConcreteTriangle ct2,
                                         List<ConcreteCongruentSegments> segmentPairs,
                                     out List<GroundedClause> congruences)
        {
            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<ConcretePoint, ConcretePoint>> pairs = new List<KeyValuePair<ConcretePoint, ConcretePoint>>();

            // Initialize congruences
            congruences = new List<GroundedClause>();

            // Miniumum information required
            if (segmentPairs.Count < 2) return pairs;

            //
            // for each combination of congruent segments, do we have a hypotenuse leg scenario?
            //
            for (int i = 0; i < segmentPairs.Count; i++)
            {
                for (int j = i + 1; j < segmentPairs.Count; j++)
                {
                    // Do we have a hypotenuse set and a leg set?
                    ConcreteSegment seg1Tri1 = ct1.SegmentBelongs(segmentPairs.ElementAt(i));
                    ConcreteSegment seg2Tri1 = ct1.SegmentBelongs(segmentPairs.ElementAt(j));

                    ConcreteSegment seg1Tri2 = ct2.SegmentBelongs(segmentPairs.ElementAt(i));
                    ConcreteSegment seg2Tri2 = ct2.SegmentBelongs(segmentPairs.ElementAt(j));

                    ConcreteAngle angleTri1 = ct1.rightAngle;
                    ConcreteAngle angleTri2 = ct2.rightAngle;

                    // Check both triangles for hypotenuse leg by checking to see if the right angle is NOT the included angle
                    if (!angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && !angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
                    {
                        // The right angles correspond; the vertices correspond directly
                        ConcretePoint vertex1 = angleTri1.GetVertex();
                        ConcretePoint vertex2 = angleTri2.GetVertex();

                        pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(vertex1, vertex2));

                        // Find the other shared vertex from the segments; this is not the right angle found above
                        ConcretePoint nextVertex1 = seg1Tri1.SharedVertex(seg2Tri1);
                        ConcretePoint nextVertex2 = seg1Tri2.SharedVertex(seg2Tri2);
                        pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(nextVertex1, nextVertex2));
                        pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(ct1.OtherPoint(vertex1, nextVertex1), ct2.OtherPoint(vertex2, nextVertex2)));

                        congruences.Add(segmentPairs.ElementAt(i));
                        congruences.Add(segmentPairs.ElementAt(j));
                        //congruences.Add(new ConcreteCongruentAngles(angleTri1, angleTri2, "HL: All right angles are congruent."));

                        return pairs;
                    }
                }
            }

            return pairs;
        }

        //
        // Acquires all of the applicable congruent segments; then checks HL
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckHL(ConcreteTriangle ct1, ConcreteTriangle ct2, List<ConcreteCongruentSegments> applicSegments)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Check all other segments
            foreach (ConcreteCongruentSegments ccs in unifyCandSegments)
            {
                // Does this segment link the two triangles?
                if (ccs.LinksTriangles(ct1, ct2))
                {
                    applicSegments.Add(ccs);
                }
            }

            List<GroundedClause> congruences;
            List<KeyValuePair<ConcretePoint, ConcretePoint>> pairs = IsHLsituation(ct1, ct2, applicSegments, out congruences);

            // If pairs is populated, we have a HL situation
            if (pairs.Any())
            {
                // Create the congruence between the triangles
                List<ConcretePoint> triangleOne = new List<ConcretePoint>();
                List<ConcretePoint> triangleTwo = new List<ConcretePoint>();
                foreach (KeyValuePair<ConcretePoint, ConcretePoint> pair in pairs)
                {
                    triangleOne.Add(pair.Key);
                    triangleTwo.Add(pair.Value);
                }

                ConcreteCongruentTriangles ccts = new ConcreteCongruentTriangles(new ConcreteTriangle(triangleOne),
                                                                                 new ConcreteTriangle(triangleTwo), NAME);

                // Hypergraph
                List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
                antecedent.Add(ct1);
                antecedent.Add(ct2);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

                // Add all the corresponding parts as new congruent clauses
                newGrounded.AddRange(ConcreteCongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments; then checks HL
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckHL(ConcreteTriangle ct1, ConcreteTriangle ct2, ConcreteCongruentSegments cc)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            List<ConcreteCongruentSegments> applicSegments = new List<ConcreteCongruentSegments>();

            // Add the congruence statement to the list
            applicSegments.Add(cc);

            return CollectAndCheckHL(ct1, ct2, applicSegments);
        }

    }
}
