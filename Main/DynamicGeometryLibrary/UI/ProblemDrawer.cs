
using DynamicGeometry.UI.RegionShading;
using GeometryTutorLib.GeometryTestbed;
using System.Collections.Generic;
namespace DynamicGeometry.UI
{
    /// <summary>
    /// Draws hard-coded problems to the canvas
    /// </summary>
    public class ProblemDrawer
    {
        private DrawingHost drawingHost;

        private Dictionary<GeometryTutorLib.ConcreteAST.Point, IPoint> points; //Keep track of logical to graphical points
        private Dictionary<GeometryTutorLib.ConcreteAST.Segment, Segment> segments; //Keep track of logical to graphical segments
        private Dictionary<GeometryTutorLib.ConcreteAST.Circle, Circle> circles; //Keep track of logical to graphical circles

        /// <summary>
        /// Create a new problem drawer.
        /// </summary>
        /// <param name="drawing">The drawingHost.</param>
        public ProblemDrawer(DrawingHost drawingHost)
        {
            this.drawingHost = drawingHost;
        }

        /// <summary>
        /// Draw the given problem to the drawing.
        /// Will remove the current drawing and shadings
        /// </summary>
        /// <param name="problem">The problem to draw.</param>
        public void draw(ActualProblem problem)
        {
            //Initialize the lookup dictionaries
            points = new Dictionary<GeometryTutorLib.ConcreteAST.Point, IPoint>();
            segments = new Dictionary<GeometryTutorLib.ConcreteAST.Segment, Segment>();
            circles = new Dictionary<GeometryTutorLib.ConcreteAST.Circle, Circle>();

            //Clear the current drawing
            drawingHost.CurrentDrawing.ClearRegionShadings();
            var figures = new List<IFigure>();
            drawingHost.CurrentDrawing.Figures.ForEach(figure => figures.Add(figure));
            figures.ForEach(figure => RemoveFigure(figure));

            //Draw the hard-coded problem.
            drawPoints(problem);
            drawSegments(problem);
            drawCircles(problem);
            shadeProblem(problem);

            //Run the problem
            problem.Run();
        }

        /// <summary>
        /// Draw each point in the problem and save it to the lookup dictionary.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawPoints(ActualProblem problem)
        {
            //Add each point to the drawing
            foreach (var pt in problem.points)
            {
                //Create and add the point
                var point = Factory.CreateFreePoint(drawingHost.CurrentDrawing, new System.Windows.Point(pt.X, pt.Y));
                point.Name = pt.name;
                Actions.Add(drawingHost.CurrentDrawing, point);

                //Save to lookup dictionary
                points.Add(pt, point);
            }
        }

        /// <summary>
        /// Draw each segment in the problem and save it to the lookup dictionary.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawSegments(ActualProblem problem)
        {
            //Add each segment to the drawing
            foreach (var seg in problem.segments)
            {
                //Look already drawn points
                var pt1 = points[seg.Point1];
                var pt2 = points[seg.Point2];

                //Create and add the segment
                var segment = Factory.CreateSegment(drawingHost.CurrentDrawing, pt1, pt2);
                Actions.Add(drawingHost.CurrentDrawing, segment);

                //Save to lookup dictionary
                segments.Add(seg, segment);
            }
        }

        /// <summary>
        /// Draw each circle in the problem and save it to the lookup dictionary.
        /// Circles in LiveGeometry need two points. We have the center, but may need a point on the circle itself.
        /// If a point exists on the circle we will use that, if not a point will be created on the circle to the direct
        /// right of the center.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawCircles(ActualProblem problem)
        {
            //Add circles to the drawing
            foreach (var circ in problem.circles)
            {
                //Lookup center point
                var center = points[circ.center];
                IPoint pointOnCircle;

                if (circ.pointsOnCircle.Count > 0) //Point on circle exists
                {
                    pointOnCircle = points[circ.pointsOnCircle[0]];
                }
                else //Does not exist, we need to make one
                {
                    pointOnCircle = Factory.CreateFreePoint(
                        drawingHost.CurrentDrawing,
                        new System.Windows.Point(circ.center.X + circ.radius, circ.center.Y));
                }

                //Create circle and add to drawing
                IPoint[] dependencies = { center, pointOnCircle };
                Circle circle = Factory.CreateCircle(drawingHost.CurrentDrawing, new List<IFigure>(dependencies));
                Actions.Add(drawingHost.CurrentDrawing, circle);

                //Save to lookup dictionary
                circles.Add(circ, circle);
            }
        }

        /// <summary>
        /// If the problem is a shaded area problem, shade the goal regions.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void shadeProblem(ActualProblem problem)
        {
            ActualShadedAreaProblem saProb = problem as ActualShadedAreaProblem;
            if (saProb != null)
            {
                //Shade each region
                foreach (var region in saProb.goalRegions)
                {
                    ShadedRegion sr = new ShadedRegion(region);
                    sr.Draw(drawingHost.CurrentDrawing, ShadedRegion.BRUSHES[1]);
                }
            }
        }

        /// <summary>
        /// Helper method used to remove figures from the drawing. 
        /// Will remove any figure that is not the coordinate grid.
        /// </summary>
        /// <param name="figure">The figure to be removed.</param>
        private void RemoveFigure(IFigure figure)
        {
            if (figure is CartesianGrid) { }
            else
            {
                Actions.Remove(figure);
            }
        }
    }
}
