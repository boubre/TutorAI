using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 144 Problem 2
    //
    public class Page144Problem02 : CongruentTrianglesProblem
    {
        public Page144Problem02(bool onoff) : base(onoff)
        {
            problemName = "Page 144 Problem 2";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 0, 20); intrinsic.Add(a);
            Point b = new Point("B", 5, 32); intrinsic.Add(b);
            Point c = new Point("C", 45, 20); intrinsic.Add(c);
            Point d = new Point("D", 40, 8); intrinsic.Add(d);
            Point e = new Point("E", 5, 20); intrinsic.Add(e);
            Point f = new Point("F", 40, 20); intrinsic.Add(f);
            

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment be = new Segment(b, e); intrinsic.Add(be);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment df = new Segment(d, f); intrinsic.Add(df);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(f);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ab), GetProblemSegment(intrinsic, cd), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ad), GetProblemSegment(intrinsic, bc), "Given"));
            given.Add(new RightAngle(a, e, b, "Given"));
            given.Add(new RightAngle(c, f, d, "Given"));

            goals.Add(new GeometricCongruentSegments(be, df, "GOAL"));
        }
    }
}