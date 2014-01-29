﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 144 Problem 1
    //
    public class Page144Problem01 : CongruentTrianglesProblem
    {
        public Page144Problem01(bool onoff) : base(onoff)
        {
            problemName = "Page 144 Problem 1";


            Point n = new Point("N", 0, 0); intrinsic.Add(n);
            Point r = new Point("R", 1, 4); intrinsic.Add(r);
            Point x = new Point("X", 3, 0); intrinsic.Add(x);
            Point e = new Point("E", 2, 2); intrinsic.Add(e);
            
            Point o = new Point("O", 10, 0); intrinsic.Add(o);
            Point l = new Point("L", 11, 4); intrinsic.Add(l);
            Point y = new Point("Y", 13, 0); intrinsic.Add(y);
            Point s = new Point("S", 12, 2); intrinsic.Add(s);

            Segment nr = new Segment(n, r); intrinsic.Add(nr);
            Segment nx = new Segment(n, x); intrinsic.Add(nx);
            Segment ne = new Segment(n, e); intrinsic.Add(ne);
            
            Segment lo = new Segment(l, o); intrinsic.Add(lo);
            Segment oy = new Segment(o, y); intrinsic.Add(oy);
            Segment os = new Segment(o, s); intrinsic.Add(os);
            

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(e);
            pts.Add(x);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(l);
            pts2.Add(s);
            pts2.Add(y);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, e)), GetProblemSegment(intrinsic, new Segment(e, x)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, e)), GetProblemSegment(intrinsic, new Segment(s, y)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, e)), GetProblemSegment(intrinsic, new Segment(l, s)), "Given"));
            given.Add(new GeometricCongruentSegments(nr, lo, "Given"));
            given.Add(new GeometricCongruentSegments(nx, oy, "Given"));

            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(n, e)), GetProblemSegment(intrinsic, new Segment(o, s)), "GOAL"));
        }
    }
}