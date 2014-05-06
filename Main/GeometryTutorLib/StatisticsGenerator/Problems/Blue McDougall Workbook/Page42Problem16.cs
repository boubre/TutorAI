using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 42 Problem 16
    //
    public class Page42Problem16 : TransversalsProblem
    {
        public Page42Problem16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 42 Problem 16";


            Point a = new Point("A", -4, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);

            Point x = new Point("X", -3, -2); intrinsic.Add(x);
            Point y = new Point("Y", 0, -5); intrinsic.Add(y);
            Point z = new Point("Z", 2, -3); intrinsic.Add(z);

            Segment bx = new Segment(b, x); intrinsic.Add(bx);
            Segment by = new Segment(b, y); intrinsic.Add(by);
            Segment bz = new Segment(b, z); intrinsic.Add(bz);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Complementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, x)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, b, y))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, x)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(y, b, z))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, b, y)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, b, z))));

            goals.Add(new Complementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(y, b, z)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(z, b, c))));
        }
    }
}