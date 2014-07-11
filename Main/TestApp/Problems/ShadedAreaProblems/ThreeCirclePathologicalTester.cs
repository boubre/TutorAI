using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class ThreeCirclePathologicalTester : ActualShadedAreaProblem
    {
        public ThreeCirclePathologicalTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -2.0, 0); points.Add(a);
            Point b = new Point("B", 2.0, 0); points.Add(b);
            Point c = new Point("C", 0, 2.0 * System.Math.Sqrt(3)); points.Add(c);

            circles.Add(new Circle(a, 2.0));
            circles.Add(new Circle(b, 2.0));
            circles.Add(new Circle(c, 2.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            // The goal is the entire area of the figure.
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.atomicRegions);
        }
    }
}