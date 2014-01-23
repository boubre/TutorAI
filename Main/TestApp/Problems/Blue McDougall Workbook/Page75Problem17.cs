using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 75 Problem 17
    //
    public class Page75Problem17 : CongruentTrianglesProblem
    {
        public Page75Problem17(bool onoff) : base(onoff)
        {
            problemName = "Page 75 Problem 17";
            numberOfOriginalTextProblems = 1;

            Point v = new Point("V", 3, -7); intrinsic.Add(v);
            Point z = new Point("Z", 0, 0); intrinsic.Add(z);
            Point y = new Point("Y", 6, 0); intrinsic.Add(y);
            Point x = new Point("X", 7, 0); intrinsic.Add(x);
            Point u = new Point("U", 10, -7); intrinsic.Add(u);
            Point w = new Point("W", 13, 0); intrinsic.Add(w);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment vz = new Segment(v, z); intrinsic.Add(vz);
            Segment vy = new Segment(v, y); intrinsic.Add(vy);
            Segment ux = new Segment(u, x); intrinsic.Add(ux);
            Segment uw = new Segment(u, w); intrinsic.Add(uw);

            List<Point> pts = new List<Point>();
            pts.Add(z);
            pts.Add(y);
            pts.Add(x);
            pts.Add(w);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(vz, ux, "Given"));
            given.Add(new GeometricParallel(vy, uw, "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(w, x)), GetProblemSegment(intrinsic, new Segment(y, z)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(w, x, u), new Triangle(y, z, v), "GOAL"));
        }
    }
}