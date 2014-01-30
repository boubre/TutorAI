using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 175 Problems 1 - 4 (bottom of page)
	//
	public class Page175WrittenExercise1to4 : CongruentTrianglesProblem
	{
        public Page175WrittenExercise1to4(bool onoff)
            : base(onoff)
		{
			Point a = new Point("A", 0, 2); intrinsic.Add(a);
			Point b = new Point("B", 1, 3); intrinsic.Add(b);
			Point c = new Point("C", 0, 0); intrinsic.Add(c);
			Point d = new Point("D", 2, 2); intrinsic.Add(d);
			Point e = new Point("E", 3, 0); intrinsic.Add(e);
			Point f = new Point("F", 4, 1); intrinsic.Add(f);
			Point x = new Point("X", 0, 4); intrinsic.Add(x);
			Point y = new Point("Y", 6, 0); intrinsic.Add(y);

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);
			Segment ef = new Segment(e, f); intrinsic.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(a);
			pts.Add(c);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(c);
			pts2.Add(e);
			pts2.Add(y);
			Collinear coll2 = new Collinear(pts2);

			List<Point> pts3 = new List<Point>();
			pts3.Add(y);
			pts3.Add(f);
			pts3.Add(d);
			Collinear coll3 = new Collinear(pts3);

			List<Point> pts4 = new List<Point>();
			pts4.Add(x);
			pts4.Add(b);
			pts4.Add(d);
			Collinear coll4 = new Collinear(pts4);

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateSegmentClauses(coll3));
			intrinsic.AddRange(GenerateSegmentClauses(coll4));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(x, a)), GetProblemSegment(intrinsic, new Segment(a, c))));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(c, e)), GetProblemSegment(intrinsic, new Segment(e, y))));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(y, f)), GetProblemSegment(intrinsic, new Segment(f, d))));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(d, b)), GetProblemSegment(intrinsic, new Segment(b, x))));
		}
	}
}