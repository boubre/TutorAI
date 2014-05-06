using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 226 problem 43
    //
    public class Page226Problem43 : CongruentTrianglesProblem
    {
        public Page226Problem43(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 43";

            Point a = new Point("A", 0, 4); intrinsic.Add(a);
            Point b = new Point("B", 7, 5); intrinsic.Add(b);
            Point c = new Point("C", 11, 10); intrinsic.Add(c);
            Point d = new Point("D", 3, 0); intrinsic.Add(d);
            Point e = new Point("E", 14, 6); intrinsic.Add(e);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ce = new Segment(c, e); intrinsic.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(b);
            pts2.Add(c);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ad, ce));
            given.Add(new GeometricCongruentSegments(ad, ce));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(e, b, c)));
        }
    }
}