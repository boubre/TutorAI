using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 145 Problem 3
    //
    public class Page145Problem03: CongruentTrianglesProblem
    {
        public Page145Problem03(bool onoff) : base(onoff)
        {
            problemName = "Page 145 Problem 3";


            Point g = new Point("G", 0, 10); intrinsic.Add(g);
            Point r = new Point("R", 1, 12); intrinsic.Add(r);
            Point j = new Point("J", 1, 10); intrinsic.Add(j);
            Point a = new Point("A", 3, 10); intrinsic.Add(a);
            Point k = new Point("K", 5, 10); intrinsic.Add(k);
            Point n = new Point("N", 5, 8); intrinsic.Add(n);
            Point t = new Point("T", 6, 10); intrinsic.Add(t);

            Segment gr = new Segment(g, r); intrinsic.Add(gr);
            Segment jr = new Segment(j, r); intrinsic.Add(jr);
            Segment kn = new Segment(k, n); intrinsic.Add(kn);
            Segment nt = new Segment(n, t); intrinsic.Add(nt);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(j);
            pts.Add(a);
            pts.Add(k);
            pts.Add(t);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(a);
            pts.Add(n);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(g, j)), GetProblemSegment(intrinsic, new Segment(k, t)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, r)), GetProblemSegment(intrinsic, new Segment(a, n)), "Given"));
            given.Add(new RightAngle(r, j, a, "Given"));
            given.Add(new RightAngle(n, k, a, "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(j, g, r)), GetProblemAngle(intrinsic, new Angle(n, t, k)), "GOAL"));
        }
    }
}