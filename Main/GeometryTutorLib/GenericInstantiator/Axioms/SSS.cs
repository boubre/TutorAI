using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SSS : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SSS";

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentSegments> candidateSegments = new List<CongruentSegments>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateSegments.Clear();
            candidateTriangles.Clear();
        }

        //      A             D
        //      /\           / \
        //     /  \         /   \
        //    /    \       /     \
        //   /______\     /_______\
        //  B        C   E         F
        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Segment(A, C), Angle(D, F)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Angle(A, B, C), Angle(D, E, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is CongruentSegments)
            {
                CongruentSegments newCss = clause as CongruentSegments;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        for (int m = 0; m < candidateSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateSegments.Count; n++)
                            {
                                newGrounded.AddRange(InstantiateSSS(candidateTriangles[i], candidateTriangles[j], candidateSegments[m], candidateSegments[n], newCss));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegments.Add(newCss);
            }
            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                foreach (Triangle oldTri in candidateTriangles)
                {
                    for (int m = 0; m < candidateSegments.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateSegments.Count - 1; n++)
                        {
                            for (int p = n + 1; p < candidateSegments.Count - 2; p++)
                            {
                                newGrounded.AddRange(InstantiateSSS(newTriangle, oldTri, candidateSegments[m], candidateSegments[n], candidateSegments[p]));
                            }
                        }
                    }
                }

                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Checks for SSS given the 5 values
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateSSS(Triangle tri1, Triangle tri2,
                                                                                               CongruentSegments css1, CongruentSegments css2, CongruentSegments css3)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // All congruence pairs must minimally relate the triangles
            //
            if (!css1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css2.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css3.LinksTriangles(tri1, tri2)) return newGrounded;

            Segment seg1Tri1 = tri1.GetSegment(css1);
            Segment seg1Tri2 = tri2.GetSegment(css1);

            Segment seg2Tri1 = tri1.GetSegment(css2);
            Segment seg2Tri2 = tri2.GetSegment(css2);

            Segment seg3Tri1 = tri1.GetSegment(css3);
            Segment seg3Tri2 = tri2.GetSegment(css3);

            //
            // The vertices of both triangles must all be distinct and cover the triangle completely.
            //
            Point vertex1Tri1 = seg1Tri1.SharedVertex(seg2Tri1);
            Point vertex2Tri1 = seg2Tri1.SharedVertex(seg3Tri1);
            Point vertex3Tri1 = seg1Tri1.SharedVertex(seg3Tri1);

            if (vertex1Tri1 == null || vertex2Tri1 == null || vertex3Tri1 == null) return newGrounded;
            if (vertex1Tri1.StructurallyEquals(vertex2Tri1) ||
                vertex1Tri1.StructurallyEquals(vertex3Tri1) ||
                vertex2Tri1.StructurallyEquals(vertex3Tri1)) return newGrounded;

            Point vertex1Tri2 = seg1Tri2.SharedVertex(seg2Tri2);
            Point vertex2Tri2 = seg2Tri2.SharedVertex(seg3Tri2);
            Point vertex3Tri2 = seg1Tri2.SharedVertex(seg3Tri2);

            if (vertex1Tri2 == null || vertex2Tri2 == null || vertex3Tri2 == null) return newGrounded;
            if (vertex1Tri2.StructurallyEquals(vertex2Tri2) ||
                vertex1Tri2.StructurallyEquals(vertex3Tri2) ||
                vertex2Tri2.StructurallyEquals(vertex3Tri2)) return newGrounded;
            
            //
            // Construct the corresponding points between the triangles
            //
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();

            triangleOne.Add(vertex1Tri1);
            triangleTwo.Add(vertex1Tri2);

            triangleOne.Add(vertex2Tri1);
            triangleTwo.Add(vertex2Tri2);

            triangleOne.Add(vertex3Tri1);
            triangleTwo.Add(vertex3Tri2);

            //
            // Construct the new clauses: congruent triangles and CPCTC
            //
            GeometricCongruentTriangles gcts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri1);
            antecedent.Add(tri2);
            antecedent.Add(css1);
            antecedent.Add(css2);
            antecedent.Add(css3);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gcts));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(gcts, triangleOne, triangleTwo));

            return newGrounded;
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

