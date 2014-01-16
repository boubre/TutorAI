using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
	//
	// Geometry; Page 147 Problem 21
	//
	public class Page147Problem21 : CongruentTrianglesProblem
	{
        public Page147Problem21(bool onoff)
            : base(onoff)
		{
			Point d = new Point("D", 6, 6); intrinsic.Add(d);
			Point b = new Point("B", 9, 0); intrinsic.Add(b);
			Point a = new Point("A", 3, 0); intrinsic.Add(a);
			Point c = new Point("C", 11, 4); intrinsic.Add(c);
			Point e = new Point("E", 1, 4); intrinsic.Add(e);

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment ac = new Segment(a, c); intrinsic.Add(ac);
			Segment ad = new Segment(a, d); intrinsic.Add(ad);
			Segment ae = new Segment(a, e); intrinsic.Add(ae);
			Segment bc = new Segment(b, c); intrinsic.Add(bc);
			Segment bd = new Segment(b, d); intrinsic.Add(bd);
			Segment be = new Segment(b, e); intrinsic.Add(be);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);
			Segment ce = new Segment(c, e); intrinsic.Add(ce);
			Segment de = new Segment(d, e); intrinsic.Add(de);

			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ae), GetProblemSegment(intrinsic, bc), "Given"));
			given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ad), GetProblemSegment(intrinsic, bd), "Given"));
			/*
			 * enter parallels here
			 * */
		}
	}
}