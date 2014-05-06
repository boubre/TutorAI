using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 226 problem 41
    //
    public class Page226Problem41 : ParallelLinesProblem
    {
        public Page226Problem41(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 41";

            Point a = new Point("A", -2, 0); intrinsic.Add(a);
            Point b = new Point("B", 8, 0); intrinsic.Add(b);
            Point c = new Point("C", -6, -4); intrinsic.Add(c);
            Point d = new Point("D", 9, -2); intrinsic.Add(d);

            Point q = new Point("Q", 3, 2); intrinsic.Add(q);
            Point r = new Point("R", 0, 0); intrinsic.Add(r);
            Point s = new Point("S", 6, 0); intrinsic.Add(s);
            
            Segment qr = new Segment(q, r); intrinsic.Add(qr);
            Segment qs = new Segment(q, s); intrinsic.Add(qs);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(r);
            pts.Add(s);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(r);
            pts.Add(q);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(s);
            pts.Add(q);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, r, q)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, s, q))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(q, r)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(q, s))));
        }
    }
}