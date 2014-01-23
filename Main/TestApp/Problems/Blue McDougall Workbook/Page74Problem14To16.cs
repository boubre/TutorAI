using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 74 Problem 14-16
    //
    public class Page74Problem14To16 : CongruentTrianglesProblem
    {
        public Page74Problem14To16(bool onoff) : base(onoff)
        {
            problemName = "Page 74 Problem 14-16";
            numberOfOriginalTextProblems = 3;

            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 5, 6); intrinsic.Add(b);
            Point c = new Point("C", 10, 0); intrinsic.Add(c);
            Point d = new Point("D", 6, 0); intrinsic.Add(d);
            Point e = new Point("E", 5, 0); intrinsic.Add(e);
            Point f = new Point("F", 4, 0); intrinsic.Add(f);

            Segment ba = new Segment(b, a); intrinsic.Add(ba);
            Segment bf = new Segment(b, f); intrinsic.Add(bf);
            Segment be = new Segment(b, e); intrinsic.Add(be);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(e);
            pts.Add(d);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(bf, bd, "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, f)), GetProblemSegment(intrinsic, new Segment(d, c)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(f, e)), GetProblemSegment(intrinsic, new Segment(e, d)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(f, a, b)), GetProblemAngle(intrinsic, new Angle(a, c, b)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, f, b)), GetProblemAngle(intrinsic, new Angle(e, d, b)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, b, f)), GetProblemAngle(intrinsic, new Angle(c, b, d)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(b, e, f), new Triangle(b, e, d), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, d, b), new Triangle(c, f, b), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, f, b), new Triangle(c, d, b), "GOAL"));
        }
    }
}