using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book i Page 119 Problem 3
    //
    public class IPage119Problem3 : ActualProblem
    {
        public IPage119Problem3(bool onoff) : base(onoff)
        {
            problemName = "I Page 119 Problem 3";

            Point a = new Point("A", 4, 0); intrinsic.Add(a);
            Point b = new Point("B", 4, 10); intrinsic.Add(b);
            Point c = new Point("C", 8, 10); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point o = new Point("O", 4, 5); intrinsic.Add(o);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(o);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(ad, bc));
            given.Add(new Perpendicular(GetProblemIntersection(intrinsic, ad, new Segment(a, b))));
            given.Add(new Perpendicular(GetProblemIntersection(intrinsic, bc, new Segment(a, b))));

            goals.Add(new SegmentBisector(GetProblemIntersection(intrinsic, GetProblemSegment(intrinsic, new Segment(c, d)), GetProblemSegment(intrinsic, new Segment(a, b))), GetProblemSegment(intrinsic, new Segment(c, d))));
        }
    }
}