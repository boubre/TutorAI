using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SSSSimilarity : Theorem
    {
        private readonly static string NAME = "SSS Similarity";

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<ProportionalSegments> candidateSegments = new List<ProportionalSegments>();

        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    ProportionalSegments(Segment(A, B), Segment(D, E)),
        //    ProportionalSegments(Segment(A, C), Segment(D, F)),
        //    ProportionalSegments(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                          Congruent(Angles(A, B, C), Angle(D, E, F)),
        //                                                          Congruent(Angles(C, A, B), Angle(F, D, E)),
        //                                                          Congruent(Angles(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Do we have a segment or triangle?
            if (!(c is ProportionalSegments) && !(c is Triangle)) return newGrounded;

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (c is ProportionalSegments)
            {
                ProportionalSegments newProp = c as ProportionalSegments;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        for (int m = 0; m < candidateSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateSegments.Count; n++)
                            {
                                newGrounded.AddRange(CheckForSSS(candidateTriangles[i], candidateTriangles[j], newProp, candidateSegments[m], candidateSegments[n]));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegments.Add(newProp);
            }
            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (c is Triangle)
            {
                Triangle candidateTri = c as Triangle;

                for (int i = 0; i < candidateTriangles.Count; i++)
                {
                    for (int m = 0; m < candidateSegments.Count - 2; m++)
                    {
                        for (int n = m + 1; n < candidateSegments.Count - 1; n++)
                        {
                            for (int p = n + 1; p < candidateSegments.Count; p++)
                            {
                                newGrounded.AddRange(CheckForSSS(candidateTri, candidateTriangles[i], candidateSegments[m], candidateSegments[n], candidateSegments[p]));
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
        // Of all the congruent segment pairs, choose a subset of 3. Exhaustively check all; if they work, return the set.
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckForSSS(Triangle ct1, Triangle ct2, ProportionalSegments pss1, ProportionalSegments pss2, ProportionalSegments pss3)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Has this similarity or congruence been established before? If so, do not deduce it again.
            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;
            if (ct1.HasEstablishedSimilarity(ct2) || ct2.HasEstablishedSimilarity(ct1)) return newGrounded;

            //
            // The proportional relationships need to link the given triangles
            //
            if (!pss1.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!pss2.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!pss3.LinksTriangles(ct1, ct2)) return newGrounded;

            //
            // Segments must be proportionally equal
            //
            if (!pss1.ProportionallyEquals(pss2)) return newGrounded;
            if (!pss1.ProportionallyEquals(pss3)) return newGrounded;
            if (!pss2.ProportionallyEquals(pss3)) return newGrounded;

            //
            // Collect all of the applicable segments
            //
            Segment seg1Tri1 = ct1.GetSegment(pss1);
            Segment seg2Tri1 = ct1.GetSegment(pss2);
            Segment seg3Tri1 = ct1.GetSegment(pss3);

            Segment seg1Tri2 = ct2.GetSegment(pss1);
            Segment seg2Tri2 = ct2.GetSegment(pss2);
            Segment seg3Tri2 = ct2.GetSegment(pss3);

            // Avoid redundant segments, if they arise
            if (seg1Tri1.StructurallyEquals(seg2Tri1) || seg1Tri1.StructurallyEquals(seg3Tri1) || seg2Tri1.StructurallyEquals(seg3Tri1)) return newGrounded;
            if (seg1Tri2.StructurallyEquals(seg2Tri2) || seg1Tri2.StructurallyEquals(seg3Tri2) || seg2Tri2.StructurallyEquals(seg3Tri2)) return newGrounded;

            //
            // For debug only
            //
            Debug.WriteLine("SSS Similarity In Triangles: " + ct1 + ", " + ct2);
            Debug.WriteLine("\t\t" + pss1);
            Debug.WriteLine("\t\t" + pss2);
            Debug.WriteLine("\t\t" + pss3);
            // End debug only

            // Indicate that these two triangles are similar to avoid deducing this again later.
            ct1.AddSimilarTriangle(ct2);
            ct2.AddSimilarTriangle(ct1);

            //
            // Collect the corresponding points
            //
            List<KeyValuePair<Point, Point>> pointPairs = new List<KeyValuePair<Point, Point>>();
            pointPairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.SharedVertex(seg2Tri1), seg1Tri2.SharedVertex(seg2Tri2)));
            pointPairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.SharedVertex(seg3Tri1), seg1Tri2.SharedVertex(seg3Tri2)));
            pointPairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.SharedVertex(seg3Tri1), seg2Tri2.SharedVertex(seg3Tri2)));

            List<GroundedClause> simTriAntecedent = new List<GroundedClause>();
            simTriAntecedent.Add(ct1);
            simTriAntecedent.Add(ct2);
            simTriAntecedent.Add(pss1);
            simTriAntecedent.Add(pss2);
            simTriAntecedent.Add(pss3);

            newGrounded.AddRange(SASSimilarity.GenerateCorrespondingParts(pointPairs, simTriAntecedent, NAME));

            return newGrounded;
        }
    }
}



        //private static KeyValuePair<Point, Point> GeneratePair(ProportionalSegments s1, ProportionalSegments s2)
        //{
        //    Point vertexOne = s1.segment1.SharedVertex(s2.segment1);
        //    vertexOne = vertexOne == null ? s1.segment2.SharedVertex(s2.segment1) : vertexOne;

        //    Point vertexTwo = s2.segment2.SharedVertex(s1.segment1);
        //    vertexTwo = vertexTwo == null ? s2.segment2.SharedVertex(s1.segment2) : vertexTwo;

        //    return new KeyValuePair<Point, Point>(vertexOne, vertexTwo);
        //}

