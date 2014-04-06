using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 243 Problem 16
    //
    public class Page243Problem16 : CongruentTrianglesProblem
    {
        public Page243Problem16(bool onoff) : base(onoff)
        {
            problemName = "Page 243 Problem 16";


            Point a = new Point("A", 1.6, 1.2); intrinsic.Add(a);
            Point b = new Point("B", 2, 6); intrinsic.Add(b);
            Point c = new Point("C", 17, 6); intrinsic.Add(c);
            Point d = new Point("D", 17.6, 13.2); intrinsic.Add(d);
            Point x = new Point("X", 8, 6); intrinsic.Add(x);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            List<Point> pts2 = new List<Point>();
            pts2.Add(a);
            pts2.Add(x);
            pts2.Add(d);
            Collinear coll2 = new Collinear(pts2);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));
        }
    }
}