using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 17 Problem 9
    //
    public class Page17Problem9 : TransversalsProblem
    {
        public Page17Problem9(bool onoff) : base(onoff)
        {
            problemName = "Page 17 Problem 9";


            Point h = new Point("H", 0, 0); intrinsic.Add(h);
            Point i = new Point("I", 2, 0); intrinsic.Add(i);
            Point j = new Point("J", 1.5, 2); intrinsic.Add(j);
            Point k = new Point("K", 4.5, 2); intrinsic.Add(k);

            Point x = new Point("X", 1, 0); intrinsic.Add(x);

            Point a = new Point("A", 3, 8); intrinsic.Add(a);
            Point m = new Point("M", 4, 0); intrinsic.Add(m);
            Point n = new Point("N", 5, 0); intrinsic.Add(n);
            Point p = new Point("P", 6, 0); intrinsic.Add(p);

            Segment jk = new Segment(j, k); intrinsic.Add(jk);

            List<Point> pts = new List<Point>();
            pts.Add(h);
            pts.Add(x);
            pts.Add(i);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(j);
            pts.Add(a);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(k);
            pts.Add(n);
            Collinear coll3 = new Collinear(pts, "Intrinsic");
            
            pts = new List<Point>();
            pts.Add(m);
            pts.Add(n);
            pts.Add(p);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Supplementary(GetProblemAngle(intrinsic, new Angle(h, x, j)), GetProblemAngle(intrinsic, new Angle(a, j, k)), "Given"));

            goals.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(h, i)), GetProblemSegment(intrinsic, new Segment(j, k)), "GOAL"));
        }
    }
}