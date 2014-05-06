using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 48 Problem 23-31
    //
    public class Page48Problem23To31 : ParallelLinesProblem
    {
        public Page48Problem23To31(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 48 Problem 23-31";


            Point a = new Point("A", 0, 5); intrinsic.Add(a);
            Point b = new Point("B", 2, 4); intrinsic.Add(b);
            Point c = new Point("C", 4, 3); intrinsic.Add(c);
            Point m = new Point("M", 3, 6); intrinsic.Add(m);

            Point x = new Point("X", -2, 1); intrinsic.Add(x);
            Point y = new Point("Y", 0, 0); intrinsic.Add(y);
            Point z = new Point("Z", 2, -1); intrinsic.Add(z);
            Point q = new Point("Q", -1, -2); intrinsic.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(m);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(m, q), new Segment(a, c))));
            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(x, z))));

            goals.Add(new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(m, q), new Segment(x, z))));
        }
    }
}