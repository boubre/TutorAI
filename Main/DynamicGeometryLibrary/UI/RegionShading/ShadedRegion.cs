using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace DynamicGeometry.UI.RegionShading
{
    public class ShadedRegion
    {
        public readonly static int[] COLORS = 
        {
            //ARGB integer colors
            unchecked((int)0xFFFF0000), //red
            unchecked((int)0xFF800000), //maroon
            unchecked((int)0xFF00FF00), //lime
            unchecked((int)0xFF008000), //green
            unchecked((int)0xFF0000FF), //blue
            unchecked((int)0xFF000080), //navy
            unchecked((int)0xFF00FFFF), //aqua
            unchecked((int)0xFF008080), //teal
            unchecked((int)0xFFFF00FF), //fuchsia
            unchecked((int)0xFF800080) //purple
        };

        public static List<ShadedRegion> ShadedRegions { get; private set; }

        public AtomicRegion Region { get; private set; }
        public Point PointInShape { get; private set; }

        /// <summary>
        /// Create a new ShadedRegion
        /// </summary>
        /// <param name="region">The region to shade</param>
        /// <param name="pt">A point in the region</param>
        private ShadedRegion(AtomicRegion region, Point pt)
        {
            Region = region;
            PointInShape = pt;
        }

        /// <summary>
        /// Create a new ShadedRegion and add it to the current list of ShadedRegions
        /// </summary>
        /// <param name="ar">The region to shade</param>
        /// <param name="pt">A point in the region</param>
        /// <returns>The created ShadedRegion</returns>
        public static ShadedRegion CreateAndAdd(AtomicRegion ar, Point pt)
        {
            ShadedRegion region = new ShadedRegion(ar, pt);

            if (ShadedRegions == null)
            {
                ShadedRegions = new List<ShadedRegion>();
            }

            ShadedRegions.Add(region);

            return region;
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
        /// Draw the shaded region and return the corresponding image.
        /// The image will be the size of the canvas, but transparent where the region is not.
        /// The Zindex, Top, and Left attributes will also be set.
        /// </summary>
        /// <param name="drawing">The current drawing</param>
        /// <param name="color1">The first color (ARGB) to shade with</param>
        /// <param name="color2">The second color (ARGB) to shadw with</param>
        /// <returns>The image, with the ShadedRegion drawn</returns>
        public Image Draw(Drawing drawing, int color1, int color2)
        {
            //These next two lines are temporary
            ShapeAtomicRegion sar = Region as ShapeAtomicRegion;
            if (sar == null) return null;

            CoordinateSystem cs = drawing.CoordinateSystem;
            WriteableBitmap bmp = new WriteableBitmap((int)cs.PhysicalSize.X, (int)cs.PhysicalSize.Y);
            Point start = ToPhysical(cs, PointInShape);

            FloodFill(bmp, start, cs, color1, color2);

            Image img = new Image();
            img.Source = bmp;
            Canvas.SetTop(img, 0);
            Canvas.SetLeft(img, 0);
            Canvas.SetZIndex(img, (int)ZOrder.Shading);
            return img;
        }

        /// <summary>
        /// Using the flood fill algorithm, shade the region.
        /// </summary>
        /// <param name="bmp">The bitmap to color</param>
        /// <param name="start">The starting physical point (pixel). Should be inside the region.</param>
        /// <param name="cs">The current coordinate system</param>
        /// <param name="color1">The first color (ARGB) to shade with</param>
        /// <param name="color2">The second color (ARGB) to shade with</param>
        private void FloodFill(WriteableBitmap bmp, Point start, CoordinateSystem cs, int color1, int color2)
        {
            /* This algorithm is recursive by nature, but with large triangles it may overflow the stack.
             * (This is experimentally pretty easy to do, so it is a real problem.)
             * Therefore, we create our own stack that will not overflow and crash the program.
             * Very large figures still take a noticeable amount of time to shade, but it seems acceptable.
             * NOTE: May be able to speed up by reducing the number of isInRegion tests.
             */
            Stack<Point> toCheck = new Stack<Point>();
            toCheck.Push(start);

            while (toCheck.Count > 0) //Simulated Recursion
            {
                Point pt = toCheck.Pop();

                if (Test(bmp, pt, cs))
                {
                    //Color the pixel and add its neighbors to the stack.
                    bmp.Pixels[ToPixel(bmp, pt)] = Color(pt, color1, color2);

                    toCheck.Push(new Point(pt.X - 1, pt.Y));
                    toCheck.Push(new Point(pt.X + 1, pt.Y));
                    toCheck.Push(new Point(pt.X, pt.Y - 1));
                    toCheck.Push(new Point(pt.X, pt.Y + 1));
                }
            }
        }

        /// <summary>
        /// Helper to FloodFill:
        /// Test a pixel to see if we should color it and add its neighbors to the stack.
        /// </summary>
        /// <param name="bmp">The bitmap we are coloring</param>
        /// <param name="pt">The physical point (pixel) to test</param>
        /// <param name="cs">The current coordinate system</param>
        /// <returns>TRUE if we should color this pixel</returns>
        private bool Test(WriteableBitmap bmp, Point pt, CoordinateSystem cs)
        {
            int pixel = ToPixel(bmp, pt);
            System.Diagnostics.Debug.Assert(pixel >= 0);

            if (bmp.Pixels[pixel] != 0) //Pixel is colored, we have visited it
            {
                return false;
            }

            Point logical = ToLogical(cs, pt);
            // public virtual bool PointLiesInOrOn(Point pt)
            //FOR NOW: USE THIS
            ShapeAtomicRegion sar = Region as ShapeAtomicRegion;
            GeometryTutorLib.ConcreteAST.Polygon shape = sar.shape as GeometryTutorLib.ConcreteAST.Polygon;
            if (shape == null || !shape.IsInPolygon(new GeometryTutorLib.ConcreteAST.Point("shadingtest", logical.X, logical.Y))) //Not in shape
            {
                return false;
            }
            //END FOR NOW

            return true;
        }

        /// <summary>
        /// Helper to FloodFill:
        /// Figure out which color a pixel should be.
        /// </summary>
        /// <param name="pt">The physical point (pixel)</param>
        /// <param name="color1">The first color (ARGB) to shade with</param>
        /// <param name="color2">The second color (ARGB) to shade with</param>
        /// <returns></returns>
        private int Color(Point pt, int color1, int color2)
        {
            int x = (int)pt.X;
            int y = (int)pt.Y;

            if ((x + y) % 6 <= 2) //Will result in diagonal stripes
                return color1;
            else
                return color2;
        }

        /// <summary>
        /// Convert a physical point (pixel) into an integer representing the corresponding pixel in the bitmap.
        /// </summary>
        /// <param name="bmp">The bitmap used to determine the pixel's value</param>
        /// <param name="pt">The physical point (pixel) to convert</param>
        /// <returns></returns>
        private int ToPixel(WriteableBitmap bmp, Point pt)
        {
            return (int)pt.Y * bmp.PixelWidth + (int)pt.X;
        }
    }
}
