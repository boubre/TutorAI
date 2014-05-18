using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class MinimalCycle : Primitive
    {
        public List<Point> points;

        public MinimalCycle()
        {
            points = new List<Point>();
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public void AddAll(List<Point> pts)
        {
            points.AddRange(pts);
        }
    }
}