//        // Resets all saved data.
//        public static void Clear()
//        {
//            unifyCandTris.Clear();
//            unifyCandAngles.Clear();
//            unifyCandSegments.Clear();
//        }

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

//                        //if (!ct1.WasDeducedCongruent(ct2))
//                        //{
//                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
//                            if (newCs.LinksTriangles(ct1, ct2))
//                            {
//                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCs));
//                            }
//                        //}
//                    }

//                    //// For Strengthened triangles
//                    //foreach (Strengthened streng in candidateStrengthened)
//                    //{
//                    //    Triangle strengTri = streng.strengthened as Triangle;

//                    //    if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
//                    //    {
//                    //        if (newCs.LinksTriangles(unifyCandTris[i], strengTri))
//                    //        {
//                    //            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                    //            applicSegments.Add(newCs);
//                    //            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                    //            newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
//                    //        }
//                    //    }
//                    //}
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

//                        //if (!ct1.WasDeducedCongruent(ct2))
//                        //{
//                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
//                            if (newCas.LinksTriangles(ct1, ct2))
//                            {
//                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCas));
//                            }
//                        //}
//                    }

//                    //// For Strengthened triangles
//                    //foreach (Strengthened streng in candidateStrengthened)
//                    //{
//                    //    Triangle strengTri = streng.strengthened as Triangle;

//                    //    if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
//                    //    {
//                    //        if (newCas.LinksTriangles(unifyCandTris[i], strengTri))
//                    //        {
//                    //            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                    //            List<CongruentAngles> applicAngles = new List<CongruentAngles>();
//                    //            applicAngles.Add(newCas);

//                    //            newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
//                    //        }
//                    //    }
//                    //}
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

//                //// For Strengthened triangles
//                //foreach (Strengthened streng in candidateStrengthened)
//                //{
//                //    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//                //    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//                //    newGrounded.AddRange(CollectAndCheckSAS(streng, candidateTri, applicSegments, applicAngles));
//                //}

//                // Add this triangle to the list of possible clauses to unify later
//                unifyCandTris.Add(candidateTri);
//            }
//            //else if (c is Strengthened)
//            //{
//            //    Strengthened streng = c as Strengthened;

//            //    if (!(streng.strengthened is Triangle)) return newGrounded;

//            //    // Remove the original triangle object from the candidate list
//            //    unifyCandTris.Remove(streng.original as Triangle);

//            //    foreach (Triangle ct in unifyCandTris)
//            //    {
//            //        List<CongruentSegments> applicSegments = new List<CongruentSegments>();
//            //        List<CongruentAngles> applicAngles = new List<CongruentAngles>();

//            //        newGrounded.AddRange(CollectAndCheckSAS(streng.strengthened as Triangle, ct, applicSegments, applicAngles));
//            //    }

//            //    // Add this triangle to the list of possible clauses to unify later
//            //    candidateStrengthened.Add(streng);
//            //}

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
//            //if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

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

//                GeometricCongruentTriangles ccts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

//                // Hypergraph
//                List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
//                antecedent.Add(generalTri);
//                antecedent.Add(ct2);

//                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

//                //
//                // We should not add any new congruences to the graph (thus creating cycles)
//                //
//                //List<KeyValuePair<List<GroundedClause>, GroundedClause>> cpctc = CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo);
//                //foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in cpctc)
//                //{
//                //    if (!congruences.Contains(part.Value))
//                //    {
//                //        newGrounded.Add(part);
//                //    }
//                //}

//                // Add all the corresponding parts as new congruent clauses
//                newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
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


////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using GeometryTutorLib.ConcreteAST;

////namespace GeometryTutorLib.GenericInstantiator
////{
////    public class SASCongruence : CongruentTriangleAxiom
////    {
////        private readonly static string NAME = "SAS";

