using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 66 Problem 16
    //
    public class Page66Problem16 : CongruentTrianglesProblem
    {
        public Page66Problem16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 66 Problem 16";


            Point a = new Point("A", 1, 5); intrinsic.Add(a);
            Point b = new Point("B", 8, 5); intrinsic.Add(b);
            Point c = new Point("C", 7, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);

            Segment ba = new Segment(b, a); intrinsic.Add(ba);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ba, cd));
            given.Add(new GeometricCongruentSegments(bc, ad));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, d, b))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, d, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, b, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(c, d, b)));
        }
    }
}