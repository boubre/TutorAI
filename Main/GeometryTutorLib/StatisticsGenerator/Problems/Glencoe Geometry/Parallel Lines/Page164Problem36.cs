using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 164 problem 36
    //
    public class Page164Problem36 : ParallelLinesProblem
    {
        public Page164Problem36(bool onoff) : base(onoff)
        {
            problemName = "Page 164 Problem 36";

            Point a = new Point("A", 11, 8); intrinsic.Add(a);
            Point b = new Point("B", 10, 8); intrinsic.Add(b);
            Point c = new Point("C", 10, 11); intrinsic.Add(c);
            Point d = new Point("D", 0, 8);  intrinsic.Add(d);
            Point e = new Point("E", 0, 0);   intrinsic.Add(e);
            Point f = new Point("F", 10, 0);  intrinsic.Add(f);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment ef = new Segment(e, f); intrinsic.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(b);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(b);
            pts2.Add(a);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, d, e)), GetProblemAngle(intrinsic, new Angle(c, b, d)), "Given"));

            goals.Add(new GeometricParallel(de, new Segment(f, c), "GOAL"));
        }
    }
}