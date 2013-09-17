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
    [Order(4)]
    public class RightTriangleCreator : TriangleCreator
    {
        public override string Name
        {
            get { return "Right Triangle"; }
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
