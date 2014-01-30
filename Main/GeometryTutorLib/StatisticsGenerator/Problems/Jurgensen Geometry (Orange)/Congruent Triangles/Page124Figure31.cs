using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    // Isosceles Triangle Figure
    //
    //      A
    //     /|\
    //    / | \
    //   /  |  \
    //  /___|___\
    //  B   M    C
    //
    public class Page124Figure31 : CongruentTrianglesProblem
    {
        public Page124Figure31(bool onoff) : base(onoff)
        {
            Point a = new Point("A", 2, 6); intrinsic.Add(a);
            Point m = new Point("M", 2, 0); intrinsic.Add(m);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment am = new Segment(a, m); intrinsic.Add(am);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new IsoscelesTriangle(ac, ab, GetProblemSegment(intrinsic, new Segment(b, c))));
            given.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(b, a, c)), am));
        }
    }
}