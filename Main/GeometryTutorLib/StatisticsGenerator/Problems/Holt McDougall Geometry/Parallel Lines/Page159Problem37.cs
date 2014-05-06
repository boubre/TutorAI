using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 159 Problem 37
    //
    public class Page159Problem37 : ParallelLinesProblem
    {
        public Page159Problem37(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 159 Problem 37";


            Point a = new Point("A", -3, -1); intrinsic.Add(a);
            Point b = new Point("B", 0, 0);   intrinsic.Add(b);
            Point c = new Point("C", 6, 2);   intrinsic.Add(c);
            Point d = new Point("D", 1, -4);  intrinsic.Add(d);
            Point x = new Point("X", -5, 7);  intrinsic.Add(x);
            Point y = new Point("Y", -2, 8);  intrinsic.Add(y);
            Point z = new Point("Z", 1, 9);   intrinsic.Add(z);
            Point q = new Point("Q", -3, 12); intrinsic.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(x);
            pts2.Add(y);
            pts2.Add(z);
            Collinear coll2 = new Collinear(pts2);

            pts2 = new List<Point>();
            pts2.Add(a);
            pts2.Add(b);
            pts2.Add(c);
            Collinear coll3 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(x, z))));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, y, q)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, c))));
        }
    }
}