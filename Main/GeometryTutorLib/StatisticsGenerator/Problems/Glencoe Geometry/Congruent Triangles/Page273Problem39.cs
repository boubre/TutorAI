using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 273 problem 39
    //
    public class Page273Problem39 : CongruentTrianglesProblem
    {
        public Page273Problem39(bool onoff) : base(onoff)
        {
            problemName = "Page 273 Problem 39";

            Point a = new Point("A", 1, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 5); intrinsic.Add(b);
            Point c = new Point("C", 3, 3); intrinsic.Add(c);
            Point d = new Point("D", 5, 6); intrinsic.Add(d);
            Point e = new Point("E", 6, 1); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(b);
            pts2.Add(c);
            pts2.Add(e);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new SegmentBisector(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(a, d), new Segment(b, e)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, d))));
            given.Add(new GeometricParallel(ab, de));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
        }
    }
}