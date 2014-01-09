using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CorrespondingAnglesOfParallelLines : Axiom
    {
        private static readonly string NAME = "If Two Parallel Lines are Cut by a Transversal, then Corresponding Angles are Congruent (Axiom)";

        private static List<Parallel> candidateParallel = new List<Parallel>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(clause is Parallel) && !(clause is Intersection)) return newGrounded;

            if (clause is Parallel)
            {
                Parallel parallel = clause as Parallel;
                for (int i = 0; i < candidateIntersection.Count; i++)
                {
                    for (int j = i + 1; j < candidateIntersection.Count; j++)
                    {
                        newGrounded.AddRange(InstantiateIntersection(parallel, candidateIntersection[i], candidateIntersection[j]));
                    }
                }
                candidateParallel.Add(parallel);
            }
            else if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;
                foreach (Intersection oldInter in candidateIntersection)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(InstantiateIntersection(parallel, newInter, oldInter));
                    }
                }
                candidateIntersection.Add(newInter);
            }

            return newGrounded;
        }


        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateIntersection(Parallel parallel, Intersection inter1, Intersection inter2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // No corresponding angles if we have:
            //
            //    |          |         |
            //    |__________|         |_________
            //                                   |
            //                                   |
            //
            if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint()) return newGrounded;

            // No corresponding angles if we have:
            //
            //    |          |
            //    |__________|
            //               |
            //               |
            //
            if ((inter1.StandsOnEndpoint() && inter2.StandsOn()) || (inter2.StandsOnEndpoint() && inter1.StandsOn())) return newGrounded;

            // No corresponding angles if we have:
            //
            //    |          |
            //    |__________|
            //    |          |
            //    |          |
            //
            if ((inter1.StandsOn() && inter2.StandsOn()) || (inter2.StandsOn() && inter1.StandsOn())) return newGrounded;

