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

            // We want two intersections that create a transversal
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            // If both intersections 'stand on' the respective segment, there are no corresponding angles
            if (inter1.StandsOn() && inter2.StandsOn()) return newGrounded;

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

            //
            // Define the 4 sets of corresponding angles; to do so we need 8 points for 4 angles
            //
            // Group 1
            Point off1 = transversal.Point1;
            Point off2 = transversal.Point2;

            // No corresponding angles if we have;                              4 sets if as follows
            //
            //    |          |                                                  |        |
            //    |__________|                                           off1 __|________|___ off2
            //    |          |                                                  |        |
            //    |          |                                                  |        |
            //
            if (coincidingParallel1.PointIsOn(off1) && coincidingParallel1.PointIsOn(off2)) return newGrounded;

            // Acquire the intersections
            Point inter1Point = inter1.intersect;
            Point inter2Point = inter2.intersect;

            // These lines give the other 4 points
            Segment parallel1 = inter1.OtherSegment(transversal);
            Segment parallel2 = inter2.OtherSegment(transversal);

            // Create a segment from these two points so we can compare distances
            Segment crossing = new Segment(parallel1.Point1, parallel2.Point1);

            //
            // Will this crossing segment intersect the real transversal in the middle of the two segments? If it DOES NOT, it is same side
            //
            Point intersection = transversal.FindIntersection(crossing);

            Point sameSideOfTransversal11 = null;
            Point sameSideOfTransversal12 = null;

            Point sameSideOfTransversal21 = null;
            Point sameSideOfTransversal22 = null;

            if (Segment.Between(intersection, inter1Point, inter2Point))
            {
                sameSideOfTransversal11 = parallel1.Point1;
                sameSideOfTransversal12 = parallel2.Point2;

                sameSideOfTransversal21 = parallel1.Point2;
                sameSideOfTransversal22 = parallel2.Point1;
            }
            else
            {
                sameSideOfTransversal11 = parallel1.Point1;
                sameSideOfTransversal12 = parallel2.Point1;

                sameSideOfTransversal21 = parallel1.Point2;
                sameSideOfTransversal22 = parallel2.Point2;
            }

            //
            // We need to check to see if the endpoints of the transversal are 'beyond' the parallel lines
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();
            if (!coincidingParallel1.PointIsOn(off1))
            {
                // 'Left' Angles
                GeometricCongruentAngles gca1 = new GeometricCongruentAngles(new Angle(off1, inter1Point, sameSideOfTransversal11),
                                                          new Angle(inter1Point, inter2Point, sameSideOfTransversal12), NAME);
                gca1.MakeAxiomatic();
                GeometricCongruentAngles gca2 = new GeometricCongruentAngles(new Angle(off1, inter1Point, sameSideOfTransversal21),
                                                          new Angle(inter1Point, inter2Point, sameSideOfTransversal22), NAME);
                gca2.MakeAxiomatic();

                newAngleRelations.Add(gca1);
                newAngleRelations.Add(gca2);
            }

            if (!coincidingParallel1.PointIsOn(off2))
            {
                // 'Right' Angles
                GeometricCongruentAngles gca3 = new  GeometricCongruentAngles(new Angle(sameSideOfTransversal11, inter1Point, inter2Point),
                                                          new Angle(sameSideOfTransversal12, inter2Point, off2), NAME);
                gca3.MakeAxiomatic();
                GeometricCongruentAngles gca4 = new  GeometricCongruentAngles(new Angle(sameSideOfTransversal21, inter1Point, inter2Point),
                                                          new Angle(sameSideOfTransversal22, inter2Point, off2), NAME);
                gca4.MakeAxiomatic();

                newAngleRelations.Add(gca3);
                newAngleRelations.Add(gca4);
            }

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