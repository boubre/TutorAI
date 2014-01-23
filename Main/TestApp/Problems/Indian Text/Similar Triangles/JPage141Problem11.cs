using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public class JPage141Problem11 : SimilarTrianglesProblem
    {
        public JPage141Problem11(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 141 Problem 11";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 5);    intrinsic.Add(a);
            Point b = new Point("B", 0, 0);    intrinsic.Add(b);
            Point c = new Point("C", 4, 0);    intrinsic.Add(c);
            Point d = new Point("D", 2, 0);    intrinsic.Add(d);
            Point e = new Point("E", -3.25, 0);  intrinsic.Add(e);
            Point f = new Point("F", 3, 2.5);  intrinsic.Add(f);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ef = new Segment(e, f); intrinsic.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(ab, GetProblemSegment(intrinsic, new Segment(a, c)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(a, d, c)), "Given"));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(e, f, c)), "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, d), new Triangle(e, c, f), "Given"));
        }
    }
}