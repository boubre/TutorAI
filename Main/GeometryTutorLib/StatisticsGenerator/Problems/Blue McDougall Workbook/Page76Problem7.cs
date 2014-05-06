using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 76 Problem 7
    //
    public class Page76Problem7 : CongruentTrianglesProblem
    {
        public Page76Problem7(bool onoff, bool complete) : base(onoff, complete)
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

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ab, cd));
            given.Add(new RightAngle(new Angle(a, d, b)));
            given.Add(new RightAngle(new Angle(d, b, c)));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, a, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, c, d))));
        }
    }
}