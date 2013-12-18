using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class ParallelImplyAltIntCongruentAngles : Theorem
    {
        private readonly static string NAME = "Parallel Lines Imply Congruent Alternate Interior Angles";

        public ParallelImplyAltIntCongruentAngles() { }

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

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
            Segment transversal = inter1.CommonSegment(inter2);
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

            // Both intersections should not be referring to an intersection point on the same parallel segment
            if (parallel.segment1.PointIsOn(inter1.intersect) && parallel.segment1.PointIsOn(inter2.intersect)) return newGrounded;
            if (parallel.segment2.PointIsOn(inter1.intersect) && parallel.segment2.PointIsOn(inter2.intersect)) return newGrounded;


            // The resultant candidate parallel segments shouldn't share any vertices
            if (coincidingParallel1.SharedVertex(coincidingParallel2) != null) return newGrounded;

            //
            // Are these angles on the opposite side of the transversal?
            //
            // Make a simple transversal from the two intersection points
            Segment simpleTransversal = new Segment(inter1.intersect, inter2.intersect);

            //    A_______M________B
            //           /
            //          /
            //   ______/_______
            //  C      N       D
            // Verify which side the outer points of the parallel segments lie; that is, is B on same side as C or alternate side?

            List<GeometricCongruentAngles> newCongruences = new List<GeometricCongruentAngles>();
            
            // Create a segment from these two points so we can compare distances
            Segment crossing = new Segment(coincidingParallel1.Point1, coincidingParallel2.Point1);

            // Will this crossing segment actually intersect the real transversal in the middle of the two segments
            Point intersection = transversal.FindIntersection(crossing);

            // If the intersection is between the two intersection points, they are points are on the opposite side
            Angle altIntAngle1 = null;  // Pair 1
            Angle altIntAngle2 = null;
            Angle altIntAngle3 = null;  // Pair 2
            Angle altIntAngle4 = null;
            if (Segment.Between(intersection, inter1.intersect, inter2.intersect))
            {
                // Do we have an enclosed situation?
                //  __________
                //      /
                //     /
                //    /_________
                //
                if (inter1.StandsOnEndpoint() || inter2.StandsOnEndpoint())
                {
                    altIntAngle1 = new Angle(coincidingParallel1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
                    altIntAngle2 = new Angle(coincidingParallel2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);
                    newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
                }
                else
                {
                    altIntAngle1 = new Angle(coincidingParallel1.Point1, inter1.intersect, inter2.intersect);
                    altIntAngle2 = new Angle(coincidingParallel2.Point1, inter2.intersect, inter1.intersect);

                    altIntAngle3 = new Angle(coincidingParallel1.Point2, inter1.intersect, inter2.intersect);
                    altIntAngle4 = new Angle(coincidingParallel2.Point2, inter2.intersect, inter1.intersect);

                    newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
                    newCongruences.Add(new GeometricCongruentAngles(altIntAngle3, altIntAngle4, NAME));
                }
            }
            else
            {
                // We cannot have an enclosed situation
                //  __________
                //      /
                //     /
                //    /_________
                //
                if (inter1.StandsOnEndpoint() || inter2.StandsOnEndpoint()) return newGrounded;

                altIntAngle1 = new Angle(coincidingParallel1.Point1, inter1.intersect, inter2.intersect);
                altIntAngle2 = new Angle(coincidingParallel2.Point2, inter2.intersect, inter1.intersect);

                altIntAngle3 = new Angle(coincidingParallel1.Point2, inter1.intersect, inter2.intersect);
                altIntAngle4 = new Angle(coincidingParallel2.Point1, inter2.intersect, inter1.intersect);

                newCongruences.Add(new GeometricCongruentAngles(altIntAngle1, altIntAngle2, NAME));
                newCongruences.Add(new GeometricCongruentAngles(altIntAngle3, altIntAngle4, NAME));
            }
            //
            // Now we have the desired construction
            //

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(parallel);

            foreach (GeometricCongruentAngles gcas in newCongruences)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gcas));
            }

            return newGrounded;
        }
    }
}