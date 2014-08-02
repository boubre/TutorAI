﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page3Prob21 : ActualShadedAreaProblem
    {
        public Page3Prob21(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 4, 4); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point p = new Point("P", 4, 2); points.Add(p);
            Point o = new Point("O", 0, 2); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts.Add(d);
            pts.Add(p);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2));
            circles.Add(new Circle(p, 2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(da, 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, b)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(p, c)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 0.5));
            wanted.Add(new Point("", 2, 3.5));
            wanted.Add(new Point("", 1, 2));
            wanted.Add(new Point("", 3, 2));
            wanted.Add(new Point("", -1, 0));
            wanted.Add(new Point("", 5, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4 * (4 + System.Math.PI));
        }
    }
}