////        private static List<Triangle> unifyCandTris = new List<Triangle>();
////        private static List<CongruentAngles> unifyCandAngles = new List<CongruentAngles>();
////        private static List<CongruentSegments> unifyCandSegments = new List<CongruentSegments>();
////        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

////        //
////        // In order for two triangles to be congruent, we require the following:
////        //    Triangle(A, B, C), Triangle(D, E, F),
////        //    Congruent(Segment(A, B), Segment(D, E)),
////        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
////        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
////        //                                               Congruent(Segment(A, C), Angle(D, F)),
////        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
////        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
////        //
////        // Note: we need to figure out the proper order of the sides to guarantee congruence
////        //
////        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
////        {
////            // Do we have a segment or triangle?
////            if (!(c is CongruentSegments) && !(c is CongruentAngles) && !(c is Triangle) && !(c is Strengthened))
////            {
////                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
////            }

////            //
////            // The list of new grounded clauses if they are deduced
////            //
////            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

////            // If this is a new segment, check for congruent triangles with this new piece of information
////            if (c is CongruentSegments)
////            {
////                CongruentSegments newCs = c as CongruentSegments;

////                // Check all combinations of triangles to see if they are congruent
////                // This congruence must include the new segment congruence
////                for (int i = 0; i < unifyCandTris.Count; i++)
////                {
////                    for (int j = i + 1; j < unifyCandTris.Count; j++)
////                    {
////                        Triangle ct1 = unifyCandTris[i];
////                        Triangle ct2 = unifyCandTris[j];

////                        if (!ct1.WasDeducedCongruent(ct2))
////                        {
////                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
////                            if (newCs.LinksTriangles(ct1, ct2))
////                            {
////                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCs));
////                            }
////                        }
////                    }

////                    // For Strengthened triangles
////                    foreach (Strengthened streng in candidateStrengthened)
////                    {
////                        Triangle strengTri = streng.strengthened as Triangle;

////                        if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
////                        {
////                            if (newCs.LinksTriangles(unifyCandTris[i], strengTri))
////                            {
////                                List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////                                applicSegments.Add(newCs);
////                                List<CongruentAngles> applicAngles = new List<CongruentAngles>();

////                                newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
////                            }
////                        }
////                    }
////                }

////                // Add this segment to the list of possible clauses to unify later
////                unifyCandSegments.Add(newCs);
////            }
////            else if (c is CongruentAngles)
////            {
////                CongruentAngles newCas = c as CongruentAngles;

////                // Check all combinations of triangles to see if they are congruent
////                // This congruence must include the new segment congruence
////                for (int i = 0; i < unifyCandTris.Count; i++)
////                {
////                    for (int j = i + 1; j < unifyCandTris.Count; j++)
////                    {
////                        Triangle ct1 = unifyCandTris[i];
////                        Triangle ct2 = unifyCandTris[j];

////                        if (!ct1.WasDeducedCongruent(ct2))
////                        {
////                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
////                            if (newCas.LinksTriangles(ct1, ct2))
////                            {
////                                newGrounded.AddRange(CollectAndCheckSAS(ct1, ct2, newCas));
////                            }
////                        }
////                    }

////                    // For Strengthened triangles
////                    foreach (Strengthened streng in candidateStrengthened)
////                    {
////                        Triangle strengTri = streng.strengthened as Triangle;

////                        if (!unifyCandTris[i].WasDeducedCongruent(strengTri))
////                        {
////                            if (newCas.LinksTriangles(unifyCandTris[i], strengTri))
////                            {
////                                List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////                                List<CongruentAngles> applicAngles = new List<CongruentAngles>();
////                                applicAngles.Add(newCas);

////                                newGrounded.AddRange(CollectAndCheckSAS(streng, unifyCandTris[i], applicSegments, applicAngles));
////                            }
////                        }
////                    }
////                }

////                // Add this segment to the list of possible clauses to unify later
////                unifyCandAngles.Add(newCas);
////            }

