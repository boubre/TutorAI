using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 62 Problem 1
    //
    // Parallel Transversals
    //
    public class Page62Problem1 : ParallelLinesProblem
    {
        public Page62Problem1(bool onoff) : base(onoff)
        {
            problemName = "Page 62 Problem 1";
            numberOfOriginalTextProblems = 3;

            Point a = new Point("A", -1, 3); intrinsic.Add(a);
            Point b = new Point("B", 4, 3);  intrinsic.Add(b);
            Point c = new Point("C", 0, 0);  intrinsic.Add(c);
            Point d = new Point("D", 5, 0);  intrinsic.Add(d);

            Point e = new Point("E", -3, 3);  intrinsic.Add(e);
            Point f = new Point("F", -2, 6); intrinsic.Add(f);
            Point g = new Point("G", 3, 6);  intrinsic.Add(g);
            Point h = new Point("H", 8, 3);  intrinsic.Add(h);
            Point i = new Point("I", 10, 0); intrinsic.Add(i);
            Point j = new Point("J", 7, -6);  intrinsic.Add(j);
            Point k = new Point("K", 1, -3);  intrinsic.Add(k);
            Point l = new Point("L", -5, 0); intrinsic.Add(l);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(a);
            pts.Add(c);
            pts.Add(k);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(a);
            pts.Add(b);
            pts.Add(h);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(b);
            pts.Add(d);
            pts.Add(j);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(c);
            pts.Add(d);
            pts.Add(i);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(b, d)), "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, a, f)), GetProblemAngle(intrinsic, new Angle(g, b, a)), "GOAL"));
            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, a, f)), GetProblemAngle(intrinsic, new Angle(c, a, b)), "GOAL"));
            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, a, f)), GetProblemAngle(intrinsic, new Angle(d, b, h)), "GOAL"));
        }
    }
}