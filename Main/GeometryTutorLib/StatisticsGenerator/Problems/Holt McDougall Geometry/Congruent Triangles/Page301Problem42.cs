﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 301 problem 42
    //
    public class Page301Problem42 : CongruentTrianglesProblem
    {
        public Page301Problem42(bool onoff) : base(onoff)
        {
            problemName = "Page 301 Problem 42";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 6); intrinsic.Add(b);
            Point c = new Point("C", 4, 3); intrinsic.Add(c);
            Point d = new Point("D", 8, 0); intrinsic.Add(d);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, c, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, d)))));
            given.Add(new RightAngle(b, a, d));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, c)), ac));
            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(d, c)), ac));
        }
    }
}