using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 175 Problems 1-2
	//
	public class Page175ClassroomExercise01to02 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise01to02(bool onoff) : base(onoff)
		{
            problemName = "Page 175 Classroom Ex 1-2";

			Point a = new Point("A", 0, 0); intrinsic.Add(a);
			Point b = new Point("B", 9, 0); intrinsic.Add(b);
			Point c = new Point("C", 7, 3); intrinsic.Add(c);
			Point d = new Point("D", 2, 3); intrinsic.Add(d);
			

			Segment ab = new Segment(a, b); intrinsic.Add(ab);
			Segment ad = new Segment(a, d); intrinsic.Add(ad);
			Segment bc = new Segment(b, c); intrinsic.Add(bc);
			Segment cd = new Segment(c, d); intrinsic.Add(cd);

			intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

			given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, a, b)), GetProblemAngle(intrinsic, new Angle(a, b, c))));
		}
	}
}