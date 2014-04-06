﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 146 Problem 14
	//
	public class Page146Problem14 : CongruentTrianglesProblem
	{
        public Page146Problem14(bool onoff) : base(onoff)
		{
            problemName = "Page 146 Problem 14";


			Point g = new Point("G", 0, 0); intrinsic.Add(g);
			Point d = new Point("D", 3, 2); intrinsic.Add(d);
			Point e = new Point("E", 7, 0); intrinsic.Add(e);
			Point h = new Point("H", 2, 0); intrinsic.Add(h);
			Point k = new Point("K", 5, 0); intrinsic.Add(k);
			Point f = new Point("F", 4, -2); intrinsic.Add(f);

			Segment dg = new Segment(d, g); intrinsic.Add(dg);
			Segment de = new Segment(d, e); intrinsic.Add(de);
			Segment dh = new Segment(d, h); intrinsic.Add(dh);
			Segment fg = new Segment(f, g); intrinsic.Add(fg);
			Segment fk = new Segment(f, k); intrinsic.Add(fk);
			Segment ef = new Segment(e, f); intrinsic.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(g);
			pts.Add(h);
			pts.Add(k);
			pts.Add(e);
			Collinear coll1 = new Collinear(pts);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(dg, ef));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, d, e)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, f, e))));
			given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(g, h)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(e, k))));

            goals.Add(new GeometricParallel(dh, fk));
		}
	}
}