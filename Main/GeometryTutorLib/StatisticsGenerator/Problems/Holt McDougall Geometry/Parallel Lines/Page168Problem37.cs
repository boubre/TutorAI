using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 168 Problem 37
    //
    public class Page168Problem37 : ParallelLinesProblem
    {
        public Page168Problem37(bool onoff) : base(onoff)
        {
            problemName = "Page 168 Problem 37";


            Point a = new Point("A", 1, 1); intrinsic.Add(a);
            Point b = new Point("B", 6, 1); intrinsic.Add(b);
            Point c = new Point("C", 10, 1); intrinsic.Add(c);
            Point d = new Point("D", 1, 3); intrinsic.Add(d);
            Point e = new Point("E", 2, 3); intrinsic.Add(e);
            Point f = new Point("F", 10, 3); intrinsic.Add(f);
            Point g = new Point("G", 0, 4); intrinsic.Add(g);
            Point h = new Point("H", 8, 0); intrinsic.Add(h);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(e);
            pts2.Add(f);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            List<Point> pts3 = new List<Point>();
            pts3.Add(g);
            pts3.Add(e);
            pts3.Add(b);
            pts3.Add(h);
            Collinear coll3 = new Collinear(pts3, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Supplementary(GetProblemAngle(intrinsic, new Angle(d, e, b)), GetProblemAngle(intrinsic, new Angle(a, b, e)), "Given"));
            goals.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(d, f)), "GOAL"));
        }
    }
}