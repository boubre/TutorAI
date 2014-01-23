using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 78 Problem 13
    //
    public class Page78Problem13 : CongruentTrianglesProblem
    {
        public Page78Problem13(bool onoff) : base(onoff)
        {
            problemName = "Page 78 Problem 13";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 2); intrinsic.Add(a);
            Point b = new Point("B", 5, 0); intrinsic.Add(b);
            Point c = new Point("C", 10, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point e = new Point("E", 8, -2); intrinsic.Add(e);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ce = new Segment(c, e); intrinsic.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(b);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, d, b)), GetProblemAngle(intrinsic, new Angle(b, c, e)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, b)), GetProblemSegment(intrinsic, new Segment(b, e)), "Given"));

            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(d, b)), GetProblemSegment(intrinsic, new Segment(c, b)), "GOAL"));
        }
    }
}