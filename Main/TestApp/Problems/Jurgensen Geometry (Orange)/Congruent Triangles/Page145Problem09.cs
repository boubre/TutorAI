﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 145 Problem 9
	//
	public class Page145Problem09 : CongruentTrianglesProblem
	{
        public Page145Problem09(bool onoff)
            : base(onoff)
		{
			Point s = new Point("S", 0, 0); intrinsic.Add(s);
			Point r = new Point("R", 2, 4); intrinsic.Add(r);
			Point t = new Point("T", 8, 4); intrinsic.Add(t);
			Point k = new Point("K", 4, 4); intrinsic.Add(k);

			Point y = new Point("Y", 10, 0); intrinsic.Add(y);
			Point x = new Point("X", 12, 4); intrinsic.Add(x);
			Point z = new Point("Z", 18, 4); intrinsic.Add(z);
			Point l = new Point("L", 14, 4); intrinsic.Add(l);
			
			Segment rs = new Segment(r, s); intrinsic.Add(rs);
			Segment st = new Segment(s, t); intrinsic.Add(st);
			Segment ks = new Segment(k, s); intrinsic.Add(ks);
			Segment xy = new Segment(x, y); intrinsic.Add(xy);
			Segment yz = new Segment(y, z); intrinsic.Add(yz);
			Segment ly = new Segment(l, y); intrinsic.Add(ly);

			List<Point> pts = new List<Point>();
			pts.Add(r);
			pts.Add(k);
			pts.Add(t);
			Collinear coll1 = new Collinear(pts, "Intrinsic");

			List<Point> pts2 = new List<Point>();
			pts2.Add(x);
			pts2.Add(l);
			pts2.Add(z);
			Collinear coll2 = new Collinear(pts2, "Intrinsic");

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, rs), GetProblemSegment(intrinsic, xy), "Given"));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, st), GetProblemSegment(intrinsic, yz), "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(r, s, k)), GetProblemAngle(intrinsic, new Angle(k, s, t)), "Given"));
			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(x, y, l)), GetProblemAngle(intrinsic, new Angle(l, y, z)), "Given"));
		}
	}
}