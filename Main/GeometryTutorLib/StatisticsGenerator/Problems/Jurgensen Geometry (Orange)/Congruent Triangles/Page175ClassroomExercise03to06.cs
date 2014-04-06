using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 175 Problems 3-6
	//
	public class Page175ClassroomExercise03to06 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise03to06(bool onoff)
            : base(onoff)
		{
			Point e = new Point("E", 0, 0); intrinsic.Add(e);
			Point f = new Point("F", 6, 4); intrinsic.Add(f);
			Point g = new Point("G", 6, 6); intrinsic.Add(g);
			Point h = new Point("H", 0, 10); intrinsic.Add(h);
			Point m = new Point("M", 3, 2); intrinsic.Add(m);
			Point n = new Point("N", 3, 8); intrinsic.Add(n);


			Segment eh = new Segment(e, h); intrinsic.Add(eh);
			Segment fg = new Segment(f, g); intrinsic.Add(fg);

			List<Point> pts = new List<Point>();
			pts.Add(e);
			pts.Add(m);
			pts.Add(f);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(h);
			pts2.Add(n);
			pts2.Add(g);
			Collinear coll2 = new Collinear(pts2);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

			given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(e, m)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(m, f))));
			given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(h, n)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(n, g))));
		}
	}
}