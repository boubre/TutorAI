using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 223 Problem 24
	//
	public class Page223Problem24 : CongruentTrianglesProblem
	{
		public Page223Problem24() : base()
		{
            problemName = "Page 223 Problem 24";
            numberOfOriginalTextProblems = 1;

			Point b = new Point("B", 2, 4); intrinsic.Add(b);
			Point n = new Point("N", 8, 4); intrinsic.Add(n);
			Point l = new Point("L", 0, 0); intrinsic.Add(l);
			Point c = new Point("C", 10, 0); intrinsic.Add(c);
			Point m = new Point("M", 5, 2.5); intrinsic.Add(m);

			Segment cl = new Segment(c, l); intrinsic.Add(cl);
			Segment bn = new Segment(b, n); intrinsic.Add(bn);

			List<Point> pts = new List<Point>();
			pts.Add(l);
			pts.Add(m);
			pts.Add(n);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(b);
			pts2.Add(m);
			pts2.Add(c);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(m, b, n)), GetProblemAngle(intrinsic, new Angle(m, c, l)), "Given"));
		}
	}
}