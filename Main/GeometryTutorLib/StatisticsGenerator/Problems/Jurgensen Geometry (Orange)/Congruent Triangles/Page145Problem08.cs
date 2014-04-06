using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 145 Problem 8
	//
	public class Page145Problem08 : CongruentTrianglesProblem
	{
        public Page145Problem08(bool onoff) : base(onoff)
		{
            problemName = "Page 145 Problem 8";


			Point f = new Point("F", 0, 5); intrinsic.Add(f);
			Point l = new Point("L", 4, 7); intrinsic.Add(l);
			Point k = new Point("K", 4, 3); intrinsic.Add(k);
			Point a = new Point("A", 5, 5); intrinsic.Add(a);
			Point j = new Point("J", 3, 5); intrinsic.Add(j);

			Segment fl = new Segment(f, l); intrinsic.Add(fl);
			Segment fk = new Segment(f, k); intrinsic.Add(fk);
			Segment al = new Segment(a, l); intrinsic.Add(al);
			Segment ak = new Segment(a, k); intrinsic.Add(ak);
			Segment jl = new Segment(j, l); intrinsic.Add(jl);
			Segment jk = new Segment(j, k); intrinsic.Add(jk);

			List<Point> pts = new List<Point>();
			pts.Add(f);
			pts.Add(j);
			pts.Add(a);
			Collinear coll1 = new Collinear(pts);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

			given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, f, k)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(f, a))));
            given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, a, k)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(f, a))));

            goals.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, j, k)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(f, a))));
		}
	}
}