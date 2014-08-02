﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page4Row1Prob7 : ActualShadedAreaProblem
    {
        public Page4Row1Prob7(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 8, 0); points.Add(b);
            Point c = new Point("C", 8, -8); points.Add(c);
            Point d = new Point("D", 0, -8); points.Add(d);
            Point o = new Point("O", 4, -4); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            circles.Add(new Circle(o, 4.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, ad));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ab, 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7.8, -.5));
            wanted.Add(new Point("", .5, -7.8));
            wanted.Add(new Point("", 7.8, -7.5));
            wanted.Add(new Point("", .5, -.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);


            SetSolutionArea(64 - 16 * System.Math.PI);
        }
    }
}