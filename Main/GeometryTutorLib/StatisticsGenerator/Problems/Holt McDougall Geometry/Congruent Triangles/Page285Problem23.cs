using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 285 problem 23
    //
    public class Page285Problem23 : CongruentTrianglesProblem
    {
        public Page285Problem23(bool onoff) : base(onoff)
        {
            problemName = "Page 285 Problem 23";


            Point q = new Point("Q", 0, 5); intrinsic.Add(q);
            Point t = new Point("T", 2.4, 1.8); intrinsic.Add(t);
            Point v = new Point("V", 0, 0); intrinsic.Add(v);
            Point s = new Point("S", -2.4, 1.8); intrinsic.Add(s);
            Point p = new Point("P", -4, 3); intrinsic.Add(p);
            Point u = new Point("U", -3.75, 0); intrinsic.Add(u);
            Point r = new Point("R", 4, 3); intrinsic.Add(r);
            Point w = new Point("W", 3.75, 0); intrinsic.Add(w);

            Segment pu = new Segment(p, u); intrinsic.Add(pu);
            Segment rw = new Segment(r, w); intrinsic.Add(rw);
            Segment qv = new Segment(q, v); intrinsic.Add(qv);

            List<Point> pts = new List<Point>();
            pts.Add(u);
            pts.Add(s);
            pts.Add(q);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(p);
            pts2.Add(s);
            pts2.Add(v);
            Collinear coll2 = new Collinear(pts2);

            List<Point> pts3 = new List<Point>();
            pts3.Add(q);
            pts3.Add(t);
            pts3.Add(w);
            Collinear coll3 = new Collinear(pts3);

            List<Point> pts4 = new List<Point>();
            pts4.Add(v);
            pts4.Add(t);
            pts4.Add(r);
            Collinear coll4 = new Collinear(pts4);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(s, v)), GetProblemSegment(intrinsic, new Segment(t, v))));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(s, q)), GetProblemSegment(intrinsic, new Segment(t, q))));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(p, s, u)), GetProblemAngle(intrinsic, new Angle(r, t, w))));
        }
    }
}