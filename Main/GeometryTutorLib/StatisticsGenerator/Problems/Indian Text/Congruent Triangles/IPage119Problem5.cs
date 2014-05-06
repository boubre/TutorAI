using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book i Page 119 Problem 5
    //
    public class IPage119Problem5 : ActualProblem
    {
        public IPage119Problem5(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "I Page 119 Problem 5";

            Point a = new Point("A", 0, 0);  intrinsic.Add(a);
            Point b = new Point("B", 10, 0); intrinsic.Add(b);
            Point p = new Point("P", 8, -4); intrinsic.Add(p);
            Point q = new Point("Q", 8, 4);  intrinsic.Add(q);

            Point x = new Point("X", 16, 8);    intrinsic.Add(x);
            Point y = new Point("Y", 16, 0);   intrinsic.Add(y);
            Point z = new Point("Z", 24, -12);   intrinsic.Add(z);
            Point o = new Point("O", -100, 0); intrinsic.Add(o);

            Segment bq = new Segment(b, q); intrinsic.Add(bq);
            Segment bp = new Segment(b, p); intrinsic.Add(bp);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(a);
            pts.Add(b);
            pts.Add(y);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(q);
            pts.Add(x);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(z);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(q, a, p)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(o, y))));
            given.Add(new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(b, p), new Segment(a, z))));
            given.Add(new Perpendicular(ClauseConstructor.GetProblemIntersection(intrinsic, new Segment(b, q), new Segment(a, x))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, p, b), new Triangle(a, q, b)));
            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, p)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, q))));
        }
    }
}