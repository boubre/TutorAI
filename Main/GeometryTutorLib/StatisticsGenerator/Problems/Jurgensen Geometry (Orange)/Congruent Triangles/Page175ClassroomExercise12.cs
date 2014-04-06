using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

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

            Point z = new Point("Z", 0, 12); intrinsic.Add(z);
			Point y = new Point("Y", 12, 0); intrinsic.Add(y);
			Point x = new Point("X", 0, 0); intrinsic.Add(x);
			Point m = new Point("M", 6, 0); intrinsic.Add(m);
			Point n = new Point("N", 6, 6); intrinsic.Add(n);
			Point t = new Point("T", 0, 6); intrinsic.Add(t);

            //Point z = new Point("Z", 0, 6); intrinsic.Add(z);
            //Point y = new Point("Y", 8, 0); intrinsic.Add(y);
            //Point x = new Point("X", 4, 0); intrinsic.Add(x);
            //Point m = new Point("M", 6, 0); intrinsic.Add(m);
            //Point n = new Point("N", 4, 3); intrinsic.Add(n);
            //Point t = new Point("T", 2, 3); intrinsic.Add(t);

			Segment mt = new Segment(m, t); intrinsic.Add(mt);
			Segment mn = new Segment(m, n); intrinsic.Add(mn);
			Segment nt = new Segment(n, t); intrinsic.Add(nt);
			
			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(m);
			pts.Add(y);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(y);
			pts2.Add(n);
			pts2.Add(z);
			Collinear coll2 = new Collinear(pts2);

			List<Point> pts3 = new List<Point>();
			pts3.Add(z);
			pts3.Add(t);
			pts3.Add(x);
			Collinear coll3 = new Collinear(pts3);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

			given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, m, new Segment(x, y))));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, n, new Segment(z, y))));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, t, new Segment(z, x))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(t, x, m)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(n, m, y)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(m, n, t)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(n, m, y)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(t, x, m)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(t, x, m), new Triangle(n, m, y)));
		}
	}
}