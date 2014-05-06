using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 155 Problem 14
    //
    public class Page155Problem14 : CongruentTrianglesProblem
    {
        public Page155Problem14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 155 Problem 14";


            Point r = new Point("R", 2, 5); intrinsic.Add(r);
            Point s = new Point("S", 0, 0); intrinsic.Add(s);
            Point t = new Point("T", 3.2, 2); intrinsic.Add(t);
            Point z = new Point("Z", 0.8, 2); intrinsic.Add(z);
            Point w = new Point("W", 4, 0); intrinsic.Add(w);

            Point x = new Point("X", 2, 1.25); intrinsic.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(z);
            pts.Add(s);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(t);
            pts.Add(w);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(z);
            pts.Add(x);
            pts.Add(w);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(t);
            pts.Add(x);
            pts.Add(s);
            Collinear coll4 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, z)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(r, t))));

            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(z, s)),
                                                     ClauseConstructor.GetProblemSegment(intrinsic, new Segment(t, w))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(r, s, t), new Triangle(r, w, z)));
        }
    }
}