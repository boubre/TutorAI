using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 72 Problem 17
    //
    public class Page72Problem17 : CongruentTrianglesProblem
    {
        public Page72Problem17(bool onoff) : base(onoff)
        {
            problemName = "Page 72 Problem 17";


            Point a = new Point("A", 2, 5); intrinsic.Add(a);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point b = new Point("B", 12, 5); intrinsic.Add(b);
            Point d = new Point("D", 10, 0); intrinsic.Add(d);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment db = new Segment(d, b); intrinsic.Add(db);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ab, cd));
            given.Add(new GeometricCongruentSegments(ab, cd));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, c, b)));
        }
    }
}