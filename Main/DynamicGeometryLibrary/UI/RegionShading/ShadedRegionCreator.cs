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
                    if (shape != null && shape.IsInPolygon(new GeometryTutorLib.ConcreteAST.Point("shadingtest", logicalPt.X, logicalPt.Y)))
                    {
                        ShadedRegion sr = ShadedRegion.CreateAndAdd(sar, logicalPt);
                        Image img = sr.Draw(Drawing, ShadedRegion.COLORS[0], ShadedRegion.COLORS[1]);
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
