using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 146 Problem 15
	//
	public class Page146Problem15 : CongruentTrianglesProblem
	{
        public Page146Problem15(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 15";


			Point t = new Point("T", 0, 0); intrinsic.Add(t);
			Point p = new Point("P", 0.5, 1.5); intrinsic.Add(p);
			Point s = new Point("S", 1, 3); intrinsic.Add(s);
			Point x = new Point("X", 3, 1); intrinsic.Add(x);
			Point q = new Point("Q", 3.5, 2.5); intrinsic.Add(q);
			Point y = new Point("Y", 4, 4); intrinsic.Add(y);
			Point o = new Point("O", 2, 2); intrinsic.Add(o);

			List<Point> pts = new List<Point>();
			pts.Add(t);
			pts.Add(o);
			pts.Add(y);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(s);
			pts2.Add(o);
			pts2.Add(x);
			Collinear coll2 = new Collinear(pts2);

			List<Point> pts3 = new List<Point>();
			pts3.Add(s);
			pts3.Add(p);
			pts3.Add(t);
			Collinear coll3 = new Collinear(pts3);

			List<Point> pts4 = new List<Point>();
			pts4.Add(y);
			pts4.Add(q);
			pts4.Add(x);
			Collinear coll4 = new Collinear(pts4);

			List<Point> pts5 = new List<Point>();
			pts5.Add(p);
			pts5.Add(o);
			pts5.Add(q);
			Collinear coll5 = new Collinear(pts5);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll5));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new SegmentBisector(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(s, x), new Segment(t, y)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(s, x))));
            given.Add(new SegmentBisector(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(s, x), new Segment(t, y)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(t, y))));

            InMiddle goalIm = ClauseConstructor.GetProblemInMiddle(intrinsic, o, new Segment(p, q));
            goals.Add(new Strengthened(goalIm, new Midpoint(goalIm)));
		}
	}
}