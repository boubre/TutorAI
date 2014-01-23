using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 66 Problem 16
    //
    public class Page66Problem16 : CongruentTrianglesProblem
    {
        public Page66Problem16(bool onoff) : base(onoff)
        {
            problemName = "Page 66 Problem 16";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 1, 5); intrinsic.Add(a);
            Point b = new Point("B", 8, 5); intrinsic.Add(b);
            Point c = new Point("C", 7, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);

            Segment ba = new Segment(b, a); intrinsic.Add(ba);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(ba, cd, "Given"));
            given.Add(new GeometricCongruentSegments(bc, ad, "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, b, d)), GetProblemAngle(intrinsic, new Angle(c, d, b)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, d, b)), GetProblemAngle(intrinsic, new Angle(c, b, d)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(c, d, b), "GOAL"));
        }
    }
}