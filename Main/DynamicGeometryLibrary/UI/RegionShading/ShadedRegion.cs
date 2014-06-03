using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DynamicGeometry.UI.RegionShading
{
    public abstract class ShadedRegion
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

        public DrawingHost DrawingHost { get; private set; }

        protected WriteableBitmap bmp;

        /// <summary>
        /// Create a new ShadedRegion
        /// </summary>
        /// <param name="DrawingHost">The drawing host</param>
        protected ShadedRegion(DrawingHost DrawingHost)
        {
            CoordinateSystem cs = DrawingHost.CurrentDrawing.CoordinateSystem;
            bmp = new WriteableBitmap((int)cs.PhysicalSize.X, (int)cs.PhysicalSize.Y);
        }
    }
}
