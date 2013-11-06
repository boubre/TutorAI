using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SSS : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SSS";

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return c is ConcreteTriangle || c is ConcreteCongruentSegments;
        }

        private static List<ConcreteTriangle> unifyCandTris = new List<ConcreteTriangle>();
        private static List<ConcreteCongruentSegments> unifyCandSegments = new List<ConcreteCongruentSegments>();

        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Segment(A, C), Segment(D, F)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Angles(A, B, C), Angle(D, E, F)),
        //                                               Congruent(Angles(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angles(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // Do we have a segment or triangle?
            if (!(c is ConcreteCongruentSegments) && !(c is ConcreteTriangle)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Do we have enough information for unification?
            //
            if (c is ConcreteCongruentSegments && (unifyCandSegments.Count < 2 || unifyCandTris.Count <= 1))
            {
                unifyCandSegments.Add((ConcreteCongruentSegments)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is ConcreteTriangle && (!unifyCandTris.Any() || unifyCandSegments.Count < 3))
            {
                unifyCandTris.Add((ConcreteTriangle)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (c is ConcreteCongruentSegments)
            {
                ConcreteCongruentSegments newCs = (ConcreteCongruentSegments)c;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < unifyCandTris.Count; i++)
                {
                    for (int j = 1; j < unifyCandTris.Count; j++)
                    {
                        // Do not compare a triangle to itself
                        if (i != j)
                        {
                            ConcreteTriangle ct1 = unifyCandTris.ElementAt(i);
                            ConcreteTriangle ct2 = unifyCandTris.ElementAt(j);
                            List<ConcreteCongruentSegments> applicCongruents = new List<ConcreteCongruentSegments>();

                            // First, compare the new congruent segment; if it fails, ignore this pair of triangles
                            if (newCs.LinksTriangles(ct1, ct2))
                            {
                                applicCongruents.Add(newCs);

                                // Check all other segments
                                foreach (ConcreteCongruentSegments ccs in unifyCandSegments)
                                {
                                    // Does this segment link the two triangles?
                                    if (ccs.LinksTriangles(ct1, ct2))
                                    {
                                        applicCongruents.Add(ccs);
                                    }
                                }

                                // Generate the new clause
                                List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = CheckForSSS(ct1, ct2, applicCongruents);
                                newGrounded.AddRange(newG);
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
                    //
                    // Is this concrete triangle congruent to the new candidate?
                    //
                    // Find all applicable congruent segments for both triangles
                    List<ConcreteCongruentSegments> applicCongruents = new List<ConcreteCongruentSegments>();
                    foreach (ConcreteCongruentSegments ccs in unifyCandSegments)
                    {
                        // Does this segment link the two triangles?
                        if (ccs.LinksTriangles(ct, candidateTri))
                        {
                            applicCongruents.Add(ccs);
                        }
                    }

                    // Generate the new clause
                    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = CheckForSSS(ct, (ConcreteTriangle)c, applicCongruents);
                    newGrounded.AddRange(newG);
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // Given these two triangles and the set of 3 congruent segment pairs, is this a complete SSS?
        //
        private static bool IsSpecificSSS(ConcreteTriangle ct1, ConcreteTriangle ct2, List<ConcreteCongruentSegments> segmentPairs)
        {
            List<ConcreteSegment> triangleOneSegments = new List<ConcreteSegment>();
            List<ConcreteSegment> triangleTwoSegments = new List<ConcreteSegment>();

            // For each congruent pair, if the segments in question are unique
            foreach (ConcreteCongruentSegments ccss in segmentPairs)
            {
                ConcreteSegment triOneSeg = ct1.GetSegment(ccss);
                ConcreteSegment triTwoSeg = ct2.GetSegment(ccss);

                // If a side of a triangle is already in the list, this will not create a true SSS scenario
                if (triangleOneSegments.Contains(triOneSeg) || triangleTwoSegments.Contains(triTwoSeg)) return false;

                triangleOneSegments.Add(triOneSeg);
                triangleTwoSegments.Add(triTwoSeg);
            }

            // They must each contain 3 segments to account for all 3 sides of the triangle
            return triangleOneSegments.Count == 3 && triangleTwoSegments.Count == 3;
        }
        
        //
        // Of all the congruent segment pairs, choose a subset of 3. Exhaustively check all; if they work, return the set.
        //
        private static List<ConcreteCongruentSegments> IsTrueSSS(ConcreteTriangle ct1, ConcreteTriangle ct2, List<ConcreteCongruentSegments> conSegments)
        {
            List<ConcreteCongruentSegments> subset;

            // construct the subset in a hack way using 3 loops to guarantee we generate ALL subsets exhaustively
            for (int one = 0; one < conSegments.Count; one++)
            {
                for (int two = one + 1; two < conSegments.Count; two++)
                {
                    subset = new List<ConcreteCongruentSegments>();
                    for (int three = two + 1; three < conSegments.Count; three++)
                    {
                        subset.Add(conSegments[one]);
                        subset.Add(conSegments[two]);
                        subset.Add(conSegments[three]);

                        if (IsSpecificSSS(ct1, ct2, subset)) return subset;
                    }
                }
            }

            return null;
        }

        //
        // Return all the corresponing points from the two triangles and Congruence Pairs
        //
        private static List<KeyValuePair<ConcretePoint, ConcretePoint>> MakePointPairs(ConcreteTriangle ct1, ConcreteTriangle ct2, List<ConcreteCongruentSegments> congruentPairs)
        {
            List<KeyValuePair<ConcretePoint, ConcretePoint>> pointPairs = new List<KeyValuePair<ConcretePoint,ConcretePoint>>();

            // we could write this without loops, but loops allow extension to more sides (if we have a generalpolygon)
            
            // Take all combinations of Congruent Segment Pairs and find the common vertex in both triangles; those are the corresponding points
            for (int i = 0; i < congruentPairs.Count - 1; i++)
            {
                for (int j = i + 1; j < congruentPairs.Count; j++)
                {
                    ConcreteSegment triOneSeg1 = ct1.GetSegment(congruentPairs[i]);
                    ConcreteSegment triTwoSeg1 = ct2.GetSegment(congruentPairs[i]);

                    ConcreteSegment triOneSeg2 = ct1.GetSegment(congruentPairs[j]);
                    ConcreteSegment triTwoSeg2 = ct2.GetSegment(congruentPairs[j]);

                    pointPairs.Add(new KeyValuePair<ConcretePoint, ConcretePoint>(triOneSeg1.SharedVertex(triOneSeg2), triTwoSeg1.SharedVertex(triTwoSeg2)));
                }
            }

            return pointPairs;
       }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckForSSS(ConcreteTriangle ct1, ConcreteTriangle ct2, List<ConcreteCongruentSegments> conSegments)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Has this congruence been established before? If so, do not deduce it again.
            // CTA: Is this a problem if a triangle can be found congruent in multiple ways?
            // That is, establish congruence, but do not generate?
            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

            // We need at least 3 congruent segments between the two triangles
            if (conSegments.Count < 3) return newGrounded;


            //
            // For debug only
            //
            Debug.WriteLine("SSS In Triangles: " + ct1 + ", " + ct2);
            foreach (GroundedClause gc in conSegments)
            {
                Debug.WriteLine("\t\t" + gc.ToString());
            }
            // End debug only


            // Acquire the 3 actual congruent pairs that make this SSS
            List<ConcreteCongruentSegments> congruencePairs = IsTrueSSS(ct1, ct2, conSegments);

            // Not SSS if this is true
            if (congruencePairs == null) return newGrounded;

            List<KeyValuePair<ConcretePoint, ConcretePoint>> correspondingVertices = MakePointPairs(ct1, ct2, congruencePairs);

            // Create the congruence between the triangles
            List<ConcretePoint> triangleOne = new List<ConcretePoint>();
            List<ConcretePoint> triangleTwo = new List<ConcretePoint>();
            foreach (KeyValuePair<ConcretePoint, ConcretePoint> pair in correspondingVertices)
            {
                triangleOne.Add(pair.Key);
                triangleTwo.Add(pair.Value);
            }

            // Indicate that these two triangles are congruent to avoid deducing this again later.
            ct1.AddCongruentTriangle(ct2);
            ct2.AddCongruentTriangle(ct1);

            ConcreteCongruentTriangles ccts = new ConcreteCongruentTriangles(new ConcreteTriangle(triangleOne),
                                                                             new ConcreteTriangle(triangleTwo), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            foreach (ConcreteCongruentSegments ccss in congruencePairs) { antecedent.Add(ccss); }
            antecedent.Add(ct1);
            antecedent.Add(ct2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(ConcreteCongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateAllAngleClauses(ConcreteCongruentTriangles ccts, GroundedClause[] facts, string NAME)
        {
            //
            // Is this triangle reflexively congruent; all three sides shared?
            //
            if (ccts.ct1.Equals(ccts.ct2))
            {
                // Generate no new information
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            //
            // This map will contain all points in Triangle 1 which corresponsd directly to the points of Triangle 2
            //
            List<ConcretePoint> orderedTriOnePts = new List<ConcretePoint>();
            List<ConcretePoint> orderedTriTwoPts = new List<ConcretePoint>();

            //
            // Is there one shared side?
            // This is a parallelogram with one vertex on each triangle not on the shared segment.
            // And we are guaranteed the folowing pattern:
            //
            //     A----------B
            //    /          /        Here, shared is AD
            //   /          /
            //  C----------D
            //
            // Triangle ABD \cong
            // Triangle DCA
            ConcreteSegment cs = ccts.ct1.SharesSide(ccts.ct2);
            if (cs != null)
            {
                orderedTriOnePts.Add(cs.Point1);
                orderedTriTwoPts.Add(cs.Point2);
                orderedTriOnePts.Add(cs.Point2);
                orderedTriTwoPts.Add(cs.Point1);

                // Seek the remaining point in each triangle
                orderedTriOnePts.Add(ccts.ct1.OtherPoint(cs));
                orderedTriTwoPts.Add(ccts.ct2.OtherPoint(cs));
            }
            else
            {
                //
                // There are no shared sides (although a point may be shared)
                //

                // Create two lists of three points from each triangle
                List<ConcretePoint> triangleOne = ccts.ct1.GetPoints();
                List<ConcretePoint> triangleTwo = ccts.ct2.GetPoints();

                // Construct the map
                for (int i = 0; i < facts.Length; i++)
                {
                    KeyValuePair<ConcretePoint, ConcretePoint> pair = GeneratePair((ConcreteCongruentSegments)facts[i],
                                                                                   (ConcreteCongruentSegments)facts[i + 1 < facts.Length ? i + 1 : 0]);

                    ConcretePoint p = pair.Key;
                    if (triangleOne.Contains(p))
                    {
                        orderedTriOnePts.Add(pair.Key);
                        orderedTriTwoPts.Add(pair.Value);
                    }
                    else
                    {
                        orderedTriOnePts.Add(pair.Value);
                        orderedTriTwoPts.Add(pair.Key);
                    }
                }
            }

            return ConcreteCongruentTriangles.GenerateCPCTC(ccts, orderedTriOnePts, orderedTriTwoPts);
        }

        private static KeyValuePair<ConcretePoint, ConcretePoint> GeneratePair(ConcreteCongruentSegments s1, ConcreteCongruentSegments s2)
        {
            ConcretePoint vertexOne = s1.GetFirstSegment().SharedVertex(s2.GetFirstSegment());
            vertexOne = vertexOne == null ? s1.GetSecondSegment().SharedVertex(s2.GetFirstSegment()) : vertexOne;

            ConcretePoint vertexTwo = s2.GetSecondSegment().SharedVertex(s1.GetFirstSegment());
            vertexTwo = vertexTwo == null ? s2.GetSecondSegment().SharedVertex(s1.GetSecondSegment()) : vertexTwo;

            return new KeyValuePair<ConcretePoint, ConcretePoint>(vertexOne, vertexTwo);
        }
    }
}