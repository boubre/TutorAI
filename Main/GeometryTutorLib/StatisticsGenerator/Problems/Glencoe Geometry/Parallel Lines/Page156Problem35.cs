using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 35
    //
    public class Page156Problem35 : ParallelLinesProblem
    {
        public Page156Problem35(bool onoff) : base(onoff)
        {
            problemName = "Page 156 Problem 35";

            Point a = new Point("A", 9, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 3); intrinsic.Add(b);
            Point c = new Point("C", 0, 9); intrinsic.Add(c);
            Point d = new Point("D", 9, 9); intrinsic.Add(d);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, d, b)), GetProblemAngle(intrinsic, new Angle(d, b, c)), "Given"));
            given.Add(new Perpendicular(GetProblemIntersection(intrinsic, new Segment(a, d), new Segment(c, d)), "Given"));

            goals.Add(new Strengthened(GetProblemIntersection(intrinsic, new Segment(b, c), new Segment(c, d)),
                      new Perpendicular(GetProblemIntersection(intrinsic, new Segment(b, c), new Segment(c, d)), "GOAL"), "GOAL"));
        }
    }
}