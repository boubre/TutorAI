using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Book i Page 120 Problem 8
    //
    public class IPage120Problem8 : ActualProblem
    {
        public IPage120Problem8(bool onoff) : base(onoff)
        {
            problemName = "Overlapping Right Triangles";
            numberOfOriginalTextProblems = 4;

            Point a = new Point("A", 0, 3); intrinsic.Add(a);
            Point m = new Point("M", 2, 1.5); intrinsic.Add(m);
            Point b = new Point("B", 4, 3); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            //given.Add(new Midpoint(m, GetProblemSegment(intrinsic, new Segment(a, c)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, m)), GetProblemSegment(intrinsic, new Segment(m, c)), "Given"));
            given.Add(new Midpoint(m, GetProblemSegment(intrinsic, new Segment(b, d)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(b, c, d)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(b, m, c), new Triangle(d, m, a), "GOAL"));
            goals.Add(new Strengthened(GetProblemAngle(intrinsic, new Angle(a, d, c)), new RightAngle(GetProblemAngle(intrinsic, new Angle(a, d, c)), "GOAL"), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, d, c), new Triangle(b, c, d), "GOAL"));

            Multiplication product = new Multiplication(new NumericValue(2), GetProblemSegment(intrinsic, new Segment(c, m)));
            goals.Add(new GeometricSegmentEquation(product, GetProblemSegment(intrinsic, new Segment(b, d)), "GOAL"));
        }
    }
}