using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 229 Problem 07
    //
    public class Page229Problem07 : CongruentTrianglesProblem
    {
        public Page229Problem07(bool onoff)
            : base(onoff)
        {
            problemName = "Page 229 Problem 07";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 6, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 8); intrinsic.Add(c);
            Point p = new Point("P", 0, 10); intrinsic.Add(p);
            Point k = new Point("K", 9, 10); intrinsic.Add(k);
            Point n = new Point("N", 9, 12); intrinsic.Add(n);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment kp = new Segment(k, p); intrinsic.Add(kp);
            Segment kn = new Segment(k, n); intrinsic.Add(kn);
            Segment pn = new Segment(p, n); intrinsic.Add(pn);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));
        }
    }
}