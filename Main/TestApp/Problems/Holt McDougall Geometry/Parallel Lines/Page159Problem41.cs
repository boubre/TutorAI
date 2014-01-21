using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 159 Problem 41
    //
    public class Page159Problem41 : ParallelLinesProblem
    {
        public Page159Problem41(bool onoff) : base(onoff)
        {
            problemName = "Page 159 Problem 41";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 1, 5); intrinsic.Add(a);
            Point b = new Point("B", 5, 5); intrinsic.Add(b);
            Point c = new Point("C", 1, 10); intrinsic.Add(c);
            Point d = new Point("D", 5, 10); intrinsic.Add(d);
            Point e = new Point("E", 2, 5); intrinsic.Add(e);
            Point f = new Point("F", 4, 10); intrinsic.Add(f);

            Segment ef = new Segment(e, f); intrinsic.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(f);
            pts2.Add(c);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, b)), GetProblemSegment(intrinsic, new Segment(c, d)), "Given"));
        }
    }
}