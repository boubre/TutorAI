using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 42 Problem 16
    //
    public class Page42Problem16 : TransversalsProblem
    {
        public Page42Problem16(bool onoff) : base(onoff)
        {
            problemName = "Page 42 Problem 16";
            numberOfOriginalTextProblems = 1;

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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Complementary(GetProblemAngle(intrinsic, new Angle(a, b, x)), GetProblemAngle(intrinsic, new Angle(x, b, y)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, b, x)), GetProblemAngle(intrinsic, new Angle(y, b, z)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(x, b, y)), GetProblemAngle(intrinsic, new Angle(c, b, z)), "Given"));

            goals.Add(new Complementary(GetProblemAngle(intrinsic, new Angle(y, b, z)), GetProblemAngle(intrinsic, new Angle(z, b, c)), "GOAL"));
        }
    }
}