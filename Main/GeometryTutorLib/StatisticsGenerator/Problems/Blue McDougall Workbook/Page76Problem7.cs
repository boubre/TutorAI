using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 76 Problem 7
    //
    public class Page76Problem7 : CongruentTrianglesProblem
    {
        public Page76Problem7(bool onoff) : base(onoff)
        {
            problemName = "Page 76 Problem 7";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 7, 3); intrinsic.Add(b);
            Point c = new Point("C", 14, 3); intrinsic.Add(c);
            Point d = new Point("D", 7, 0); intrinsic.Add(d);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(ab, cd, "Given"));
            given.Add(new RightAngle(new Angle(a, d, b), "Given"));
            given.Add(new RightAngle(new Angle(d, b, c), "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, a, b)), GetProblemAngle(intrinsic, new Angle(b, c, d)), "GOAL"));
        }
    }
}