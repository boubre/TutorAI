using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 144 CE #2
    //
    public class Page144ClassroomExercise02 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise02(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Classroom Ex 2";


            Point p = new Point("P", 0, 6); intrinsic.Add(p);
            Point x = new Point("X", 1, 4); intrinsic.Add(x);
            Point l = new Point("L", 2, 2); intrinsic.Add(l);

            Point a = new Point("A", 6, 4); intrinsic.Add(a);

            Point n = new Point("N", 10, 6); intrinsic.Add(n);
            Point y = new Point("Y", 11, 4); intrinsic.Add(y);
            Point k = new Point("K", 12, 2); intrinsic.Add(k);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(x);
            pts.Add(l);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(n);
            pts2.Add(y);
            pts2.Add(k);
            Collinear coll2 = new Collinear(pts2);

            List<Point> pts3 = new List<Point>();
            pts3.Add(p);
            pts3.Add(a);
            pts3.Add(k);
            Collinear coll3 = new Collinear(pts3);

            List<Point> pts4 = new List<Point>();
            pts4.Add(x);
            pts4.Add(a);
            pts4.Add(y);
            Collinear coll4 = new Collinear(pts4);

            List<Point> pts5 = new List<Point>();
            pts5.Add(n);
            pts5.Add(a);
            pts5.Add(l);
            Collinear coll5 = new Collinear(pts5);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll5));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(l, a)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, n))));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(p, a)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, k))));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, x)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, y))));
        }
    }
}