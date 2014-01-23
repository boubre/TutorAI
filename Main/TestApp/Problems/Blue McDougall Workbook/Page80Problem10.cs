using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 80 Problem 10
    //
    public class Page80Problem10 : CongruentTrianglesProblem
    {
        public Page80Problem10(bool onoff) : base(onoff)
        {
            problemName = "Page 80 Problem 10";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 7); intrinsic.Add(a);
            Point b = new Point("B", 8, 7); intrinsic.Add(b);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point d = new Point("D", 10, 0); intrinsic.Add(d);
            Point e = new Point("E", 5, 4.375); intrinsic.Add(e);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(b);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(c, a, d)), GetProblemAngle(intrinsic, new Angle(c, b, d)), "Given"));
            given.Add(new GeometricCongruentSegments(ac, bd, "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, a, b)), GetProblemAngle(intrinsic, new Angle(c, b, a)), "GOAL"));
        }
    }
}