////            // If this is a new triangle, check for triangles which may be congruent to this new triangle
////            else if (c is Triangle)
////            {
////                Triangle candidateTri = (Triangle)c;
////                foreach (Triangle ct in unifyCandTris)
////                {
////                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

////                    newGrounded.AddRange(CollectAndCheckSAS(candidateTri, ct, applicSegments, applicAngles));
////                }

////                // For Strengthened triangles
////                foreach (Strengthened streng in candidateStrengthened)
////                {
////                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

////                    newGrounded.AddRange(CollectAndCheckSAS(streng, candidateTri, applicSegments, applicAngles));
////                }

////                // Add this triangle to the list of possible clauses to unify later
////                unifyCandTris.Add(candidateTri);
////            }
////            else if (c is Strengthened)
////            {
////                Strengthened streng = c as Strengthened;

////                if (!(streng.strengthened is Triangle)) return newGrounded;

////                // Remove the original triangle object from the candidate list
////                unifyCandTris.Remove(streng.original as Triangle);

////                foreach (Triangle ct in unifyCandTris)
////                {
////                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

////                    newGrounded.AddRange(CollectAndCheckSAS(streng.strengthened as Triangle, ct, applicSegments, applicAngles));
////                }

////                // Add this triangle to the list of possible clauses to unify later
////                candidateStrengthened.Add(streng);
////            }

////            return newGrounded;
////        }

////        //
////        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
////        //
////        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(GroundedClause generalTri, Triangle ct2,
////                                                                                                   List<CongruentSegments> applicSegments,
////                                                                                                   List<CongruentAngles> applicAngles)
////        {
////            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

////            Triangle ct1 = null;
////            if (generalTri is Strengthened)
////            {
////                ct1 = (generalTri as Strengthened).strengthened as Triangle;
////            }
////            else
////            {
////                ct1 = generalTri as Triangle;
////            }

////            // Has this congruence been established before? If so, do not deduce it again.
////            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

////            // Check all other segments
////            foreach (CongruentSegments ccs in unifyCandSegments)
////            {
////                // Does this segment link the two triangles?
////                if (ccs.LinksTriangles(ct1, ct2))
////                {
////                    applicSegments.Add(ccs);
////                }
////            }

////            // Check all Angles
////            foreach (CongruentAngles cca in unifyCandAngles)
////            {
////                // Do these angles link the two triangles?
////                if (cca.LinksTriangles(ct1, ct2))
////                {
////                    applicAngles.Add(cca);
////                }
////            }

////            List<GroundedClause> congruences;
////            List<KeyValuePair<Point, Point>> pairs = IsSASsituation(ct1, ct2, applicSegments, applicAngles, out congruences);

////            // If pairs is populated, we have a SAS situation
////            if (pairs.Any())
////            {
////                // Create the congruence between the triangles
////                List<Point> triangleOne = new List<Point>();
////                List<Point> triangleTwo = new List<Point>();
////                foreach (KeyValuePair<Point, Point> pair in pairs)
////                {
////                    triangleOne.Add(pair.Key);
////                    triangleTwo.Add(pair.Value);
////                }

////                // Indicate that these two triangles are congruent to avoid deducing this again later.
////                ct1.AddCongruentTriangle(ct2);
////                ct2.AddCongruentTriangle(ct1);

////                CongruentTriangles ccts = new CongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

////                // Hypergraph
////                List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
////                antecedent.Add(generalTri);
////                antecedent.Add(ct2);

////                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

////                //
////                // We should not add any new congruences to the graph (thus creating cycles)
////                //
////                List<KeyValuePair<List<GroundedClause>, GroundedClause>> cpctc = CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo);
////                foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in cpctc)
////                {
////                    if (!congruences.Contains(part.Value))
////                    {
////                        newGrounded.Add(part);
////                    }
////                }

////                // Add all the corresponding parts as new congruent clauses
////                //newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));
////            }

////            return newGrounded;
////        }

////        //
////        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
////        //
////        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckSAS(Triangle ct1, Triangle ct2, Congruent cc)
////        {
////            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
////            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

