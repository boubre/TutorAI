using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    class Page307Theorem7_5 : CirclesProblem
    {
        //Demonstrates: A diameter perpendicular to a chord bisects the chord

        public Page307Theorem7_5(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -3, -4); points.Add(a);
            Point b = new Point("B", 3, -4); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);
            Point d = new Point("D", 0, -5); points.Add(d);
            Point z = new Point("Z", 0, -4); points.Add(z);

            //Segment ao = new Segment(a, o); segments.Add(ao);
            //Segment bo = new Segment(b, o); segments.Add(bo);

            List<Point> pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(o);
            pnts.Add(z);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(z);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 5.0);
            circles.Add(circle);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Intersection inter = (Intersection)parser.Get(new Intersection(z, (Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Strengthened(inter, new Perpendicular(inter)));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, z)), (Segment)parser.Get(new Segment(b, z))));
        }
    }
}