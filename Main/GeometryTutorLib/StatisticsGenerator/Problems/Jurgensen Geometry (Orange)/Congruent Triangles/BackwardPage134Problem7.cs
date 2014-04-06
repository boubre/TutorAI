using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 134 #7
    //
    public class BackwardPage134Problem7 : CongruentTrianglesProblem
    {
        public BackwardPage134Problem7(bool onoff) : base(onoff)
        {
            problemName = "Backward Page 134 Problem 7";

            Point s = new Point("S", 1, 4); intrinsic.Add(s);
            Point t = new Point("T", 5, 4); intrinsic.Add(t);
            Point o = new Point("O", 3, 2.4); intrinsic.Add(o);
            Point r = new Point("R", 0, 0); intrinsic.Add(r);
            Point a = new Point("A", 6, 0); intrinsic.Add(a);

            Segment st = new Segment(s, t); intrinsic.Add(st);
            Segment sr = new Segment(s, r); intrinsic.Add(sr);
            Segment ta = new Segment(t, a); intrinsic.Add(ta);
            Segment ra = new Segment(r, a); intrinsic.Add(ra);

            List<Point> pts = new List<Point>();
            pts.Add(s);
            pts.Add(o);
            pts.Add(a);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(o);
            pts.Add(t);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(s, t)),
                                            ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, a))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(o, a)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(o, r))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, s)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, t))));
            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, t)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, s))));
        }
    }
}