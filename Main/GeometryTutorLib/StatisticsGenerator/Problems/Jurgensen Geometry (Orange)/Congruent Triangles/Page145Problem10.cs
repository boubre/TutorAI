using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 145 Problem 10
	//
	public class Page145Problem10 : CongruentTrianglesProblem
	{
        public Page145Problem10(bool onoff) : base(onoff)
		{
            problemName = "Page 145 Problem 10";


            Point a = new Point("A", 0, 5); intrinsic.Add(a);
			Point b = new Point("B", 1, 6); intrinsic.Add(b);
			Point c = new Point("C", 5, 7); intrinsic.Add(c);
			Point d = new Point("D", 6, 5); intrinsic.Add(d);
			Point e = new Point("E", 5, 3); intrinsic.Add(e);
			Point f = new Point("F", 1, 4); intrinsic.Add(f);

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment bc = new Segment(b, c); intrinsic.Add(bc);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);
			Segment de = new Segment(d, e); intrinsic.Add(de);
			Segment ef = new Segment(e, f); intrinsic.Add(ef);
			Segment af = new Segment(a, f); intrinsic.Add(af);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);

            // Constructed
            Segment ae = new Segment(a, e); intrinsic.Add(ae);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentSegments(ab, af));
			given.Add(new GeometricCongruentSegments(cd, de));
			given.Add(new GeometricCongruentSegments(bc, ef));
  			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, d, a)), GetProblemAngle(intrinsic, new Angle(c, d, a))));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(a, b, c)), GetProblemAngle(intrinsic, new Angle(a, f, e))));
        }
	}
}