using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 229 Problem 03
    //
    public class Page229Problem03 : CongruentTrianglesProblem
    {
        public Page229Problem03(bool onoff) : base(onoff)
        {
            problemName = "Page 229 Problem 03";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 12, 0); intrinsic.Add(b);
            //Point c = new Point("C", 153.0 / 32.0, 63.0 * 3.8729833462074168851792653997824 / 32.0); intrinsic.Add(c);  // square root of 15 = 3.8729833462074168851792653997824
            Point c = new Point("C", 153.0 / 32.0, 63.0 * Math.Sqrt(15.0) / 32.0); intrinsic.Add(c);  
            Point x = new Point("X", 18, 0); intrinsic.Add(x);
            Point r = new Point("R", 26, 0); intrinsic.Add(r);
            Point n = new Point("N", 339.0 / 16.0, 21.0 * Math.Sqrt(15.0) / 16.0); intrinsic.Add(n);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment nx = new Segment(n, x); intrinsic.Add(nx);
            Segment nr = new Segment(n, r); intrinsic.Add(nr);
            Segment rx = new Segment(r, x); intrinsic.Add(rx);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
        }
    }
}