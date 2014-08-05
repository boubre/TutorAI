using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace DynamicGeometry.UI.RegionShading
{
    public class ShadedRegion
    {
        public static readonly Brush[] BRUSHES = 
        {
            MakeHatchBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), Color.FromArgb(0xFF, 0x80, 0x00, 0x00)), // red & maroon
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), Color.FromArgb(0xFF, 0x00, 0x80, 0x00)), // lime & green
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), Color.FromArgb(0xFF, 0x00, 0x00, 0x80)), // blue & navy
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF), Color.FromArgb(0xFF, 0x00, 0x80, 0x80)), // aqua & teal
            MakeHatchBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF), Color.FromArgb(0xFF, 0x80, 0x00, 0x80))  // fuschia & purple
        };


        public AtomicRegion Region { get; private set; }

        /// <summary>
        /// Create a new ShadedRegion
        /// </summary>
        /// <param name="region">The region to shade</param>
        public ShadedRegion(AtomicRegion region)
        {
            Region = region;
        }

        /// <summary>
        /// Convert a physical point (pixel) to a logical point.
        /// </summary>
        /// <param name="cs">The current coordiante system</param>
        /// <param name="physical">The point to convert</param>
        /// <returns>The converted point</returns>
        public static Point ToLogical(CoordinateSystem cs, Point physical)
        {
            return new Point(cs.ToLogical(physical.X - cs.Origin.X), cs.ToLogical(cs.Origin.Y - physical.Y));
        }

        /// <summary>
        /// Convert a logical point to a nearby physical point (pixel).
        /// </summary>
        /// <param name="cs">The current coordinate system</param>
        /// <param name="logical">The point to convert</param>
        /// <returns>The converted point</returns>
        public static Point ToPhysical(CoordinateSystem cs, Point logical)
        {
            return cs.ToPhysical(logical);
        }

        /// <summary>
        /// Draw the shaded region as an approximated polygon and return the polygon
        /// The Zindex, Top, and Left attributes will also be set.
        /// </summary>
        /// <param name="drawing">The current drawing</param>
        /// <param name="shadingBrush">The brush to shade the region with</param>
        /// <returns>The graphical representation of the region.</returns>
        public UIElement Draw(Drawing drawing, Brush shadingBrush)
        {
            CoordinateSystem cs = drawing.CoordinateSystem;

            var shading = new System.Windows.Shapes.Polygon();
            PointCollection points = new PointCollection();
            foreach (GeometryTutorLib.ConcreteAST.Point p in Region.GetPolygonalized().points)
            {
                points.Add(ToPhysical(cs, new Point(p.X, p.Y)));
            }
            shading.Points = points;

            shading.Fill = shadingBrush;
            shading.Stroke = new SolidColorBrush(Colors.DarkGray);
            shading.StrokeThickness = 1;
            Canvas.SetTop(shading, 0);
            Canvas.SetLeft(shading, 0);
            Canvas.SetZIndex(shading, (int)ZOrder.Shading);
            return shading;
        }

        public static Brush MakeHatchBrush(Color c1, Color c2)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.MappingMode = BrushMappingMode.Absolute;
            brush.SpreadMethod = GradientSpreadMethod.Repeat;
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(6, 6);
            GradientStop gs = new GradientStop();
            gs.Color = c1;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c1;
            gs.Offset = 0.5;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c2;
            gs.Offset = .5;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c2;
            gs.Offset = 1;
            brush.GradientStops.Add(gs);
            return brush;
        }
    }
}
