using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 168 Problem 34
    //
    public class Page168Problem34 : ParallelLinesProblem
    {
        public Page168Problem34(bool onoff) : base(onoff)
        {
            problemName = "Page 168 Problem 34";


            Point a = new Point("A", 1, 7); intrinsic.Add(a);
            Point b = new Point("B", 1, 4); intrinsic.Add(b);
            Point c = new Point("C", 8, 0); intrinsic.Add(c);
            Point d = new Point("D", 8, 4); intrinsic.Add(d);
            Point e = new Point("E", 4, 4); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(b);
            pts2.Add(e);
            pts2.Add(d);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, a, e)),
                                                   GetProblemAngle(intrinsic, new Angle(a, e, b))));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, e, c)),
                                                    GetProblemAngle(intrinsic, new Angle(e, c, d))));

            goals.Add(new GeometricParallel(ab, cd));
        }
    }
}