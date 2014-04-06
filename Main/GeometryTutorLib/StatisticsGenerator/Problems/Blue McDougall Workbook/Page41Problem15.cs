using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 41 Problem 15
    //
    public class Page41Problem15 : TransversalsProblem
    {
        public Page41Problem15(bool onoff) : base(onoff)
        {
            problemName = "Page 41 Problem 15";


            Point a = new Point("A", 1, -1); intrinsic.Add(a);
            Point b = new Point("B", 3, 1); intrinsic.Add(b);
            Point c = new Point("C", 5, 1); intrinsic.Add(c);
            Point d = new Point("D", 7, -1); intrinsic.Add(d);

            Point w = new Point("W", 0, 0); intrinsic.Add(w);
            Point x = new Point("X", 2, 0); intrinsic.Add(x);
            Point y = new Point("Y", 6, 0); intrinsic.Add(y);
            Point z = new Point("Z", 11, 0); intrinsic.Add(z);

            List<Point> pts = new List<Point>();
            pts.Add(w);
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, x, y)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, y, x))));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, x, w)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, y, z))));
        }
    }
}