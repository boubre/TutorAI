using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 135 #21
    //
    public class Page135Problem21 : CongruentTrianglesProblem
    {
        public Page135Problem21(bool onoff) : base(onoff)
        {
            problemName = "Page 135 Problem 21";
            numberOfOriginalTextProblems = 1;

            Point f = new Point("F", -3, 4);   intrinsic.Add(f);
            Point l = new Point("L", -1, 4);  intrinsic.Add(l);
            Point a = new Point("A", 1, 4);   intrinsic.Add(a);
            Point k = new Point("K", 3, 4); intrinsic.Add(k);
            Point m = new Point("M", -1.5, 2);   intrinsic.Add(m);
            Point n = new Point("N", 1.5, 2);  intrinsic.Add(n);
            Point s = new Point("S", 0, 0);   intrinsic.Add(s);

            Point x = new Point("X", 0, 3.2); intrinsic.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(m);
            pts.Add(s);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(l);
            pts.Add(a);
            pts.Add(k);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(x);
            pts.Add(n);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(m);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(n);
            pts.Add(s);
            Collinear coll5 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateSegmentClauses(coll5));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(f, l)),
                                                     GetProblemSegment(intrinsic, new Segment(a, k)), "Given"));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(s, f)),
                                                     GetProblemSegment(intrinsic, new Segment(s, k)), "Given"));

            given.Add(new Midpoint(m, GetProblemSegment(intrinsic, new Segment(s, f)), "Given"));
            given.Add(new Midpoint(n, GetProblemSegment(intrinsic, new Segment(s, k)), "Given"));

            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, m)),
                                                     GetProblemSegment(intrinsic, new Segment(l, n)), "GOAL"));
        }
    }
}