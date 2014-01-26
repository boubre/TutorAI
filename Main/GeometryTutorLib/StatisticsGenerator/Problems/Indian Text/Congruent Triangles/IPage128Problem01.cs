using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book I: Page 128 Problem 1
    //
    public class IPage128Problem01 : CongruentTrianglesProblem
    {
        public IPage128Problem01(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 128 Problem 1";

            Point a = new Point("A", 2, 7); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point d = new Point("D", 2, 3); intrinsic.Add(d);
            Point p = new Point("P", 2, 0); intrinsic.Add(p);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(p);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new IsoscelesTriangle(GetProblemTriangle(intrinsic, new Triangle(a, b, c)), "Given"));
            given.Add(new IsoscelesTriangle(GetProblemTriangle(intrinsic, new Triangle(d, b, c)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(a, c, d), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, p), new Triangle(a, c, p), "GOAL"));
            goals.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(b, a, c)), GetProblemSegment(intrinsic, new Segment(a, p)), "GOAL"));
            goals.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(b, d, c)), GetProblemSegment(intrinsic, new Segment(a, p)), "GOAL"));
            goals.Add(new PerpendicularBisector(GetProblemIntersection(intrinsic, new Segment(a, p), new Segment(b, c)),
                                                GetProblemSegment(intrinsic, new Segment(a, p)), "GOAL"));
        }
    }
}