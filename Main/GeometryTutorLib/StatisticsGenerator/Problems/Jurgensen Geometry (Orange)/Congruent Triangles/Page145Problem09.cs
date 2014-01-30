using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 145 Problem 9
	//
	public class Page145Problem09 : CongruentTrianglesProblem
	{
        public Page145Problem09(bool onoff) : base(onoff)
		{
            problemName = "Page 145 Problem 9";


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
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(x);
			pts2.Add(l);
			pts2.Add(z);
			Collinear coll2 = new Collinear(pts2);

			intrinsic.AddRange(GenerateSegmentClauses(coll1));
			intrinsic.AddRange(GenerateSegmentClauses(coll2));
			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentTriangles(new Triangle(r, s, t), new Triangle(x, y, z)));
			given.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(r, s, t)), ks));
            given.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(x, y, z)), ly));

            goals.Add(new GeometricCongruentSegments(ly, ks));
		}
	}
}