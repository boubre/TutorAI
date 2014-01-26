using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 284 problem 18
    //
    public class Page284Problem18 : CongruentTrianglesProblem
    {
        public Page284Problem18(bool onoff) : base(onoff)
        {
            problemName = "Page 284 Problem 18";


            Point d = new Point("D", 0, 0);  intrinsic.Add(d);
            Point e = new Point("E", 6, -2); intrinsic.Add(e);
            Point f = new Point("F", 6, 0);  intrinsic.Add(f);
            Point g = new Point("G", 6, 2);  intrinsic.Add(g);
            Point h = new Point("H", 12, 0); intrinsic.Add(h);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(f);
            pts.Add(h);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(g);
            pts2.Add(f);
            pts2.Add(e);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(de, gh, "GivenS"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(e, f)), GetProblemSegment(intrinsic, new Segment(f, g)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(e, f, d)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(g, f, h)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(e, f, d), new Triangle(g, f, h), "GOAL"));
        }
    }
}