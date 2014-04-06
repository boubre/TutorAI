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
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, JustificationSwitch.SSS_SIMILARITY);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<ProportionalSegments> candidateSegments = new List<ProportionalSegments>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateTriangles.Clear();
            candidateSegments.Clear();
        }

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
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (clause is ProportionalSegments)
            {
                ProportionalSegments newProp = clause as ProportionalSegments;

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
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                foreach (Triangle oldTriangle in candidateTriangles)
                {
                    for (int m = 0; m < candidateSegments.Count - 2; m++)
                    {
                        for (int n = m + 1; n < candidateSegments.Count - 1; n++)
                        {
                            for (int p = n + 1; p < candidateSegments.Count; p++)
                            {
                                newGrounded.AddRange(CheckForSSS(newTriangle, oldTriangle, candidateSegments[m], candidateSegments[n], candidateSegments[p]));
                            }
                        }
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Of all the congruent segment pairs, choose a subset of 3. Exhaustively check all; if they work, return the set.
        //
        private static List<EdgeAggregator> CheckForSSS(Triangle ct1, Triangle ct2, ProportionalSegments pss1, ProportionalSegments pss2, ProportionalSegments pss3)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

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

            // The smaller segments must belong to one triangle, same for larger segments.
            if (!(ct1.HasSegment(pss1.smallerSegment) && ct1.HasSegment(pss2.smallerSegment) && ct1.HasSegment(pss3.smallerSegment) &&
                  ct2.HasSegment(pss1.largerSegment) && ct2.HasSegment(pss2.largerSegment) && ct2.HasSegment(pss3.largerSegment))  &&
                !(ct1.HasSegment(pss1.largerSegment) && ct1.HasSegment(pss2.largerSegment) && ct1.HasSegment(pss3.largerSegment) &&
                  ct2.HasSegment(pss1.smallerSegment) && ct2.HasSegment(pss2.smallerSegment) && ct2.HasSegment(pss3.smallerSegment)))
                return newGrounded;

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

            newGrounded.AddRange(SASSimilarity.GenerateCorrespondingParts(pointPairs, simTriAntecedent, annotation));

            return newGrounded;
        }
    }
}