using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DynamicGeometry.Figures.Shapes
{
    [Category(BehaviorCategories.Shapes)]
    [Order(3)]
    public class EquilateralTriangleCreator : TriangleCreator
    {
        public override string Name
        {
            get { return "Equilateral Triangle"; }
        }

        public override string HintText
        {
            get
            {
                return "Click 3 points to construct a triangle.";
            }
        }

        public override FrameworkElement CreateIcon()
        {
            double sideLength = 1.0;
            return IconBuilder
                .BuildIcon()
                .Polygon(
                    Factory.CreateDefaultFillBrush(),
                    new SolidColorBrush(Colors.Black),
                    new Point(sideLength / 2, 0.0),
                    new Point(0.0, Math.SquareRoot(3) * sideLength / 2),
                    new Point(sideLength, Math.SquareRoot(3) * sideLength / 2))
                .Canvas;
        }
    }
}
