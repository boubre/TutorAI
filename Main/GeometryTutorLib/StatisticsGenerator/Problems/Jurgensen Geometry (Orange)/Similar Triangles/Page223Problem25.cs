using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 223 Problem 25
	//
	public class Page223Problem25 : CongruentTrianglesProblem
	{
        public Page223Problem25(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 25";


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
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(b);
			pts2.Add(m);
			pts2.Add(c);
			Collinear coll2 = new Collinear(pts2);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

			given.Add(new GeometricParallel(bn, cl));
		}
	}
}