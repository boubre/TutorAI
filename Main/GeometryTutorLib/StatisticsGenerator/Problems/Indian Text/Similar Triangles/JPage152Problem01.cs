using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book J: Page 152 Problem 1
    //
    public class JPage152Problem01 : SimilarTrianglesProblem
    {
        public JPage152Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 152 Problem 1";


            Point p = new Point("P", 2.5, 3); intrinsic.Add(p);
            Point q = new Point("Q", 0, 0); intrinsic.Add(q);
            Point s = new Point("S", 2.5, 0); intrinsic.Add(s);
            Point r = new Point("R", 5, 0); intrinsic.Add(r);

            Segment pq = new Segment(p, q); intrinsic.Add(pq);
            Segment ps = new Segment(p, s); intrinsic.Add(ps);
            Segment pr = new Segment(p, r); intrinsic.Add(pr);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(s);
            pts.Add(r);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(q, p, r)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(p, s))));
        }
    }
}