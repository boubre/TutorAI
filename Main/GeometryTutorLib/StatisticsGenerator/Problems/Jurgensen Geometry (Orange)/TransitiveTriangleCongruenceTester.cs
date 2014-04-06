using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class TranstiveTriangleCongruenceTester : TransversalsProblem
    {
        public TranstiveTriangleCongruenceTester(bool onoff) : base(onoff)
        {
            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 4);  intrinsic.Add(b);
            Point c = new Point("C", 2, 0);  intrinsic.Add(c);

            Point d = new Point("D", 5, 0); intrinsic.Add(d);
            Point e = new Point("E", 5, 4); intrinsic.Add(e);
            Point f = new Point("F", 7, 0); intrinsic.Add(f);

            Point m = new Point("M", 12, 0); intrinsic.Add(m);
            Point n = new Point("N", 12, 4); intrinsic.Add(n);
            Point p = new Point("P", 14, 0); intrinsic.Add(p);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment ef = new Segment(e, f); intrinsic.Add(ef);
            Segment df = new Segment(d, f); intrinsic.Add(df);

            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment np = new Segment(n, p); intrinsic.Add(np);
            Segment mp = new Segment(m, p); intrinsic.Add(mp);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, f)));
            given.Add(new GeometricCongruentTriangles(new Triangle(f, d, e), new Triangle(p, m, n)));

            goals.Add(new AlgebraicCongruentTriangles(new Triangle(a, b, c), new Triangle(m, n, p)));
        }
    }
}