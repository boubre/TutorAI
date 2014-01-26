using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 284 problem 17
    //
    public class Page284Problem17 : CongruentTrianglesProblem
    {
        public Page284Problem17(bool onoff) : base(onoff)
        {
            problemName = "Page 284 Problem 17";


            Point q = new Point("Q", 0, 6); intrinsic.Add(q);
            Point r = new Point("R", 0, 0); intrinsic.Add(r);
            Point s = new Point("S", 4, 3); intrinsic.Add(s);
            Point t = new Point("T", 8, 6); intrinsic.Add(t);
            Point u = new Point("U", 8, 0); intrinsic.Add(u);

            Segment qr = new Segment(q, r); intrinsic.Add(qr);
            Segment tu = new Segment(t, u); intrinsic.Add(tu);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(s);
            pts.Add(t);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(q);
            pts2.Add(s);
            pts2.Add(u);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
           
            // There are more congruences implied.
            given.Add(new GeometricCongruentSegments(new Segment(q, s), new Segment(s, u), "Given"));
            given.Add(new GeometricCongruentSegments(new Segment(r, s), new Segment(s, t), "Given"));
            given.Add(new GeometricCongruentSegments(new Segment(r, s), new Segment(s, u), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(q, r, s), new Triangle(t, u, s), "GOAL"));
        }
    }
}