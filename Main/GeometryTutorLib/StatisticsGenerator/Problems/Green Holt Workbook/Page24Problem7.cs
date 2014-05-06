using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 24 Problem 7
    //
    public class Page24Problem7 : TransversalsProblem
    {
        public Page24Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 24 Problem 7";


            Point a = new Point("A", 1, 4); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 3.5, 3); intrinsic.Add(c);
            Point d = new Point("D", 6, 2); intrinsic.Add(d);
            Point e = new Point("E", 7, 6); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            System.Diagnostics.Debug.WriteLine("Intersection: " + new Segment(b, e).FindIntersection(new Segment(a, d)));

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, c, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, d)))));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, c, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, e)))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
        }
    }
}