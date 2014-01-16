using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 113 Problem 7
    //
    public class RightAngleStrengthening : CongruentTrianglesProblem
    {
        public RightAngleStrengthening(bool onoff)
            : base(onoff)
        {
            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 4); intrinsic.Add(b);
            Point c = new Point("C", 3, 0); intrinsic.Add(c);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new RightAngle(b, a, c, "Given"));
        }
    }
}