using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class JPage140Problem7 : SimilarTrianglesProblem
    {
        public JPage140Problem7(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 140 Problem 7";


            Point a = new Point("A", 0, 0);    intrinsic.Add(a);
            Point b = new Point("B", 18, 0);   intrinsic.Add(b);
            Point c = new Point("C", 2, 8);   intrinsic.Add(c);
            Point d = new Point("D", 3.6, 7.2);  intrinsic.Add(d);
            Point e = new Point("E", 2, 0);    intrinsic.Add(e);
            Point p = new Point("P", 2, 4); intrinsic.Add(p);

            Segment ac = new Segment(a, c); intrinsic.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(p);
            pts.Add(a);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(p);
            pts.Add(e);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll4 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Altitude(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(a, b, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, d))));
            given.Add(new Altitude(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(a, b, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, e))));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(c, d, p)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, d), new Triangle(c, b, e)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(a, d, b)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(p, d, c), new Triangle(b, e, c)));
        }
    }
}