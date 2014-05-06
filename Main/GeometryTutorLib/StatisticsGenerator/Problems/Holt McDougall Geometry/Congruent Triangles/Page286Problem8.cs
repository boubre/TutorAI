using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 286 problem 8
    //
    public class Page286Problem8 : CongruentTrianglesProblem
    {
        public Page286Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 286 Problem 8";


            Point a = new Point("A", 0, 8); intrinsic.Add(a);
            Point b = new Point("B", 8, 8); intrinsic.Add(b);
            Point c = new Point("C", 4, 4); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point e = new Point("E", 8, 0); intrinsic.Add(e);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment de = new Segment(d, e); intrinsic.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(c);
            pts2.Add(b);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, e))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, b))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(e, d, c)));
        }
    }
}