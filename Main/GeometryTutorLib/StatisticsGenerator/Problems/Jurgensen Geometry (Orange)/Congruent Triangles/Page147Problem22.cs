using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 147 Problem 22
	//
	public class Page147Problem22 : CongruentTrianglesProblem
	{
        public Page147Problem22(bool onoff)
            : base(onoff)
		{
			Point a = new Point("A", 3, 9); intrinsic.Add(a);
			Point b = new Point("B", 0, 0); intrinsic.Add(b);
			Point c = new Point("C", 12, 6); intrinsic.Add(c);
			Point d = new Point("D", 12, 0); intrinsic.Add(d);
			Point e = new Point("E", 3, 0); intrinsic.Add(e);
			Point f = new Point("F", 12, 9); intrinsic.Add(f);
			Point m = new Point("M", 6, 3); intrinsic.Add(m);

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment ac = new Segment(a, c); intrinsic.Add(ac);
			Segment ad = new Segment(a, d); intrinsic.Add(ad);
			Segment ae = new Segment(a, e); intrinsic.Add(ae);
			Segment af = new Segment(a, f); intrinsic.Add(af);
			Segment am = new Segment(a, m); intrinsic.Add(am);
			Segment bc = new Segment(b, c); intrinsic.Add(bc);

			List<Point> pts = new List<Point>();
			pts.Add(b);
			pts.Add(e);
			pts.Add(d);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(b);
			pts2.Add(m);
			pts2.Add(c);
			Collinear coll2 = new Collinear(pts2);

			List<Point> pts3 = new List<Point>();
			pts3.Add(d);
			pts3.Add(c);
			pts3.Add(f);
			Collinear coll3 = new Collinear(pts3);

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
			
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(b, m)), GetProblemSegment(intrinsic, new Segment(m, c))));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, d, a)), GetProblemAngle(intrinsic, new Angle(a, d, c))));		
			given.Add(new RightTriangle(GetProblemSegment(intrinsic, ab), GetProblemSegment(intrinsic, ae), GetProblemSegment(intrinsic, new Segment(b, e))));
			given.Add(new RightTriangle(GetProblemSegment(intrinsic, ac), GetProblemSegment(intrinsic, am), GetProblemSegment(intrinsic, new Segment(m, c))));
			given.Add(new RightTriangle(GetProblemSegment(intrinsic, ac), GetProblemSegment(intrinsic, af), GetProblemSegment(intrinsic, new Segment(c, f))));
            
		}
	}
}