﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 242 Problem 21
    //
    public class Page242Problem21 : CongruentTrianglesProblem
    {
        public Page242Problem21(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 242 Problem 21";


            Point t = new Point("T", 0, 0); intrinsic.Add(t);
            Point s = new Point("S", 6, 8); intrinsic.Add(s);
            Point o = new Point("O", 9, 12); intrinsic.Add(o);
            Point v = new Point("V", 13, 8); intrinsic.Add(v);
            Point w = new Point("W", 21, 0); intrinsic.Add(w);

            Segment tw = new Segment(t, w); intrinsic.Add(tw);
            Segment sv = new Segment(s, v); intrinsic.Add(sv);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(o);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(o);
            pts2.Add(v);
            pts2.Add(w);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, tw), ClauseConstructor.GetProblemSegment(intrinsic, sv)));
        }
    }
}