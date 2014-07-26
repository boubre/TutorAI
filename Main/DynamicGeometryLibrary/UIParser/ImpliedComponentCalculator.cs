using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Live Geometry does not define ALL components of the figure, we must acquire those implied components.
    /// </summary>
    public class ImpliedComponentCalculator
    {
        //
        // Minimum components that are determined externally.
        //
        public List<GeometryTutorLib.ConcreteAST.Point> points { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Segment> segments { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Circle> circles { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons { get; private set; }

        //
        // Implied components calculated in this class.
        //
        public List<Collinear> collinear { get; private set; }
        public List<InMiddle> inMiddles { get; private set; }

        // UI named points and those unlabeled points due to intersection: points we can see in a drawing.
        public List<GeometryTutorLib.ConcreteAST.Point> allFigurePoints { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Point> unlabeledPoints { get; private set; }

        public List<Intersection> ssIntersections { get; private set; }
        public List<Angle> angles { get; private set; }
        public List<MinorArc> minorArcs { get; private set; }
        public List<MajorArc> majorArcs { get; private set; }
        public List<Semicircle> semiCircles { get; private set; }
        public List<Sector> minorSectors { get; private set; }
        public List<Sector> majorSectors { get; private set; }
        public List<ArcInMiddle> arcInMiddle { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.CircleSegmentIntersection> csIntersections { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.CircleCircleIntersection> ccIntersections { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.SegmentBisector> segmentBisectors { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.AngleBisector> angleBisectors { get; private set; }

        // For private use in constructing intersections only
        public List<GeometryTutorLib.ConcreteAST.Segment> maximalSegments { get; private set; }

        // For atomic region id: graph construction.
        public List<GeometryTutorLib.ConcreteAST.Segment> minimalSegments { get; private set; }

        // The atomic regions for this figure.
        public List<AtomicRegion> atomicRegions { get; private set; }

        // Imaginary points due to constructed segments.
        public List<ImaginaryPoint> imagPoints { get; private set; }

        //
        // Construction requires this minimal set from the UI.
        //
        public ImpliedComponentCalculator(List<GeometryTutorLib.ConcreteAST.Point> pts,
                                          List<GeometryTutorLib.ConcreteAST.Segment> segs,
                                          List<GeometryTutorLib.ConcreteAST.Circle> circs,
                                          List<GeometryTutorLib.ConcreteAST.Polygon>[] polys)
        {
            points = pts;
            segments = segs;
            circles = circs;
            polygons = polys;

            collinear = new List<Collinear>();

            ConstructCommonComponents();
        }

        //
        // Construct requires this minimal set from a hard-coded test.
        //
        public ImpliedComponentCalculator(List<GeometryTutorLib.ConcreteAST.Point> pts,
                                          List<GeometryTutorLib.ConcreteAST.Collinear> coll,
                                          List<GeometryTutorLib.ConcreteAST.Segment> segs,
                                          List<GeometryTutorLib.ConcreteAST.Circle> circs)
        {
            points = pts;
            collinear = coll;
            segments = segs;
            circles = circs;
            polygons = DirectComponentsFromUI.ConstructPolygonContainer();

            ConstructCommonComponents();
        }

        private void ConstructCommonComponents()
        {
            inMiddles = new List<InMiddle>();
            allFigurePoints = new List<Point>(points);
            unlabeledPoints = new List<Point>();
            ssIntersections = new List<Intersection>();
            angles = new List<Angle>();
            minorArcs = new List<MinorArc>();
            majorArcs = new List<MajorArc>();
            semiCircles = new List<Semicircle>();
            minorSectors = new List<Sector>();
            majorSectors = new List<Sector>();
            arcInMiddle = new List<ArcInMiddle>();
            csIntersections = new List<CircleSegmentIntersection>();
            ccIntersections = new List<CircleCircleIntersection>();
            segmentBisectors = new List<GeometryTutorLib.ConcreteAST.SegmentBisector>();
            angleBisectors = new List<GeometryTutorLib.ConcreteAST.AngleBisector>();

            maximalSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            minimalSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();

            imagPoints = new List<ImaginaryPoint>();
            atomicRegions = new List<AtomicRegion>();
        }

        public void ConstructAllImplied()
        {
            //
            // Calculate all the important points of intersection among shapes (as well as Circle-Circle intersections).
            //
            ShapeIntersectionCalculator shapeIntCalc = new ShapeIntersectionCalculator(this);
            List<CircleCircleIntersection> tempCCInter = new List<CircleCircleIntersection>();
            shapeIntCalc.CalcCircleCircleIntersections(out tempCCInter);
            ccIntersections = tempCCInter;

            //
            // Generate ALL segment clauses
            //
            // Have we done this before? Hard-coded provides collinear relationships.
            CalculateCollinear();
            GenerateSegmentClauses();

            // Acquire all of the unlabeled points from the UI drawing.
            DrawingPointCalculator unlabeledCalc = new DrawingPointCalculator(points, segments, circles);
            unlabeledPoints = unlabeledCalc.GetUnlabeledPoints();

            // Add the implied points to the complete list of points.
            allFigurePoints.AddRange(unlabeledPoints);

            // Using only segments, identify all polygons which are implied.
            PolygonCalculator polyCalc = new PolygonCalculator(segments);
            polygons = polyCalc.GetPolygons();

            shapeIntCalc.CalcCirclePolygonIntersectionPoints();
            shapeIntCalc.CalcPolygonPolygonIntersectionPoints();

            // Determine what shapes are contained within what other shapes.
            ShapeContainmentCalculator shapeContainCalc = new ShapeContainmentCalculator(this);
            shapeContainCalc.CalcCircleCircleContainment();
            shapeContainCalc.CalcCirclePolygonContainment();
            shapeContainCalc.CalcPolygonPolygonContainment();

            // Calculate all (selective) Segment-Segment Intersections
            CalculateIntersections();

            // Find all circle-segment intersection points.
            csIntersections = shapeIntCalc.FindCircleSegmentIntersections();

            // Find all the angles based on intersections; duplicates are removed.
            CalculateAngles();

            // Add the implied points found from circle interactions.
            // allFigurePoints.AddRange(impliedCirclePoints);

            // Identify inscribed and circumscribed situations between a circle and polygon.
            AnalyzeCirclePolygonInscription();

            // Determine which of the UI and implied (intersection) points apply to each circle.
            // Generates all arc clauses and arcInMiddle clauses.
            AnalyzeAllCirclePointRelationships();

            // Generate all implicit chords and diameters (for atomic region id) and associated extended points
            //GenerateImplicitChords();

            //
            // All of the following calculations are used in stating the assumptions (user-defined givens)
            //
            CalculateSegmentBisectors();
            CalculateAngleBisectors();

// Define in Properties->Build->Compilation Symbols to turn off this section
#if !ATOMIC_REGION_OFF

            List<Sector> localMinorSectors = null;
            List<Sector> localMajorSectors = null;
            List<Semicircle> localSemicircles = null;
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                circle.ConstructImpliedAreaBasedSectors(out localMinorSectors, out localMajorSectors, out localSemicircles);
            }

            //
            // Atomic region identification
            //
            atomicRegions = AtomicRegionIdentifier.AtomicIdentifierMain.GetAtomicRegions(allFigurePoints, circles, polygons);
#endif
        }        

        /// <summary>
        /// Determine which points are collinear with the UI-defined segments.
        /// </summary>
        private void CalculateCollinear()
        {
            //
            // Find the points that lie in the middle of existing segments.
            //
            foreach (Point p in points)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment seg in segments)
                {
                    if (seg.PointLiesOnAndExactlyBetweenEndpoints(p))
                    {
                        seg.AddCollinearPoint(p);
                    }
                }
            }

            //
            // Create the actual collinear statements.
            //
            foreach (GeometryTutorLib.ConcreteAST.Segment seg in segments)
            {
                if (seg.DefinesCollinearity()) collinear.Add(new Collinear(seg.collinear));
            }
        }

        /// <summary>
        /// Given a series of points, generate all objects associated with segments and InMiddles
        /// </summary>
        private void GenerateSegmentClauses()
        {
            foreach (Collinear coll in collinear)
            {
                for (int p1 = 0; p1 < coll.points.Count - 1; p1++)
                {
                    for (int p2 = p1 + 1; p2 < coll.points.Count; p2++)
                    {
                        GeometryTutorLib.ConcreteAST.Segment newSegment = new GeometryTutorLib.ConcreteAST.Segment(coll.points[p1], coll.points[p2]);
                        segments.Add(newSegment);
                        for (int imIndex = p1 + 1; imIndex < p2; imIndex++)
                        {
                            inMiddles.Add(new InMiddle(coll.points[imIndex], newSegment));
                        }
                    }
                }
            }

            // Remove any duplicates which may have arisen.
            segments = GeometryTutorLib.Utilities.RemoveDuplicates<GeometryTutorLib.ConcreteAST.Segment>(segments);
        }

        //// Simple function for creating a point (if needed since it is implied).
        //private Point HandleIntersectionPoint(List<Point> containment, List<Point> toAdd, GeometryTutorLib.ConcreteAST.Segment segment, Point pt)
        //{
        //    if (pt == null) return null;

        //    // The point must be between the endpoints of the segment
        //    if (!segment.PointLiesOnAndBetweenEndpoints(pt)) return null;

        //    return HandleIntersectionPoint(containment, toAdd, pt);
        //}

        //// Simple function for creating a point (if needed since it is implied).
        //private Point HandleIntersectionPoint(List<Point> containment, List<Point> toAdd, Point pt)
        //{
        //    if (pt == null) return null;

        //    // If this point was defined by the UI, do nothing
        //    Point uiPoint = GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Point>(containment, pt);
        //    if (uiPoint != null) return uiPoint;

        //    // else create the point.
        //    Point newPoint = PointFactory.GeneratePoint(pt.X, pt.Y);
        //    GeometryTutorLib.Utilities.AddStructurallyUnique<GeometryTutorLib.ConcreteAST.Point>(toAdd, newPoint);
        //    return newPoint;
        //}

        ////
        //// Find every point of intersection among segments (if they are not labeled in the UI) -- name them.
        ////
        //private void FindUnlabeledSegmentIntersectionPoints()
        //{
        //    for (int s1 = 0; s1 < segments.Count - 1; s1++)
        //    {
        //        for (int s2 = s1 + 1; s2 < segments.Count; s2++)
        //        {
        //            // If there exists a point of intersection that is between the endpoints of both segments
        //            Point inter = segments[s1].FindIntersection(segments[s2]);

        //            // Avoid parallel line intersections at infinity
        //            if (inter != null && !double.IsInfinity(inter.X) && !double.IsInfinity(inter.Y) && !double.IsNaN(inter.X) && !double.IsNaN(inter.Y))
        //            {
        //                if (segments[s1].PointLiesOnAndExactlyBetweenEndpoints(inter) && segments[s2].PointLiesOnAndExactlyBetweenEndpoints(inter))
        //                {
        //                    HandleIntersectionPoint(points, impliedSegmentPoints, inter);
        //                }
        //                // This is an extended point (beyond the two segments; the intersection is not apparent in the drawing)
        //                else
        //                {
        //                    HandleIntersectionPoint(points, extendedSegmentPoints, inter);
        //                }
        //            }
        //        }
        //    }
        //}



        //
        // Generate all covering intersection clauses; that is, generate maximal intersections (a subset of all intersections)
        //
        private void CalculateIntersections()
        {
            //
            // Iterate over all polygons
            //
            for (int sidesIndex = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; sidesIndex < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; sidesIndex++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[sidesIndex])
                {
                    //
                    // Add intersection clauses for all sides the polygon
                    //
                    List<GeometryTutorLib.ConcreteAST.Segment> sides = poly.orderedSides;
                    for (int s1 = 0; s1 < sides.Count - 1; s1++)
                    {
                        for (int s2 = s1 + 1; s2 < sides.Count; s2++)
                        {
                            // The shared vertex must be a vertex of the polygon
                            Point vertex = sides[s1].SharedVertex(sides[s2]);
                            if (vertex != null && poly.points.Contains(vertex))
                            {
                                GeometryTutorLib.Utilities.AddStructurallyUnique(ssIntersections, new Intersection(vertex, sides[s1], sides[s2]));
                            }
                        }
                    }

                    //
                    // Handle quadrilateral diagonals
                    //
                    if (poly is Quadrilateral)
                    {
                        Quadrilateral quad = poly as Quadrilateral;

                        if (GetMaximalProblemSegment(quad.bottomLeftTopRightDiagonal) == null)
                        {
                            quad.SetBottomRightDiagonalInValid();
                        }

                        if (GetMaximalProblemSegment(quad.topLeftBottomRightDiagonal) == null)
                        {
                            quad.SetTopLeftDiagonalInValid();
                        }

                        // If both diagonals exist in the figure, create an intersection and provide the quadrilateral with the intersection.
                        if (quad.TopLeftDiagonalIsValid() && quad.BottomRightDiagonalIsValid())
                        {
                            // The calculated intersection
                            Point inter = quad.bottomLeftTopRightDiagonal.FindIntersection(quad.topLeftBottomRightDiagonal);

                            // The actual point in the figure
                            Point knownPt = GeometryTutorLib.Utilities.GetStructurally<Point>(allFigurePoints, inter);

                            if (knownPt == null)
                            {
                                throw new Exception("Expected to find the point (did not):");
                            }

                            Intersection diagInter = new Intersection(knownPt, quad.bottomLeftTopRightDiagonal, quad.topLeftBottomRightDiagonal);

                            GeometryTutorLib.Utilities.AddStructurallyUnique<Intersection>(ssIntersections, diagInter);

                            quad.SetIntersection(diagInter);
                        }
                    }
                }
            }


            CalculateMaximalMinimalSegments();
            ConstructMaximalSegmentSegmentIntersections();
        }

        //
        // Find the maximal segments (remove all sub-segments from the list)
        //
        private void CalculateMaximalMinimalSegments()
        {
            bool[] marked = new bool[segments.Count];
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool isMaximal = true;
                bool isMinimal = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s1].HasSubSegment(segments[s2])) isMinimal = false;
                        if (segments[s2].HasSubSegment(segments[s1])) isMaximal = false;
                    }
                }
                if (isMinimal) minimalSegments.Add(segments[s1]);
                if (isMaximal) maximalSegments.Add(segments[s1]);
            }
        }

        //
        // Acquire all intersections from the maximal segment list
        //
        private void ConstructMaximalSegmentSegmentIntersections()
        {
            for (int s1 = 0; s1 < maximalSegments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < maximalSegments.Count; s2++)
                {
                    // An intersection should not be between collinear segments
                    if (!maximalSegments[s1].IsCollinearWith(maximalSegments[s2]))
                    {
                        // The point must be 'between' both segment endpoints
                        Point numericInter = maximalSegments[s1].FindIntersection(maximalSegments[s2]);
                        if (maximalSegments[s1].PointLiesOnAndBetweenEndpoints(numericInter) &&
                            maximalSegments[s2].PointLiesOnAndBetweenEndpoints(numericInter))
                        {
                            // The actual point in the figure
                            Point knownPt = GeometryTutorLib.Utilities.GetStructurally<Point>(allFigurePoints, numericInter);

                            Intersection newInter = new Intersection(knownPt, maximalSegments[s1], maximalSegments[s2]);

                            GeometryTutorLib.Utilities.AddStructurallyUnique<Intersection>(ssIntersections, newInter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all angles in the drawing.
        /// </summary>
        private void CalculateAngles()
        {
            foreach (Intersection inter in ssIntersections)
            {
                // 1 angle
                if (inter.StandsOnEndpoint())
                {
                    angles.Add(new Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect)));
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

                    angles.Add(new Angle(left, inter.intersect, up));
                    angles.Add(new Angle(right, inter.intersect, up));
                }
                // 4 angles
                else
                {
                    angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                    angles.Add(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                    angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                    angles.Add(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
                }
            }

            angles = GeometryTutorLib.Utilities.RemoveDuplicates<Angle>(angles);
        }

        //
        // Identify inscribed and circumscribed situations between a circle and polygon.
        //
        private void AnalyzeCirclePolygonInscription()
        {
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
                {
                    foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                    {
                        circle.AnalyzePolygonInscription(poly);
                    }
                }
            }
        }

        //
        // Acquire the exact segment if it exists...otherwise return the maximal segment
        //
        private GeometryTutorLib.ConcreteAST.Segment GetMaximalProblemSegment(GeometryTutorLib.ConcreteAST.Segment thatSegment)
        {
            // Exact segment
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                if (segment.StructurallyEquals(thatSegment)) return segment;
            }

            // Maximal Segment
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                if (segment.HasSubSegment(thatSegment)) return segment;
            }

            return null;
        }

        //
        // Determine which (UI and intersection-based) points belong to each circle.
        //
        private void AnalyzeAllCirclePointRelationships()
        {
            //
            // Find the points that are on the given circle; 
            //
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
#if !ATOMIC_REGION_OFF //Define in Properties->Build->Compilation Symbols to turn off this section

                // CTA: We need to use all UI AND implied points from intersections to generate clauses for shaded area stuff
                // The goal is to remove these guards...deduction engine (GeoTutor) needs to be robust enough.
                circle.SetPointsOnCircle(circle.GetIntersectingPoints());
#else
                List<Point> pointsOnCircle = new List<Point>();

                // UI Points
                foreach (Point pt in points)
                {
                    if (circle.PointLiesOn(pt)) pointsOnCircle.Add(pt);
                }

                circle.SetPointsOnCircle(pointsOnCircle);
#endif
                // Since we know all points on this circle, we generate all arc clauses
                GenerateSemicircleClauses(circle);
                GenerateArcClauses(circle);
            }
        }

        //
        // Detect diameters and generate all of the Semicircle Arc and ArcInMiddle clauses
        //
        private void GenerateSemicircleClauses(GeometryTutorLib.ConcreteAST.Circle circle)
        {
            foreach (GeometryTutorLib.ConcreteAST.Segment seg in segments)
            {
                if (circle.DefinesDiameter(seg))
                {
                    //Get the endpoints of the diameter and the indices of these endpoints
                    Point e1 = seg.Point1;
                    Point e2 = seg.Point2;
                    int p1 = circle.pointsOnCircle.IndexOf(e1);
                    int p2 = circle.pointsOnCircle.IndexOf(e2);

                    //For partitioning purposes, order of the endpoints matters. Make sure p1 holds the lower of the two indices
                    if (p1 > p2)
                    {
                        int p3 = p1;
                        p1 = p2;
                        p2 = p3;
                    }

                    // Partition the remaining points on the circle
                    List<Point> minorArcPoints;
                    List<Point> majorArcPoints;
                    PartitionSemiCircleArcPoints(circle.pointsOnCircle, p1, p2, out minorArcPoints, out majorArcPoints);

                    // Semicircle requires 3 points to be defined - the two endpoints and a point inbetween
                    // The minorArcPoints and majorArcPoints lists contain all the potential inbetween points for either side of the diameter
                    // Handle 'side' 1:
                    for (int i = 0; i < majorArcPoints.Count; ++i)
                    {
                        Semicircle semi = new Semicircle(circle, e1, e2, majorArcPoints[i], minorArcPoints, majorArcPoints, seg);
                        AddSemicircleClauses(semi);
                    }
                    // Handle 'side' 2:
                    for (int i = 0; i < minorArcPoints.Count; ++i)
                    {
                        Semicircle semi = new Semicircle(circle, e1, e2, minorArcPoints[i], majorArcPoints, minorArcPoints, seg);
                        AddSemicircleClauses(semi);
                    }
                }
            }
        }

        private void AddSemicircleClauses(GeometryTutorLib.ConcreteAST.Semicircle semi)
        {
            if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.Semicircle>(semiCircles, semi))
            {
                semiCircles.Add(semi);
            }

            //Add arcInMiddle
            //For semicircles, only considering the defining middle point as an inMiddle point
            //This is to avoid arc equations such as MinorArc(RX) + MinorArc(XT) = Semicircle(RST), which might not make sense to a user
            GeometryTutorLib.Utilities.AddStructurallyUnique<ArcInMiddle>(arcInMiddle, new ArcInMiddle(semi.middlePoint, semi));
        }

        //
        // All points x, from p1 < x < p2, are arc 'major' points (belong to the semicircle)
        // All points x, from x < p1 or x > p2, are arc 'minor' points (are outside of the semicircle)
        //
        // We assume an ordered list of points here.
        //
        private void PartitionSemiCircleArcPoints(List<Point> points, int endpt1, int endpt2, out List<Point> minorArcPoints, out List<Point> majorArcPoints)
        {
            minorArcPoints = new List<Point>();
            majorArcPoints = new List<Point>();

            // Traverse points and add to the appropriate list
            for (int i = 0; i < points.Count; i++)
            {
                if (i > endpt1 && i < endpt2) majorArcPoints.Add(points[i]);
                else if (i < endpt1 || i > endpt2) minorArcPoints.Add(points[i]);
            }
        }

        //
        // Generate all of the Major/Minor Arc and ArcInMiddle clauses; similar to generating for collinear points on segments.
        //
        private void GenerateArcClauses(GeometryTutorLib.ConcreteAST.Circle circle)
        {
            //
            // Generate all Arc objects with their minor / major arc points; also generate ArcInMiddle clauses.
            //
            for (int p1 = 0; p1 < circle.pointsOnCircle.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < circle.pointsOnCircle.Count; p2++)
                {
                    // Do these endpoints form a diameter? If so, the semicircle arcs should have already been handled by GenerateSemicircleClauses()
                    GeometryTutorLib.ConcreteAST.Segment seg = new GeometryTutorLib.ConcreteAST.Segment(circle.pointsOnCircle[p1], circle.pointsOnCircle[p2]);
                    if (!circle.DefinesDiameter(seg)) CreateMajorMinorArcs(circle, p1, p2);
                }
            }
        }

        private void CreateMajorMinorArcs(GeometryTutorLib.ConcreteAST.Circle circle, int p1, int p2)
        {
            List<Point> minorArcPoints;
            List<Point> majorArcPoints;
            PartitionArcPoints(circle, p1, p2, out minorArcPoints, out majorArcPoints);

            MinorArc newMinorArc = new MinorArc(circle, circle.pointsOnCircle[p1], circle.pointsOnCircle[p2], minorArcPoints, majorArcPoints);
            MajorArc newMajorArc = new MajorArc(circle, circle.pointsOnCircle[p1], circle.pointsOnCircle[p2], minorArcPoints, majorArcPoints);
            Sector newMinorSector = new Sector(newMinorArc);
            Sector newMajorSector = new Sector(newMajorArc);
            if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.MinorArc>(minorArcs, newMinorArc))
            {
                minorArcs.Add(newMinorArc);
                minorSectors.Add(newMinorSector);
                majorSectors.Add(newMajorSector);
            }
            if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.MajorArc>(majorArcs, newMajorArc))
            {
                majorArcs.Add(newMajorArc);
                majorSectors.Add(newMajorSector);
            }

            circle.AddMinorArc(newMinorArc);
            circle.AddMajorArc(newMajorArc);
            circle.AddMinorSector(newMinorSector);
            circle.AddMajorSector(newMajorSector);

            // Generate ArcInMiddle clauses for minor arc and major arc
            for (int imIndex = 0; imIndex < newMinorArc.arcMinorPoints.Count; imIndex++)
            {
                GeometryTutorLib.Utilities.AddStructurallyUnique<ArcInMiddle>(arcInMiddle, new ArcInMiddle(newMinorArc.arcMinorPoints[imIndex], newMinorArc));
            }
            for (int imIndex = 0; imIndex < newMajorArc.arcMajorPoints.Count; imIndex++)
            {
                GeometryTutorLib.Utilities.AddStructurallyUnique<ArcInMiddle>(arcInMiddle, new ArcInMiddle(newMajorArc.arcMajorPoints[imIndex], newMajorArc));
            }
        }

        private void PartitionArcPoints(GeometryTutorLib.ConcreteAST.Circle circle, int endpt1, int endpt2, out List<Point> minorArcPoints, out List<Point> majorArcPoints)
        {
            minorArcPoints = new List<Point>();
            majorArcPoints = new List<Point>();
            Point e1 = circle.pointsOnCircle[endpt1];
            Point e2 = circle.pointsOnCircle[endpt2];

            // Traverse points and add to the appropriate list
            for (int i = 0; i < circle.pointsOnCircle.Count; i++)
            {
                if (i != endpt1 && i != endpt2)
                {
                    Point m = circle.pointsOnCircle[i];
                    if (Arc.BetweenMinor(m, new MinorArc(circle, e1, e2))) minorArcPoints.Add(m);
                    else majorArcPoints.Add(m);
                }
            }
        }

        //private void ConstructImaginaryIntersectionPoint(GeometryTutorLib.ConcreteAST.Segment s1, GeometryTutorLib.ConcreteAST.Segment s2)
        //{
        //    Point intersection = s1.FindIntersection(s2);

        //    if (!double.IsInfinity(intersection.X) && !double.IsInfinity(intersection.Y))
        //    {
        //        if (s1.PointLiesOnAndBetweenEndpoints(intersection) && s2.PointLiesOnAndBetweenEndpoints(intersection))
        //        {
        //            if (!GeometryTutorLib.Utilities.HasStructurally<Point>(allFigurePoints, intersection))
        //            {
        //                GeometryTutorLib.Utilities.AddStructurallyUnique<ImaginaryPoint>(imagPoints, new ImaginaryPoint(intersection));
        //            }
        //        }
        //    }
        //}

        // Check to see if the given segment already exists directly or indirectly as collinear points.
        private bool DoesSegmentExistExplicitly(GeometryTutorLib.ConcreteAST.Segment seg)
        {
            foreach (GeometryTutorLib.ConcreteAST.Segment maximal in maximalSegments)
            {
                if (maximal.HasSubSegment(seg)) return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate segment bisctors.
        /// </summary>
        private void CalculateSegmentBisectors()
        {
            // Check each intersetion...
            foreach (Intersection i in ssIntersections)
            {
                //... and create two new lines for each segment split by the point of intersection.
                var lhs1 = new GeometryTutorLib.ConcreteAST.Segment(i.lhs.Point1, i.intersect);
                var lhs2 = new GeometryTutorLib.ConcreteAST.Segment(i.intersect, i.lhs.Point2);
                var rhs1 = new GeometryTutorLib.ConcreteAST.Segment(i.rhs.Point1, i.intersect);
                var rhs2 = new GeometryTutorLib.ConcreteAST.Segment(i.intersect, i.rhs.Point2);

                // Is the lhs bisected by rhs?
                if (GeometryTutorLib.Utilities.CompareValues(lhs1.Length, lhs2.Length))
                {
                    segmentBisectors.Add(new GeometryTutorLib.ConcreteAST.SegmentBisector(i, i.rhs));
                }

                // Is the rhs bisected by lhs?
                if (GeometryTutorLib.Utilities.CompareValues(rhs1.Length, rhs2.Length))
                {
                    segmentBisectors.Add(new GeometryTutorLib.ConcreteAST.SegmentBisector(i, i.lhs));
                }
            }
        }

        /// <summary>
        /// Calculate angle bisectors.
        /// </summary>
        private void CalculateAngleBisectors()
        {
            //Check each angle...
            foreach (Angle a in angles)
            {
                //... and see if a segment passes through point B of the angle.
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
                {
                    if (segment.PointLiesOnAndBetweenEndpoints(a.GetVertex()))
                    {
                        // Create new angles with this segment and see if they are the same measure
                        if (GeometryTutorLib.Utilities.CompareValues(new Angle(a.A, a.GetVertex(), segment.Point1).measure, new Angle(a.C, a.GetVertex(), segment.Point1).measure))
                        {
                            // We found an angle bisector!
                            angleBisectors.Add(new GeometryTutorLib.ConcreteAST.AngleBisector(a, segment));
                        }
                    }
                }
            }
        }

        //
        // Take a list of points and identify which are known / named in the figure, return those points.
        //
        public List<Point> NormalizePointsToDrawing(List<Point> original)
        {
            List<Point> normalized = new List<Point>();

            foreach (Point pt in original)
            {
                Point normed = GeometryTutorLib.Utilities.GetStructurally<Point>(allFigurePoints, pt);
                if (normed == null)
                {
                    throw new ArgumentException("Point not find in dictionary of points: " + pt);
                }

                normalized.Add(normed);
            }
            return normalized;
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.AppendLine("Points");
            foreach (Point p in points)
            {
                str.AppendLine("\t" + p.ToString());
            }

            str.AppendLine("Segments");
            foreach (GeometryTutorLib.ConcreteAST.Segment s in segments)
            {
                str.AppendLine("\t" + s.ToString());
            }

            str.AppendLine("InMiddles");
            foreach (InMiddle im in inMiddles)
            {
                str.AppendLine("\t" + im.ToString());
            }

            str.AppendLine("Polygons");
            for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                {
                    str.AppendLine("\t" + poly.ToString());
                }
            }

            str.AppendLine("Circles");
            foreach (GeometryTutorLib.ConcreteAST.Circle c in circles)
            {
                str.AppendLine("\t" + c.ToString());
            }

            //
            // Implied information
            //
            //str.AppendLine("Implied Segment Points");
            //foreach (Point p in impliedSegmentPoints)
            //{
            //    str.AppendLine("\t" + p.ToString());
            //}

            //str.AppendLine("Implied Circle Points");
            //foreach (Point p in impliedCirclePoints)
            //{
            //    str.AppendLine("\t" + p.ToString());
            //}

            //str.AppendLine("Implied Chords");
            //foreach (KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Circle>> chordPair in impliedChords)
            //{
            //    str.Append("\t" + chordPair.Key.ToString() + ": ");
            //    foreach (GeometryTutorLib.ConcreteAST.Circle circle in chordPair.Value)
            //    {
            //        str.Append(" " + circle.ToString());
            //    }
            //}

            int a = 1;
            foreach (AtomicRegion atom in atomicRegions)
            {
                str.AppendLine((a++) + ": " + atom.ToString());
            }

            return str.ToString();
        }

        //
        // Acquire all the regions.
        //
        public List<AtomicRegion> GetAllAtomicRegions()
        {
            return atomicRegions;
        }

        //
        // Search the list of atomic regions using a single point to determine whether the point is inside the region or not.
        //
        public AtomicRegion GetAtomicRegionByPoint(Point pt)
        {
            foreach (AtomicRegion atom in atomicRegions)
            {
                if (atom.PointLiesInside(pt)) return atom;
            }

            return null;
        }

        //
        // Search the list of atomic regions using a single set of points to determine whether the point is inside the region or not.
        //
        public List<AtomicRegion> GetAtomicRegionsByPoints(List<Point> points)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            foreach (Point pt in points)
            {
                foreach (AtomicRegion atom in atomicRegions)
                {
                    if (atom.PointLiesInside(pt)) atoms.Add(atom);
                }
            }

            return atoms;
        }

        //
        // Search the list of atomic regions using a single point to determine whether the point is inside the region or not.
        //
        public List<AtomicRegion> GetAllAtomicRegionsWithoutPoint(Point pt)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atomicRegions)
            {
                if (!atom.PointLiesInside(pt)) atoms.Add(atom);
            }

            return atoms;
        }

        //
        // Search the list of atomic regions using a set of point to determine whether the point is inside the region or not.
        //
        public List<AtomicRegion> GetAllAtomicRegionsWithoutPoints(List<Point> thePoints)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atomicRegions)
            {
                bool inside = false;
                foreach (Point pt in thePoints)
                {
                    if (atom.PointLiesInside(pt))
                    {
                        inside = true;
                        break;
                    }
                }
                if (!inside) atoms.Add(atom);
            }

            return atoms;
        }
    }
}