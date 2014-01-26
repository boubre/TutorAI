using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 229 Problem 05
    //
    public class Page229Problem05 : CongruentTrianglesProblem
    {
        public Page229Problem05(bool onoff) : base(onoff)
        {
            problemName = "Page 229 Problem 05";


            Point a = new Point("A", 13.0 / 2.0, 3.0); intrinsic.Add(a);
            Point b = new Point("B", 0, 3); intrinsic.Add(b);
            Point c = new Point("C", 2, 0); intrinsic.Add(c);
            Point e = new Point("E", 13.0 / 3.0, 3); intrinsic.Add(e);
            Point f = new Point("F", 5, 2); intrinsic.Add(f);

            Segment bc = new Segment(c, b); intrinsic.Add(bc);
            Segment ef = new Segment(e, f); intrinsic.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(b);
            pts2.Add(e);
            pts2.Add(a);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
        }
    }
}