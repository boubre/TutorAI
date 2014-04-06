using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 90 Problem 22
    //
    public class Page90Problem22 : CongruentTrianglesProblem
    {
        public Page90Problem22(bool onoff) : base(onoff)
        {
            problemName = "Page 90 Problem 22";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 5, 0); intrinsic.Add(b);
            Point c = new Point("C", 2.5, 20); intrinsic.Add(c);
            Point d = new Point("D", 2.5, 0); intrinsic.Add(d);

            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment cb = new Segment(c, b); intrinsic.Add(cb);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new PerpendicularBisector(ClauseConstructor.GetProblemIntersection(intrinsic, cd, new Segment(a, b)), cd));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, c, d), new Triangle(b, c, d)));
        }
    }
}