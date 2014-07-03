﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Determine all points of intersection among shapes (circles and polygons)
    /// </summary>
    public class ShapeIntersectionCalculator
    {
        private ImpliedComponentCalculator implied;

        public ShapeIntersectionCalculator(ImpliedComponentCalculator imp)
        {
            implied = imp;
        }

        /// <summary>
        /// Calculate all points of intersection between circles.
        /// </summary>
        public void CalcCircleCircleIntersections(out List<GeometryTutorLib.ConcreteAST.CircleCircleIntersection> ccIntersections)
        {
            ccIntersections = new List<CircleCircleIntersection>();
            
            for (int c1 = 0; c1 < implied.circles.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < implied.circles.Count; c2++)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    implied.circles[c1].FindIntersection(implied.circles[c2], out inter1, out inter2);

                    List<Point> intersectionPts = new List<Point>();
                    if (inter1 != null) intersectionPts.Add(inter1);
                    if (inter2 != null) intersectionPts.Add(inter2);

                    // normalized to drawing point (names)
                    intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                    // and add to each figure (circle and polygon).
                    implied.circles[c1].AddIntersectingPoints(intersectionPts);
                    implied.circles[c2].AddIntersectingPoints(intersectionPts);

                    //
                    // Construct the intersections
                    //
                    CircleCircleIntersection ccInter = null;

                    if (inter1 != null)
                    {
                        ccInter = new CircleCircleIntersection(inter1, implied.circles[c1], implied.circles[c2]);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleCircleIntersection>(ccIntersections, ccInter);
                    }
                    if (inter2 != null)
                    {
                        ccInter = new CircleCircleIntersection(inter2, implied.circles[c1], implied.circles[c2]);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleCircleIntersection>(ccIntersections, ccInter);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between circles and polygons.
        /// </summary>
        public void CalcCirclePolygonIntersectionPoints()
        {
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                // Iterate over all polygons
                for (int sidesIndex = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                     sidesIndex < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                     sidesIndex++)
                {
                    foreach (GeometryTutorLib.ConcreteAST.Polygon poly in implied.polygons[sidesIndex])
                    {
                        // Get the list of intersection points
                        List<Point> intersectionPts = poly.FindIntersections(circle);

                        // normalized to drawing point (names)
                        intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                        // and add to each figure (circle and polygon).
                        poly.AddIntersectingPoints(intersectionPts);
                        circle.AddIntersectingPoints(intersectionPts);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between polygons and polygons.
        /// </summary>
        public void CalcPolygonPolygonIntersectionPoints()
        {
            for (int s1 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                 s1 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                 s1++)
            {
                for (int p1 = 0; p1 < implied.polygons[s1].Count; p1++)
                {
                    for (int s2 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                         s2 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                         s2++)
                    {
                        for (int p2 = 0; p2 < implied.polygons[s2].Count; p2++)
                        {
                            if (s1 != s2 || p1 != p2)
                            {
                                // Get the list of intersection points
                                List<Point> intersectionPts = implied.polygons[s1][p1].FindIntersections(implied.polygons[s2][p2]);

                                // normalized to drawing point (names)
                                intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                                // and add to each figure (circle and polygon).
                                implied.polygons[s1][p1].AddIntersectingPoints(intersectionPts);
                                implied.polygons[s2][p2].AddIntersectingPoints(intersectionPts);
                            }
                        }
                    }
                }
            }
        }

        //
        // Find every point of intersection among segments (if they are not labeled in the UI) -- name them.
        //
        public List<CircleSegmentIntersection> FindCircleSegmentIntersections()
        {
            List<CircleSegmentIntersection> intersections = new List<CircleSegmentIntersection>();

            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.maximalSegments)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    circle.FindIntersection(segment, out inter1, out inter2);
                    if (!segment.PointIsOnAndBetweenEndpoints(inter1)) inter1 = null;
                    if (!segment.PointIsOnAndBetweenEndpoints(inter2)) inter2 = null;

                    // Add them to the list (possibly)
                    List<Point> intersectionPts = new List<Point>();
                    if (inter1 != null) intersectionPts.Add(inter1);
                    if (inter2 != null) intersectionPts.Add(inter2);

                    // normalized to drawing point (names)
                    intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                    //
                    // Construct the intersections
                    //
                    CircleSegmentIntersection csInter = null;

                    if (inter1 != null)
                    {
                        csInter = new CircleSegmentIntersection(inter1, circle, segment);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleSegmentIntersection>(intersections, csInter);

                        // Analyze this segment w.r.t. to this circle: tangent, secant, chord.
                        circle.AnalyzeSegment(segment, implied.allFigurePoints);
                    }
                    if (inter2 != null)
                    {
                        csInter = new CircleSegmentIntersection(inter2, circle, segment);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleSegmentIntersection>(intersections, csInter);

                        // Analyze this segment w.r.t. to this circle: tangent, secant, chord.
                        circle.AnalyzeSegment(segment, implied.allFigurePoints);
                    }
                }

                // Complete any processing attributed to the circle and all the segments.
                circle.CleanUp();
            }

            return intersections;
        }
    }
}