using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Midpoint Theorem for testing precomputation of segment bisector, etc.
    //
    public class SegmentBisectorTester: TransversalsProblem
    {
        public SegmentBisectorTester()
            : base()
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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(m, GetProblemSegment(intrinsic, new Segment(a, b)), "Given"));
        }
    }
}