////            // Add the congruence statement to the appropriate list
////            if (cc is CongruentSegments)
////            {
////                applicSegments.Add(cc as CongruentSegments);
////            }
////            else
////            {
////                applicAngles.Add(cc as CongruentAngles);
////            }

////            return CollectAndCheckSAS(ct1, ct2, applicSegments, applicAngles);
////        }

////        //
////        // Is this a true SAS situation?
////        //
////        private static List<KeyValuePair<Point, Point>> IsSASsituation(Triangle ct1, Triangle ct2,
////                                      List<CongruentSegments> segmentPairs,
////                                      List<CongruentAngles> anglePairs,
////                                  out List<GroundedClause> congruences)
////        {
////            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
////            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

////            // Initialize congruences
////            congruences = new List<GroundedClause>();

////            // Miniumum information required
////            if (!anglePairs.Any() || segmentPairs.Count < 2) return pairs;

////            // for each pair of congruent segments, is the given angle the included angle?
////            for (int i = 0; i < segmentPairs.Count - 1; i++)
////            {
////                for (int j = i + 1; j < segmentPairs.Count; j++)
////                {
////                    // Check if any of the angles is an included angle
////                    foreach (CongruentAngles ccas in anglePairs)
////                    {
////                        Segment seg1Tri1 = ct1.GetSegment(segmentPairs[i]);
////                        Segment seg2Tri1 = ct1.GetSegment(segmentPairs[j]);

////                        Segment seg1Tri2 = ct2.GetSegment(segmentPairs[i]);
////                        Segment seg2Tri2 = ct2.GetSegment(segmentPairs[j]);

////                        // Segments must be distinct for the two triangles
////                        if (!seg1Tri1.Equals(seg2Tri1) && !seg1Tri2.Equals(seg2Tri2))
////                        {
////                            Angle angleTri1 = ct1.NormalizeAngle(ct1.AngleBelongs(ccas));
////                            Angle angleTri2 = ct2.NormalizeAngle(ct2.AngleBelongs(ccas));

////                            // Check both triangles if this is the included angle; if it is, we have SAS
////                            if (angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) && angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2))
////                            {
////                                Point vertex1 = angleTri1.GetVertex();
////                                Point vertex2 = angleTri2.GetVertex();

////                                // The vertices of the angles correspond
////                                pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

////                                // For the segments, look at the congruences and select accordingly
////                                pairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.OtherPoint(vertex1), seg1Tri2.OtherPoint(vertex2)));
////                                pairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.OtherPoint(vertex1), seg2Tri2.OtherPoint(vertex2)));

////                                congruences.Add(segmentPairs[i]);
////                                congruences.Add(segmentPairs[j]);
////                                congruences.Add(ccas);

////                                return pairs;
////                            }
////                        }
////                    }
////                }
////            }

////            return pairs;
////        }
////    }
////}




//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;
//using GeometryTutorLib.ConcreteAST;

//namespace GeometryTutorLib.GenericInstantiator
//{
//    public class SSS : CongruentTriangleAxiom
//    {
//        private readonly static string NAME = "SSS";

//        private static List<Triangle> candidateTriangles = new List<Triangle>();
//        private static List<CongruentSegments> candidateCongruences = new List<CongruentSegments>();

//        // Resets all saved data.
//        public static void Clear()
//        {
//            candidateTriangles.Clear();
//            candidateCongruences.Clear();
//        }

//        //
//        // In order for two triangles to be congruent, we require the following:
//        //    Triangle(A, B, C), Triangle(D, E, F),
//        //    Congruent(Segment(A, B), Segment(D, E)),
//        //    Congruent(Segment(A, C), Segment(D, F)),
//        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
//        //                                               Congruent(Angles(A, B, C), Angle(D, E, F)),
//        //                                               Congruent(Angles(C, A, B), Angle(F, D, E)),
//        //                                               Congruent(Angles(B, C, A), Angle(E, F, D)),
//        //
//        // Note: we need to figure out the proper order of the sides to guarantee congruence
//        //
//        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
//        {
//            // Do we have a segment or triangle?
//            if (!(c is CongruentSegments) && !(c is Triangle)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

