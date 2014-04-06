using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 73 Problem 9
    //
    public class Page73Problem9 : CongruentTrianglesProblem
    {
        public Page73Problem9(bool onoff) : base(onoff)
        {
            problemName = "Page 73 Problem 9";


            Point g = new Point("G", 5, 5); intrinsic.Add(g);
            Point h = new Point("H", 5, 3); intrinsic.Add(h);
            Point k = new Point("K", 0, 0); intrinsic.Add(k);
            Point n = new Point("N", 5, 0); intrinsic.Add(n);
            Point l = new Point("L", 8, 0); intrinsic.Add(l);

//     System.Diagnostics.Debug.WriteLine(new Segment(k, h).FindIntersection(new Segment(m, g)));

            Segment kh = new Segment(k, h); intrinsic.Add(kh);
            Segment lg = new Segment(l, g); intrinsic.Add(lg);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(n);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(n);
            pts.Add(l);
            Collinear coll2 = new Collinear(pts);
            
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(n, h, k)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(n, l, g))));
            given.Add(new GeometricCongruentSegments(lg, kh));
            given.Add(new RightAngle(new Angle(g, n, k)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(k, n, h), new Triangle(g, n, l)));
        }
    }
}