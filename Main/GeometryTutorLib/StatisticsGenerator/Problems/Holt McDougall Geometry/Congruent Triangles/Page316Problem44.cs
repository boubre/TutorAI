using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 316 problem 44
    //
    public class Page316Problem44 : CongruentTrianglesProblem
    {
        public Page316Problem44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 316 Problem 44";


            Point v = new Point("V", -3, 0); intrinsic.Add(v);
            Point w = new Point("W", 0, 3); intrinsic.Add(w);
            Point x = new Point("X", 100, 0); intrinsic.Add(x);
            Point y = new Point("Y", 0, -3); intrinsic.Add(y);
            Point z = new Point("Z", 0, 0); intrinsic.Add(z);

            Segment vw = new Segment(v, w); intrinsic.Add(vw);
            Segment vy = new Segment(v, y); intrinsic.Add(vy);
            Segment wx = new Segment(w, x); intrinsic.Add(wx);
            Segment xy = new Segment(x, y); intrinsic.Add(xy);

            List<Point> pts = new List<Point>();
            pts.Add(v);
            pts.Add(z);
            pts.Add(x);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(w);
            pts2.Add(z);
            pts2.Add(y);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(w, v, z)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(y, v, z))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(w, x, z)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(y, x, z))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(z, w, x), new Triangle(z, y, x)));
        }
    }
}