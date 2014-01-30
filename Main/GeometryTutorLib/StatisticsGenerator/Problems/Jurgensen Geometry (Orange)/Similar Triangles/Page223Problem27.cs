using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 223 Problem 27
	//
	public class Page223Problem27 : CongruentTrianglesProblem
	{
        public Page223Problem27(bool onoff)
            : base(onoff)
		{
            problemName = "Page 223 Problem 27";


			Point r = new Point("R", 0, 0); intrinsic.Add(r);
            Point v = new Point("V", 6, 0); intrinsic.Add(v);
            Point s = new Point("S", 10, 0); intrinsic.Add(s);

            Point p = new Point("P", 4, 6); intrinsic.Add(p);

            Point q = new Point("Q", 2, 3); intrinsic.Add(q);
			Point u = new Point("U", 5, 3); intrinsic.Add(u);
            Point t = new Point("T", 7, 3); intrinsic.Add(t);
			
			List<Point> pts = new List<Point>();
			pts.Add(p);
			pts.Add(q);
			pts.Add(r);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(p);
			pts2.Add(u);
			pts2.Add(v);
			Collinear coll2 = new Collinear(pts2);

			List<Point> pts3 = new List<Point>();
			pts3.Add(p);
			pts3.Add(t);
			pts3.Add(s);
			Collinear coll3 = new Collinear(pts3);

			List<Point> pts4 = new List<Point>();
			pts4.Add(r);
			pts4.Add(v);
			pts4.Add(s);
			Collinear coll4 = new Collinear(pts4);

            List<Point> pts5 = new List<Point>();
            pts5.Add(q);
            pts5.Add(u);
            pts5.Add(t);
            Collinear coll5 = new Collinear(pts5);

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateSegmentClauses(coll5));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(q, t)), GetProblemSegment(intrinsic, new Segment(r, s))));
		}
	}
}