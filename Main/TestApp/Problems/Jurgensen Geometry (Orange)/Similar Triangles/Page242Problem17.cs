using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 242 Problem 17
    //
    public class Page242Problem17 : SimilarTrianglesProblem
    {
        public Page242Problem17(bool onoff) : base(onoff)
        {
            problemName = "Page 242 Problem 17";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 1, 5); intrinsic.Add(a);
            Point b = new Point("B", 5, 5); intrinsic.Add(b);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point d = new Point("D", 6, 0); intrinsic.Add(d);
            Point n = new Point("N", 3, 3); intrinsic.Add(n);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(n);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(a);
            pts2.Add(n);
            pts2.Add(d);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(c, b, a)), GetProblemAngle(intrinsic, new Angle(a, d, c)), "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(n, c, d), new Triangle(n, a, b), "GOAL"));
        }
    }
}