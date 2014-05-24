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

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("Filament { ");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].ToString());
                if (p < points.Count - 1) str.Append(", ");
            }
            str.Append(" }");

            return str.ToString();
        }
    }
}