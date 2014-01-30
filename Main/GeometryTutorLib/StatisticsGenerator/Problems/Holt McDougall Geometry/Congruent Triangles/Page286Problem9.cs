using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 286 problem 9
    //
    public class Page286Problem9 : CongruentTrianglesProblem
    {
        public Page286Problem9(bool onoff) : base(onoff)
        {
            problemName = "Page 286 Problem 9";


            Point f = new Point("F", 0, 0); intrinsic.Add(f);
            Point g = new Point("G", 5, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(g);
            Point h = new Point("H", 10, 0); intrinsic.Add(h);
            Point j = new Point("J", 15, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(j);
            Point k = new Point("K", 20, 0); intrinsic.Add(k);
            Point l = new Point("L", 25, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(l);

            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment fh = new Segment(f, h); intrinsic.Add(fh);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment jk = new Segment(j, k); intrinsic.Add(jk);
            Segment jl = new Segment(j, l); intrinsic.Add(jl);
            Segment kl = new Segment(k, l); intrinsic.Add(kl);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(fg, fh));
            given.Add(new GeometricCongruentSegments(fg, gh));
            given.Add(new GeometricCongruentSegments(fg, jk));
            given.Add(new GeometricCongruentSegments(fg, jl));
            given.Add(new GeometricCongruentSegments(fg, kl));
            
            goals.Add(new GeometricCongruentTriangles(new Triangle(f, g, h), new Triangle(j, k, l)));
        }
    }
}