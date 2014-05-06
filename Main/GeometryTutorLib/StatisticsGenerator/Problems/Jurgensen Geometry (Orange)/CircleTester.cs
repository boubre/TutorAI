using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class CircleTester : ActualProblem
    {
        //
        // A  _______M________ B
        //   |                |
        //   |                |
        //   | Right Triangle |
        // N |        O       | P
        //   |                |
        //   |                |
        //D  |________________| C
        //
        public CircleTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 4); intrinsic.Add(a);
            Point m = new Point("M", 2, 4);  intrinsic.Add(m);
            Point b = new Point("B", 4, 4);  intrinsic.Add(b);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point o = new Point("O", 2, 2); intrinsic.Add(o);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point n = new Point("N", 0, 2); intrinsic.Add(n);
            Point p = new Point("P", 4, 2); intrinsic.Add(p);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(n);
            pts.Add(d);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(o);
            pts.Add(p);
            Collinear coll4 = new Collinear(pts);

            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment mp = new Segment(m, p); intrinsic.Add(mp);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            // Add circles last;
            Circle circ = new Circle(o, 2.0);
            intrinsic.Add(circ);

            given.Add(new GeometricCongruentSegments(mn, mp));

            goals.Add(new GeometricCongruentSegments(cd, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(p, n))));
        }
    }
}