using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class TriangleProportionality : Theorem
    {
        private readonly static string NAME = "A Line Parallel to One Side of a Triangle and Intersects the Other Two Sides, then it Divides the Sides Proportionally";

        public TriangleProportionality() { }

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Triangle> candTriangle = new List<Triangle>();
        private static List<Parallel> candParallel = new List<Parallel>();

        //
        // Triangle(A, B, C),
        // Intersection(D, Segment(A,B), Segment(D, E)),
        // Intersection(E, Segment(A,C), Segment(D, E)),
        // Parallel(Segment(D, E), Segment(B, C)) -> Proportional(Segment(A, D), Segment(A, B)),
        //                                           Proportional(Segment(A, E), Segment(A, C))
        //            A
        //           /\
        //          /  \
        //         /    \
        //      D /------\ E
        //       /        \
        //    B /__________\ C
        //
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Parallel) && !(c is Intersection) && !(c is Triangle)) return newGrounded;

            if (c is Parallel)
            {
                Parallel parallel = c as Parallel;

                foreach (Triangle tri in candTriangle)
                {
                    for (int i = 0; i < candIntersection.Count; i++)
                    {
                        for (int j = i + 1; j < candIntersection.Count; j++)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(tri, candIntersection[i], candIntersection[j], parallel));
                        }
                    }
                }

                candParallel.Add(parallel);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                foreach (Triangle tri in candTriangle)
                {
                    foreach (Intersection inter in candIntersection)
                    {
                        foreach (Parallel parallel in candParallel)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(tri, newIntersection, inter, parallel));
                        }
                    }
                }

                candIntersection.Add(newIntersection);
            }
            else if (c is Triangle)
            {
                Triangle newTriangle = c as Triangle;

                for (int i = 0; i < candIntersection.Count; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        foreach (Parallel parallel in candParallel)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(newTriangle, candIntersection[i], candIntersection[j], parallel));
                        }
                    }
                }

                candTriangle.Add(newTriangle);
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckAndGenerateProportionality(Triangle tri,
                                                                                                           Intersection inter1,
                                                                                                           Intersection inter2, Parallel parallel)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The two intersections should not be at the same vertex
            if (inter1.intersect.Equals(inter2.intersect)) return newGrounded;

            //
            // Do these intersections share a segment? That is, do they share the transversal?
            //
            Segment transversal = inter1.CommonSegment(inter2);

            if (transversal == null) return newGrounded;

            //
            // Is the transversal a side of the triangle? It should not be.
            //
            if (tri.LiesOn(transversal)) return newGrounded;

            //
            // Determine if one parallel segment is a side of the triangle
            //
            Segment coinciding = tri.DoesParallelCoincideWith(parallel);

            if (coinciding == null) return newGrounded;

            // The transversal and common segment must be distinct
            if (coinciding.IsCollinearWith(transversal)) return newGrounded;

            //
            // Determine if the simplified transversal is within the parallel relationship.
            //
            Segment parallelTransversal = parallel.OtherSegment(coinciding);
            Segment simpleParallelTransversal = new Segment(inter1.intersect, inter2.intersect);

            if (!parallelTransversal.IsCollinearWith(simpleParallelTransversal)) return newGrounded;

            //
            // Are the endpoints of the simplified transversal on opposite sides of the triangle (sides distinct from the coinciding line)
            //
            KeyValuePair<Segment, Segment> otherSides = tri.OtherSides(coinciding);
            Segment otherSide1 = otherSides.Key;
            Segment otherSide2 = otherSides.Value;

            // Acquire the exact points
            Point pointOnSide1 = null;
            Point pointOnSide2 = null;
            if (otherSide1.PointIsOnAndBetweenEndpoints(simpleParallelTransversal.Point1))
            {
                pointOnSide1 = simpleParallelTransversal.Point1;
                if (otherSide2.PointIsOnAndBetweenEndpoints(simpleParallelTransversal.Point2))
                {
                    pointOnSide2 = simpleParallelTransversal.Point2;
                }
            }
            else if (otherSide2.PointIsOnAndBetweenEndpoints(simpleParallelTransversal.Point1))
            {
                pointOnSide2 = simpleParallelTransversal.Point1;
                if (otherSide1.PointIsOnAndBetweenEndpoints(simpleParallelTransversal.Point2))
                {
                    pointOnSide1 = simpleParallelTransversal.Point2;
                }
            }

            // Failed to find points on each side directly (between the endpoints)
            if (pointOnSide1 == null || pointOnSide2 == null) return newGrounded;

            // If the second point is not on the side of the triangle (this should never happen)
            if (pointOnSide1.Equals(pointOnSide2)) return newGrounded;

            //
            // Construct the new proprtional relationships
            //
            Point sharedVertex = otherSide1.SharedVertex(otherSide2);
            GeometricProportionalSegments newProp1 = new GeometricProportionalSegments(new Segment(sharedVertex, pointOnSide1), otherSide1, NAME);
            GeometricProportionalSegments newProp2 = new GeometricProportionalSegments(new Segment(sharedVertex, pointOnSide2), otherSide2, NAME);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(parallel);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newProp1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newProp2));

            return newGrounded;
        }
    }
}