using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Midpoint Theorem for testing precomputation of segment bisector, etc.
    //
    public class SegmentBisectorTester: TransversalsProblem
    {
        public SegmentBisectorTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -3, 0);   intrinsic.Add(a);
            Point m = new Point("M", 0, 0);  intrinsic.Add(m);
            Point b = new Point("B", 3, 0);   intrinsic.Add(b);

            Point d = new Point("D", 12, 12); intrinsic.Add(d);
            Segment dm = new Segment(d, m); intrinsic.Add(dm);

            Point q = new Point("Q", 0, 18); intrinsic.Add(q);
            Segment qm = new Segment(q, m); intrinsic.Add(qm);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, m, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)))));
        }
    }
}