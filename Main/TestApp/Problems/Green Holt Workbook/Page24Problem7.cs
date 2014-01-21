using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 24 Problem 7
    //
    public class Page24Problem7 : TransversalsProblem
    {
        public Page24Problem7(bool onoff) : base(onoff)
        {
            problemName = "Page 24 Problem 7";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 1, 4); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 3.5, 3); intrinsic.Add(c);
            Point d = new Point("D", 6, 2); intrinsic.Add(d);
            Point e = new Point("E", 7, 6); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            System.Diagnostics.Debug.WriteLine("Intersection: " + new Segment(b, e).FindIntersection(new Segment(a, d)));

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(c, GetProblemSegment(intrinsic, new Segment(a, d)), "Given"));
            given.Add(new Midpoint(c, GetProblemSegment(intrinsic, new Segment(b, e)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c), "GOAL"));
        }
    }
}