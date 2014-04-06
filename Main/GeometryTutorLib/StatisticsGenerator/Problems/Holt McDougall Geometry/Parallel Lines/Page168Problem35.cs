using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 168 Problem 35
    //
    public class Page168Problem35 : CongruentTrianglesProblem
    {
        public Page168Problem35(bool onoff) : base(onoff)
        {
            problemName = "Page 168 Problem 35";


            Point a = new Point("A", 1, 0); intrinsic.Add(a);
            Point b = new Point("B", 5, 0); intrinsic.Add(b);
            Point c = new Point("C", 9, 0); intrinsic.Add(c);
            Point d = new Point("D", 0, 2); intrinsic.Add(d);
            Point e = new Point("E", 4, 2); intrinsic.Add(e);
            Point f = new Point("F", 3, 4); intrinsic.Add(f);
            Point g = new Point("G", 7, 4); intrinsic.Add(g);
            Point h = new Point("H", 6, 6); intrinsic.Add(h);


            Point i = new Point("I", 0, 10); intrinsic.Add(i);
            Point j = new Point("J", 4, 10); intrinsic.Add(j);
            Point k = new Point("K", 12, 10); intrinsic.Add(k);
            Point l = new Point("L", 16, 10); intrinsic.Add(l);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(g);
            pts.Add(l);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(f);
            pts2.Add(h);
            pts2.Add(k);
            Collinear coll2 = new Collinear(pts2);

            List<Point> pts3 = new List<Point>();
            pts3.Add(b);
            pts3.Add(e);
            pts3.Add(f);
            pts3.Add(i);
            Collinear coll3 = new Collinear(pts3);

            List<Point> pts4 = new List<Point>();
            pts4.Add(c);
            pts4.Add(g);
            pts4.Add(h);
            pts4.Add(j);
            Collinear coll4 = new Collinear(pts4);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));
            
            given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(f, h, g)),
                                                   ClauseConstructor.GetProblemAngle(intrinsic, new Angle(f, e, g))));
            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, l)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(d, k))));

            goals.Add(new GeometricParallel(new Segment(b, i), new Segment(c, j)));
        }
    }
}