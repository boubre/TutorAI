using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 26 Problem 2
    //
    public class Page26Problem2 : CongruentTrianglesProblem
    {
        public Page26Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 26 Problem 2";


            Point l = new Point("L", 0, 0); intrinsic.Add(l);
            Point m = new Point("M", 4, 1); intrinsic.Add(m);
            Point k = new Point("K", 1, 6); intrinsic.Add(k);
            Point j = new Point("J", 5, 7); intrinsic.Add(j);

            Segment kj = new Segment(k, j); intrinsic.Add(kj);
            Segment kl = new Segment(k, l); intrinsic.Add(kl);
            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment mj = new Segment(m, j); intrinsic.Add(mj);
            Segment km = new Segment(k, m); intrinsic.Add(km);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(k, l, m)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(k, j, m))));
            given.Add(new GeometricParallel(kj, lm));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, k, m)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(j, m, k))));
        }
    }
}