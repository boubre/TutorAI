using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 160 Problem 42
    //
    public class Page160Problem42 : CongruentTrianglesProblem
    {
        public Page160Problem42(bool onoff) : base(onoff)
        {
            problemName = "Page 160 Problem 42";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", -3, -1); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 2); intrinsic.Add(c);
            Point d = new Point("D", 1, -3); intrinsic.Add(d);
            Point x = new Point("X", -5, 5); intrinsic.Add(x);
            Point y = new Point("Y", -2, 6); intrinsic.Add(y);
            Point z = new Point("Z", 1, 7); intrinsic.Add(z);
            Point q = new Point("Q", -3, 9); intrinsic.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(x);
            pts2.Add(y);
            pts2.Add(z);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            pts2 = new List<Point>();
            pts2.Add(a);
            pts2.Add(b);
            pts2.Add(c);
            Collinear coll3 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(x, z)), "Given"));
            given.Add(new Perpendicular(GetProblemIntersection(intrinsic, GetProblemSegment(intrinsic, new Segment(q, d)), GetProblemSegment(intrinsic, new Segment(x, z))), "Given"));

            goals.Add(new Supplementary(GetProblemAngle(intrinsic, new Angle(c, b, y)), GetProblemAngle(intrinsic, new Angle(z, y, b)), "GOAL"));
        }
    }
}