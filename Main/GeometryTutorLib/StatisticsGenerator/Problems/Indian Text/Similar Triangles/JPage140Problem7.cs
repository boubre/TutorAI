using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class JPage140Problem7 : SimilarTrianglesProblem
    {
        public JPage140Problem7(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 140 Problem 7";


            Point a = new Point("A", 0, 0);    intrinsic.Add(a);
            Point b = new Point("B", 18, 0);   intrinsic.Add(b);
            Point c = new Point("C", 2, 8);   intrinsic.Add(c);
            Point d = new Point("D", 3.6, 7.2);  intrinsic.Add(d);
            Point e = new Point("E", 2, 0);    intrinsic.Add(e);
            Point p = new Point("P", 2, 4); intrinsic.Add(p);

            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(p);
            pts.Add(a);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(p);
            pts.Add(e);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Altitude(GetProblemTriangle(intrinsic, new Triangle(a, b, c)), GetProblemSegment(intrinsic, new Segment(a, d)), "Given"));
            given.Add(new Altitude(GetProblemTriangle(intrinsic, new Triangle(a, b, c)), GetProblemSegment(intrinsic, new Segment(c, e)), "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(c, d, p), "Given"));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, d), new Triangle(c, b, e), "Given"));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(a, d, b), "Given"));
            goals.Add(new GeometricSimilarTriangles(new Triangle(p, d, c), new Triangle(b, e, c), "Given"));
        }
    }
}