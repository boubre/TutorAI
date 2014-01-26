using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 78 Problem 12
    //
    public class Page78Problem12 : CongruentTrianglesProblem
    {
        public Page78Problem12(bool onoff) : base(onoff)
        {
            problemName = "Page 78 Problem 12";


            Point m = new Point("M", 2, 5); intrinsic.Add(m);
            Point n = new Point("N", 0, 0); intrinsic.Add(n);
            Point q = new Point("Q", 12, 5); intrinsic.Add(q);
            Point t = new Point("T", 10, 0); intrinsic.Add(t);

            Segment mq = new Segment(m, q); intrinsic.Add(mq);
            Segment mt = new Segment(m, t); intrinsic.Add(mt);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment nt = new Segment(n, t); intrinsic.Add(nt);
            Segment tq = new Segment(t, q); intrinsic.Add(tq);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(mq, nt, "Given"));
            given.Add(new GeometricCongruentSegments(mq, nt, "Given"));

            goals.Add(new GeometricCongruentSegments(mn, tq, "GOAL"));
        }
    }
}