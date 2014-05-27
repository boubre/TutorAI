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
using GeometryTutorLib.Area_Based_Analyses;

namespace DynamicGeometry.UI.RegionShading
{
    public class ShadedShapeRegion : ShadedRegion
    {
        public ShapeAtomicRegion Region { get; private set; }
        
        /// <summary>
        /// Create a new ShadedRegion
        /// </summary>
        /// <param name="DrawingHost">The drawing host</param>
        /// <param name="Region">The region this shading corresponds to</param>      
        public ShadedShapeRegion(DrawingHost DrawingHost, ShapeAtomicRegion Region) : base(DrawingHost)
        {
            this.Region = Region;
        }
    }
}
