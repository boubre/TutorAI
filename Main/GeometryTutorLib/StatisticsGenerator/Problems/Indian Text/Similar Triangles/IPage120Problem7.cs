using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class IPage120Problem7 : CongruentTrianglesProblem
    {
        public IPage120Problem7(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 120 Problem 7";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 10, 0); intrinsic.Add(b);
            Point p = new Point("P", 5, 0); intrinsic.Add(p);
            Point d = new Point("D", 7, 8); intrinsic.Add(d);
            Point e = new Point("E", 3, 8); intrinsic.Add(e);

//            System.Diagnostics.Debug.Write(new Segment(q, r).FindIntersection(new Segment(p, s)));

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment be = new Segment(b, e); intrinsic.Add(be);
            Segment ep = new Segment(e, p); intrinsic.Add(ep);
            Segment dp = new Segment(d, p); intrinsic.Add(dp);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(GetProblemInMiddle(intrinsic, p, GetProblemSegment(intrinsic, new Segment(a, b)))));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, a, d)), GetProblemAngle(intrinsic, new Angle(a, b, e))));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(e, p, a)), GetProblemAngle(intrinsic, new Angle(d, p, b))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(d, a, p), new Triangle(e, b, p)));
            goals.Add(new GeometricCongruentSegments(ad, be));
        }
    }
}