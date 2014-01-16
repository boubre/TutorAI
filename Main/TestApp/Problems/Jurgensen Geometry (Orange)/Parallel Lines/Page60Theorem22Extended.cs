using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 62 Problems 1-6
    //
    // Parallel Transversals
    //
    public class Page60Theorem22Extended : ParallelLinesProblem
    {
        public Page60Theorem22Extended(bool onoff)
            : base(onoff)
        {
            problemName = "Page 60 Theorem 22 Extended";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", -1, 3); intrinsic.Add(a);
            Point b = new Point("B", 4, 3);  intrinsic.Add(b);
            Point c = new Point("C", 0, 0);  intrinsic.Add(c);
            Point d = new Point("D", 5, 0);  intrinsic.Add(d);

            Point x = new Point("X", 2, 3);  intrinsic.Add(x);
            Point y = new Point("Y", 1, 0); intrinsic.Add(y);
            Point p = new Point("P", 3, 6); intrinsic.Add(p);
            Point q = new Point("Q", 0, -3); intrinsic.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(x);
            pts.Add(y);
            pts.Add(q);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, b)), GetProblemSegment(intrinsic, new Segment(c, d)), "Given"));
        }
    }
}