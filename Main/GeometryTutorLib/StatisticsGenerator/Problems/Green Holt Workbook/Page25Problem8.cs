using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 25 Problem 8
    //
    public class Page25Problem8 : TransversalsProblem
    {
        public Page25Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 25 Problem 8";


            Point p = new Point("P", -5, 5); intrinsic.Add(p);
            Point q = new Point("Q", -4, 4); intrinsic.Add(q);
            Point r = new Point("R", 0, 0); intrinsic.Add(r);
            Point s = new Point("S", 4, 4); intrinsic.Add(s);
            Point t = new Point("T", 6, 6); intrinsic.Add(t);
            Point u = new Point("U", 0, 4); intrinsic.Add(u);

            Segment ur = new Segment(u, r); intrinsic.Add(ur);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(u);
            pts.Add(s);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(s);
            pts.Add(t);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(p, q, u)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(t, s, u))));
            given.Add(new RightAngle(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(q, u, r))));
            given.Add(new RightAngle(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(s, u, r))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(r, u, q), new Triangle(r, u, s)));
        }
    }
}