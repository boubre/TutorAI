using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class JPage140Problem6 : SimilarTrianglesProblem
    {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\
            //
            //
        public JPage140Problem6(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 140 Problem 6";


            Point a = new Point("A", 2, 12);       intrinsic.Add(a);
            Point d = new Point("D", 1.5, 9);      intrinsic.Add(d);
            Point e = new Point("E", 2.5, 9);      intrinsic.Add(e);
            Point x = new Point("X", 2, 18 / 2.5); intrinsic.Add(x);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(x);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(x);
            pts.Add(b);
            Collinear coll4 = new Collinear(pts);


            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentTriangles(new Triangle(a, b, e), new Triangle(a, c, d)));

            goals.Add(new GeometricSimilarTriangles(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(a, d, e)), ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(a, b, c))));
        }
    }
}