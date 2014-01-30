using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 166 Problem 25
    //
    public class Page166Problem25 : ParallelLinesProblem
    {
        public Page166Problem25(bool onoff) : base(onoff)
        {
            problemName = "Page 166 Problem 25";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 2, 4); intrinsic.Add(b);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);
            Point d = new Point("D", 8, 4); intrinsic.Add(d);
            Point e = new Point("E", 1, 2); intrinsic.Add(e);
            Point f = new Point("F", 7, 2); intrinsic.Add(f);
            Point g = new Point("G", 0, 2); intrinsic.Add(g);
            Point h = new Point("H", 10, 2); intrinsic.Add(h);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(c);
            pts2.Add(f);
            pts2.Add(d);
            Collinear coll2 = new Collinear(pts2);

            List<Point> pts3 = new List<Point>();
            pts3.Add(g);
            pts3.Add(e);
            pts3.Add(f);
            pts3.Add(h);
            Collinear coll3 = new Collinear(pts3);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Supplementary(GetProblemAngle(intrinsic, new Angle(b, e, g)), GetProblemAngle(intrinsic, new Angle(e, f, c))));

            goals.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(a, b)), GetProblemSegment(intrinsic, new Segment(c, d))));
        }
    }
}