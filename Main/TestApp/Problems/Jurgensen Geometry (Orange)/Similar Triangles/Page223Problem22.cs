using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 223 Problem 22
	//
	public class Page223Problem22 : CongruentTrianglesProblem
	{
        public Page223Problem22(bool onoff) : base(onoff)
		{
            problemName = "Page 223 Problem 22";
            numberOfOriginalTextProblems = 1;

            Point x = new Point("X", 1, 0); intrinsic.Add(x);
			Point f = new Point("F", 3, 4); intrinsic.Add(f);
			Point s = new Point("S", 4, 6); intrinsic.Add(s);
			Point e = new Point("E", 5, 0); intrinsic.Add(e);
			Point r = new Point("R", 7, 0); intrinsic.Add(r);
			
			Segment rs = new Segment(r, s); intrinsic.Add(rs);
			Segment ef = new Segment(e, f); intrinsic.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(e);
			pts.Add(r);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(x);
			pts2.Add(f);
			pts2.Add(s);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricParallel(rs, ef, "Given"));

            goals.Add(new GeometricSimilarTriangles(GetProblemTriangle(intrinsic, new Triangle(f, x, e)), GetProblemTriangle(intrinsic, new Triangle(s, x, r)), "GOAL"));
        }
	}
}