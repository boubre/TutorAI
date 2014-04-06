using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class IPage120Problem6 : CongruentTrianglesProblem
    {
        public IPage120Problem6(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 120 Problem 6";

            Point a = new Point("A", 2, 6);  intrinsic.Add(a);
            Point b = new Point("B", 0, 0);  intrinsic.Add(b);
            Point c = new Point("C", 10, 0); intrinsic.Add(c);
            Point d = new Point("D", 4, 0);  intrinsic.Add(d);
            Point e = new Point("E", 12, 6); intrinsic.Add(e);


            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ae = new Segment(a, e); intrinsic.Add(ae);
            Segment ec = new Segment(e, c); intrinsic.Add(ec);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ac, ae));
            given.Add(new GeometricCongruentSegments(ab, ad));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(e, a, c))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, c)), de));
        }
    }
}