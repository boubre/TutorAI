using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 175 Problems 7-12
	//
	public class Page175ClassroomExercise07to12 : CongruentTrianglesProblem
	{
		public Page175ClassroomExercise07to12()
			: base()
		{
			Point z = new Point("Z", 0, 6); intrinsic.Add(z);
			Point y = new Point("Y", 8, 0); intrinsic.Add(y);
			Point x = new Point("X", 4, 0); intrinsic.Add(x);
			Point m = new Point("M", 6, 0); intrinsic.Add(m);
			Point n = new Point("N", 4, 3); intrinsic.Add(n);
			Point t = new Point("T", 2, 3); intrinsic.Add(t);

			Segment mt = new Segment(m, t); intrinsic.Add(mt);
			Segment mn = new Segment(m, n); intrinsic.Add(mn);
			Segment nt = new Segment(n, t); intrinsic.Add(nt);
			
			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(m);
			pts.Add(y);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(y);
			pts2.Add(n);
			pts2.Add(z);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			List<Point> pts3 = new List<Point>();
			pts3.Add(z);
			pts3.Add(t);
			pts3.Add(x);
			Collinear coll3 = new Collinear(pts3, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(z, t)), GetProblemSegment(intrinsic, new Segment(t, x)), "Given"));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(x, m)), GetProblemSegment(intrinsic, new Segment(m, y)), "Given"));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(y, n)), GetProblemSegment(intrinsic, new Segment(n, z)), "Given"));
		}
	}
}