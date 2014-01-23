using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 79 Problem 8
    //
    public class Page79Problem8 : SimilarTrianglesProblem
    {
        public Page79Problem8(bool onoff) : base(onoff)
        {
            problemName = "Page 79 Problem 8";
            numberOfOriginalTextProblems = 1;

            Point k = new Point("K", 3, 12);  intrinsic.Add(k);
            Point l = new Point("L", 1, 4);  intrinsic.Add(l);
            Point m = new Point("M", 9, 4);  intrinsic.Add(m);
            Point p = new Point("P", 4, 0);  intrinsic.Add(p);
            Point a = new Point("A", 0, 0);  intrinsic.Add(a);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment km = new Segment(k, m); intrinsic.Add(km);
            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment lp = new Segment(l, p); intrinsic.Add(lp);
            Segment ap = new Segment(a, p); intrinsic.Add(ap);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(l);
            pts.Add(a);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(km, lp, "Given"));
            given.Add(new GeometricParallel(lm, ap, "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(k, m, l), new Triangle(l, p, a), "GOAL"));
        }
    }
}