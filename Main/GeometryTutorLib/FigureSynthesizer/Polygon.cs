using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a general polygon (which consists of n >= 3 points)
    /// </summary>
    public partial class Polygon  : Figure
    {
        // Number of recursive midpoints to add to as snap-to points: A---B with 1 means A--Mid--B; A---B with 2 means A-m1-Mid-m3-B
        public const int NUM_MID_SEGMENT_POINTS = 1;

        Polygon(List<Point> orderedVerts)
        {
            vertices = new List<Point>(orderedVerts);

            snapToPoints = ConstructAllMidpoints();
        }

        /// <summary>
        /// Constructs all midpoints for all segments of the polygon.
        /// </summary>
        private List<Point> ConstructAllMidpoints()
        {
            List<Point> middlePoints = new List<Point>();
            for (int i = 0; i < vertices.Count; i++)
            {
                // Construct all the necessary midpoints.
                List<Point> currMiddlePoints = ConstructMidpoints(vertices[i], vertices[(i+1) % vertices.Count], 0);

                // Add to the overall list.
                allPoints.Add(vertices[i]);
                allPoints.AddRange(currMiddlePoints);

                // Add to the distinct snapping set of midpoints.
                middlePoints.AddRange(currMiddlePoints);
            }

            return middlePoints;
        }

        private List<Point> ConstructMidpoints(Point end1, Point end2, int level)
        {
            List<Point> middlePoints = new List<Point>();

            // Recursive Base Case
            if (level == NUM_MID_SEGMENT_POINTS) return middlePoints;

            Point midpoint = end1.Midpoint(end2);

            //
            // Recursively construct all points between
            //
            middlePoints.AddRange(ConstructMidpoints(end1, midpoint, level + 1));
            middlePoints.Add(midpoint);
            middlePoints.AddRange(ConstructMidpoints(midpoint, end2, level + 1));

            return middlePoints;
        }



        public override List<Figure> ComposeSubtraction(Figure betaInner)
        {
            return new List<Figure>();
        }

        public override List<Figure> ComposeAddition(Figure betaOuter)
        {
            return new List<Figure>();
        }
    }
}
