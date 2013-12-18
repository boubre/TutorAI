using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AASimilarity : Axiom
    {
        private readonly static string NAME = "AA Similarity";

        private static List<Triangle> unifyCandTris = new List<Triangle>();
        private static List<CongruentAngles> unifyCandAngles = new List<CongruentAngles>();

        //
        // In order for two triangles to be Similar, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Angle(B, C, A), Angle(E, F, D))
        //    Congruent(Angle(A, B, C), Angle(D, E, F)) -> Similar(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                 Proportional(Segment(A, C), Angle(D, F)),
        //                                                 Proportional(Segment(A, B), Segment(D, E)),
        //                                                 Proportional(Segment(B, C), Segment(E, F))
        //                                                 Congruent(Angle(C, A, B), Angle(F, D, E)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee similarity
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Do we have a segment or triangle?
            if (!(c is CongruentAngles) && !(c is Triangle))
            {
                return newGrounded;
            }

            //
            // The list of new grounded clauses if they are deduced
            //
            List<CongruentAngles> applicAngles = new List<CongruentAngles>();

            if (c is CongruentAngles)
            {
                CongruentAngles newCas = c as CongruentAngles;

                // Check all combinations of triangles to see if they are Similar
                // This congruence must include the new segment congruence
                for (int i = 0; i < unifyCandTris.Count; i++)
                {
                    for (int j = i + 1; j < unifyCandTris.Count; j++)
                    {
                        Triangle ct1 = unifyCandTris[i];
                        Triangle ct2 = unifyCandTris[j];

                        if (!ct1.WasDeducedSimilar(ct2))
                        {
                            // First, compare the new Similar segment; if it fails, ignore this pair of triangles
                            if (newCas.LinksTriangles(ct1, ct2))
                            {
                                applicAngles.Clear();
                                applicAngles.Add(newCas);
                                newGrounded.AddRange(CollectAndCheckAA(ct1, ct2, applicAngles));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                unifyCandAngles.Add(newCas);
            }

            // If this is a new triangle, check for triangles which may be Similar to this new triangle
            else if (c is Triangle)
            {
                Triangle candidateTri = c as Triangle;
                foreach (Triangle ct in unifyCandTris)
                {
                    applicAngles.Clear();
                    newGrounded.AddRange(CollectAndCheckAA(candidateTri, ct, applicAngles));
                }

                // Add this triangle to the list of possible clauses to unify later
                unifyCandTris.Add(candidateTri);
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable proportional segments as well as congruent angles. Then checks for SAS
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CollectAndCheckAA(Triangle ct1, Triangle ct2,
                                                                                                  List<CongruentAngles> applicAngles)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Has this congruence or similarity been established before? If so, do not deduce it again.
            if (ct1.HasEstablishedSimilarity(ct2) || ct2.HasEstablishedSimilarity(ct1)) return newGrounded;
            if (ct1.HasEstablishedCongruence(ct2) || ct2.HasEstablishedCongruence(ct1)) return newGrounded;

            // Check all Angles
            foreach (CongruentAngles cca in unifyCandAngles)
            {
                // Do these angles link the two triangles?
                if (cca.LinksTriangles(ct1, ct2))
                {
                    applicAngles.Add(cca);
                }
            }

            // Construct the AA situation finding the facts
            List<GroundedClause> facts;
            List<KeyValuePair<Point, Point>> pairs = IsAAsituation(ct1, ct2, applicAngles, out facts);

            // If pairs is populated, we have a AA situation; if not, no AA
            if (!pairs.Any()) return newGrounded;

            // Create the congruence between the triangles
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();
            foreach (KeyValuePair<Point, Point> pair in pairs)
            {
                triangleOne.Add(pair.Key);
                triangleTwo.Add(pair.Value);
            }

            // Indicate that these two triangles are Similar to avoid deducing this again later.
            ct1.AddSimilarTriangle(ct2);
            ct2.AddSimilarTriangle(ct1);

            GeometricSimilarTriangles simTris = new GeometricSimilarTriangles(new Triangle(triangleOne), new Triangle(triangleTwo), NAME);

            // Hypergraph edge
            List<GroundedClause> antecedent = new List<GroundedClause>(facts);
            antecedent.Add(ct1);
            antecedent.Add(ct2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, simTris));

            //
            // We should not add any new relationships to the graph (thus creating cycles)
            //
            //List<KeyValuePair<List<GroundedClause>, GroundedClause>> componentsRelations = SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo);
            //foreach (KeyValuePair<List<GroundedClause>, GroundedClause> part in componentsRelations)
            //{
            //    if (!facts.Contains(part.Value))
            //    {
            //        newGrounded.Add(part);
            //    }
            //}

            // Add all the corresponding parts as new Similar clauses
            newGrounded.AddRange(SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo));

            return newGrounded;
        }

        //
        // Is this a true SAS situation?
        //
        private static List<KeyValuePair<Point, Point>> IsAAsituation(Triangle ct1, Triangle ct2,
                                                                      List<CongruentAngles> anglePairs,
                                                                  out List<GroundedClause> facts)
        {
            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

            // Initialize the facts used to prove similarity
            facts = new List<GroundedClause>();

            // Miniumum information required
            if (anglePairs.Count < 2) return pairs;

            // Do we have a set of two distinct congruent angle pairs
            for (int i = 0; i < anglePairs.Count; i++)
            {
                // Reflexivity is ok for similarity because two triangles may share an angle (this is common for similarity)
                Angle angle1Tri1 = ct1.AngleBelongs(anglePairs[i]);
                Angle angle1Tri2 = ct2.AngleBelongs(anglePairs[i]);

                for (int j = i + 1; j < anglePairs.Count; j++)
                {
                    Angle angle2Tri1 = ct1.AngleBelongs(anglePairs[j]);
                    Angle angle2Tri2 = ct2.AngleBelongs(anglePairs[j]);

                    // We just need two distinct angles from each triangle
                    if (!angle1Tri1.StructurallyEquals(angle2Tri1) && !angle1Tri2.StructurallyEquals(angle2Tri2))
                    {
                        // The vertices of the angles correspond
                        pairs.Add(new KeyValuePair<Point, Point>(angle1Tri1.GetVertex(), angle1Tri2.GetVertex()));
                        pairs.Add(new KeyValuePair<Point, Point>(angle2Tri1.GetVertex(), angle2Tri2.GetVertex()));
                        pairs.Add(new KeyValuePair<Point, Point>(ct1.OtherPoint(angle1Tri1.GetVertex(), angle2Tri1.GetVertex()),
                                                                 ct2.OtherPoint(angle1Tri2.GetVertex(), angle2Tri2.GetVertex())));

                        facts.Add(anglePairs[i]);
                        facts.Add(anglePairs[j]);

                        return pairs;
                    }
                }
            }

            return pairs;
        }
    }
}