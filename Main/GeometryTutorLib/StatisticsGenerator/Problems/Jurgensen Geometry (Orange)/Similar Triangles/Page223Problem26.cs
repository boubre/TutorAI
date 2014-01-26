using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 223 Problem 26
	//
	public class Page223Problem26 : CongruentTrianglesProblem
	{
        public Page223Problem26(bool onoff)
            : base(onoff)
		{
            problemName = "Page 223 Problem 26";


			Point d = new Point("D", 0, 0); intrinsic.Add(d);
			Point g = new Point("G", 4, 0); intrinsic.Add(g);
			Point a = new Point("A", 0, 8); intrinsic.Add(a);
			Point h = new Point("H", 2, 4); intrinsic.Add(h);
			Point e = new Point("E", 0, 3); intrinsic.Add(e);

			Segment eh = new Segment(e, h); intrinsic.Add(eh);
			Segment dg = new Segment(d, g); intrinsic.Add(dg);

			List<Point> pts = new List<Point>();
			pts.Add(d);
			pts.Add(e);
			pts.Add(a);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(a);
			pts2.Add(h);
			pts2.Add(g);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new RightAngle(a, d, g, "Given"));
			given.Add(new RightAngle(e, h, a, "Given"));
		}
	}
}