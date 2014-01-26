using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 223 Problem 32
    //
    public class Page223Problem32 : CongruentTrianglesProblem
    {
        public Page223Problem32(bool onoff)
            : base(onoff)
        {
            problemName = "Page 223 Problem 32";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 6); intrinsic.Add(b);
            Point c = new Point("C", 6, 0); intrinsic.Add(c);
            Point d = new Point("D", 6, 12); intrinsic.Add(d);
            Point m = new Point("M", 2, 4); intrinsic.Add(m);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(b);
            pts2.Add(m);
            pts2.Add(c);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");
            
            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, ab), GetProblemSegment(intrinsic, cd), "Given"));
            given.Add(new RightAngle(b, a, c, "Given"));
            given.Add(new RightAngle(a, c, d, "Given"));
        }
    }
}