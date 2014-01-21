using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 160 Problem 42
    //
    public class Page160Problem42 : CongruentTrianglesProblem
    {
        public Page160Problem42(bool onoff) : base(onoff)
        {
            problemName = "Page 160 Problem 42";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 4); intrinsic.Add(a);
            Point b = new Point("B", 4, 8); intrinsic.Add(b);
            Point c = new Point("C", 10, 0); intrinsic.Add(c);
            Point d = new Point("D", 20, 0); intrinsic.Add(d);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
            given.Add(new RightAngle(c, a, b, "Given"));
            given.Add(new RightAngle(a, b, d, "Given"));
            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, ac), GetProblemSegment(intrinsic, bd), "Given"));
        }
    }
}