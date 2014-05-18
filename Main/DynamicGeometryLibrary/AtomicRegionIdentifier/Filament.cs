using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class Filament : Primitive
    {
        public List<Point> points;

        public Filament()
        {
            points = new List<Point>();
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }
    }
}