using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AAS : Theorem
    {
        private readonly static string NAME = "AAS";

        private static List<Triangle> unifyCandTris = new List<Triangle>();
        private static List<CongruentAngles> unifyCandAngles = new List<CongruentAngles>();
        private static List<CongruentSegments> unifyCandSegments = new List<CongruentSegments>();

        //       A              D
        //      /\             /\
        //     /  \           /  \
        //    /    \         /    \
        //   /______\       /______\
        //  B        C      E       F
        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Congruent(Angle(A, C, B), Angle(D, F, E))
        //    Congruent(Segment(A, B), Segment(D, E)) OR Congruent(Segment(A, C), Segment(D, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                                                          Congruent(Segment(A, B), Angle(D, E)),
        //                                                                                          Congruent(Segment(A, C), Angle(D, F)),
        //                                                                                          Congruent(Angle(B, A, C), Angle(E, D, F)),
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            //
            // The list of new grounded clauses if they are deduced
            //
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Do we have a segment or triangle?
            if (!(c is CongruentSegments) && !(c is CongruentAngles) && !(c is Triangle))
            {
                return newGrounded;
            }

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
                                newGrounded.AddRange(CollectAndCheckAAS(ct1, ct2, newCs));
                            }
                        //}
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandSegments.Add(newCs);
            }
            else if (c is CongruentAngles)
            {
                CongruentAngles newCs = c as CongruentAngles;

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
                                newGrounded.AddRange(CollectAndCheckAAS(ct1, ct2, newCs));
                            }
                        //}
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandAngles.Add(newCs);
            }

            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (c is Triangle)
            {
                Triangle candidateTri = (Triangle)c;
                foreach (Triangle ct in unifyCandTris)
                {
                    List<CongruentSegments> applicSegments = new List<CongruentSegments>();
                    List<CongruentAngles> applicAngles = new List<CongruentAngles>();

                    newGrounded.AddRange(CollectAndCheckAAS(candidateTri, ct, applicSegments, applicAngles));
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckAAS(Triangle ct1, Triangle ct2,
                                                                                                   List<CongruentSegments> applicSegments,
                                                                                                   List<CongruentAngles> applicAngles)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

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
            List<KeyValuePair<Point, Point>> pairs = IsAASsituation(ct1, ct2, applicSegments, applicAngles, out congruences);

            // If pairs is populated, we have a SAS situation
            if (!pairs.Any()) return newGrounded;

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

            GeometricCongruentTriangles cts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>(congruences);
            antecedent.Add(ct1);
            antecedent.Add(ct2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cts));

            //
            // We should not add any new congruences to the graph (thus creating cycles)
            //
            //List<KeyValuePair<List<GroundedClause>, GroundedClause>> cpctc = CongruentTriangles.GenerateCPCTC(cts, triangleOne, triangleTwo);
            //foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in cpctc)
            //{
            //    if (!congruences.Contains(part.Value))
            //    {
            //        newGrounded.Add(part);
            //    }
            //}

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(cts, triangleOne, triangleTwo));

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckAAS(Triangle ct1, Triangle ct2, Congruent cc)
        {
            List<CongruentSegments> applicSegments = new List<CongruentSegments>();
            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

            // Add the congruence statement to the appropriate list
            if (cc is CongruentSegments)
            {
                applicSegments.Add(cc as CongruentSegments);
            }
            else if (cc is CongruentAngles)
            {
                applicAngles.Add(cc as CongruentAngles);
            }

            return CollectAndCheckAAS(ct1, ct2, applicSegments, applicAngles);
        }

        //
        // Is this a true SAS situation?
        //
        private static List<KeyValuePair<Point, Point>> IsAASsituation(Triangle ct1, Triangle ct2,
                                      List<CongruentSegments> segmentPairs,
                                      List<CongruentAngles> anglePairs,
                                  out List<GroundedClause> congruences)
        {
            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

            // Initialize congruences
            congruences = new List<GroundedClause>();

            // Miniumum information required
            if (anglePairs.Count < 2 || !segmentPairs.Any()) return pairs;

            // for each pair of congruent angles, is the given angle the included side?
            for (int i = 0; i < anglePairs.Count; i++)
            {
                for (int j = i + 1; j < anglePairs.Count; j++)
                {
                    // Check if any of the angles is an included angle
                    foreach (CongruentSegments css in segmentPairs)
                    {
                        Angle angle1Tri1 = ct1.NormalizeAngle(ct1.AngleBelongs(anglePairs[i]));
                        Angle angle1Tri2 = ct2.NormalizeAngle(ct2.AngleBelongs(anglePairs[i]));

                        Angle angle2Tri1 = ct1.NormalizeAngle(ct1.AngleBelongs(anglePairs[j]));
                        Angle angle2Tri2 = ct2.NormalizeAngle(ct2.AngleBelongs(anglePairs[j]));

                        // We need distinct angles in this congruence
                        if (!angle1Tri1.Equals(angle2Tri1) && !angle1Tri2.Equals(angle2Tri2))
                        {
                            Segment segTri1 = ct1.GetSegment(css);
                            Segment segTri2 = ct2.GetSegment(css);

                            // Check both triangles if this is NOT an included side; if it is NOT the included side, we have AAS
                            if (!segTri1.IsIncludedSegment(angle1Tri1, angle2Tri1) && !segTri2.IsIncludedSegment(angle1Tri2, angle2Tri2))
                            {
                                // The vertices of the angles correspond
                                pairs.Add(new KeyValuePair<Point, Point>(angle1Tri1.GetVertex(), angle1Tri2.GetVertex()));
                                pairs.Add(new KeyValuePair<Point, Point>(angle2Tri1.GetVertex(), angle2Tri2.GetVertex()));

                                // Acquire the shared, 'included' side so we can acquire the other point.
                                Segment includedSide1 = angle1Tri1.SharedRay(angle2Tri1);
                                Segment includedSide2 = angle1Tri2.SharedRay(angle2Tri2);

                                // For the segments, look at the congruences and select accordingly
                                pairs.Add(new KeyValuePair<Point, Point>(ct1.OtherPoint(includedSide1), ct2.OtherPoint(includedSide2)));

                                congruences.Add(anglePairs[i]);
                                congruences.Add(anglePairs[j]);
                                congruences.Add(css);

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