using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 164 problem 44
    //
    public class Page164Problem44 : ParallelLinesProblem
    {
        public Page164Problem44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 164 Problem 44";

            Point l = new Point("L", 0, 9); intrinsic.Add(l);
            Point m = new Point("M", 12, 9); intrinsic.Add(m);
            Point n = new Point("N", 6, 0); intrinsic.Add(n);
            Point a = new Point("A", 4, 3); intrinsic.Add(a);
            Point b = new Point("B", 8, 3); intrinsic.Add(b);

            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment ab = new Segment(a, b); intrinsic.Add(ab);

            List<Point> pts = new List<Point>();
            pts.Add(n);
            pts.Add(a);
            pts.Add(l);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(n);
            pts2.Add(b);
            pts2.Add(m);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(n, l)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(n, m))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, l)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, m))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, n)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, n))));
        }
    }
}