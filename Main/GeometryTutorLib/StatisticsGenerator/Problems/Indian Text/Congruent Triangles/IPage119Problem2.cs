using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book i Page 119 Problem 2
    //
    public class IPage119Problem2 : ActualProblem
    {
        public IPage119Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "I Page 119 Problem 2";

            Point a = new Point("A", 5, 4); intrinsic.Add(a);
            Point x = new Point("X", 3, 2.4); intrinsic.Add(x);
            Point b = new Point("B", 1, 4); intrinsic.Add(b);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point d = new Point("D", 6, 0); intrinsic.Add(d);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

System.Diagnostics.Debug.WriteLine(new Segment(a, c).FindIntersection(new Segment(b, d)));

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            //given.Add(new Midpoint(m, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c))));
            given.Add(new GeometricCongruentSegments(ad, bc));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, c)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(b, a, c)));
            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, d)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, c))));
        }
    }
}