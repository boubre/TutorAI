using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page1Col1Prob1 : ActualShadedAreaProblem
    {
        public Page1Col1Prob1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", Math.Sqrt(2), Math.Sqrt(2)); points.Add(a);
            Point b = new Point("B", 2, 0); points.Add(b);
            Point c = new Point("C", -2, 0); points.Add(c);
            Point d = new Point("D", 0, -2); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);
        
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment od = new Segment(o, d); segments.Add(od);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goalRegions.Add(parser.implied.GetAtomicRegionByPoint(new Point("", 1, -1)));
            goalRegions.Add(parser.implied.GetAtomicRegionByPoint(new Point("", -1, 1)));
        }
    }
}