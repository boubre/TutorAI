﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 316 problem 42
    //
    public class Page316Problem42 : CongruentTrianglesProblem
    {
        public Page316Problem42(bool onoff) : base(onoff)
        {

            problemName = "Page 316 Problem 42";


            Point p = new Point("P", 10, 0); intrinsic.Add(p);
            Point q = new Point("Q", 0, 0); intrinsic.Add(q);
            Point l = new Point("L", 0, 6); intrinsic.Add(l);
            Point m = new Point("M", 10, 6); intrinsic.Add(m);
            Point n = new Point("N", 3, 3); intrinsic.Add(n);

            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment ln = new Segment(l, n); intrinsic.Add(ln);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment np = new Segment(n, p); intrinsic.Add(np);
            Segment nq = new Segment(n, q); intrinsic.Add(nq);
            Segment pq = new Segment(p, q); intrinsic.Add(pq);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
            
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(m, l, n)), GetProblemAngle(intrinsic, new Angle(n, q, p)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(n, m, l)), GetProblemAngle(intrinsic, new Angle(n, p, q)), "Given"));
            given.Add(new GeometricCongruentSegments(ln, nq, "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(l, m, n), new Triangle(q, p, n), "GOAL"));
        }
    }
}