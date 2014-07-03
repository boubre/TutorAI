using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    class Page306Theorem7_4_1 : CirclesProblem
    {
        //Demonstrates: congruent chords have congruent arcs

        public Page306Theorem7_4_1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", 2, Math.Sqrt(21)); points.Add(r);
            Point s = new Point("S", 3, -4); points.Add(s);
            Point t = new Point("T", -2, Math.Sqrt(21)); points.Add(t);
            Point u = new Point("U", -3, -4); points.Add(u);

            Segment rs = new Segment(r, s); segments.Add(rs);
            Segment tu = new Segment(t, u); segments.Add(tu);

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(rs, tu));

            MinorArc a1 = (MinorArc)parser.Get(new MinorArc(c, s, r));
            MinorArc a2 = (MinorArc)parser.Get(new MinorArc(c, u, t));
            goals.Add(new GeometricCongruentArcs(a1, a2));
        }
    }
}
