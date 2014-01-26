using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 34
    //
    public class Page156Problem34 : ParallelLinesProblem
    {
        public Page156Problem34(bool onoff) : base(onoff)
        {
            problemName = "Page 156 Problem 34";

            Point s = new Point("S", 0, 10); intrinsic.Add(s);
            Point t = new Point("T", 0, 0); intrinsic.Add(t);
            Point u = new Point("U", 10, 0); intrinsic.Add(u);
            Point v = new Point("V", 10, 10); intrinsic.Add(v);
            Point w = new Point("W", 5, 5); intrinsic.Add(w);

            Segment st = new Segment(s, t); intrinsic.Add(st);
            Segment uv = new Segment(u, v); intrinsic.Add(uv);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(w);
            pts.Add(v);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(s);
            pts2.Add(w);
            pts2.Add(u);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(w, v, u)), GetProblemAngle(intrinsic, new Angle(w, s, t)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(w, t, s)), GetProblemAngle(intrinsic, new Angle(w, s, t)), "Given"));

            goals.Add(new GeometricParallel(st, uv, "GOAL"));
        }
    }
}