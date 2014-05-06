using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 284 example 4.5
    //
    public class Page284Exameple45 : CongruentTrianglesProblem
    {
        public Page284Exameple45(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 284 example 4.5";


            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);
            Point b = new Point("B", 9, 4); intrinsic.Add(b);
            Point a = new Point("A", 3, 4); intrinsic.Add(a);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ab, cd));
            given.Add(new GeometricParallel(ad, bc));

            goals.Add(new GeometricCongruentTriangles(new Triangle(d, a, c), new Triangle(b, c, a)));
        }
    }
}