System.Diagnostics.Debug.WriteLine("Working on: \n\t" + inter1.ToString() + "\n\t" + inter2.ToString());

            //
            // Verify we have a parallel / intersection situation using the given information
            //
            // We want two intersections that create a transversal
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            //
            // Ensure the non-traversal segments align with the parallel segments
            //
            Segment nonTraversal1 = inter1.OtherSegment(transversal);
            Segment nonTraversal2 = inter2.OtherSegment(transversal);

            // The non-transversals should not be the same (coinciding)
            if (nonTraversal1.IsCollinearWith(nonTraversal2)) return newGrounded;

            Segment coincidingParallel1 = parallel.CoincidesWith(nonTraversal1);
            Segment coincidingParallel2 = parallel.CoincidesWith(nonTraversal2);

            // The pair of non-transversals needs to align exactly with the parallel pair of segments
            if (coincidingParallel1 == null || coincidingParallel2 == null) return newGrounded;

            // Corresponding angles if:
            //
            //      |          |           |         |
            //    __|__________|           |_________|___
            //      |                                |
            //      |                                |
            //
            Intersection crossingInter = null;
            Intersection standsOn = null;
            if (inter1.StandsOnEndpoint() && inter2.Crossing())
            {
                standsOn = inter1;
                crossingInter = inter2;
            }
            else if (inter2.StandsOnEndpoint() && inter1.Crossing())
            {
                standsOn = inter2;
                crossingInter = inter1;
            }
            if (standsOn != null && crossingInter != null) return InstantiateSingleIntersection(parallel, standsOn, crossingInter);

            // Corresponding angles if:
            //
            //      |          |           |         |
            //    __|__________|           |_________|___
            //      |          |           |         |
            //      |          |           |
            //
            crossingInter = null;
            standsOn = null;
            if (inter1.StandsOn() && inter2.Crossing())
            {
                standsOn = inter1;
                crossingInter = inter2;
            }
            else if (inter2.StandsOn() && inter1.Crossing())
            {
                standsOn = inter2;
                crossingInter = inter1;
            }
            if (standsOn != null && crossingInter != null) return InstantiateMixedIntersection(parallel, standsOn, crossingInter);

            // Corresponding angles if:
            //
            //      |          |  
            //   ___|__________|__
            //      |          |  
            //      |          | 
            //
            crossingInter = null;
            Intersection crossingInter2 = null;
            if (crossingInter.Crossing() && crossingInter2.Crossing())
            {
                crossingInter = inter1;
                crossingInter2 = inter2;
            }
            if (crossingInter != null && crossingInter2 != null) return InstantiateCompleteIntersection(parallel, crossingInter, crossingInter2);

            return newGrounded;
        }

        // Corresponding angles if:
        //
        //      |          |           |         |
        //    __|__________|           |_________|___
        //      |                                |
        //      |                                |
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateSingleIntersection(Parallel parallel, Intersection standsOn, Intersection crossingInter)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment transversal = standsOn.AcquireTransversal(crossingInter);

            // Ensure the non-traversal segments align with the parallel segments
            Segment nonTransversalStands = standsOn.OtherSegment(transversal);
            Segment nonTransversalCrossing = crossingInter.OtherSegment(transversal);

            Segment crossingTransversalSegment = crossingInter.OtherSegment(nonTransversalCrossing);
            Segment standsOnTransversalSegment = standsOn.OtherSegment(nonTransversalStands);

            // The intersections must have a physical intersection to be valid
            // Otherwise we would need to know the transversal lines are geometrically (proven) to be collinear
            if (!crossingTransversalSegment.HasSubSegment(transversal) || !standsOnTransversalSegment.HasSubSegment(transversal)) return newGrounded;

            Point standsOnTop = nonTransversalStands.OtherPoint(standsOn.intersect);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(standsOnTop, nonTransversalCrossing.Point1);
            Point testingIntersection = transversal.FindIntersection(testingCrossSegment);

            Point crossingTop = null;
            Point crossingBottom = null;

            if (Segment.Between(testingIntersection, standsOn.intersect, crossingInter.intersect))
            {
                crossingTop = nonTransversalCrossing.Point2;
                crossingBottom = nonTransversalCrossing.Point1;
            }
            else
            {
                crossingTop = nonTransversalCrossing.Point1;
                crossingBottom = nonTransversalCrossing.Point2;
            }

            // Point that is outside of the parallel lines and transversal
            Point offCrossing = Point.calcDistance(crossingTransversalSegment.Point1, crossingInter.intersect) < Point.calcDistance(crossingTransversalSegment.Point1, crossingInter.intersect) ?
                                                   crossingTransversalSegment.Point1 : crossingTransversalSegment.Point2;
            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(standsOnTop, standsOn.intersect, crossingInter.intersect),
                                                                         new Angle(crossingTop, crossingInter.intersect, offCrossing), NAME);
            gca.MakeAxiomatic();
            newAngleRelations.Add(gca);

            // It is possible for the crossing intersection to extend the 'standOn' intersection to create a second angle set
            //Point offCrossing2 = crossingTransversalSegment.OtherPoint(offCrossing);
            //if (standsOn.intersect.Equals(offCrossing2))
            //{
            //    gca = new GeometricCongruentAngles(new Angle(standsOnTop, standsOn.intersect, crossingInter.intersect),
            //                                       new Angle(crossingTop, crossingInter.intersect, offCrossing), NAME);
            //    gca1.MakeAxiomatic();
            //    newAngleRelations.Add(gca1);
            //}

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(standsOn);
            antecedent.Add(crossingInter);

            foreach (CongruentAngles newAngles in newAngleRelations)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAngles));
            }

            return newGrounded;
        }

        // Corresponding angles if (we have 7 points here)
        //
        //      |          |           |         |
        //    __|__________|           |_________|___ off
        //      |          |           |         |
        //      |          |           |
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateMixedIntersection(Parallel parallel, Intersection standsOn, Intersection crossingInter)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment transversal = standsOn.AcquireTransversal(crossingInter);

            // Ensure the non-traversal segments align with the parallel segments
            Segment nonTransversalStands = standsOn.OtherSegment(transversal);
            Segment nonTransversalCrossing = crossingInter.OtherSegment(transversal);

            Segment crossingTransversalSegment = crossingInter.OtherSegment(nonTransversalCrossing);
            Segment standsOnTransversalSegment = standsOn.OtherSegment(nonTransversalStands);
            
            // Avoid:
            //      |          |
            //    __|  ________|
            //      |          |
            //      |          |
            // Both intersections (transversal segments) must contain the actual transversal
            if (!crossingTransversalSegment.HasSubSegment(transversal) || !standsOnTransversalSegment.HasSubSegment(transversal)) return newGrounded;

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(nonTransversalStands.Point1, nonTransversalCrossing.Point1);
            Point testingIntersection = transversal.FindIntersection(testingCrossSegment);

            Point standsOnTop = null;
            Point standsOnBottom = null;

            Point crossingTop = null;
            Point crossingBottom = null;

            if (Segment.Between(testingIntersection, standsOn.intersect, crossingInter.intersect))
            {
                standsOnTop = nonTransversalStands.Point1;
                standsOnBottom = nonTransversalStands.Point2;

                crossingTop = nonTransversalCrossing.Point2;
                crossingBottom = nonTransversalCrossing.Point1;
            }
            else
            {
                standsOnTop = nonTransversalStands.Point1;
                standsOnBottom = nonTransversalStands.Point2;

                crossingTop = nonTransversalCrossing.Point1;
                crossingBottom = nonTransversalCrossing.Point2;
            }


            // Point that is outside of the parallel lines and transversal
            //Point offCrossing = crossingInter.OtherSegment(nonTransversalCrossing).OtherPoint(standsOn.intersect);
            Point offCrossing = Point.calcDistance(crossingTransversalSegment.Point1, crossingInter.intersect) < Point.calcDistance(crossingTransversalSegment.Point1, crossingInter.intersect) ?
                                                   crossingTransversalSegment.Point1 : crossingTransversalSegment.Point2;

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca1 = new GeometricCongruentAngles(new Angle(standsOnTop, standsOn.intersect, crossingInter.intersect),
                                                                         new Angle(crossingTop, crossingInter.intersect, offCrossing), NAME);
            gca1.MakeAxiomatic();
            GeometricCongruentAngles gca2 = new GeometricCongruentAngles(new Angle(standsOnBottom, standsOn.intersect, crossingInter.intersect),
                                                                         new Angle(crossingBottom, crossingInter.intersect, offCrossing), NAME);
            gca2.MakeAxiomatic();

            newAngleRelations.Add(gca1);
            newAngleRelations.Add(gca2);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(standsOn);
            antecedent.Add(crossingInter);

            foreach (CongruentAngles newAngles in newAngleRelations)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAngles));
            }

            return newGrounded;
        }

        // Corresponding angles if (we have 8 points here)
        //
        //           |          |
        //    off1 __|__________|__ off2
        //           |          |
        //           |          |
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateCompleteIntersection(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            // Ensure the non-traversal segments align with the parallel segments
            Segment nonTraversalLeft = crossingInterLeft.OtherSegment(transversal);
            Segment nonTraversalRight = crossingInterRight.OtherSegment(transversal);

            Segment crossingLeftParallel = parallel.CoincidesWith(nonTraversalLeft);
            Segment crossingRightParallel = parallel.CoincidesWith(nonTraversalRight);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point testingIntersection = transversal.FindIntersection(testingCrossSegment);

            Point crossingLeftTop = null;
            Point crossingLeftBottom = null;

            Point crossingRightTop = null;
            Point crossingRightBottom = null;

            if (Segment.Between(testingIntersection, crossingInterLeft.intersect, crossingInterRight.intersect))
            {
                crossingLeftTop = crossingLeftParallel.Point1;
                crossingLeftBottom = crossingLeftParallel.Point2;

                crossingRightTop = crossingRightParallel.Point2;
                crossingRightBottom = crossingRightParallel.Point1;
            }
            else
            {
                crossingLeftTop = crossingLeftParallel.Point1;
                crossingLeftBottom = crossingLeftParallel.Point2;

                crossingRightTop = crossingRightParallel.Point1;
                crossingRightBottom = crossingRightParallel.Point2;
            }


            // Point that is outside of the parallel lines and transversal
            Segment leftNonParallel = crossingInterLeft.OtherSegment(crossingLeftParallel);
            Segment rightNonParallel = crossingInterRight.OtherSegment(crossingRightParallel);

            Point offCrossingLeft = Point.calcDistance(leftNonParallel.Point1, crossingInterLeft.intersect) < Point.calcDistance(leftNonParallel.Point1, crossingInterRight.intersect) ?
                                    leftNonParallel.Point1 : leftNonParallel.Point2;
            Point offCrossingRight = Point.calcDistance(rightNonParallel.Point1, crossingInterRight.intersect) < Point.calcDistance(rightNonParallel.Point1, crossingInterLeft.intersect) ?
                                     rightNonParallel.Point1 : rightNonParallel.Point2;

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca1 = new GeometricCongruentAngles(new Angle(crossingLeftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                         new Angle(crossingRightTop, crossingInterRight.intersect, offCrossingRight), NAME);
            gca1.MakeAxiomatic();
            GeometricCongruentAngles gca2 = new GeometricCongruentAngles(new Angle(crossingLeftTop, crossingInterLeft.intersect, offCrossingLeft),
                                                                         new Angle(crossingRightTop, crossingInterRight.intersect, crossingInterLeft.intersect), NAME);
            gca2.MakeAxiomatic();
            GeometricCongruentAngles gca3 = new GeometricCongruentAngles(new Angle(crossingLeftBottom, crossingInterLeft.intersect, offCrossingLeft),
                                                                         new Angle(crossingRightBottom, crossingInterRight.intersect, crossingInterLeft.intersect), NAME);
            gca3.MakeAxiomatic();
            GeometricCongruentAngles gca4 = new GeometricCongruentAngles(new Angle(crossingLeftBottom, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                         new Angle(crossingRightBottom, crossingInterRight.intersect, offCrossingRight), NAME);
            gca4.MakeAxiomatic();

            newAngleRelations.Add(gca1);
            newAngleRelations.Add(gca2);
            newAngleRelations.Add(gca3);
            newAngleRelations.Add(gca4);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(crossingInterLeft);
            antecedent.Add(crossingInterRight);

            foreach (CongruentAngles newAngles in newAngleRelations)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAngles));
            }

            return newGrounded;
        }
    }
}