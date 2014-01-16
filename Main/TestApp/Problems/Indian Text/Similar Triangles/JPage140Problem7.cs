using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public class JPage140Problem7 : SimilarTrianglesProblem
    {
        public JPage140Problem7(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 140 Problem 7";
            numberOfOriginalTextProblems = 4;

            Point a = new Point("A", 0, 0);    intrinsic.Add(a);
            Point b = new Point("B", 20, 0);   intrinsic.Add(b);
            Point c = new Point("C", 6, 10);   intrinsic.Add(c);
            Point d = new Point("D", 1.5, 9);  intrinsic.Add(d);
            Point p = new Point("E", 6, 0);    intrinsic.Add(p);
            Point e = new Point("X", 2, 18 / 2.5); intrinsic.Add(e);

            //System.Diagnostics.Debug.WriteLine("Intersection: " new Segment(e, c));

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
        }
    }
}