using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Book J: Page 153 Problem 9
    //
    public class JPage153Problem09 : SimilarTrianglesProblem
    {
        public JPage153Problem09(bool onoff)
            : base(onoff)
        {
            problemName = "Book J Page 153 Problem 9";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 3); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 5, 0); intrinsic.Add(c);
            Point d = new Point("D", 2.5, 0); intrinsic.Add(d);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricProportionalSegments(GetProblemSegment(intrinsic, new Segment(b, d)), GetProblemSegment(intrinsic, new Segment(c, d)), "Given"));
            given.Add(new GeometricProportionalSegments(ab, ac, "Given"));
        }
    }
}