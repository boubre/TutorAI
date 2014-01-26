using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 36
    //
    public class Page156Problem36 : ParallelLinesProblem
    {
        public Page156Problem36(bool onoff) : base(onoff)
        {
            problemName = "Page 156 Problem 36";

            Point j = new Point("J", 0, 10);  intrinsic.Add(j);
            Point k = new Point("K", 10, 10); intrinsic.Add(k);
            Point l = new Point("L", 20, 10); intrinsic.Add(l);
            Point m = new Point("M", 5, 0);   intrinsic.Add(m);
            Point n = new Point("N", 15, 0);  intrinsic.Add(n);

            Segment jm = new Segment(j, m); intrinsic.Add(jm);
            Segment km = new Segment(k, m); intrinsic.Add(km);
            Segment kn = new Segment(k, n); intrinsic.Add(kn);
            Segment ln = new Segment(l, n); intrinsic.Add(ln);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(k);
            pts.Add(l);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(m, j, k)), GetProblemAngle(intrinsic, new Angle(m, k, j)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(n, k, l)), GetProblemAngle(intrinsic, new Angle(n, l, k)), "Given"));
            given.Add(new GeometricParallel(jm, kn, "Given"));

            goals.Add(new GeometricParallel(km, ln, "GOAL"));
        }
    }
}