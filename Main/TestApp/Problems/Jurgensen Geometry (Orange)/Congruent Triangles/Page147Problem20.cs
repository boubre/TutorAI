using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 147 Problem 20
	//
	public class Page147Problem20 : CongruentTrianglesProblem
	{
        public Page147Problem20(bool onoff) : base(onoff)
		{
            problemName = "Page 147 Problem 20";
            numberOfOriginalTextProblems = 1;

			Point m = new Point("M", 8, 1); intrinsic.Add(m);
			Point n = new Point("N", 5, 1); intrinsic.Add(n);
			Point p = new Point("P", 0, 2); intrinsic.Add(p);
			Point q = new Point("Q", 12, 2); intrinsic.Add(q);
			Point r = new Point("R", 6, 2); intrinsic.Add(r);
			Point s = new Point("S", 10, 0); intrinsic.Add(s);
			Point t = new Point("T", 4, 0); intrinsic.Add(t);

			Segment st = new Segment(s, t); intrinsic.Add(st);

			List<Point> pts = new List<Point>();
			pts.Add(t);
			pts.Add(n);
			pts.Add(r);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(r);
			pts2.Add(m);
			pts2.Add(s);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			List<Point> pts3 = new List<Point>();
			pts3.Add(p);
			pts3.Add(n);
			pts3.Add(s);
			Collinear coll3 = new Collinear(pts3, "Intrinsic");

			List<Point> pts4 = new List<Point>();
			pts4.Add(q);
			pts4.Add(m);
			pts4.Add(t);
			Collinear coll4 = new Collinear(pts4, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateSegmentClauses(coll4));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(p, n)), GetProblemSegment(intrinsic, new Segment(n, s)), "Given"));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(t, m)), GetProblemSegment(intrinsic, new Segment(q, m)), "Given"));
		}
	}
}