using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 23 Problem 9
    //
    public class Page23Problem9 : CongruentTrianglesProblem
    {
        public Page23Problem9(bool onoff) : base(onoff)
        {
            problemName = "Page 23 Problem 9";


            Point u = new Point("U", 0, 0); intrinsic.Add(u);
            Point v = new Point("V", 2, 4); intrinsic.Add(v);
            Point w = new Point("W", 4, 0); intrinsic.Add(w);
            Point x = new Point("X", 7, 0); intrinsic.Add(x);
            Point y = new Point("Y", 9, 4); intrinsic.Add(y);
            Point z = new Point("Z", 11, 0); intrinsic.Add(z);

            Segment uv = new Segment(u, v); intrinsic.Add(uv);
            Segment vw = new Segment(v, w); intrinsic.Add(vw);
            Segment yx = new Segment(y, x); intrinsic.Add(yx);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);

            List<Point> pts = new List<Point>();
            pts.Add(u);
            pts.Add(w);
            pts.Add(x);
            pts.Add(z);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(v, u, w)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(u, w, v))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(z, x, y)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(u, w, v))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(z, x, y)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, z, y))));

            given.Add(new GeometricCongruentSegments(uv, vw));
            given.Add(new GeometricCongruentSegments(yx, vw));
            given.Add(new GeometricCongruentSegments(yx, yz));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(u, x)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(w, z))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(u, v, w), new Triangle(x, y, z)));
        }
    }
}