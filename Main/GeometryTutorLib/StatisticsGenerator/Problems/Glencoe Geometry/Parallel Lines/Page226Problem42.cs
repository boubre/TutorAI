using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 226 problem 42
    //
    public class Page226Problem42 : ParallelLinesProblem
    {
        public Page226Problem42(bool onoff) : base(onoff)
        {
            problemName = "Page 226 Problem 42";

            Point j = new Point("J", 0, 0); intrinsic.Add(j);
            Point k = new Point("K", 0, 12); intrinsic.Add(k);
            Point l = new Point("L", 3, 2); intrinsic.Add(l);
            Point m = new Point("M", 3, 10); intrinsic.Add(m);
            Point n = new Point("N", 9, 6); intrinsic.Add(n);

            Segment jk = new Segment(j, k); intrinsic.Add(jk);
            Segment lm = new Segment(l, m); intrinsic.Add(lm);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(l);
            pts.Add(n);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(k);
            pts2.Add(m);
            pts2.Add(n);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new IsoscelesTriangle(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(k, n)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(j, n)), jk));
            given.Add(new GeometricParallel(jk, lm));

            goals.Add(new Strengthened(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(n, m, l)),  new IsoscelesTriangle(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(n, m, l)))));
        }
    }
}