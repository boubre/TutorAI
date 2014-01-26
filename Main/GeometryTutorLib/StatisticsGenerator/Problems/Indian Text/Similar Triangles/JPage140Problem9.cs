using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book J: Page 140 Problem 9
    //
    public class JPage140Problem9 : SimilarTrianglesProblem
    {
        public JPage140Problem9(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 140 Problem 9";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 6, 0); intrinsic.Add(b);
            Point c = new Point("C", 6, 6); intrinsic.Add(c);
            Point m = new Point("M", 5, 5); intrinsic.Add(m);
            Point p = new Point("P", 10, 0); intrinsic.Add(p);

            Point x = new Point("X", 6, 4); intrinsic.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(x);
            pts.Add(p);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(p);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(a, b, c)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(a, m, p)), "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, c), new Triangle(a, m, p), "Given"));
//            given.Add(new GeometricProportionalSegments(ab, ac, "Given"));
        }
    }
}