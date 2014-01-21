using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Book I: Page 123 Example 5
    //
    public class IPage123Example5 : CongruentTrianglesProblem
    {
        public IPage123Example5(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 123 Example 5";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 3, 4); intrinsic.Add(a);
            Point e = new Point("E", 1.5, 2); intrinsic.Add(e);
            Point f = new Point("F", 4.5, 2); intrinsic.Add(f);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);

            Point x = new Point("X", 3, 4.0 / 3.0); intrinsic.Add(x);

            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(x);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(x);
            pts.Add(b);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(e, GetProblemSegment(intrinsic, new Segment(a, b)), "Given"));
            given.Add(new Midpoint(f, GetProblemSegment(intrinsic, new Segment(a, c)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a,b)), GetProblemSegment(intrinsic, new Segment(a, c)), "Given"));

            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(b, f)), GetProblemSegment(intrinsic, new Segment(c, e)), "GOAL"));
        }
    }
}