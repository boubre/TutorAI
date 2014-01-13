using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SASCongruence : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SAS";

        private static List<Triangle> unifyCandTris = new List<Triangle>();
        private static List<CongruentAngles> unifyCandAngles = new List<CongruentAngles>();
        private static List<CongruentSegments> unifyCandSegments = new List<CongruentSegments>();

        // Resets all saved data.
        public static void Clear()
        {
            unifyCandAngles.Clear();
            unifyCandAngles.Clear();
            unifyCandSegments.Clear();
        }

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
            if (!(c is CongruentSegments) && !(c is CongruentAngles) && !(c is Triangle) && !(c is Strengthened))
            {
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            //
            // The list of new grounded clauses if they are deduced
            //
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (c is CongruentSegments)
            {
                CongruentSegments newCs = c as CongruentSegments;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < unifyCandTris.Count; i++)
                {
                    for (int j = i + 1; j < unifyCandTris.Count; j++)
                    {
                        Triangle ct1 = unifyCandTris[i];
                        Triangle ct2 = unifyCandTris[j];

                        //if (!ct1.WasDeducedCongruent(ct2))
                        //{
                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                            if (newCs.LinksTriangles(ct1, ct2))
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCs));
                            }
                        //}
                    }

                    //// For Strengthened triangles
                    //foreach (Strengthened streng in candidateStrengthened)
                    //{
                    //    Triangle strengTri = streng.strengthened as Triangle;

                    //    if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
                    //    {
                    //        if (newCs.LinksTriangles(unifyCandTris[i], strengTri))
                    //        {
                    //            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
                    //            applicSegments.Add(newCs);
                    //            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

                    //            newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
                    //        }
                    //    }
                    //}
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandSegments.Add(newCs);
            }
            else if (c is CongruentAngles)
            {
                CongruentAngles newCas = c as CongruentAngles;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < unifyCandTris.Count; i++)
                {
                    for (int j = i + 1; j < unifyCandTris.Count; j++)
                    {
                        Triangle ct1 = unifyCandTris[i];
                        Triangle ct2 = unifyCandTris[j];

                        //if (!ct1.WasDeducedCongruent(ct2))
                        //{
                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                            if (newCas.LinksTriangles(ct1, ct2))
                            {
                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCas));
                            }
                        //}
                    }

                    //// For Strengthened triangles
                    //foreach (Strengthened streng in candidateStrengthened)
                    //{
                    //    Triangle strengTri = streng.strengthened as Triangle;

                    //    if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
                    //    {
                    //        if (newCas.LinksTriangles(unifyCandTris[i], strengTri))
                    //        {
                    //            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
                    //            List<CongruentAngles> applicAngles = new List<CongruentAngles>();
                    //            applicAngles.Add(newCas);

                    //            newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
                    //        }
                    //    }
                    //}
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandAngles.Add(newCas);
            }

            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (c is Triangle)
            {
                Triangle candidateTri = (Triangle)c;
                foreach (Triangle ct in unifyCandTris)
                {
                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

                    newGrounded.AddRange(CollectAndCheckSAS(candidateTri, ct, applicSegments, applicAngles));
                }

                //// For Strengthened triangles
                //foreach (Strengthened streng in candidateStrengthened)
                //{
                //    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
                //    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

                //    newGrounded.AddRange(CollectAndCheckSAS(streng, candidateTri, applicSegments, applicAngles));
                //}

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }
            //else if (c is Strengthened)
            //{
            //    Strengthened streng = c as Strengthened;

            //    if (!(streng.strengthened is Triangle)) return newGrounded;

            //    // Remove the original triangle object from the candidate list
            //    unifyCandTris.Remove(streng.original as Triangle);

            //    foreach (Triangle ct in unifyCandTris)
            //    {
            //        List<CongruentSegments> applicSegments = new List<CongruentSegments>();
            //        List<CongruentAngles> applicAngles = new List<CongruentAngles>();

            //        newGrounded.AddRange(CollectAndCheckSAS(streng.strengthened as Triangle, ct, applicSegments, applicAngles));
            //    }

            //    // Add this triangle to the list of possible clauses to unify later
            //    candidateStrengthened.Add(streng);
            //}

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(GroundedClause generalTri, Triangle ct2,
                                                                                                   List<CongruentSegments> applicSegments,
                                                                                                   List<CongruentAngles> applicAngles)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Triangle ct1 = null;
            if (generalTri is Strengthened)
            {
                ct1 = (generalTri as Strengthened).strengthened as Triangle;
            }
            else
            {
                ct1 = generalTri as Triangle;
            }

            // Has this congruence been established before? If so, do not deduce it again.
            //if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

            // Check all other segments
            foreach (CongruentSegments ccs in unifyCandSegments)
            {
                // Does this segment link the two triangles?
                if (ccs.LinksTriangles(ct1, ct2))
                {
                    applicSegments.Add(ccs);
                }
            }

            // Check all Angles
            foreach (CongruentAngles cca in unifyCandAngles)
            {
                // Do these angles link the two triangles?
                if (cca.LinksTriangles(ct1, ct2))
                {
                    applicAngles.Add(cca);
                }
            }

            List<GroundedClause> congruences;
            List<KeyValuePair<Point, Point>> pairs = IsSASsituation(ct1, ct2, applicSegments, applicAngles, out congruences);

            // If pairs is populated, we have a SAS situation
            if (pairs.Any())
            {
                // Create the congruence between the triangles
                List<Point> triangleOne = new List<Point>();
                List<Point> triangleTwo = new List<Point>();
                foreach (KeyValuePair<Point, Point> pair in pairs)
                {
                    triangleOne.Add(pair.Key);
                    triangleTwo.Add(pair.Value);
                }

                // Indicate that these two triangles are congruent to avoid deducing this again later.
                ct1.AddCongruentTriangle(ct2);
                ct2.AddCongruentTriangle(ct1);

                GeometricCongruentTriangles ccts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

                // Hypergraph
                List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
                antecedent.Add(generalTri);
                antecedent.Add(ct2);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

                //
                // We should not add any new congruences to the graph (thus creating cycles)
                //
                //List<KeyValuePair<List<GroundedClause>, GroundedClause>> cpctc = CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo);
                //foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in cpctc)
                //{
                //    if (!congruences.Contains(part.Value))
                //    {
                //        newGrounded.Add(part);
                //    }
                //}

                // Add all the corresponding parts as new congruent clauses
                newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(Triangle ct1, Triangle ct2, Congruent cc)
        {
            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

            // Add the congruence statement to the appropriate list
            if (cc is CongruentSegments)
            {
                applicSegments.Add(cc as CongruentSegments);
            }
            else
            {
                applicAngles.Add(cc as CongruentAngles);
            }

            return CollectAndCheckSAS(ct1, ct2, applicSegments, applicAngles);
        }

        //
        // Is this a true SAS situation?
        //
        private static List<KeyValuePair<Point, Point>> IsSASsituation(Triangle ct1, Triangle ct2,
                                      List<CongruentSegments> segmentPairs,
                                      List<CongruentAngles> anglePairs,
                                  out List<GroundedClause> congruences)
        {
            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

            // Initialize congruences
            congruences = new List<GroundedClause>();

            // Miniumum information required
            if (!anglePairs.Any() || segmentPairs.Count < 2) return pairs;

            // for each pair of congruent segments, is the given angle the included angle?
            for (int i = 0; i < segmentPairs.Count - 1; i++)
            {
                for (int j = i + 1; j < segmentPairs.Count; j++)
                {
                    // Check if any of the angles is an included angle
                    foreach (CongruentAngles ccas in anglePairs)
                    {
                        Segment seg1Tri1 = ct1.GetSegment(segmentPairs[i]);
                        Segment seg2Tri1 = ct1.GetSegment(segmentPairs[j]);

                        Segment seg1Tri2 = ct2.GetSegment(segmentPairs[i]);
                        Segment seg2Tri2 = ct2.GetSegment(segmentPairs[j]);

                        // Segments must be distinct for the two triangles
                        if (!seg1Tri1.Equals(seg2Tri1) && !seg1Tri2.Equals(seg2Tri2))
                        {
                            Angle angleTri1 = ct1.NormalizeAngle(ct1.AngleBelongs(ccas));
                            Angle angleTri2 = ct2.NormalizeAngle(ct2.AngleBelongs(ccas));

                            // Check both triangles if this is the included angle; if it is, we have SAS
                            if (angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
                            {
                                Point vertex1 = angleTri1.GetVertex();
                                Point vertex2 = angleTri2.GetVertex();

                                // The vertices of the angles correspond
                                pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

                                // For the segments, look at the congruences and select accordingly
                                pairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
                                pairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));

                                congruences.Add(segmentPairs[i]);
                                congruences.Add(segmentPairs[j]);
                                congruences.Add(ccas);

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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using GeometryTutorLib.ConcreteAST;

//namespace GeometryTutorLib.GenericInstantiator
//{
//    public class SASCongruence : CongruentTriangleAxiom
//    {
//        private readonly static string NAME = "SAS";

//        private static List<Triangle> unifyCandTris = new List<Triangle>();
//        private static List<CongruentAngles> unifyCandAngles = new List<CongruentAngles>();
//        private static List<CongruentSegments> unifyCandSegments = new List<CongruentSegments>();
//        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

//        //
//        // In order for two triangles to be congruent, we require the following:
//        //    Triangle(A, B, C), Triangle(D, E, F),
//        //    Congruent(Segment(A, B), Segment(D, E)),
//        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
//        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
//        //                                               Congruent(Segment(A, C), Angle(D, F)),
//        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
//        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
//        //
//        // Note: we need to figure out the proper order of the sides to guarantee congruence
//        //
//        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
//        {
//            // Do we have a segment or triangle?
//            if (!(c is CongruentSegments) && !(c is CongruentAngles) && !(c is Triangle) && !(c is Strengthened))
//            {
//                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
//            }

//            //
//            // The list of new grounded clauses if they are deduced
//            //
//            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

//            // If this is a new segment, check for congruent triangles with this new piece of information
//            if (c is CongruentSegments)
//            {
//                CongruentSegments newCs = c as CongruentSegments;

//                // Check all combinations of triangles to see if they are congruent
//                // This congruence must include the new segment congruence
//                for (int i = 0; i < unifyCandTris.Count; i++)
//                {
//                    for (int j = i + 1; j < unifyCandTris.Count; j++)
//                    {
//                        Triangle ct1 = unifyCandTris[i];
//                        Triangle ct2 = unifyCandTris[j];

//                        if (!ct1.WasDeducedCongruent(ct2))
//                        {
//                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
//                            if (newCs.LinksTriangles(ct1, ct2))
//                            {
//                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCs));
//                            }
//                        }
//                    }

//                    // For Strengthened triangles
//                    foreach (Strengthened streng in candidateStrengthened)
//                    {
//                        Triangle strengTri = streng.strengthened as Triangle;

//                        if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
//                        {
//                            if (newCs.LinksTriangles(unifyCandTris[i], strengTri))
//                            {
//                                List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                                applicSegments.Add(newCs);
//                                List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                                newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
//                            }
//                        }
//                    }
//                }

//                // Add this segment to the list of possible clauses to unify later
//                unifyCandSegments.Add(newCs);
//            }
//            else if (c is CongruentAngles)
//            {
//                CongruentAngles newCas = c as CongruentAngles;

//                // Check all combinations of triangles to see if they are congruent
//                // This congruence must include the new segment congruence
//                for (int i = 0; i < unifyCandTris.Count; i++)
//                {
//                    for (int j = i + 1; j < unifyCandTris.Count; j++)
//                    {
//                        Triangle ct1 = unifyCandTris[i];
//                        Triangle ct2 = unifyCandTris[j];

//                        if (!ct1.WasDeducedCongruent(ct2))
//                        {
//                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
//                            if (newCas.LinksTriangles(ct1, ct2))
//                            {
//                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCas));
//                            }
//                        }
//                    }

//                    // For Strengthened triangles
//                    foreach (Strengthened streng in candidateStrengthened)
//                    {
//                        Triangle strengTri = streng.strengthened as Triangle;

//                        if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
//                        {
//                            if (newCas.LinksTriangles(unifyCandTris[i], strengTri))
//                            {
//                                List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                                List<CongruentAngles> applicAngles = new List<CongruentAngles>();
//                                applicAngles.Add(newCas);

//                                newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
//                            }
//                        }
//                    }
//                }

//                // Add this segment to the list of possible clauses to unify later
//                unifyCandAngles.Add(newCas);
//            }

//            // If this is a new triangle, check for triangles which may be congruent to this new triangle
//            else if (c is Triangle)
//            {
//                Triangle candidateTri = (Triangle)c;
//                foreach (Triangle ct in unifyCandTris)
//                {
//                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                    newGrounded.AddRange(CollectAndCheckSAS(candidateTri, ct, applicSegments, applicAngles));
//                }

//                // For Strengthened triangles
//                foreach (Strengthened streng in candidateStrengthened)
//                {
//                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                    newGrounded.AddRange(CollectAndCheckSAS(streng, candidateTri, applicSegments, applicAngles));
//                }

//                // Add this triangle to the list of possible clauses to unify later
//                unifyCandTris.Add(candidateTri);
//            }
//            else if (c is Strengthened)
//            {
//                Strengthened streng = c as Strengthened;

//                if (!(streng.strengthened is Triangle)) return newGrounded;

//                // Remove the original triangle object from the candidate list
//                unifyCandTris.Remove(streng.original as Triangle);

//                foreach (Triangle ct in unifyCandTris)
//                {
//                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                    newGrounded.AddRange(CollectAndCheckSAS(streng.strengthened as Triangle, ct, applicSegments, applicAngles));
//                }

//                // Add this triangle to the list of possible clauses to unify later
//                candidateStrengthened.Add(streng);
//            }

//            return newGrounded;
//        }

//        //
//        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
//        //
//        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(GroundedClause generalTri, Triangle ct2,
//                                                                                                   List<CongruentSegments> applicSegments,
//                                                                                                   List<CongruentAngles> applicAngles)
//        {
//            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

//            Triangle ct1 = null;
//            if (generalTri is Strengthened)
//            {
//                ct1 = (generalTri as Strengthened).strengthened as Triangle;
//            }
//            else
//            {
//                ct1 = generalTri as Triangle;
//            }

//            // Has this congruence been established before? If so, do not deduce it again.
//            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

//            // Check all other segments
//            foreach (CongruentSegments ccs in unifyCandSegments)
//            {
//                // Does this segment link the two triangles?
//                if (ccs.LinksTriangles(ct1, ct2))
//                {
//                    applicSegments.Add(ccs);
//                }
//            }

//            // Check all Angles
//            foreach (CongruentAngles cca in unifyCandAngles)
//            {
//                // Do these angles link the two triangles?
//                if (cca.LinksTriangles(ct1, ct2))
//                {
//                    applicAngles.Add(cca);
//                }
//            }

//            List<GroundedClause> congruences;
//            List<KeyValuePair<Point, Point>> pairs = IsSASsituation(ct1, ct2, applicSegments, applicAngles, out congruences);

//            // If pairs is populated, we have a SAS situation
//            if (pairs.Any())
//            {
//                // Create the congruence between the triangles
//                List<Point> triangleOne = new List<Point>();
//                List<Point> triangleTwo = new List<Point>();
//                foreach (KeyValuePair<Point, Point> pair in pairs)
//                {
//                    triangleOne.Add(pair.Key);
//                    triangleTwo.Add(pair.Value);
//                }

//                // Indicate that these two triangles are congruent to avoid deducing this again later.
//                ct1.AddCongruentTriangle(ct2);
//                ct2.AddCongruentTriangle(ct1);

//                CongruentTriangles ccts = new CongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

//                // Hypergraph
//                List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
//                antecedent.Add(generalTri);
//                antecedent.Add(ct2);

//                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

//                //
//                // We should not add any new congruences to the graph (thus creating cycles)
//                //
//                List<KeyValuePair<List<GroundedClause>, GroundedClause>> cpctc = CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo);
//                foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in cpctc)
//                {
//                    if (!congruences.Contains(part.Value))
//                    {
//                        newGrounded.Add(part);
//                    }
//                }

//                // Add all the corresponding parts as new congruent clauses
//                //newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
//            }

//            return newGrounded;
//        }

//        //
//        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
//        //
//        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(Triangle ct1, Triangle ct2, Congruent cc)
//        {
//            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//            // Add the congruence statement to the appropriate list
//            if (cc is CongruentSegments)
//            {
//                applicSegments.Add(cc as CongruentSegments);
//            }
//            else
//            {
//                applicAngles.Add(cc as CongruentAngles);
//            }

//            return CollectAndCheckSAS(ct1, ct2, applicSegments, applicAngles);
//        }

//        //
//        // Is this a true SAS situation?
//        //
//        private static List<KeyValuePair<Point, Point>> IsSASsituation(Triangle ct1, Triangle ct2,
//                                      List<CongruentSegments> segmentPairs,
//                                      List<CongruentAngles> anglePairs,
//                                  out List<GroundedClause> congruences)
//        {
//            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
//            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

//            // Initialize congruences
//            congruences = new List<GroundedClause>();

//            // Miniumum information required
//            if (!anglePairs.Any() || segmentPairs.Count < 2) return pairs;

//            // for each pair of congruent segments, is the given angle the included angle?
//            for (int i = 0; i < segmentPairs.Count - 1; i++)
//            {
//                for (int j = i + 1; j < segmentPairs.Count; j++)
//                {
//                    // Check if any of the angles is an included angle
//                    foreach (CongruentAngles ccas in anglePairs)
//                    {
//                        Segment seg1Tri1 = ct1.GetSegment(segmentPairs[i]);
//                        Segment seg2Tri1 = ct1.GetSegment(segmentPairs[j]);

//                        Segment seg1Tri2 = ct2.GetSegment(segmentPairs[i]);
//                        Segment seg2Tri2 = ct2.GetSegment(segmentPairs[j]);

//                        // Segments must be distinct for the two triangles
//                        if (!seg1Tri1.Equals(seg2Tri1) && !seg1Tri2.Equals(seg2Tri2))
//                        {
//                            Angle angleTri1 = ct1.NormalizeAngle(ct1.AngleBelongs(ccas));
//                            Angle angleTri2 = ct2.NormalizeAngle(ct2.AngleBelongs(ccas));

//                            // Check both triangles if this is the included angle; if it is, we have SAS
//                            if (angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
//                            {
//                                Point vertex1 = angleTri1.GetVertex();
//                                Point vertex2 = angleTri2.GetVertex();

//                                // The vertices of the angles correspond
//                                pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

//                                // For the segments, look at the congruences and select accordingly
//                                pairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
//                                pairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));

//                                congruences.Add(segmentPairs[i]);
//                                congruences.Add(segmentPairs[j]);
//                                congruences.Add(ccas);

//                                return pairs;
//                            }
//                        }
//                    }
//                }
//            }

//            return pairs;
//        }
//    }
//}