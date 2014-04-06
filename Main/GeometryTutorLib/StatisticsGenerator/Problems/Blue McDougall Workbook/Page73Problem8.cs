using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 73 Problem 8
    //
    public class Page73Problem8 : CongruentTrianglesProblem
    {
        public Page73Problem8(bool onoff) : base(onoff)
        {
            problemName = "Page 73 Problem 8";


            Point t = new Point("T", -4, 5); intrinsic.Add(t);
            Point n = new Point("N", 0, 5); intrinsic.Add(n);
            Point s = new Point("S", 0, 0); intrinsic.Add(s);
            Point h = new Point("H", 0, -5); intrinsic.Add(h);
            Point u = new Point("U", 4, -5); intrinsic.Add(u);

            Segment nt = new Segment(n, t); intrinsic.Add(nt);
            Segment hu = new Segment(h, u); intrinsic.Add(hu);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(u);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(s);
            pts.Add(h);
            Collinear coll2 = new Collinear(pts);
            
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(s, u, h)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(n, t, s))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(n, s)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(s, h))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(s, n, t), new Triangle(s, h, u)));
        }
    }
}