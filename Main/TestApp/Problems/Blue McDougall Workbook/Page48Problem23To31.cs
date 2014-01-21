using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 48 Problem 23-31
    //
    public class Page48Problem23To31 : ParallelLinesProblem
    {
        public Page48Problem23To31(bool onoff) : base(onoff)
        {
            problemName = "Page 48 Problem 23-31";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 0, 5); intrinsic.Add(a);
            Point b = new Point("B", 2, 4); intrinsic.Add(b);
            Point c = new Point("C", 4, 3); intrinsic.Add(c);
            Point m = new Point("M", 3, 6); intrinsic.Add(m);

            Point x = new Point("X", -2, 1); intrinsic.Add(x);
            Point y = new Point("Y", 0, 0); intrinsic.Add(y);
            Point z = new Point("Z", 2, -1); intrinsic.Add(z);
            Point q = new Point("Q", -1, -2); intrinsic.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(m);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Perpendicular(GetProblemIntersection(intrinsic, new Segment(m, q), new Segment(a, c)), "Given"));
            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(x, z)), "Given"));

            goals.Add(new Perpendicular(GetProblemIntersection(intrinsic, new Segment(m, q), new Segment(x, z)), "GOAL"));
        }
    }
}