////
//// Return all the corresponing points from the two triangles and Congruence Pairs
////
//private static List<KeyValuePair<Point, Point>> MakePointPairs(Triangle ct1, Triangle ct2, List<ProportionalSegments> propPairs)
//{
//    List<KeyValuePair<Point, Point>> pointPairs = new List<KeyValuePair<Point, Point>>();

//    // we could write this without loops, but loops allow extension to more sides (if we have a generalpolygon)

//    // Take all combinations of Congruent Segment Pairs and find the common vertex in both triangles; those are the corresponding points
//    for (int i = 0; i < propPairs.Count - 1; i++)
//    {
//        for (int j = i + 1; j < propPairs.Count; j++)
//        {
//            Segment triOneSeg1 = ct1.GetSegment(propPairs[i]);
//            Segment triTwoSeg1 = ct2.GetSegment(propPairs[i]);

//            Segment triOneSeg2 = ct1.GetSegment(propPairs[j]);
//            Segment triTwoSeg2 = ct2.GetSegment(propPairs[j]);

//            pointPairs.Add(new KeyValuePair<Point, Point>(triOneSeg1.SharedVertex(triOneSeg2), triTwoSeg1.SharedVertex(triTwoSeg2)));
//        }
//    }

//    return pointPairs;
//}


//private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckForSSS(Triangle ct1, Triangle ct2, List<ProportionalSegments> propSegments)
//{

//}

//private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateAllAngleClauses(CongruentTriangles ccts, GroundedClause[] facts, string NAME)
//{
//    //
//    // Is this triangle reflexively congruent; all three sides shared?
//    //
//    if (ccts.ct1.Equals(ccts.ct2))
//    {
//        // Generate no new information
//        return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
//    }

//    //
//    // This map will contain all points in Triangle 1 which corresponsd directly to the points of Triangle 2
//    //
//    List<Point> orderedTriOnePts = new List<Point>();
//    List<Point> orderedTriTwoPts = new List<Point>();

//    //
//    // Is there one shared side?
//    // This is a parallelogram with one vertex on each triangle not on the shared segment.
//    // And we are guaranteed the folowing pattern:
//    //
//    //     A----------B
//    //    /          /        Here, shared is AD
//    //   /          /
//    //  C----------D
//    //
//    // Triangle ABD \cong
//    // Triangle DCA
//    Segment cs = ccts.ct1.SharesSide(ccts.ct2);
//    if (cs != null)
//    {
//        orderedTriOnePts.Add(cs.Point1);
//        orderedTriTwoPts.Add(cs.Point2);
//        orderedTriOnePts.Add(cs.Point2);
//        orderedTriTwoPts.Add(cs.Point1);

//        // Seek the remaining point in each triangle
//        orderedTriOnePts.Add(ccts.ct1.OtherPoint(cs));
//        orderedTriTwoPts.Add(ccts.ct2.OtherPoint(cs));
//    }
//    else
//    {
//        //
//        // There are no shared sides (although a point may be shared)
//        //

//        // Create two lists of three points from each triangle
//        List<Point> triangleOne = ccts.ct1.GetPoints();
//        List<Point> triangleTwo = ccts.ct2.GetPoints();

//        // Construct the map
//        for (int i = 0; i < facts.Length; i++)
//        {
//            KeyValuePair<Point, Point> pair = GeneratePair((ProportionalSegments)facts[i],
//                                                           (ProportionalSegments)facts[i + 1 < facts.Length ? i + 1 : 0]);

//            Point p = pair.Key;
//            if (triangleOne.Contains(p))
//            {
//                orderedTriOnePts.Add(pair.Key);
//                orderedTriTwoPts.Add(pair.Value);
//            }
//            else
//            {
//                orderedTriOnePts.Add(pair.Value);
//                orderedTriTwoPts.Add(pair.Key);
//            }
//        }
//    }

//    return CongruentTriangles.GenerateCPCTC(ccts, orderedTriOnePts, orderedTriTwoPts);
//}

        ////
        //// Given these two triangles and the set of 3 congruent segment pairs, is this a complete SSS?
        ////
        //private static bool IsSpecificSSS(Triangle ct1, Triangle ct2, List<ProportionalSegments> segmentPairs)
        //{
        //    List<Segment> triangleOneSegments = new List<Segment>();
        //    List<Segment> triangleTwoSegments = new List<Segment>();

        //    // All segments must be proportional to each other
        //    for (int p = 0; p < segmentPairs.Count - 1; p++)
        //    {
        //        if (!segmentPairs[p].ProportionallyEquals(segmentPairs[p + 1 < segmentPairs.Count ? p + 1 : 0]))
        //        {
        //            return false;
        //        }
        //    }

        //    // For each congruent pair, if the segments in question are unique
        //    foreach (ProportionalSegments propSegments in segmentPairs)
        //    {
        //        Segment triOneSeg = ct1.GetSegment(propSegments);
        //        Segment triTwoSeg = ct2.GetSegment(propSegments);

        //        // If a side of a triangle is already in the list, this will not create a true SSS scenario
        //        if (triangleOneSegments.Contains(triOneSeg) || triangleTwoSegments.Contains(triTwoSeg)) return false;

        //        triangleOneSegments.Add(triOneSeg);
        //        triangleTwoSegments.Add(triTwoSeg);
        //    }

        //    // They must each contain 3 segments to account for all 3 sides of the triangle
        //    return triangleOneSegments.Count == 3 && triangleTwoSegments.Count == 3;
        //}