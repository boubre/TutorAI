using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class SAS : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SAS";

        private static List<ConcreteTriangle> unifyCandTris = new List<ConcreteTriangle>();
        private static List<ConcreteCongruentAngles> unifyCandAngles = new List<ConcreteCongruentAngles>();
        private static List<ConcreteCongruentSegments> unifyCandSegments = new List<ConcreteCongruentSegments>();

        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Segment(A, C), Angle(D, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // Do we have a segment or triangle?
            if (!(c is ConcreteCongruentSegments) && !(c is ConcreteCongruentAngles) && !(c is ConcreteTriangle))
            {
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            //
            // Do we have enough information for unification?
            //
            if (c is ConcreteCongruentSegments && (!unifyCandSegments.Any() || unifyCandTris.Count <= 1 || !unifyCandAngles.Any()))
            {
                unifyCandSegments.Add((ConcreteCongruentSegments)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is ConcreteCongruentAngles && (unifyCandSegments.Count < 2 || !unifyCandTris.Any()))
            {
                unifyCandAngles.Add((ConcreteCongruentAngles)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is ConcreteTriangle && (!unifyCandTris.Any() || unifyCandSegments.Count < 2 || !unifyCandAngles.Any()))
            {
                unifyCandTris.Add((ConcreteTriangle)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            //
            // The list of new grounded clauses if they are deduced
            //
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

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

                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                            if (newCs.LinksTriangles(ct1, ct2))
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(unifyCandTris.ElementAt(i), unifyCandTris.ElementAt(j), newCs));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandSegments.Add(newCs);
            }
            else if (c is ConcreteCongruentAngles)
            {
                ConcreteCongruentAngles newCs = (ConcreteCongruentAngles)c;

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

                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                            if (newCs.LinksTriangles(ct1, ct2))
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(unifyCandTris.ElementAt(i), unifyCandTris.ElementAt(j), newCs));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandAngles.Add(newCs);
            }

            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (c is ConcreteTriangle)
            {
                ConcreteTriangle candidateTri = (ConcreteTriangle)c;
                foreach (ConcreteTriangle ct in unifyCandTris)
                {
                    List<ConcreteCongruentSegments> applicSegments = new List<ConcreteCongruentSegments>();
                    List<ConcreteCongruentAngles> applicAngles = new List<ConcreteCongruentAngles>();

                    newGrounded.AddRange(CollectAndCheckSAS(candidateTri, ct, applicSegments, applicAngles));
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(ConcreteTriangle ct1, ConcreteTriangle ct2,
                                                                                                   List<ConcreteCongruentSegments> applicSegments,
                                                                                                   List<ConcreteCongruentAngles> applicAngles)
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

            // Check all Angles
            foreach (ConcreteCongruentAngles cca in unifyCandAngles)
            {
                // Do these angles link the two triangles?
                if (cca.LinksTriangles(ct1, ct2))
                {
                    applicAngles.Add(cca);
                }
            }

            List<KeyValuePair<ConcretePoint, ConcretePoint>> pairs = IsSASsituation(ct1, ct2, applicSegments, applicAngles);

            // If pairs is populated, we have a SAS situation
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
//                int congreuntKey = deducedCongruentTriangles.Count;
//                deducedCongruentTriangles.Add(ccts);

                // Hypergraph
                List<GroundedClause> antecedent = new List<GroundedClause>();
                antecedent.Add(ct1);
                antecedent.Add(ct2);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));
                GroundedClause.ConstructClauseLinks(antecedent, ccts);


                // Add all the corresponding parts as new congruent clauses
                newGrounded.AddRange(ConcreteCongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(ConcreteTriangle ct1, ConcreteTriangle ct2, ConcreteCongruent cc)
        {
            List<ConcreteCongruentSegments> applicSegments = new List<ConcreteCongruentSegments>();
            List<ConcreteCongruentAngles> applicAngles = new List<ConcreteCongruentAngles>();
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            // Add the congruence statement to the appropriate list
            if (cc is ConcreteCongruentSegments)
            {
                applicSegments.Add((ConcreteCongruentSegments)cc);
            }
            else
            {
                applicAngles.Add((ConcreteCongruentAngles)cc);
            }

            return CollectAndCheckSAS(ct1, ct2, applicSegments, applicAngles);
        }

        //
        // Is this a true SAS situation?
        //
        private static List<KeyValuePair<ConcretePoint, ConcretePoint>> IsSASsituation(ConcreteTriangle ct1, ConcreteTriangle ct2,
                                      List<ConcreteCongruentSegments> segmentPairs,
                                      List<ConcreteCongruentAngles> anglePairs)
        {
            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<ConcretePoint, ConcretePoint>> pairs = new List<KeyValuePair<ConcretePoint, ConcretePoint>>();

            // Miniumum information required
            if (!anglePairs.Any() || segmentPairs.Count < 2) return pairs;

            // for each pair of congruent segments, is the given angle the included angle?
            for (int i = 0; i < segmentPairs.Count; i++)
            {
                for (int j = 0; j < anglePairs.Count; j++)
                {
                    // Don't compare a set of congruent segments to itself
                    if (i != j)
                    {
                        // Check if any of the angles is an included angle
                        foreach (ConcreteCongruentAngles ccas in anglePairs)
                        {
                            ConcreteSegment seg1Tri1 = ct1.SegmentBelongs(segmentPairs.ElementAt(i));
                            ConcreteSegment seg2Tri1 = ct1.SegmentBelongs(segmentPairs.ElementAt(j));

                            ConcreteSegment seg1Tri2 = ct2.SegmentBelongs(segmentPairs.ElementAt(i));
                            ConcreteSegment seg2Tri2 = ct2.SegmentBelongs(segmentPairs.ElementAt(j));

                            ConcreteAngle angleTri1 = ct1.AngleBelongs(ccas);
                            ConcreteAngle angleTri2 = ct2.AngleBelongs(ccas);

                            // Check both triangles if this is the included angle; if it is, we have SAS
                            if (angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
                            {
                                ConcretePoint vertex1 = angleTri1.GetVertex();
                                ConcretePoint vertex2 = angleTri2.GetVertex();

                                // The vertices of the angles correspond
                                pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(vertex1, vertex2));

                                // For the segments, look at the congruences and select accordingly
                                pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
                                pairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));

                                return pairs;
                            }
                        }
                    }
                }
            }

            return pairs;
        }
    }
}