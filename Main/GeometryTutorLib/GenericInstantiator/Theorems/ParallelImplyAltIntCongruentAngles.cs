using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ParallelImplyAltIntCongruentAngles : Theorem
    {
        private readonly static string NAME = "Alternate Interior Angles"; //"Parallel Lines Imply Congruent Alternate Interior Angles";

        public ParallelImplyAltIntCongruentAngles() { }

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

        // Resets all saved data.
        public static void Clear()
        {
            candIntersection.Clear();
            candidateParallel.Clear();
        }

        //
        // Parallel(Segment(C, D), Segment(E, F)),
        // Intersection(M, Segment(A,B), Segment(C, D)),
        // Intersection(N, Segment(A,B), Segment(E, F))-> Congruent(Angle(F, N, M), Angle(N, M, C))
        //                                                Congruent(Angle(E, N, M), Angle(N, M, D))
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                             E---------/-----------F
        //                                      / N
        //                                     A
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Parallel) && !(c is Intersection)) return newGrounded;

            if (c is Parallel)
            {
                Parallel parallel = c as Parallel;

                // Find two candidate lines cut by the same transversal
                for (int i = 0; i < candIntersection.Count - 1; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGenerateParallelImplyAlternateInterior(candIntersection[i], candIntersection[j], parallel));
                    }
                }

                candidateParallel.Add(parallel);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                // Find two candidate lines cut by the same transversal
                foreach (Intersection inter in candIntersection)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(CheckAndGenerateParallelImplyAlternateInterior(newIntersection, inter, parallel));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckAndGenerateParallelImplyAlternateInterior(Intersection inter1, Intersection inter2, Parallel parallel)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The two intersections should not be at the same vertex
            if (inter1.intersect.Equals(inter2.intersect)) return newGrounded;

            // Determine the transversal
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            //
            // Ensure the non-traversal segments align with the parallel segments
            //
            Segment parallel1 = inter1.OtherSegment(transversal);
            Segment parallel2 = inter2.OtherSegment(transversal);

            // The non-transversals should not be the same (coinciding)
            if (parallel1.IsCollinearWith(parallel2)) return newGrounded;

            Segment coincidingParallel1 = parallel.CoincidesWith(parallel1);
            Segment coincidingParallel2 = parallel.CoincidesWith(parallel2);

            // The pair of non-transversals needs to align exactly with the parallel pair of segments
            if (coincidingParallel1 == null || coincidingParallel2 == null) return newGrounded;

            // Both intersections should not be referring to an intersection point on the same parallel segment
            if (parallel.segment1.PointIsOn(inter1.intersect) && parallel.segment1.PointIsOn(inter2.intersect)) return newGrounded;
            if (parallel.segment2.PointIsOn(inter1.intersect) && parallel.segment2.PointIsOn(inter2.intersect)) return newGrounded;

            // The resultant candidate parallel segments shouldn't share any vertices
            if (coincidingParallel1.SharedVertex(coincidingParallel2) != null) return newGrounded;

            if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint())
            {
                //
                // Creates a basic S-Shape with standsOnEndpoints
                //
                //   offThis   ______
                //                   |
                //   offThat   ______|
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> cShapePoints = inter1.CreatesBasicCShape(inter2);
                if (cShapePoints.Key != null && cShapePoints.Value != null) return newGrounded;

                //
                // Creates a basic S-Shape with standsOnEndpoints
                //
                //                  ______ offThat       _______
                //                 |                     \
                //   offThis ______|                 _____\
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> sShapePoints = inter1.CreatesBasicSShape(inter2);
                if (sShapePoints.Key != null && sShapePoints.Value != null)
                {
                    return GenerateSimpleS(parallel, inter1, sShapePoints.Key, inter2, sShapePoints.Value);
                }

                return newGrounded;
            }

            //     _______/________
            //           /
            //          /
            //   ______/_______
            //        /
            if (inter1.Crossing() && inter2.Crossing()) return GenerateDualCrossings(parallel, inter1, inter2);

            // Alt. Int if an H-Shape
            //
            // |     |
            // |_____|
            // |     |
            // |     |
            //
            if (inter1.CreatesHShape(inter2)) return GenerateH(parallel, inter1, inter2);

            // Corresponding if a flying-Shape
            //
            // |     |
            // |_____|___ offCross
            // |     |
            // |     |
            //
            Point offCross = inter1.CreatesFlyingShapeWithCrossing(inter2);
            if (offCross != null)
            {
                return GenerateFlying(parallel, inter1, inter2, offCross);
            }

            //
            // Creates a shape like an extended t
            //     offCross                          offCross  
            //      |                                   |
            // _____|____                         ______|______
            //      |                                   |
            //      |_____ offStands     offStands _____|
            //
            // Returns <offStands, offCross>
            KeyValuePair<Point, Point> tShapePoints = inter1.CreatesCrossedTShape(inter2);
            if (tShapePoints.Key != null && tShapePoints.Value != null)
            {
                return GenerateCrossedT(parallel, inter1, inter2, tShapePoints.Key, tShapePoints.Value);
            }

            return newGrounded;

            ////
            //// Are these angles on the opposite side of the transversal?
            ////
            //// Make a simple transversal from the two intersection points
            //Segment simpleTransversal = new Segment(inter1.intersect, inter2.intersect);

            ////    A_______M________B
            ////           /
            ////          /
            ////   ______/_______
            ////  C      N       D
            //// Verify which side the outer points of the parallel segments lie; that is, is B on same side as C or alternate side?

            //List<GeometricCongruentAngles> newCongruences = new List<GeometricCongruentAngles>();
            
            //// Create a segment from these two points so we can compare distances
            //Segment crossing = new Segment(coincidingParallel1.Point1, coincidingParallel2.Point1);

            //// Will this crossing segment actually intersect the real transversal in the middle of the two segments
            //Point intersection = transversal.FindIntersection(crossing);

            //// If the intersection is between the two intersection points, they are points are on the opposite side
            //Angle altIntAngle1 = null;  // Pair 1
            //Angle altIntAngle2 = null;
            //Angle altIntAngle3 = null;  // Pair 2
            //Angle altIntAngle4 = null;
            //if (Segment.Between(intersection, inter1.intersect, inter2.intersect))
            //{
            //    // Do we have an enclosed situation?
            //    //  __________
            //    //      /
            //    //     /
            //    //    /_________
            //    //
            //    if (inter1.StandsOnEndpoint() || inter2.StandsOnEndpoint())
            //    {
            //        altIntAngle1 = new Angle(coincidingParallel1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
            //        altIntAngle2 = new Angle(coincidingParallel2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);
            //        newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
            //    }
            //    else
            //    {
            //        altIntAngle1 = new Angle(coincidingParallel1.Point1, inter1.intersect, inter2.intersect);
            //        altIntAngle2 = new Angle(coincidingParallel2.Point1, inter2.intersect, inter1.intersect);

            //        altIntAngle3 = new Angle(coincidingParallel1.Point2, inter1.intersect, inter2.intersect);
            //        altIntAngle4 = new Angle(coincidingParallel2.Point2, inter2.intersect, inter1.intersect);

            //        newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
            //        newCongruences.Add(new GeometricCongruentAngles(altIntAngle3, altIntAngle4, NAME));
            //    }
            //}
            //else
            //{
            //    // We cannot have an enclosed situation
            //    //  __________
            //    //      /
            //    //     /
            //    //    /_________
            //    //
            //    if (inter1.StandsOnEndpoint() || inter2.StandsOnEndpoint()) return newGrounded;

            //    altIntAngle1 = new Angle(coincidingParallel1.Point1, inter1.intersect, inter2.intersect);
            //    altIntAngle2 = new Angle(coincidingParallel2.Point2, inter2.intersect, inter1.intersect);

            //    altIntAngle3 = new Angle(coincidingParallel1.Point2, inter1.intersect, inter2.intersect);
            //    altIntAngle4 = new Angle(coincidingParallel2.Point1, inter2.intersect, inter1.intersect);

            //    newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
            //    newCongruences.Add(new GeometricCongruentAngles(altIntAngle3, altIntAngle4, NAME));
            //}
            ////
            //// Now we have the desired construction
            ////

            //// Construct hyperedge
            //List<GroundedClause> antecedent = new List<GroundedClause>();
            //antecedent.Add(inter1);
            //antecedent.Add(inter2);
            //antecedent.Add(parallel);

            //foreach (GeometricCongruentAngles gcas in newCongruences)
            //{
            //    newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gcas));
            //}
        }

        //
        // Creates a shape like an extended t
        //           offCross                           offCross  
        //              |                                   |
        // oppSide _____|____ sameSide       sameSide ______|______ oppSide
        //              |                                   |
        //              |_____ offStands     offStands _____|
        //
        // Returns <offStands, offCross>
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateCrossedT(Parallel parallel, Intersection inter1, Intersection inter2, Point offStands, Point offCross)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (inter1.Crossing() && inter2.StandsOnEndpoint())
            {
                crossingInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.Crossing() && inter1.StandsOnEndpoint())
            {
                crossingInter = inter2;
                standsInter = inter1;
            }

            // Determine the side of the crossing needed for the angle
            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallelStands = standsInter.OtherSegment(transversal);
            Segment parallelCrossing = crossingInter.OtherSegment(transversal);

            Segment crossingTester = new Segment(offStands, parallelCrossing.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point sameSide = transversal.PointIsOnAndBetweenEndpoints(intersection) ? parallelCrossing.Point2 : parallelCrossing.Point1;
            Point oppSide = parallelCrossing.OtherPoint(sameSide);

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offStands, standsInter.intersect, crossingInter.intersect),
                                                                        new Angle(oppSide, crossingInter.intersect, standsInter.intersect), NAME);
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }


        //
        // Creates a basic S-Shape with standsOnEndpoints
        //
        //                  ______ offThat
        //                 |
        //   offThis ______|
        //
        // Return <offThis, offThat>
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateSimpleS(Parallel parallel, Intersection inter1, Point offThis, Intersection inter2, Point offThat)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offThat, inter2.intersect, inter1.intersect),
                                                                        new Angle(offThis, inter1.intersect, inter2.intersect), NAME);
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        //                 offCross
        //                     |
        // leftCross     ______|______   rightCross
        //                     |
        // leftStands     _____|_____    rightStands
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateFlying(Parallel parallel, Intersection inter1, Intersection inter2, Point offCross)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (inter1.Crossing())
            {
                crossingInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.Crossing())
            {
                crossingInter = inter2;
                standsInter = inter1;
            }

            // Determine the side of the crossing needed for the angle
            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallelStands = standsInter.OtherSegment(transversal);
            Segment parallelCrossing = crossingInter.OtherSegment(transversal);

            // Determine point orientation in the plane
            Point leftStands = parallelStands.Point1;
            Point rightStands = parallelStands.Point2;
            Point leftCross = null;
            Point rightCross = null;

            Segment crossingTester = new Segment(leftStands, parallelCrossing.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);
            if (transversal.PointIsOnAndBetweenEndpoints(intersection))
            {
                leftCross = parallelCrossing.Point2;
                rightCross = parallelCrossing.Point1;
            }
            else
            {
                leftCross = parallelCrossing.Point1;
                rightCross = parallelCrossing.Point2;
            }

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(rightCross, crossingInter.intersect, standsInter.intersect),
                                                                        new Angle(leftStands, standsInter.intersect, crossingInter.intersect), NAME);
            newAngleRelations.Add(gca);

            gca = new GeometricCongruentAngles(new Angle(leftCross, crossingInter.intersect, standsInter.intersect),
                                               new Angle(rightStands, standsInter.intersect, crossingInter.intersect), NAME);
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        //     leftTop    rightTop
        //         |         |
        //         |_________|
        //         |         |
        //         |         |
        //   leftBottom rightBottom
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateH(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            //
            // Find tops and bottoms
            //
            Segment crossingLeftParallel = crossingInterLeft.OtherSegment(transversal);
            Segment crossingRightParallel = crossingInterRight.OtherSegment(transversal);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point intersection = transversal.FindIntersection(testingCrossSegment);

            Point leftTop = crossingLeftParallel.Point1;
            Point leftBottom = crossingLeftParallel.Point2;

            Point rightTop = null;
            Point rightBottom = null;
            if (transversal.PointIsOnAndBetweenEndpoints(intersection))
            {
                rightTop = crossingRightParallel.Point2;
                rightBottom = crossingRightParallel.Point1;
            }
            else
            {
                rightTop = crossingRightParallel.Point1;
                rightBottom = crossingRightParallel.Point2;
            }

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                        new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect), NAME);
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect),
                                               new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect), NAME);
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, crossingInterLeft, crossingInterRight);
        }

        //          leftTop    rightTop
        //              |         |
        //  offleft ____|_________|_____ offRight
        //              |         |
        //              |         |
        //         leftBottom rightBottom
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateDualCrossings(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            //
            // Find offLeft and offRight
            //
            Segment crossingLeftParallel = crossingInterLeft.OtherSegment(transversal);
            Segment crossingRightParallel = crossingInterRight.OtherSegment(transversal);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point intersection = transversal.FindIntersection(testingCrossSegment);

            Point leftTop = crossingLeftParallel.Point1;
            Point leftBottom = crossingLeftParallel.Point2;

            Point rightTop = null;
            Point rightBottom = null;
            if (transversal.PointIsOnAndBetweenEndpoints(intersection))
            {
                rightTop = crossingRightParallel.Point2;
                rightBottom = crossingRightParallel.Point1;
            }
            else
            {
                rightTop = crossingRightParallel.Point1;
                rightBottom = crossingRightParallel.Point2;
            }

            // Point that is outside of the parallel lines and transversal
            Segment leftTransversal = crossingInterLeft.GetCollinearSegment(transversal);
            Segment rightTransversal = crossingInterRight.GetCollinearSegment(transversal);

            Point offLeft = Segment.Between(crossingInterLeft.intersect, leftTransversal.Point1, crossingInterRight.intersect) ? leftTransversal.Point1 : leftTransversal.Point2;
            Point offRight = Segment.Between(crossingInterRight.intersect, crossingInterLeft.intersect, rightTransversal.Point1) ? rightTransversal.Point1 : rightTransversal.Point2;

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                        new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect), NAME);
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect),
                                               new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect), NAME);
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, crossingInterLeft, crossingInterRight);
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> MakeRelations(List<CongruentAngles> newAngleRelations, Parallel parallel, Intersection inter1, Intersection inter2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(inter1);
            antecedent.Add(inter2);

            foreach (CongruentAngles newAngles in newAngleRelations)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAngles));
            }

            return newGrounded;
        }
    }
}