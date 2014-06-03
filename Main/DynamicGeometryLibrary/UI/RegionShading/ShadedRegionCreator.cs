using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses;
using LiveGeometry.TutorParser;

namespace DynamicGeometry.UI.RegionShading
{
    [Category(BehaviorCategories.Regions)]
    [Order(1)]
    public class ShadedRegionCreator : Behavior
    {
        public override string Name
        {
            get { return "Mark Region"; }
        }

        public override string HintText
        {
            get { return "Click a region to mark it."; }
        }

        public override FrameworkElement CreateIcon()
        {
            var uri = new Uri("LiveGeometry;component/MarkRegion.png", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            BitmapImage source = new BitmapImage();
            source.SetSource(streamInfo.Stream);
            return new Image() { Source = source, Stretch = Stretch.None };
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            DrawingParserMain parser = new DrawingParserMain(Drawing);
            parser.Parse();
            CoordinateSystem cs = Drawing.CoordinateSystem;
            Point pt = new Point(e.GetPosition(Drawing.Canvas).X, e.GetPosition(Drawing.Canvas).Y);
            Point logicalPt = new Point(cs.ToLogical(pt.X - cs.Origin.X), cs.ToLogical(cs.Origin.Y - pt.Y));
            foreach (AtomicRegion ar in parser.IdentifyAtomicRegions())
            {
                if (ar is ShapeAtomicRegion)
                {
                    ShapeAtomicRegion sar = ar as ShapeAtomicRegion;
                    GeometryTutorLib.ConcreteAST.Polygon shape = sar.shape as GeometryTutorLib.ConcreteAST.Polygon;
                    if (shape != null && shape.IsInConvexPolygon(new GeometryTutorLib.ConcreteAST.Point("shadingtest", logicalPt.X, logicalPt.Y)))
                    {
                        //Just add a square bitmap for now
                        WriteableBitmap bmp = new WriteableBitmap(100, 100);
                        for (int i = 0; i < 100; i++)
                            for (int j = 0; j < 100; j++)
                            {
                                int pixel;
                                if ((i + j) % 12 <= 3)
                                    pixel = unchecked((int)0xFFFF0000);
                                else if ((i + j) % 12 <= 7)
                                    pixel = unchecked((int)0xFF00FF00);
                                else
                                    pixel = unchecked((int)0xFF0000FF);

                                bmp.Pixels[i * 100 + j] = pixel;
                            }

                        System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                        img.Source = bmp;
                        Canvas.SetTop(img, pt.Y);
                        Canvas.SetLeft(img, pt.X);
                        Canvas.SetZIndex(img, (int)ZOrder.Shading);
                        Drawing.Canvas.Children.Add(img);
                    }
                }
                else if (ar is NonShapeAtomicRegion)
                {
                    //Ignore for now
                }
            } 
        }
    }
}
