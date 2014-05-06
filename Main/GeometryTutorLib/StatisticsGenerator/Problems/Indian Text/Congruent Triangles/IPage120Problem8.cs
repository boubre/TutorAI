using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book i Page 120 Problem 8
    //
    public class IPage120Problem8 : ActualProblem
    {
        public IPage120Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Overlapping Right Triangles";


            Point a = new Point("A", 0, 3); intrinsic.Add(a);
            Point m = new Point("M", 2, 1.5); intrinsic.Add(m);
            Point b = new Point("B", 4, 3); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);

            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            //given.Add(new Midpoint(m, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, m)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(m, c))));
            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, m, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, d)))));
            given.Add(new RightAngle(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, c, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(b, m, c), new Triangle(d, m, a)));
            goals.Add(new Strengthened(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, d, c)), new RightAngle(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, d, c)))));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, d, c), new Triangle(b, c, d)));

            Multiplication product = new Multiplication(new NumericValue(2), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, m)));
            goals.Add(new GeometricSegmentEquation(product, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, d))));
        }
    }
}