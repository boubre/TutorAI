using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 146 Problem 12
	//
	public class Page146Problem12 : CongruentTrianglesProblem
	{
        public Page146Problem12(bool onoff)
            : base(onoff)
		{
            problemName = "Page 146 Problem 12";
            numberOfOriginalTextProblems = 1;

			Point l = new Point("L", 0, 5); intrinsic.Add(l);
			Point m = new Point("M", 2, 0); intrinsic.Add(m);
			Point n = new Point("N", 6, 0); intrinsic.Add(n);
			Point x = new Point("X", 0, 0); intrinsic.Add(x);

			Point r = new Point("R", 10, 5); intrinsic.Add(r);
			Point s = new Point("S", 12, 0); intrinsic.Add(s);
			Point t = new Point("T", 16, 0); intrinsic.Add(t);
			Point y = new Point("Y", 10, 0); intrinsic.Add(y);
			
			Segment lm = new Segment(l, m); intrinsic.Add(lm);
			Segment ln = new Segment(l, n); intrinsic.Add(ln);
			Segment lx = new Segment(l, x); intrinsic.Add(lx);
			Segment rs = new Segment(r, s); intrinsic.Add(rs);
			Segment rt = new Segment(r, t); intrinsic.Add(rt);
			Segment ry = new Segment(r, y); intrinsic.Add(ry);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(m);
			pts.Add(n);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(y);
			pts2.Add(s);
			pts2.Add(t);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            Triangle tOne = new Triangle(l, m, n);
            Triangle tTwo = new Triangle(r, s, t);

            given.Add(new GeometricCongruentTriangles(GetProblemTriangle(intrinsic, tOne), GetProblemTriangle(intrinsic, tTwo), "Given"));
            given.Add(new Altitude(GetProblemTriangle(intrinsic, tOne), lx, "Given"));
            given.Add(new Altitude(GetProblemTriangle(intrinsic, tTwo), ry, "Given"));
		}
	}
}