using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 26 Problem 2
    //
    public class Page26Problem2 : CongruentTrianglesProblem
    {
        public Page26Problem2(bool onoff) : base(onoff)
        {
            problemName = "Page 26 Problem 2";
            numberOfOriginalTextProblems = 1;

            Point l = new Point("L", 0, 0); intrinsic.Add(l);
            Point m = new Point("M", 4, 1); intrinsic.Add(m);
            Point k = new Point("K", 1, 6); intrinsic.Add(k);
            Point j = new Point("J", 5, 7); intrinsic.Add(j);

            Segment kj = new Segment(k, j); intrinsic.Add(kj);
            Segment kl = new Segment(k, l); intrinsic.Add(kl);
            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment mj = new Segment(m, j); intrinsic.Add(mj);
            Segment km = new Segment(k, m); intrinsic.Add(km);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(k, l, m)), GetProblemAngle(intrinsic, new Angle(k, j, m)), "Given"));
            given.Add(new GeometricParallel(kj, lm, "Given"));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(l, k, m)), GetProblemAngle(intrinsic, new Angle(j, m, k)), "GOAL"));
        }
    }
}