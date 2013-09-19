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
            FreePoint[] points = new FreePoint[] {
                FoundDependencies[0] as FreePoint,
                FoundDependencies[1] as FreePoint,
                FoundDependencies[2] as FreePoint
            };

            double slope = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            double orthoSlope = -1.0 / slope;
            double b = points[1].Y - orthoSlope * points[1].X;
            points[2].X = points[2].X;
            points[2].Y = orthoSlope * points[2].X + b;

            yield return Factory.CreatePolygon(Drawing, points);
            for (int i = 0; i < points.Length; i++)
            {
                // get two consecutive vertices of the polygon
                int j = (i + 1) % points.Length;
                IPoint p1 = points[i] as IPoint;
                IPoint p2 = points[j] as IPoint;
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
