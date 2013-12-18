using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SASSimilarity : Theorem
    {
        private readonly static string NAME = "SAS Similarity";

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentAngles> candidateCongruentAngles = new List<CongruentAngles>();
        private static List<ProportionalSegments> candidatePropSegments = new List<ProportionalSegments>();

        //
        // In order for two triangles to be Similar, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Proportional(Segment(A, B), Segment(D, E)),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Proportional(Segment(B, C), Segment(E, F)) -> Similar(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                  Proportional(Segment(A, C), Angle(D, F)),
        //                                                  Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                                  Congruent(Angle(B, C, A), Angle(E, F, D))
        //
        // Note: we need to figure out the proper order of the sides to guarantee similarity
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Do we have a segment or triangle?
            if (!(c is ProportionalSegments) && !(c is CongruentAngles) && !(c is Triangle) && !(c is CongruentSegments)) return newGrounded;

            // If this is a new segment, check for Similar triangles with this new piece of information
            if (c is ProportionalSegments)
            {
                ProportionalSegments newProp = c as ProportionalSegments;

                // Check all combinations of triangles to see if they are Similar
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentAngles cas in candidateCongruentAngles)
                        {
                            foreach (ProportionalSegments oldProp in candidatePropSegments)
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(candidateTriangles[i], candidateTriangles[j], cas, newProp, oldProp));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidatePropSegments.Add(newProp);
            }
            else if (c is CongruentAngles)
            {
                CongruentAngles newCas = c as CongruentAngles;

                // Check all combinations of triangles to see if they are Similar
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        for (int m = 0; m < candidatePropSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidatePropSegments.Count; n++)
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(candidateTriangles[i], candidateTriangles[j], newCas, candidatePropSegments[m], candidatePropSegments[n]));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateCongruentAngles.Add(newCas);
            }

            // If this is a new triangle, check for triangles which may be Similar to this new triangle
            else if (c is Triangle)
            {
                Triangle candidateTri = c as Triangle;

                foreach (Triangle oldTriangle in candidateTriangles)
                {
                    foreach (CongruentAngles cas in candidateCongruentAngles)
                    {
                        for (int m = 0; m < candidatePropSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidatePropSegments.Count; n++)
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(candidateTri, oldTriangle, cas, candidatePropSegments[m], candidatePropSegments[n]));
                            }
                        }
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                candidateTriangles.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // 
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(Triangle ct1, Triangle ct2, CongruentAngles cas, ProportionalSegments pss1, ProportionalSegments pss2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (ct1.HasEstablishedSimilarity(ct2) || ct2.HasEstablishedSimilarity(ct1)) return newGrounded;
            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

            // Proportions must actually equate
            if (!pss1.ProportionallyEquals(pss2)) return newGrounded;

            // The smaller and larger segments of the proportionality must be distinct, respectively.
            if (!pss1.IsDistinctFrom(pss2)) return newGrounded;

            //
            // The proportional relationships need to link the given triangles
            //
            if (!cas.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!pss1.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!pss2.LinksTriangles(ct1, ct2)) return newGrounded;

            Segment seg1Tri1 = ct1.GetSegment(pss1);
            Segment seg2Tri1 = ct1.GetSegment(pss2);

            Segment seg1Tri2 = ct2.GetSegment(pss1);
            Segment seg2Tri2 = ct2.GetSegment(pss2);

            // Avoid redundant segments, if they arise
            if (seg1Tri1.StructurallyEquals(seg2Tri1)) return newGrounded;
            if (seg1Tri2.StructurallyEquals(seg2Tri2)) return newGrounded;

            Angle angleTri1 = ct1.AngleBelongs(cas);
            Angle angleTri2 = ct2.AngleBelongs(cas);

            // Check both triangles if this is the included angle; if it is, we have SAS
            if (!angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1)) return newGrounded;
            if (!angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2)) return newGrounded;

            //
            // Generate Similar Triangles
            //
            ct1.AddSimilarTriangle(ct2);
            ct2.AddSimilarTriangle(ct1);

            Point vertex1 = angleTri1.GetVertex();
            Point vertex2 = angleTri2.GetVertex();

            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

            // The vertices of the angles correspond
            pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

            // For the segments, look at the congruences and select accordingly
            pairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
            pairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));
            
            List<GroundedClause> simTriAntecedent = new List<GroundedClause>();
            simTriAntecedent.Add(ct1);
            simTriAntecedent.Add(ct2);
            simTriAntecedent.Add(cas);
            simTriAntecedent.Add(pss1);
            simTriAntecedent.Add(pss2);

            newGrounded.AddRange(GenerateCorrespondingParts(pairs, simTriAntecedent, NAME));

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateCorrespondingParts(List<KeyValuePair<Point, Point>> pairs, List<GroundedClause> antecedent, string justification)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // If pairs is populated, we have a SAS situation
            if (!pairs.Any()) return newGrounded;

            // Create the similarity between the triangles
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();
            foreach (KeyValuePair<Point, Point> pair in pairs)
            {
                triangleOne.Add(pair.Key);
                triangleTwo.Add(pair.Value);
            }

            GeometricSimilarTriangles simTris = new GeometricSimilarTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), justification);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, simTris));

            //
            // We should not add any new relationships to the graph (thus creating cycles)
            //
            //List<KeyValuePair<List<GroundedClause>, GroundedClause>> componentsRelations = SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo);
            //foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in componentsRelations)
            //{
            //    if (!antecedent.Contains(part.Value))
            //    {
            //        newGrounded.Add(part);
            //    }
            //}

            // Add all the corresponding parts as new Similar clauses
            newGrounded.AddRange(SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}





        ////
        //// Acquires all of the applicable proportional segments as well as congruent angles. Then checks for SAS
        ////
        //private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(Triangle ct1, Triangle ct2,
        //                                                                                           List<ProportionalSegments> applicSegments,
        //                                                                                           List<CongruentAngles> applicAngles)
        //{



        //    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

        //    // Has this congruence or similarity been established before? If so, do not deduce it again.
        //    if (ct1.HasEstablishedSimilarity(ct2) || ct2.HasEstablishedSimilarity(ct1)) return newGrounded;
        //    if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

        //    // Check all other segments
        //    foreach (ProportionalSegments ccs in candidatePropSegments)
        //    {
        //        // Does this segment link the two triangles?
        //        if (ccs.LinksTriangles(ct1, ct2))
        //        {
        //            applicSegments.Add(ccs);
        //        }
        //    }

        //    // Check all Angles
        //    foreach (CongruentAngles cca in candidateCongruentAngles)
        //    {
        //        // Do these angles link the two triangles?
        //        if (cca.LinksTriangles(ct1, ct2))
        //        {
        //            applicAngles.Add(cca);
        //        }
        //    }

        //    List<GroundedClause> facts;
        //    List<KeyValuePair<Point, Point>> pairs = IsSASsituation(ct1, ct2, applicSegments, applicAngles, out facts);

        //    // If pairs is populated, we have a SAS situation
        //    if (pairs.Any())
        //    {
        //        // Create the congruence between the triangles
        //        List<Point> triangleOne = new List<Point>();
        //        List<Point> triangleTwo = new List<Point>();
        //        foreach (KeyValuePair<Point, Point> pair in pairs)
        //        {
        //            triangleOne.Add(pair.Key);
        //            triangleTwo.Add(pair.Value);
        //        }

        //        // Indicate that these two triangles are Similar to avoid deducing this again later.
        //        ct1.AddSimilarTriangle(ct2);
        //        ct2.AddSimilarTriangle(ct1);

        //        GeometricSimilarTriangles simTris = new GeometricSimilarTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

        //        // Hypergraph
        //        List<GroundedClause> antecedent = new List<GroundedClause>(facts);
        //        antecedent.Add(ct1);
        //        antecedent.Add(ct2);

        //        newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, simTris));

        //        //
        //        // We should not add any new relationships to the graph (thus creating cycles)
        //        //
        //        List<KeyValuePair<List<GroundedClause>, GroundedClause>> componentsRelations = SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo);
        //        foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in componentsRelations)
        //        {
        //            if (!facts.Contains(part.Value))
        //            {
        //                newGrounded.Add(part);
        //            }
        //        }

        //        // Add all the corresponding parts as new Similar clauses
        //        //newGrounded.AddRange(SimilarTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
        //    }

        //    return newGrounded;
        //}

        ////
        //// Is this a true SAS situation?
        ////
        //private static List<KeyValuePair<Point, Point>> IsSASsituation(Triangle ct1, Triangle ct2,
        //                                                               List<ProportionalSegments> segmentPairs,
        //                                                               List<CongruentAngles> anglePairs,
        //                                                           out List<GroundedClause> facts)
        //{
        //    // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
        //    List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

        //    // Initialize congruences
        //    facts = new List<GroundedClause>();

        //    // Miniumum information required
        //    if (!anglePairs.Any() || segmentPairs.Count < 2) return pairs;

        //    // for each pair of Similar segments, is the given angle the included angle?
        //    for (int i = 0; i < segmentPairs.Count; i++)
        //    {
        //        for (int j = i + 1; j < segmentPairs.Count; j++)
        //        {
        //            if (segmentPairs[i].ProportionallyEquals(segmentPairs[j]))
        //            {
        //                // Check if any of the angles is an included angle
        //                foreach (CongruentAngles ccas in anglePairs)
        //                {
        //                    Segment seg1Tri1 = ct1.GetSegment(segmentPairs[i]);
        //                    Segment seg2Tri1 = ct1.GetSegment(segmentPairs[j]);

        //                    Segment seg1Tri2 = ct2.GetSegment(segmentPairs[i]);
        //                    Segment seg2Tri2 = ct2.GetSegment(segmentPairs[j]);

        //                    Angle angleTri1 = ct1.AngleBelongs(ccas);
        //                    Angle angleTri2 = ct2.AngleBelongs(ccas);

        //                    // Check both triangles if this is the included angle; if it is, we have SAS
        //                    if (angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
        //                    {
        //                        Point vertex1 = angleTri1.GetVertex();
        //                        Point vertex2 = angleTri2.GetVertex();

        //                        // The vertices of the angles correspond
        //                        pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

        //                        // For the segments, look at the congruences and select accordingly
        //                        pairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
        //                        pairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));

        //                        facts.Add(segmentPairs[i]);
        //                        facts.Add(segmentPairs[j]);
        //                        facts.Add(ccas);

        //                        return pairs;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return pairs;
        //}