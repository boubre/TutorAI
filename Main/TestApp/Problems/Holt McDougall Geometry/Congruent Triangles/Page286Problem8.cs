using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 286 problem 8
    //
    public class Page286Problem8 : CongruentTrianglesProblem
    {
        public Page286Problem8(bool onoff) : base(onoff)
        {
            problemName = "Page 286 Problem 8";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 0, 8); intrinsic.Add(a);
            Point b = new Point("B", 8, 8); intrinsic.Add(b);
            Point c = new Point("C", 4, 4); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point e = new Point("E", 8, 0); intrinsic.Add(e);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(c);
            pts2.Add(b);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(c, e)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(c, b)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, c)), GetProblemSegment(intrinsic, new Segment(c, d)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(e, d, c), "GOAL"));
        }
    }
}