using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 36
    //
    public class Page156Problem36 : ParallelLinesProblem
    {
        public Page156Problem36(bool onoff, bool complete) : base(onoff, complete)
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
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(m, j, k)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(m, k, j))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(n, k, l)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(n, l, k))));
            given.Add(new GeometricParallel(jm, kn));

            goals.Add(new GeometricParallel(km, ln));
        }
    }
}