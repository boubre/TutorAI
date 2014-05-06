using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 35
    //
    public class Page156Problem35 : ParallelLinesProblem
    {
        public Page156Problem35(bool onoff, bool complete) : base(onoff, complete)
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

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, d, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, c))));
            given.Add(new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(a, d), new Segment(c, d))));

            goals.Add(new Strengthened(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(b, c), new Segment(c, d)),
                      new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(b, c), new Segment(c, d)))));
        }
    }
}