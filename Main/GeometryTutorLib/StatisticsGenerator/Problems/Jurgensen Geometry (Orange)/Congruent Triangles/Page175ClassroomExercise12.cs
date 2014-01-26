using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 175 Problems 7-12
	//
	public class Page175ClassroomExercise12 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise12(bool onoff) : base(onoff)
		{
            problemName = "Page 175 CLassroom Ex 12";

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

			given.Add(new Midpoint(m, GetProblemSegment(intrinsic, new Segment(x, y)), "Given"));
  			given.Add(new Midpoint(n, GetProblemSegment(intrinsic, new Segment(z, y)), "Given"));
 			given.Add(new Midpoint(t, GetProblemSegment(intrinsic, new Segment(z, x)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(t, x, m), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(n, m, y), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(m, n, t), "GOAL"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(n, m, y), "GOAL"));
            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(t, x, m), "GOAL"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(t, x, m), new Triangle(n, m, y), "GOAL"));
		}
	}
}