﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 160 Problem 42
    //
    public class Page160Problem42 : CongruentTrianglesProblem
    {
        public Page160Problem42(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 160 Problem 42";


            Point a = new Point("A", -3, -1); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 6, 2); points.Add(c);
            Point d = new Point("D", 1, -3); points.Add(d);
            Point x = new Point("X", -5, 5); points.Add(x);
            Point y = new Point("Y", -2, 6); points.Add(y);
            Point z = new Point("Z", 1, 7); points.Add(z);
            Point q = new Point("Q", -3, 9); points.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(x, z))));
            given.Add(new Perpendicular(parser.GetIntersection((Segment)parser.Get(new Segment(q, d)), (Segment)parser.Get(new Segment(x, z)))));

            goals.Add(new Supplementary((Angle)parser.Get(new Angle(c, b, y)), (Angle)parser.Get(new Angle(z, y, b))));
        }
    }
}