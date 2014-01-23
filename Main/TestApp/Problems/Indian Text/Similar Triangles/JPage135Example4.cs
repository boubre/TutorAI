using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public class JPage135Example4 : SimilarTrianglesProblem
    {
        public JPage135Example4(bool onoff) : base(onoff)
        {
            problemName = "Book J Page 135 Example 4";
            numberOfOriginalTextProblems = 1;

            Point p = new Point("P", 1, 7);       intrinsic.Add(p);
            Point q = new Point("Q", 0, 0);       intrinsic.Add(q);
            Point r = new Point("R", 11.5, 11.5); intrinsic.Add(r);
            Point s = new Point("S", 10, 1);      intrinsic.Add(s);
            Point o = new Point("O", 4.6, 4.6);   intrinsic.Add(o);

            System.Diagnostics.Debug.Write(new Segment(q, r).FindIntersection(new Segment(p, s)));

            Segment pq = new Segment(p, q); intrinsic.Add(pq);
            Segment rs = new Segment(r, s); intrinsic.Add(rs);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(o);
            pts.Add(s);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(o);
            pts.Add(r);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(pq, rs, "Given"));

            goals.Add(new GeometricSimilarTriangles(new Triangle(p, o, q), new Triangle(s, o, r), "Given"));
        }
    }
}