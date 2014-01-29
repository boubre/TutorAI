using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(o);
            pts.Add(t);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(s, t)),
                                            GetProblemSegment(intrinsic, new Segment(r, a)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(o, a)),
                                                     GetProblemSegment(intrinsic, new Segment(o, r)), "Given"));

            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, s)),
                                                     GetProblemSegment(intrinsic, new Segment(a, t)), "Given"));
            goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, t)),
                                                     GetProblemSegment(intrinsic, new Segment(a, s)), "Given"));
        }
    }
}