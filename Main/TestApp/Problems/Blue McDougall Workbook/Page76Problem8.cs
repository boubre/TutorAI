using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 76 Problem 8
    //
    public class Page76Problem8 : SimilarTrianglesProblem
    {
        public Page76Problem8(bool onoff) : base(onoff)
        {
            problemName = "Page 76 Problem 8";
            numberOfOriginalTextProblems = 1;

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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(st, rq, "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(s, r)), GetProblemSegment(intrinsic, new Segment(r, p)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(s, r, t)), GetProblemAngle(intrinsic, new Angle(r, p, q)), "Given"));

            goals.Add(new GeometricCongruentSegments(st, rq, "GOAL"));
        }
    }
}