//            //
//            // Do we have enough information for unification?
//            //
//            if (c is CongruentSegments && (candidateCongruences.Count < 2 || candidateTriangles.Count <= 1))
//            {
//                candidateCongruences.Add((CongruentSegments)c);
//                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
//            }
//            else if (c is Triangle && (!candidateTriangles.Any() || candidateCongruences.Count < 3))
//            {
//                candidateTriangles.Add((Triangle)c);
//                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
//            }

//            // The list of new grounded clauses if they are deduced
//            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

//            // If this is a new segment, check for congruent triangles with this new piece of information
//            if (c is CongruentSegments)
//            {
//                CongruentSegments newCs = (CongruentSegments)c;

//                // Check all combinations of triangles to see if they are congruent
//                // This congruence must include the new segment congruence
//                for (int i = 0; i < candidateTriangles.Count; i++)
//                {
//                    for (int j = i + 1; j < candidateTriangles.Count; j++)
//                    {
//                        Triangle ct1 = candidateTriangles[i];
//                        Triangle ct2 = candidateTriangles[j];

//                        if (!ct1.WasDeducedCongruent(ct2))
//                        {
//                            List<CongruentSegments> applicCongruents = new List<CongruentSegments>();

//                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
//                            if (newCs.LinksTriangles(ct1, ct2))
//                            {
//                                applicCongruents.Add(newCs);

//                                // Check all other segments
//                                foreach (CongruentSegments ccs in candidateCongruences)
//                                {
//                                    // Does this segment link the two triangles?
//                                    if (ccs.LinksTriangles(ct1, ct2))
//                                    {
//                                        applicCongruents.Add(ccs);
//                                    }
//                                }

//                                // Generate the new clause
//                                List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = CheckForSSS(ct1, ct2, applicCongruents);
//                                newGrounded.AddRange(newG);
//                            }
//                        }
//                    }
//                }

//                // Add this segment to the list of possible clauses to unify later
//                candidateCongruences.Add(newCs);
//            }
//            // If this is a new triangle, check for triangles which may be congruent to this new triangle
//            else if (c is Triangle)
//            {
//                Triangle candidateTri = (Triangle)c;
//                foreach (Triangle ct in candidateTriangles)
//                {
//                    //
//                    // Is this concrete triangle congruent to the new candidate?
//                    //
//                    // Find all applicable congruent segments for both triangles
//                    List<CongruentSegments> applicCongruents = new List<CongruentSegments>();
//                    foreach (CongruentSegments ccs in candidateCongruences)
//                    {
//                        // Does this segment link the two triangles?
//                        if (ccs.LinksTriangles(ct, candidateTri))
//                        {
//                            applicCongruents.Add(ccs);
//                        }
//                    }

//                    // Generate the new clause
//                    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = CheckForSSS(ct, (Triangle)c, applicCongruents);
//                    newGrounded.AddRange(newG);
//                }

//                // Add this triangle to the list of possible clauses to unify later
//                candidateTriangles.Add(candidateTri);
//            }

//            return newGrounded;
//        }

//        //
//        // Given these two triangles and the set of 3 congruent segment pairs, is this a complete SSS?
//        //
//        private static bool IsSpecificSSS(Triangle ct1, Triangle ct2, List<CongruentSegments> segmentPairs)
//        {
//            List<Segment> triangleOneSegments = new List<Segment>();
//            List<Segment> triangleTwoSegments = new List<Segment>();

//            // For each congruent pair, if the segments in question are unique
//            foreach (CongruentSegments ccss in segmentPairs)
//            {
//                Segment triOneSeg = ct1.GetSegment(ccss);
//                Segment triTwoSeg = ct2.GetSegment(ccss);

//                // If a side of a triangle is already in the list, this will not create a true SSS scenario
//                if (triangleOneSegments.Contains(triOneSeg) || triangleTwoSegments.Contains(triTwoSeg)) return false;

//                triangleOneSegments.Add(triOneSeg);
//                triangleTwoSegments.Add(triTwoSeg);
//            }

