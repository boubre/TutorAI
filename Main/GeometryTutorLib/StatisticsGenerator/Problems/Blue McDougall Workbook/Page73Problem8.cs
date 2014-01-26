using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 73 Problem 8
    //
    public class Page73Problem8 : CongruentTrianglesProblem
    {
        public Page73Problem8(bool onoff) : base(onoff)
        {
            problemName = "Page 73 Problem 8";


            Point t = new Point("T", -4, 5); intrinsic.Add(t);
            Point n = new Point("N", 0, 5); intrinsic.Add(n);
            Point s = new Point("S", 0, 0); intrinsic.Add(s);
            Point h = new Point("H", 0, -5); intrinsic.Add(h);
            Point u = new Point("U", 4, -5); intrinsic.Add(u);

            Segment nt = new Segment(n, t); intrinsic.Add(nt);
            Segment hu = new Segment(h, u); intrinsic.Add(hu);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(u);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(s);
            pts.Add(h);
            Collinear coll2 = new Collinear(pts, "Intrinsic");
            
            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(s, u, h)), GetProblemAngle(intrinsic, new Angle(n, t, s)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(n, s)), GetProblemSegment(intrinsic, new Segment(s, h)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(s, n, t), new Triangle(s, h, u), "GOAL"));
        }
    }
}