
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
        /// </summary>
        /// <param name="problem">The problem to draw.</param>
        public void draw(ActualProblem problem)
        {
            var points = new Dictionary<GeometryTutorLib.ConcreteAST.Point, IPoint>(); //Keep track of logical to graphical points
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

            var segments = new Dictionary<GeometryTutorLib.ConcreteAST.Segment, Segment>(); //Keep track of logical to graphical segments
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

            //Add circles to the drawing
            var circles = new Dictionary<GeometryTutorLib.ConcreteAST.Circle, Circle>(); //Keep track of logical to graphical circles
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
                IPoint[] dependencies = {center, pointOnCircle};
                Circle circle = Factory.CreateCircle(drawingHost.CurrentDrawing, new List<IFigure>(dependencies));
                Actions.Add(drawingHost.CurrentDrawing, circle);
                
                //Save to lookup dictionary
                circles.Add(circ, circle);
            }
        }
    }
}
