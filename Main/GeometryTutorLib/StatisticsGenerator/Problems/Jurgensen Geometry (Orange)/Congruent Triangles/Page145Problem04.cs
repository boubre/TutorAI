using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 145 Problem 4
    //
    public class Page145Problem04 : CongruentTrianglesProblem
    {
        public Page145Problem04(bool onoff) : base(onoff)
        {
            problemName = "Page 145 Problem 4";


            Point a = new Point("A", 0, 0); intrinsic.Add(a);
            Point b = new Point("B", 5, 0); intrinsic.Add(b);
            Point d = new Point("D", 1, 4); intrinsic.Add(d);
            Point c = new Point("C", 6, 4); intrinsic.Add(c);
            Point l = new Point("L", 2, 3); intrinsic.Add(l);
            Point m = new Point("M", 4, 1); intrinsic.Add(m);
            
            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment al = new Segment(a, l); intrinsic.Add(al);
            Segment cm = new Segment(c, m); intrinsic.Add(cm);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(l);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(ab, cd));
			given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, d, l)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(m, b, a))));
			given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, a, l)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(m, c, b))));

            goals.Add(new GeometricCongruentSegments(al, cm));
        }
    }
}