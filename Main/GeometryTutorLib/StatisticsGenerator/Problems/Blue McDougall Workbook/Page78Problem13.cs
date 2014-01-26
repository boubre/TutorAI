using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 79 Problem 7
    //
    public class Page79Problem7 : CongruentTrianglesProblem
    {
        public Page79Problem7(bool onoff) : base(onoff)
        {
            problemName = "Page 79 Problem 7";


            Point a = new Point("A", 0, 8); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 5, 0); intrinsic.Add(c);
            Point d = new Point("D", 5, 8); intrinsic.Add(d);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, a, c)), GetProblemAngle(intrinsic, new Angle(c, d, b)), "Given"));
            given.Add(new RightAngle(new Angle(a, b, c), "Given"));
            given.Add(new RightAngle(new Angle(b, c, d), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, c, b), "GOAL"));
        }
    }
}