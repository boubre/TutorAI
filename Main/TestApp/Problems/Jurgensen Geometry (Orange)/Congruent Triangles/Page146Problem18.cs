using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 146 Problem 18
	//
	public class Page146Problem18 : CongruentTrianglesProblem
	{
		public Page146Problem18(bool onoff)
            : base(onoff)
		{
			Point a = new Point("A", 5, 6); intrinsic.Add(a);
			Point b = new Point("B", 0, 2); intrinsic.Add(b);
			Point c = new Point("C", 2, 0); intrinsic.Add(c);
			Point d = new Point("D", 8, 0); intrinsic.Add(d);
			Point e = new Point("E", 10, 2); intrinsic.Add(e);
			Point g = new Point("G", 3, 2); intrinsic.Add(g);
			Point f = new Point("F", 7, 2); intrinsic.Add(f);

			Segment ab = new Segment(b, c); intrinsic.Add(ab);
			Segment ae = new Segment(a, d); intrinsic.Add(ae);
			Segment bc = new Segment(c, d); intrinsic.Add(bc);
			Segment cd = new Segment(b, d); intrinsic.Add(cd);
			Segment de = new Segment(a, c); intrinsic.Add(de);

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

			Angle angle1 = new Angle(g, a, b); intrinsic.Add(angle1);
			Angle angle2 = new Angle(f, a, e); intrinsic.Add(angle2);

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, g, f)), GetProblemAngle(intrinsic, new Angle(a, f, g)), "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, c, g)), GetProblemAngle(intrinsic, new Angle(f, d, c)), "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, angle1), GetProblemAngle(intrinsic, angle2), "Given"));
		}
	}
}