using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 301 problem 51
    //
    public class Page301Problem51 : CongruentTrianglesProblem
    {
        public Page301Problem51(bool onoff) : base(onoff)
        {
        
            problemName = "Page 301 Problem 51";


            Point a = new Point("A", 0, 5); intrinsic.Add(a);
            Point b = new Point("B", 3, 10); intrinsic.Add(b);
            Point c = new Point("C", 10, 5); intrinsic.Add(c);
            Point d = new Point("D", 7, 0); intrinsic.Add(d);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, c, d))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, c, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(c, d, a)));
        }
    }
}