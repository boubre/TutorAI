using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 285 problem 21
    //
    public class Page285Problem21 : CongruentTrianglesProblem
    {
        public Page285Problem21(bool onoff) : base(onoff)
        {
            problemName = "Page 285 Problem 21";


            Point a = new Point("A", 3, 4); intrinsic.Add(a);
            Point b = new Point("B", 9, 4); intrinsic.Add(b);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point d = new Point("D", 6, 0); intrinsic.Add(d);
            Point e = new Point("E", 12, 0); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment be = new Segment(b, e); intrinsic.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, d)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(d, e))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, c, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, e, d))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, e))));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, a))));
        }
    }
}