//            // They must each contain 3 segments to account for all 3 sides of the triangle
//            return triangleOneSegments.Count == 3 && triangleTwoSegments.Count == 3;
//        }

//        //
//        // Of all the congruent segment pairs, choose a subset of 3. Exhaustively check all; if they work, return the set.
//        //
//        private static List<CongruentSegments> IsTrueSSS(Triangle ct1, Triangle ct2, List<CongruentSegments> conSegments)
//        {
//            List<CongruentSegments> subset;

//            // construct the subset in a hack way using 3 loops to guarantee we generate ALL subsets exhaustively
//            for (int one = 0; one < conSegments.Count; one++)
//            {
//                for (int two = one + 1; two < conSegments.Count; two++)
//                {
//                    subset = new List<CongruentSegments>();
//                    for (int three = two + 1; three < conSegments.Count; three++)
//                    {
//                        subset.Add(conSegments[one]);
//                        subset.Add(conSegments[two]);
//                        subset.Add(conSegments[three]);

//                        if (IsSpecificSSS(ct1, ct2, subset)) return subset;
//                    }
//                }
//            }

//            return null;
//        }

//        //
//        // Return all the corresponing points from the two triangles and Congruence Pairs
//        //
//        private static List<KeyValuePair<Point, Point>> MakePointPairs(Triangle ct1, Triangle ct2, List<CongruentSegments> congruentPairs)
//        {
//            List<KeyValuePair<Point, Point>> pointPairs = new List<KeyValuePair<Point, Point>>();

//            // we could write this without loops, but loops allow extension to more sides (if we have a generalpolygon)

//            // Take all combinations of Congruent Segment Pairs and find the common vertex in both triangles; those are the corresponding points
//            for (int i = 0; i < congruentPairs.Count - 1; i++)
//            {
//                for (int j = i + 1; j < congruentPairs.Count; j++)
//                {
//                    Segment triOneSeg1 = ct1.GetSegment(congruentPairs[i]);
//                    Segment triTwoSeg1 = ct2.GetSegment(congruentPairs[i]);

//                    Segment triOneSeg2 = ct1.GetSegment(congruentPairs[j]);
//                    Segment triTwoSeg2 = ct2.GetSegment(congruentPairs[j]);

//                    pointPairs.Add(new KeyValuePair<Point, Point>(triOneSeg1.SharedVertex(triOneSeg2), triTwoSeg1.SharedVertex(triTwoSeg2)));
//                }
//            }

//            return pointPairs;
//        }

//        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckForSSS(Triangle ct1, Triangle ct2, List<CongruentSegments> conSegments)
//        {
//            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

//            // Has this congruence been established before? If so, do not deduce it again.
//            // CTA: Is this a problem if a triangle can be found congruent in multiple ways?
//            // That is, establish congruence, but do not generate?
//            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

//            // We need at least 3 congruent segments between the two triangles
//            if (conSegments.Count < 3) return newGrounded;


//            //
//            // For debug only
//            //
//            //Debug.WriteLine("SSS In Triangles: " + ct1 + ", " + ct2);
//            //foreach (GroundedClause gc in conSegments)
//            //{
//            //    Debug.WriteLine("\t\t" + gc.ToString());
//            //}
//            // End debug only


//            // Acquire the 3 actual congruent pairs that make this SSS
//            List<CongruentSegments> congruencePairs = IsTrueSSS(ct1, ct2, conSegments);

//            // Not SSS if this is true
//            if (congruencePairs == null) return newGrounded;

//            List<KeyValuePair<Point, Point>> correspondingVertices = MakePointPairs(ct1, ct2, congruencePairs);

//            // Create the congruence between the triangles
//            List<Point> triangleOne = new List<Point>();
//            List<Point> triangleTwo = new List<Point>();
//            foreach (KeyValuePair<Point, Point> pair in correspondingVertices)
//            {
//                triangleOne.Add(pair.Key);
//                triangleTwo.Add(pair.Value);
//            }

