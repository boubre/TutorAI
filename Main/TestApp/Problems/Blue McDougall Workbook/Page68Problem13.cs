using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 68 Problem 13
    //
    public class Page68Problem13 : CongruentTrianglesProblem
    {
        public Page68Problem13(bool onoff) : base(onoff)
        {
            problemName = "Page 68 Problem 13";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 1, 5); intrinsic.Add(b);
            Point c = new Point("C", 8, 5); intrinsic.Add(c);
            Point d = new Point("D", 7, 0); intrinsic.Add(d);

            Segment ba = new Segment(b, a); intrinsic.Add(ba);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(ba, cd, "Given"));
            given.Add(new GeometricCongruentSegments(bc, ad, "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(c, d, a), "GOAL"));
        }
    }
}