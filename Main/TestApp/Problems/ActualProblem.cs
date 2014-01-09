using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public abstract class ActualProblem
    {
        protected List<GroundedClause> intrinsic;
        protected List<GroundedClause> given;

        public ActualProblem()
        {
            intrinsic = new List<GroundedClause>();
            given = new List<GroundedClause>();
        }

        public virtual void Run()
        {
            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, given);
        }

        public virtual void Report()
        {

        }

        //
        // Given a series of points, generate all objects associated with segments and InMiddles
        //
        protected List<GroundedClause> GenerateSegmentClauses(Collinear collinear)
        {
            List<GroundedClause> newClauses = new List<GroundedClause>();

            //
            // Generate all Segment and InMiddle objects
            //
            for (int p1 = 0; p1 < collinear.points.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < collinear.points.Count; p2++)
                {
                    Segment newSegment = new Segment(collinear.points[p1], collinear.points[p2]);
                    newClauses.Add(newSegment);
                    for (int imIndex = p1 + 1; imIndex < p2; imIndex++)
                    {
                        newClauses.Add(new InMiddle(collinear.points[imIndex], newSegment, "INTRINSIC"));
                    }
                }
            }

            return newClauses;
        }

        //
        // Given a series of points, generate all objects associated with segments and InMiddles
        //
        protected List<GroundedClause> GenerateAngleIntersectionTriangleClauses(List<GroundedClause> clauses)
        {
            List<GroundedClause> newClauses = new List<GroundedClause>();

            // Find all the Segment and Point objects
            List<Segment> segments = new List<Segment>();
            List<Point> points = new List<Point>();
            foreach (GroundedClause clause in clauses)
            {
                if (clause is Segment) segments.Add(clause as Segment);
                if (clause is Point) points.Add(clause as Point);
            }

            List<GroundedClause> newIntersections = new List<GroundedClause>();
            //
            // Generate all Intersection objects
            //
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    // An intersection should not be between collinear segments
                    if (!segments[s1].IsCollinearWith(segments[s2]))
                    {
                        // The point must be 'between' both segment endpoints
                        Point numericInter = segments[s1].FindIntersection(segments[s2]);
                        if (segments[s1].PointIsOnAndBetweenEndpoints(numericInter) && segments[s2].PointIsOnAndBetweenEndpoints(numericInter))
                        {
                            // Find the actual point for which there is an intersection between the segments
                            Point actInter = null;
                            foreach (Point pt in points)
                            {
                                if (numericInter.StructurallyEquals(pt))
                                {
                                    actInter = pt;
                                    break;
                                }
                            }

                            // Create the intersection
                            if (actInter != null)
                            {
                                newIntersections.Add(new Intersection(actInter, segments[s1], segments[s2], "Intrinsic"));
                            }
                        }
                    }
                }
            }

            //
            // Generate all Angle objects
            //
            List<Angle> newAngles = new List<Angle>();
            foreach (Intersection inter in newIntersections)
            {
                // 1 angle
                if (inter.StandsOnEndpoint())
                {
                    AddAngle(newAngles, (new Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect))));
                }
                // 2 angles
                else if (inter.StandsOn())
                {
                    Point up = null;
                    Point left = null;
                    Point right = null;
                    if (inter.lhs.HasPoint(inter.intersect))
                    {
                        up = inter.lhs.OtherPoint(inter.intersect);
                        left = inter.rhs.Point1;
                        right = inter.rhs.Point2;
                    }
                    else
                    {
                        up = inter.rhs.OtherPoint(inter.intersect);
                        left = inter.lhs.Point1;
                        right = inter.lhs.Point2;
                    }

                    AddAngle(newAngles, new Angle(left, inter.intersect, up));
                    AddAngle(newAngles, new Angle(right, inter.intersect, up));
                }
                // 4 angles
                else
                {
                    AddAngle(newAngles, new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                    AddAngle(newAngles, new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                    AddAngle(newAngles, new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                    AddAngle(newAngles, new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
                }
            }

            //
            // Generate Triangle Clauses
            //
            List<Triangle> newTriangles = new List<Triangle>();
            for (int s1 = 0; s1 < segments.Count - 2; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count - 1; s2++)
                {
                    Point vertex1 = segments[s1].SharedVertex(segments[s2]);
                    if (vertex1 != null)
                    {
                        for (int s3 = s2 + 1; s3 < segments.Count; s3++)
                        {
                            Point vertex2 = segments[s3].SharedVertex(segments[s1]);
                            Point vertex3 = segments[s3].SharedVertex(segments[s2]);
                            if (vertex2 != null && vertex3 != null)
                            {
                                // Vertices must be distinct
                                if (!vertex1.Equals(vertex2) && !vertex1.Equals(vertex3) && !vertex2.Equals(vertex3))
                                {
                                    // Vertices must be non-collinear
                                    Segment side1 = new Segment(vertex1, vertex2);
                                    Segment side2 = new Segment(vertex2, vertex3);
                                    Segment side3 = new Segment(vertex1, vertex3);
                                    if (!side1.IsCollinearWith(side2))
                                    {
                                        // Construct the triangle based on the sides to ensure reflexivity clauses are generated

                                        newTriangles.Add(new Triangle(GetProblemSegment(clauses, side1), GetProblemSegment(clauses, side2), GetProblemSegment(clauses, side3), "Intrinsic"));
                                        System.Diagnostics.Debug.WriteLine(newTriangles[newTriangles.Count - 1].ToString());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            newClauses.AddRange(newIntersections);
            newClauses.AddRange(newAngles);
            newClauses.AddRange(newTriangles);

            return newClauses;
        }

        // Add an angle to the list uniquely
        private void AddAngle(List<Angle> angles, Angle thatAngle)
        {
            if (thatAngle.measure == 0 || thatAngle.measure == 180)
            {
                System.Diagnostics.Debug.WriteLine("");
            }

            foreach (Angle thisAngle in angles)
            {
                if (thisAngle.Equates(thatAngle)) return;
            }

            angles.Add(thatAngle);
        }

        // Add an angle to the list uniquely
        protected Segment GetProblemSegment(List<GroundedClause> clauses, Segment thatSegment)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatSegment)) return clause as Segment;
            }

            return null;
        }

        // Add an angle to the list uniquely
        protected Angle GetProblemAngle(List<GroundedClause> clauses, Angle thatAngle)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatAngle)) return clause as Angle;
            }

            return null;
        }
    }
}