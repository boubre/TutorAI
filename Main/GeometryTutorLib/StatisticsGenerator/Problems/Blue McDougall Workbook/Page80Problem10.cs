using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 80 Problem 10
    //
    public class Page80Problem10 : CongruentTrianglesProblem
    {
        public Page80Problem10(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 80 Problem 10";


            Point a = new Point("A", 2, 7); intrinsic.Add(a);
            Point b = new Point("B", 8, 7); intrinsic.Add(b);
            Point c = new Point("C", 0, 0); intrinsic.Add(c);
            Point d = new Point("D", 10, 0); intrinsic.Add(d);
            Point e = new Point("E", 5, 4.375); intrinsic.Add(e);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(b);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, d)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, b, d))));
            given.Add(new GeometricCongruentSegments(ac, bd));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, a, b)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, b, a))));
        }
    }
}