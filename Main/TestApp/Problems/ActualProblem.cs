using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public abstract class ActualProblem
    {
        public static System.TimeSpan TotalTime = new System.TimeSpan();
        public static int TotalInterestingProblems = 0;
        public static int TotalOriginalBookProblems = 0;
        public static int TotalOriginalBookProblemsGenerated = 0;

        // Hard-coded intrinsic problem characteristics
        protected List<GroundedClause> intrinsic;

        // Boolean facts
        protected List<GroundedClause> given;

        // Boolean the book problem is attempting to prove; use in validation of figures / generated problems
        // One <figure, given> pair may contain multiple goals
        protected List<GroundedClause> goals;

        // The number of original problems for the figure in the textbook
        protected int numberOfOriginalTextProblems;
        protected int numberOfOriginalTextProblemsGenerated;
        private int numberOfInterestingProblems;

        // Formatted Labeling of the Problem
        protected string problemName;

        // For timing of generating all problems from a figure
        private System.TimeSpan timeSpanToGenerate;
        private System.Diagnostics.Stopwatch stopwatch;

        public bool problemIsOn { get; private set; }

        public ActualProblem(bool runOrNot)
        {
            intrinsic = new List<GroundedClause>();
            given = new List<GroundedClause>();
            stopwatch = new System.Diagnostics.Stopwatch();

            // Default to only 1 original problem from the text
            numberOfOriginalTextProblems = 1;
            numberOfOriginalTextProblemsGenerated = 0;
            numberOfInterestingProblems = 0;

            problemName = "TODO: NAME ME" + this.GetType();

            problemIsOn = runOrNot;

            goals = new List<GroundedClause>();
        }

        public void Run()
        {
            // Create the analyzer
            GeometryTutorLib.FigureAnalyzerMain analyzer = new GeometryTutorLib.FigureAnalyzerMain(intrinsic, given, goals);

            // Perform and time the analysis
            stopwatch.Start();
            KeyValuePair<int, int> returnPair = analyzer.AnalyzeFigure();
            stopwatch.Stop();

            numberOfInterestingProblems = returnPair.Key;
            numberOfOriginalTextProblemsGenerated = returnPair.Value;

            // How long did problem generation take?
            timeSpanToGenerate = stopwatch.Elapsed;

            // Add to the cumulative statistics
            ActualProblem.AddToTotalTime(stopwatch);
            ActualProblem.TotalInterestingProblems += numberOfInterestingProblems;
            ActualProblem.TotalOriginalBookProblems += numberOfOriginalTextProblems;
            ActualProblem.TotalOriginalBookProblemsGenerated += numberOfOriginalTextProblemsGenerated;
        }

        private static void AddToTotalTime(System.Diagnostics.Stopwatch stopwatch)
        {
            ActualProblem.TotalTime = ActualProblem.TotalTime.Add(stopwatch.Elapsed);
        }

        public override string ToString()
        {
            string statsString = "";

            statsString += this.problemName;
            int sizeSpace = this.problemName.Length;
            if (sizeSpace > 34) statsString += "\t";
            else
            {
                while (sizeSpace <= 34)
                {
                    statsString += "\t";
                    sizeSpace += 4;
                }
            }

            statsString += this.numberOfOriginalTextProblems + "(" + numberOfOriginalTextProblemsGenerated + ")\t\t\t\t";
            statsString += this.numberOfInterestingProblems + "\t\t\t\t\t\t\t";
            string ratio = string.Format("{0:F2}", ((double)(this.numberOfInterestingProblems) / this.numberOfOriginalTextProblems));
            statsString += ratio + "\t\t\t\t\t\t\t\t";

            // Format and display the elapsed time for this problem
            string elapsedTime = System.String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                      timeSpanToGenerate.Hours, timeSpanToGenerate.Minutes, timeSpanToGenerate.Seconds, timeSpanToGenerate.Milliseconds / 10);
            statsString += elapsedTime;

            return statsString;
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

            List<Triangle> newTriangles = GenerateTriangleClauses(clauses, segments);
            List<Intersection> newIntersections = GenerateIntersectionClauses(newTriangles, segments, points);
            List<Angle> newAngles = GenerateAngleClauses(newIntersections);

            newClauses.AddRange(newAngles);
            newClauses.AddRange(newIntersections);
            newClauses.AddRange(newTriangles);

            if (GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("----------------------------------------");
                foreach (GroundedClause gc in newClauses)
                {
                    System.Diagnostics.Debug.WriteLine(gc.ToString());
                }
            }

            return newClauses;
        }

        //
        // Generate all Triangle clauses based on segments
        //
        private List<Triangle> GenerateTriangleClauses(List<GroundedClause> clauses, List<Segment> segments)
        {
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
                                        if (GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
                                        {
                                            System.Diagnostics.Debug.WriteLine(newTriangles[newTriangles.Count - 1].ToString());
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return newTriangles;
        }

        //
        // Generate all covering intersection clauses; that is, generate maximal intersections (a subset of all intersections)
        //
        private List<Intersection> GenerateIntersectionClauses(List<Triangle> triangles, List<Segment> segments, List<Point> points)
        {
            List<Intersection> newIntersections = new List<Intersection>();

            //
            // Each triangle has 3 valid intersections
            //
            foreach (Triangle triangle in triangles)
            {
                Point vertex = triangle.SegmentA.SharedVertex(triangle.SegmentB);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentA, triangle.SegmentB, "Intrinsic"));

                vertex = triangle.SegmentB.SharedVertex(triangle.SegmentC);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentB, triangle.SegmentC, "Intrinsic"));
                
                vertex = triangle.SegmentA.SharedVertex(triangle.SegmentC);
                AddIntersection(newIntersections, new Intersection(vertex, triangle.SegmentA, triangle.SegmentC, "Intrinsic"));
            }

            //
            // Find the maximal segments (remove all sub-segments from the list)
            //
            List<Segment> maximalSegments = new List<Segment>();
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool isSubsegment = false;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s2].HasSubSegment(segments[s1]))
                        {
                            isSubsegment = true;
                            break;
                        }
                    }
                }
                if (!isSubsegment) maximalSegments.Add(segments[s1]);
            }

            //
            // Acquire all intersections from the maximal segment list
            //
            for (int s1 = 0; s1 < maximalSegments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < maximalSegments.Count; s2++)
                {
                    // An intersection should not be between collinear segments
                    if (!maximalSegments[s1].IsCollinearWith(maximalSegments[s2]))
                    {
                        // The point must be 'between' both segment endpoints
                        Point numericInter = maximalSegments[s1].FindIntersection(maximalSegments[s2]);
                        if (maximalSegments[s1].PointIsOnAndBetweenEndpoints(numericInter) &&
                            maximalSegments[s2].PointIsOnAndBetweenEndpoints(numericInter))
                        {
                            // Find the actual point for which there is an intersection between the segments
                            Point actualInter = null;
                            foreach (Point pt in points)
                            {
                                if (numericInter.StructurallyEquals(pt))
                                {
                                    actualInter = pt;
                                    break;
                                }
                            }

                            // Create the intersection
                            if (actualInter != null)
                            {
                                AddIntersection(newIntersections, new Intersection(actualInter, maximalSegments[s1], maximalSegments[s2], "Intrinsic"));
                            }
                        }
                    }
                }
            }

            return newIntersections;
        }

        //
        // Generate all angles based on the intersections
        //
        private List<Angle> GenerateAngleClauses(List<Intersection> intersections)
        {
            List<Angle> newAngles = new List<Angle>();

            foreach (Intersection inter in intersections)
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

            return newAngles;
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

        // Add an intersection to the list uniquely
        private void AddIntersection(List<Intersection> intersections, Intersection thatInter)
        {
            foreach (Intersection inter in intersections)
            {
                if (inter.StructurallyEquals(thatInter)) return;
            }

            intersections.Add(thatInter);
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

        // Acquire an established angle
        protected Angle GetProblemAngle(List<GroundedClause> clauses, Angle thatAngle)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatAngle)) return clause as Angle;
            }

            return null;
        }

        // Acquire an established triangle
        protected Triangle GetProblemTriangle(List<GroundedClause> clauses, Triangle thatTriangle)
        {
            foreach (GroundedClause clause in clauses)
            {
                if (clause.StructurallyEquals(thatTriangle)) return clause as Triangle;
            }

            return null;
        }

        // Acquire an established intersection
        protected Intersection GetProblemIntersection(List<GroundedClause> clauses, Segment segment1, Segment segment2)
        {
            foreach (GroundedClause clause in clauses)
            {
                Intersection inter = clause as Intersection;
                if (inter != null)
                {
                    if (inter.HasSegment(segment1) && inter.HasSegment(segment2)) return inter;
                }
            }

            return null;
        }
    }
}