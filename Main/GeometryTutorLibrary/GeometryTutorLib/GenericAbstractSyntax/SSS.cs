using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
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
                            List<GroundedClause> applicCongruents = new List<GroundedClause>();

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
                                List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = GenNewGroundedClause(ct1, ct2, applicCongruents);
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
                    List<GroundedClause> applicCongruents = new List<GroundedClause>();
                    foreach (ConcreteCongruentSegments ccs in unifyCandSegments)
                    {
                        // Does this segment link the two triangles?
                        if (ct.HasSegment(ccs.GetFirstSegment()) && candidateTri.HasSegment(ccs.GetSecondSegment()) ||
                            ct.HasSegment(ccs.GetSecondSegment()) && candidateTri.HasSegment(ccs.GetFirstSegment()))
                        {
                            applicCongruents.Add(ccs);
                        }
                    }

                    // Generate the new clause
                    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newG = GenNewGroundedClause(ct, (ConcreteTriangle)c, applicCongruents);
                    newGrounded.AddRange(newG);
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenNewGroundedClause(ConcreteTriangle ct1, ConcreteTriangle ct2, List<GroundedClause> conSegments)
        {
            // Here, we need three congruent segments between the two triangles
            if (conSegments.Count < 3) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (conSegments.Count > 3)
            {
                Debug.WriteLine("Expected to find only 3 congruent segments in SSS; found " + conSegments.Count);
                foreach (GroundedClause gc in conSegments)
                {
                    Debug.WriteLine("\t\t" + gc.ToString());
                }
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            ConcreteCongruentTriangles ccts = new ConcreteCongruentTriangles(ct1, ct2, NAME); // CTA: The vertices may not be ordered: PROBLEM?

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>(conSegments);
            antecedent.Add(ct1);
            antecedent.Add(ct2);
            GroundedClause.ConstructClauseLinks(antecedent, ccts);

            // There are 3 distinct congruent segments
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccts));

            // Add all remaining clauses, in this case 3 angles
            GroundedClause[] conFacts = conSegments.ToArray();
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> returned = GenerateAllAngleClauses(ccts, conFacts, NAME);
            newGrounded.AddRange(returned);

            // Construct linkages with the CPCTC angles
            // Does this duplicate what was done in CPCTC
            //foreach (GroundedClause returnedAngle in returned)
            //{
            //    GroundedClause.ConstructClauseLinks(Utilities.MakeList<GroundedClause>(ccts), returnedAngle);
            //}

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