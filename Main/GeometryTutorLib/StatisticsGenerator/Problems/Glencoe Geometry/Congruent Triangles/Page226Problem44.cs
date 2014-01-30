using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 226 problem 44
    //
    public class Page226Problem44 : CongruentTrianglesProblem
    {
        public Page226Problem44(bool onoff) : base(onoff)
        {
            problemName = "Page 226 Problem 44";

            Point v = new Point("V", 4, 12); intrinsic.Add(v);
            Point w = new Point("W", 12, 6); intrinsic.Add(w);
            Point x = new Point("X", 8, 4); intrinsic.Add(x);
            Point y = new Point("Y", 10, 0); intrinsic.Add(y);
            Point z = new Point("Z", 0, 0); intrinsic.Add(z);

            Segment vw = new Segment(v, w); intrinsic.Add(vw);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);

            List<Point> pts = new List<Point>();
            pts.Add(z);
            pts.Add(x);
            pts.Add(w);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(v);
            pts2.Add(x);
            pts2.Add(y);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(x, v, w)), GetProblemAngle(intrinsic, new Angle(x, z, y))));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(w, x)), GetProblemSegment(intrinsic, new Segment(x, y))));

            goals.Add(new GeometricCongruentSegments(vw, yz));
        }
    }
}