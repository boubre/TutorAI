using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 301 problem 52
    //
    public class Page301Problem52 : ParallelLinesProblem
    {
        public Page301Problem52(bool onoff)
            : base(onoff)
        {

            problemName = "Page 301 Problem 52";


            Point p = new Point("P", 0, 5); intrinsic.Add(p);
            Point q = new Point("Q", 10, 5); intrinsic.Add(q);
            Point r = new Point("R", 10, 0); intrinsic.Add(r);
            Point s = new Point("S", 0, 0); intrinsic.Add(s);

            Segment pq = new Segment(p, q); intrinsic.Add(pq);
            Segment pr = new Segment(p, r); intrinsic.Add(pr);
            Segment ps = new Segment(p, s); intrinsic.Add(ps);
            Segment qr = new Segment(q, r); intrinsic.Add(qr);
            Segment rs = new Segment(r, s); intrinsic.Add(rs);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(q, p, r)), GetProblemAngle(intrinsic, new Angle(p, r, s)), "Given"));
            given.Add(new RightAngle(p, s, r, "Given"));
            given.Add(new RightAngle(p, q, r, "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(p, s, r), new Triangle(r, q, p), "GOAL"));
        }
    }
}