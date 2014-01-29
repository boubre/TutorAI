﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 156 problem 37
    //
    public class Page156Problem37 : ParallelLinesProblem
    {
        public Page156Problem37(bool onoff) : base(onoff)
        {
            problemName = "Page 156 Problem 37";

            Point p = new Point("P", 9, 0); intrinsic.Add(p);
            Point q = new Point("Q", 3, 0); intrinsic.Add(q);
            Point r = new Point("R", 0, 5); intrinsic.Add(r);
            Point s = new Point("S", 6, 5); intrinsic.Add(s);
            
            Segment pq = new Segment(p, q); intrinsic.Add(pq);
            Segment ps = new Segment(p, s); intrinsic.Add(ps);
            Segment qr = new Segment(q, r); intrinsic.Add(qr);
            Segment qs = new Segment(q, s); intrinsic.Add(qs);
            Segment rs = new Segment(r, s); intrinsic.Add(rs);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(r, s, p)), GetProblemAngle(intrinsic, new Angle(p, q, r)), "Given"));
            given.Add(new Supplementary(GetProblemAngle(intrinsic, new Angle(q, r, s)), GetProblemAngle(intrinsic, new Angle(p, q, r)), "Given"));

            goals.Add(new GeometricParallel(ps, qr, "GOAL"));
        }
    }
}