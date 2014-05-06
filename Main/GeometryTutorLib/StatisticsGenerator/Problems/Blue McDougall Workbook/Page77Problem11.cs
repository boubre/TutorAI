using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 77 Problem 11
    //
    public class Page77Problem11 : CongruentTrianglesProblem
    {
        public Page77Problem11(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 77 Problem 11";


            Point x = new Point("X", 0, 8); intrinsic.Add(x);
            Point y = new Point("Y", -3, 5); intrinsic.Add(y);
            Point w = new Point("W", 3, 5); intrinsic.Add(w);
            Point z = new Point("Z", 0, 0); intrinsic.Add(z);
            Point o = new Point("O", 0, 5); intrinsic.Add(o);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment xy = new Segment(x, y); intrinsic.Add(xy);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);
            Segment zw = new Segment(z, w); intrinsic.Add(zw);
            Segment xw = new Segment(x, w); intrinsic.Add(xw);

            List<Point> pts = new List<Point>();
            pts.Add(y);
            pts.Add(o);
            pts.Add(w);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(o);
            pts.Add(z);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(y, x, w)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(z, x))));
            given.Add(new GeometricCongruentSegments(xy, xw));

            goals.Add(new GeometricCongruentSegments(yz, zw));
        }
    }
}