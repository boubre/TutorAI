using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{
    public class ParallelImplySameSideInteriorSupplementary : Theorem
    {
        private readonly static string NAME = "If two parallel lines are cut by a transversal, then Same-Side Interior Angles are Supplementary.";

        public ParallelImplySameSideInteriorSupplementary() { }

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

        //
        // Parallel(Segment(C, D), Segment(E, F)),
        // Intersection(M, Segment(A,B), Segment(C, D)),
        // Intersection(N, Segment(A,B), Segment(E, F))-> Supplementary(Angle(F, N, M), Angle(N, M, D))
        //                                                Supplementary(Angle(E, N, M), Angle(N, M, C))
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
            // Check to see if the intersections coincide with the parallel segments
            //
            Segment parallelSegment1 = inter1.OtherSegment(transversal);
            Segment parallelSegment2 = inter2.OtherSegment(transversal);

            // The resultant candidate parallel segments shouldn't share any vertices
            if (parallelSegment1.SharedVertex(parallelSegment2) != null) return newGrounded;

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

            // Create a segment from these two points so we can compare distances
            Segment crossing = new Segment(parallelSegment1.Point1, parallelSegment2.Point1);

            // Will this crossing segment actually intersect the real transversal in the middle of the two segments
            Point intersection = transversal.FindIntersection(crossing);

            // If the intersection is between the two intersection points, they are points are on the opposite side
            Angle sameSideIntAngle1 = null;  // Pair 1
            Angle sameSideIntAngle2 = null;
            Angle sameSideIntAngle3 = null;  // Pair 2
            Angle sameSideIntAngle4 = null;
            // This is the opposite side with the two points
            List<Supplementary> newSupps = new List<Supplementary>();
            if (Segment.Between(intersection, inter1.intersect, inter2.intersect))
            {
                // Do we have an enclosed situation?
                //  __________
                //      /
                //     /
                //    /_________
                //
                if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                if (inter1.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.Point2, inter2.intersect, inter1.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                else if (inter2.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment1.Point1, inter1.intersect, inter2.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                else
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.Point1, inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.Point2, inter2.intersect, inter1.intersect);

                    sameSideIntAngle3 = new Angle(parallelSegment1.Point2, inter1.intersect, inter2.intersect);
                    sameSideIntAngle4 = new Angle(parallelSegment2.Point1, inter2.intersect, inter1.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                    newSupps.Add(new Supplementary(sameSideIntAngle3, sameSideIntAngle4, NAME));
                }
            }
            // This is the same side with the two points
            else
            {
                // Do we have an enclosed situation?
                //  __________
                //      /
                //     /
                //    /_________
                //
                if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                if (inter1.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.OtherPoint(inter1.intersect), inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.Point1, inter2.intersect, inter1.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                else if (inter2.StandsOnEndpoint())
                {
                    sameSideIntAngle1 = new Angle(parallelSegment2.OtherPoint(inter2.intersect), inter2.intersect, inter1.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment1.Point2, inter1.intersect, inter2.intersect);

                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                }
                else
                {
                    sameSideIntAngle1 = new Angle(parallelSegment1.Point1, inter1.intersect, inter2.intersect);
                    sameSideIntAngle2 = new Angle(parallelSegment2.Point1, inter2.intersect, inter1.intersect);

                    sameSideIntAngle3 = new Angle(parallelSegment1.Point2, inter1.intersect, inter2.intersect);
                    sameSideIntAngle4 = new Angle(parallelSegment2.Point2, inter2.intersect, inter1.intersect);
                
                    newSupps.Add(new Supplementary(sameSideIntAngle1, sameSideIntAngle2, NAME));
                    newSupps.Add(new Supplementary(sameSideIntAngle3, sameSideIntAngle4, NAME));
                }
            }

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(parallel);

            foreach (Supplementary supp in newSupps)
            {
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp));
            }

            return newGrounded;
        }
    }
}