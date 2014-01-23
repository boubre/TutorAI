using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 76 Problem 4
    //
    public class Page76Problem4 : CongruentTrianglesProblem
    {
        public Page76Problem4(bool onoff) : base(onoff)
        {
            problemName = "Page 76 Problem 4";
            numberOfOriginalTextProblems = 1;

            Point k = new Point("K", 0, 8); intrinsic.Add(k);
            Point i = new Point("I", 3.5, 2.4); intrinsic.Add(i);
            Point n = new Point("N", 0, 0); intrinsic.Add(n);
            Point g = new Point("G", 2, 0); intrinsic.Add(g);
            Point h = new Point("H", 5, 0); intrinsic.Add(h);
            Point t = new Point("T", 7, 0); intrinsic.Add(t);
            Point m = new Point("M", 7, 8); intrinsic.Add(m);

     System.Diagnostics.Debug.WriteLine(new Segment(k, h).FindIntersection(new Segment(m, g)));

            Segment kn = new Segment(k, n); intrinsic.Add(kn);
            Segment mt = new Segment(m, t); intrinsic.Add(mt);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(i);
            pts.Add(h);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(i);
            pts.Add(g);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(g);
            pts.Add(h);
            pts.Add(t);
            Collinear coll3 = new Collinear(pts, "Intrinsic");


            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(n, k, h)), GetProblemAngle(intrinsic, new Angle(t, m, g)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(n, g)), GetProblemSegment(intrinsic, new Segment(h, t)), "Given"));
            given.Add(new RightAngle(new Angle(k, n, t), "Given"));
            given.Add(new RightAngle(new Angle(m, t, n), "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(k, h, n)), GetProblemAngle(intrinsic, new Angle(m, g, t)), "Given"));
        }
    }
}