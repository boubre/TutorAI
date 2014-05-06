using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book I: Page 128 Problem 1
    //
    public class IPage128Problem01 : CongruentTrianglesProblem
    {
        public IPage128Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 128 Problem 1";

            Point a = new Point("A", 2, 7); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point d = new Point("D", 2, 3); intrinsic.Add(d);
            Point p = new Point("P", 2, 0); intrinsic.Add(p);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment bd = new Segment(b, d); intrinsic.Add(bd);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(p);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new IsoscelesTriangle(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(a, b, c))));
            given.Add(new IsoscelesTriangle(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(d, b, c))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(a, c, d)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, p), new Triangle(a, c, p)));
            goals.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, p))));
            goals.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, d, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, p))));
            goals.Add(new PerpendicularBisector(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(a, p), new Segment(b, c)),
                                                ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, p))));
        }
    }
}