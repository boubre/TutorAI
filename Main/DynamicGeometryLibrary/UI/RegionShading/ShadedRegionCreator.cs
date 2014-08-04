using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using LiveGeometry.TutorParser;

namespace DynamicGeometry.UI.RegionShading
{
    [Category(BehaviorCategories.Regions)]
    [Order(1)]
    public class ShadedRegionCreator : Behavior
    {
        private List<AtomicRegion> atoms = null;

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

        // private System.Collections.Generic.List<AtomicRegion> atoms = null;
        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            //
            // Parse the figure once...how to do we determine if the figure was updated since the last parsing?
            // That is, how do we handle mutability of the figure.
            // I suggest we have a Clear method that the user clicks to remove all shading and start over.
            //
            if (atoms == null)
            {
                DrawingParserMain parser = new DrawingParserMain(Drawing);
                parser.Parse();

                atoms = parser.GetAtomicRegions();
            }

            CoordinateSystem cs = Drawing.CoordinateSystem;
            Point pt = new Point(e.GetPosition(Drawing.Canvas).X, e.GetPosition(Drawing.Canvas).Y);
            Point logicalPt = new Point(cs.ToLogical(pt.X - cs.Origin.X), cs.ToLogical(cs.Origin.Y - pt.Y));

            foreach (AtomicRegion ar in atoms)
            {
                if (ar.PointLiesInside(new GeometryTutorLib.ConcreteAST.Point("shadingtest", logicalPt.X, logicalPt.Y)))
                {
                    ShadedRegion sr = ShadedRegion.CreateAndAdd(ar, logicalPt);
                    Image img = sr.Draw(Drawing, ShadedRegion.COLORS[0], ShadedRegion.COLORS[1]);
                    Drawing.Canvas.Children.Add(img);
                }
            }
        }
    }
}
