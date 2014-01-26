using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 146 Problem 17
	//
	public class Page146Problem17 : CongruentTrianglesProblem
	{
        public Page146Problem17(bool onoff) : base(onoff)
		{
            problemName = "Page 146 Problem 17";


            Point d = new Point("D", 2, 3); intrinsic.Add(d);
			Point b = new Point("B", 10, 0); intrinsic.Add(b);
			Point a = new Point("A", 0, 0); intrinsic.Add(a);
			Point c = new Point("C", 8, 3); intrinsic.Add(c);
			Point m = new Point("M", 5, 0); intrinsic.Add(m);

			Segment bc = new Segment(b, c); intrinsic.Add(bc);
			Segment ad = new Segment(a, d); intrinsic.Add(ad);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);
			Segment bd = new Segment(b, d); intrinsic.Add(bd);
			Segment ac = new Segment(a, c); intrinsic.Add(ac);
			Segment dm = new Segment(d, m); intrinsic.Add(dm);
			Segment cm = new Segment(c, m); intrinsic.Add(cm);
			

			List<Point> pts = new List<Point>();
			pts.Add(a);
			pts.Add(m);
			pts.Add(b);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(a, m)), GetProblemSegment(intrinsic, new Segment(b, m)), "Given"));
			given.Add(new GeometricCongruentSegments(ad, bc, "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(m, d, c)), GetProblemAngle(intrinsic, new Angle(m, c, d)), "Given"));
		}
	}
}