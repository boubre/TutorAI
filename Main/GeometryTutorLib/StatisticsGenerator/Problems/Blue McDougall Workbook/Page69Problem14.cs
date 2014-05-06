using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 69 Problem 14
    //
    public class Page69Problem14 : CongruentTrianglesProblem
    {
        public Page69Problem14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 69 Problem 14";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 7, -6); intrinsic.Add(b);
            Point c = new Point("C", 14, 0); intrinsic.Add(c);
            Point d = new Point("D", 7, 0); intrinsic.Add(d);

            Segment ba = new Segment(b, a); intrinsic.Add(ba);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ba, bc));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, d, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(c, b, d)));
        }
    }
}