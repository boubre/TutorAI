using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 76 Problem 8
    //
    public class Page76Problem8 : SimilarTrianglesProblem
    {
        public Page76Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 76 Problem 8";


            Point s = new Point("S", 2, 8);  intrinsic.Add(s);
            Point r = new Point("R", 1, 4);  intrinsic.Add(r);
            Point t = new Point("T", 5, 4);  intrinsic.Add(t);
            Point q = new Point("Q", 4, 0);  intrinsic.Add(q);
            Point p = new Point("P", 0, 0);  intrinsic.Add(p);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(p, d)));

            Segment st = new Segment(s, t); intrinsic.Add(st);
            Segment rt = new Segment(r, t); intrinsic.Add(rt);
            Segment rq = new Segment(r, q); intrinsic.Add(rq);
            Segment pq = new Segment(p, q); intrinsic.Add(pq);

            List<Point> pts = new List<Point>();
            pts.Add(s);
            pts.Add(r);
            pts.Add(p);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(st, rq));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(s, r)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, p))));
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(s, r, t)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(r, p, q))));

            goals.Add(new GeometricCongruentSegments(st, rq));
        }
    }
}