//            // Indicate that these two triangles are congruent to avoid deducing this again later.
//            ct1.AddCongruentTriangle(ct2);
//            ct2.AddCongruentTriangle(ct1);

//            GeometricCongruentTriangles ccts = new GeometricCongruentTriangles(new Triangle(triangleOne),
//                                                                               new Triangle(triangleTwo), NAME);

//            // Hypergraph
//            List<GroundedClause> antecedent = new List<GroundedClause>();
//            foreach (CongruentSegments ccss in congruencePairs) { antecedent.Add(ccss); }
//            antecedent.Add(ct1);
//            antecedent.Add(ct2);

//            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

//            // Add all the corresponding parts as new congruent clauses
//            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));

//            return newGrounded;
//        }

//        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateAllAngleClauses(CongruentTriangles ccts, GroundedClause[] facts, string NAME)
//        {
//            //
//            // Is this triangle reflexively congruent; all three sides shared?
//            //
//            if (ccts.ct1.Equals(ccts.ct2))
//            {
//                // Generate no new information
//                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
//            }

//            //
//            // This map will contain all points in Triangle 1 which corresponsd directly to the points of Triangle 2
//            //
//            List<Point> orderedTriOnePts = new List<Point>();
//            List<Point> orderedTriTwoPts = new List<Point>();

//            //
//            // Is there one shared side?
//            // This is a parallelogram with one vertex on each triangle not on the shared segment.
//            // And we are guaranteed the folowing pattern:
//            //
//            //     A----------B
//            //    /          /        Here, shared is AD
//            //   /          /
//            //  C----------D
//            //
//            // Triangle ABD \cong
//            // Triangle DCA
//            Segment cs = ccts.ct1.SharesSide(ccts.ct2);
//            if (cs != null)
//            {
//                orderedTriOnePts.Add(cs.Point1);
//                orderedTriTwoPts.Add(cs.Point2);
//                orderedTriOnePts.Add(cs.Point2);
//                orderedTriTwoPts.Add(cs.Point1);

//                // Seek the remaining point in each triangle
//                orderedTriOnePts.Add(ccts.ct1.OtherPoint(cs));
//                orderedTriTwoPts.Add(ccts.ct2.OtherPoint(cs));
//            }
//            else
//            {
//                //
//                // There are no shared sides (although a point may be shared)
//                //

//                // Create two lists of three points from each triangle
//                List<Point> triangleOne = ccts.ct1.GetPoints();
//                List<Point> triangleTwo = ccts.ct2.GetPoints();

//                // Construct the map
//                for (int i = 0; i < facts.Length; i++)
//                {
//                    KeyValuePair<Point, Point> pair = GeneratePair((CongruentSegments)facts[i],
//                                                                                   (CongruentSegments)facts[i + 1 < facts.Length ? i + 1 : 0]);

//                    Point p = pair.Key;
//                    if (triangleOne.Contains(p))
//                    {
//                        orderedTriOnePts.Add(pair.Key);
//                        orderedTriTwoPts.Add(pair.Value);
//                    }
//                    else
//                    {
//                        orderedTriOnePts.Add(pair.Value);
//                        orderedTriTwoPts.Add(pair.Key);
//                    }
//                }
//            }

//            return CongruentTriangles.GenerateCPCTC(ccts, orderedTriOnePts, orderedTriTwoPts);
//        }

//        private static KeyValuePair<Point, Point> GeneratePair(CongruentSegments s1, CongruentSegments s2)
//        {
//            Point vertexOne = s1.GetFirstSegment().SharedVertex(s2.GetFirstSegment());
//            vertexOne = vertexOne == null ? s1.GetSecondSegment().SharedVertex(s2.GetFirstSegment()) : vertexOne;

//            Point vertexTwo = s2.GetSecondSegment().SharedVertex(s1.GetFirstSegment());
//            vertexTwo = vertexTwo == null ? s2.GetSecondSegment().SharedVertex(s1.GetSecondSegment()) : vertexTwo;

//            return new KeyValuePair<Point, Point>(vertexOne, vertexTwo);
//        }
//    }
//}