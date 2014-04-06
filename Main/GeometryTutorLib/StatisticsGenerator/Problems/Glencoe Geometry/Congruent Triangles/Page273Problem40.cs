using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 273 problem 40
    //
    public class Page273Problem40 : CongruentTrianglesProblem
    {
        public Page273Problem40(bool onoff) : base(onoff)
        {
            problemName = "Page 273 Problem 40";

            Point l = new Point("L", 6, 0);               intrinsic.Add(l);
            Point m = new Point("M", 0, 0);               intrinsic.Add(m);
            Point n = new Point("N", 3, Math.Sqrt(27.0)); intrinsic.Add(n);
            Point o = new Point("O", Math.Sqrt(12.0), 2); intrinsic.Add(o);
            
            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment lo = new Segment(l, o); intrinsic.Add(lo);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment mo = new Segment(m, o); intrinsic.Add(mo);
            Segment no = new Segment(n, o); intrinsic.Add(no);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, m, n)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(o, m))));
            given.Add(new GeometricCongruentSegments(lm, mn));

            goals.Add(new GeometricCongruentTriangles(new Triangle(m, o, l), new Triangle(m, o, n)));
        }
    }
}