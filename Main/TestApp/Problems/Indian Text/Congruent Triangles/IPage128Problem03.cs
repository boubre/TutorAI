using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Book I: Page 128 Problem 3
    //
    public class IPage128Problem03 : CongruentTrianglesProblem
    {
        public IPage128Problem03(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 128 Problem 3";
            numberOfOriginalTextProblems = 2;

            Point a = new Point("A", 2, 3); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, -1); intrinsic.Add(c);
            Point m = new Point("M", 2, -0.5); intrinsic.Add(m);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment am = new Segment(a, m); intrinsic.Add(am);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            Point p = new Point("P", 12, 5); intrinsic.Add(p);
            Point q = new Point("Q", 10, 2); intrinsic.Add(q);
            Point n = new Point("N", 12, 1.5); intrinsic.Add(n);
            Point r = new Point("R", 14, 1); intrinsic.Add(r);

            Segment pq = new Segment(p, q); intrinsic.Add(pq);
            Segment pn = new Segment(p, n); intrinsic.Add(pn);
            Segment pr = new Segment(p, r); intrinsic.Add(pr);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(n);
            pts.Add(r);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Median(am, GetProblemTriangle(intrinsic, new Triangle(a, b, c)), "Given"));
            given.Add(new Median(pn, GetProblemTriangle(intrinsic, new Triangle(p, q, r)), "Given"));
            given.Add(new GeometricCongruentSegments(ab, pq, "Given"));
            given.Add(new GeometricCongruentSegments(am, pn, "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(b, m)), GetProblemSegment(intrinsic, new Segment(q, n)), "Given"));

            goals.Add(new GeometricCongruentTriangles(GetProblemTriangle(intrinsic, new Triangle(a, b, m)), GetProblemTriangle(intrinsic, new Triangle(p, q, n)), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(GetProblemTriangle(intrinsic, new Triangle(a, b, c)), GetProblemTriangle(intrinsic, new Triangle(p, q, r)), "GOAL"));

        }
    }
}