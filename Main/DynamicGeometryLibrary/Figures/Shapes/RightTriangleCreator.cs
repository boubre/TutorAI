using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DynamicGeometry.Figures.Shapes
{
    [Category(BehaviorCategories.Shapes)]
    [Order(4)]
    public class RightTriangleCreator : TriangleCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPointPoint;
        }

        protected override IEnumerable<IFigure> CreateFigures()
        {
            IPoint[] points = new IPoint[] {
                FoundDependencies[0] as IPoint,
                FoundDependencies[1] as IPoint,
                FoundDependencies[2] as IPoint
            };

            double slope = (points[1].Coordinates.Y - points[0].Coordinates.Y) / (points[1].Coordinates.X - points[0].Coordinates.X);
            double orthoSlope = -1.0 / slope;
            double b = points[1].Coordinates.Y - orthoSlope * points[1].Coordinates.X;
            IMovable point2 = points[2] as IMovable;
            point2.MoveTo(new Point(point2.Coordinates.X, orthoSlope * point2.Coordinates.X + b));
            
            yield return Factory.CreatePolygon(Drawing, FoundDependencies);
            for (int i = 0; i < FoundDependencies.Count; i++)
            {
                // get two consecutive vertices of the polygon
                int j = (i + 1) % FoundDependencies.Count;
                IPoint p1 = FoundDependencies[i] as IPoint;
                IPoint p2 = FoundDependencies[j] as IPoint;
                // try to find if there is already a line connecting them
                if (Drawing.Figures.FindLine(p1, p2) == null)
                {
                    // if not, create a new segment
                    var segment = Factory.CreateSegment(Drawing, new[] { p1, p2 });
                    yield return segment;
                }
            }
        }

        protected override IFigure CreateIntermediateFigure()
        {
            if (FoundDependencies.Count == 2)
            {
                return Factory.CreateSegment(Drawing, FoundDependencies);
            }
            return null;
        }

        public override string Name
        {
            get { return "Right Triangle"; }
        }

        public override string HintText
        {
            get
            {
                return "Click 3 points to construct a right triangle. The last point will automatically conform to the triangle.";
            }
        }

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder
                .BuildIcon()
                .Polygon(
                    Factory.CreateDefaultFillBrush(),
                    new SolidColorBrush(Colors.Black),
                    new Point(0.0, 0.0),
                    new Point(0.0, 1.0),
                    new Point(0.5, 1.0))
                .Canvas;
        }
    }
}
