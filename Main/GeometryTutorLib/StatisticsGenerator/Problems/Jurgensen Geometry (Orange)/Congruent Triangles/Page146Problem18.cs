using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 146 Problem 18
	//
	public class Page146Problem18 : CongruentTrianglesProblem
	{
		public Page146Problem18(bool onoff) : base(onoff)
		{
            problemName = "Page 146 Problem 18";


			Point a = new Point("A", 5, 6); intrinsic.Add(a);
			Point b = new Point("B", 0, 2); intrinsic.Add(b);
			Point c = new Point("C", 2, 0); intrinsic.Add(c);
			Point d = new Point("D", 8, 0); intrinsic.Add(d);
			Point e = new Point("E", 10, 2); intrinsic.Add(e);
			Point g = new Point("G", 3, 2); intrinsic.Add(g);
			Point f = new Point("F", 7, 2); intrinsic.Add(f);

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment ae = new Segment(a, e); intrinsic.Add(ae);
			Segment bc = new Segment(b, c); intrinsic.Add(bc);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);
			Segment de = new Segment(d, e); intrinsic.Add(de);

			List<Point> pts = new List<Point>();
			pts.Add(a);
			pts.Add(g);
			pts.Add(c);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(a);
			pts2.Add(f);
			pts2.Add(d);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			List<Point> pts3 = new List<Point>();
			pts3.Add(b);
			pts3.Add(g);
			pts3.Add(f);
			pts3.Add(e);
			Collinear coll3 = new Collinear(pts3, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, g, f)), GetProblemAngle(intrinsic, new Angle(a, f, g)), "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, c, g)), GetProblemAngle(intrinsic, new Angle(f, d, c)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(g, a, b)), GetProblemAngle(intrinsic, new Angle(f, a, e)), "Given"));

            goals.Add(new GeometricCongruentSegments(bc, de, "GOAL"));
		}
	}
}