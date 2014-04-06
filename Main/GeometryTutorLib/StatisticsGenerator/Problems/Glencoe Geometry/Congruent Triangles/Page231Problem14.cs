using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 231 problem 14
    //
    public class Page231Problem14 : CongruentTrianglesProblem
    {
        public Page231Problem14(bool onoff) : base(onoff)
        {
            problemName = "Page 231 Problem 14";

            Point j = new Point("J", 0, 0); intrinsic.Add(j);
            Point k = new Point("K", 3, 2); intrinsic.Add(k);
            Point l = new Point("L", 9, 0); intrinsic.Add(l);
            Point m = new Point("M", 5, 0); intrinsic.Add(m);
            Point n = new Point("N", 3, -2); intrinsic.Add(n);

            Segment jk = new Segment(j, k); intrinsic.Add(jk);
            Segment jn = new Segment(j, n); intrinsic.Add(jn);
            Segment kl = new Segment(k, l); intrinsic.Add(kl);
            Segment km = new Segment(k, m); intrinsic.Add(km);
            Segment ln = new Segment(l, n); intrinsic.Add(ln);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(m);
            pts.Add(l);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentTriangles(new Triangle(j, k, m), new Triangle(j, n, m)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(j, k, l), new Triangle(j, n, l)));
        }
    }
}