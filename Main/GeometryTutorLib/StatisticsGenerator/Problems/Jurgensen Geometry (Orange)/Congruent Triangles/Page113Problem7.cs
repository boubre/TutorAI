using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 113 Problem 7
    //
    public class Page113Problem7 : CongruentTrianglesProblem
    {
        public Page113Problem7(bool onoff) : base(onoff)
        {
            problemName = "Page 113 Problem 7";


            Point a = new Point("A", -4, 3);          intrinsic.Add(a);
            Point b = new Point("B", -4.0 / 3.0, -1); intrinsic.Add(b);
            Point c = new Point("C", 0, 0);           intrinsic.Add(c);
            Point p = new Point("P", 4, 3);           intrinsic.Add(p);
            Point q = new Point("Q", 4.0 / 3.0, -1);  intrinsic.Add(q);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment pq = new Segment(p, q); intrinsic.Add(pq);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(q);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(c);
            pts.Add(b);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, q))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, p))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(p, q, c)));
        }
    }
}