using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 41 Problem 15
    //
    public class Page41Problem15 : TransversalsProblem
    {
        public Page41Problem15(bool onoff) : base(onoff)
        {
            problemName = "Page 41 Problem 15";
            numberOfOriginalTextProblems = 1;

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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, x, y)), GetProblemAngle(intrinsic, new Angle(w, x, c)), "GOAL"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, x, w)), GetProblemAngle(intrinsic, new Angle(d, y, z)), "GOAL"));
        }
    }
}