using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 75 Problem 18
    //
    public class Page75Problem18 : CongruentTrianglesProblem
    {
        public Page75Problem18(bool onoff) : base(onoff)
        {
            problemName = "Page 75 Problem 18";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 2, 4); intrinsic.Add(b);
            Point c = new Point("C", 3, 3); intrinsic.Add(c);
            Point d = new Point("D", 4, 4); intrinsic.Add(d);
            Point e = new Point("E", 6, 0); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment ae = new Segment(a, e); intrinsic.Add(ae);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(e);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, b, e)), GetProblemAngle(intrinsic, new Angle(a, d, e))));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(c, e))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(e, d, c)));
        }
    }
}