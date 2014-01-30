using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 301 problem 50
    //
    public class Page301Problem50 : CongruentTrianglesProblem
    {
        public Page301Problem50(bool onoff) : base(onoff)
        {
            problemName = "Page 301 Problem 50";


            Point w = new Point("W", 5, 0); intrinsic.Add(w);
            Point x = new Point("X", 0, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(x);
            Point y = new Point("Y", 10, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(y);
            Point z = new Point("Z", 15, 0); intrinsic.Add(z);

            Segment wx = new Segment(w, x); intrinsic.Add(wx);
            Segment wy = new Segment(w, y); intrinsic.Add(wy);
            Segment wz = new Segment(w, z); intrinsic.Add(wz);
            Segment xy = new Segment(x, y); intrinsic.Add(xy);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(wx, wz));
            given.Add(new GeometricCongruentSegments(wx, yz));
            given.Add(new GeometricCongruentSegments(wx, xy));

            goals.Add(new GeometricCongruentTriangles(new Triangle(w, x, y), new Triangle(w, y, z)));
        }
    }
}