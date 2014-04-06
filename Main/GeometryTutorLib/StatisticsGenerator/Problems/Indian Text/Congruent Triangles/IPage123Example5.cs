using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book I: Page 123 Example 5
    //
    public class IPage123Example5 : CongruentTrianglesProblem
    {
        public IPage123Example5(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 123 Example 5";


            Point a = new Point("A", 3, 4); intrinsic.Add(a);
            Point e = new Point("E", 1.5, 2); intrinsic.Add(e);
            Point f = new Point("F", 4.5, 2); intrinsic.Add(f);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);

            Point x = new Point("X", 3, 4.0 / 3.0); intrinsic.Add(x);

            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(x);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(x);
            pts.Add(b);
            Collinear coll4 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, e, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)))));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, f, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a,b)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, f)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, e))));
        }
    }
}