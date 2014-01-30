using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book I: Page 123 Example 6
    //
    public class IPage123Example6 : CongruentTrianglesProblem
    {
        public IPage123Example6(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 123 Example 6";


            Point a = new Point("A", 3.5, 4); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 7, 0); intrinsic.Add(c);
            Point d = new Point("D", 2, 0); intrinsic.Add(d);
            Point e = new Point("E", 5, 0); intrinsic.Add(e);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);
            Segment ae = new Segment(a, e); intrinsic.Add(ae);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(e);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new IsoscelesTriangle(GetProblemTriangle(intrinsic, new Triangle(a, b, c))));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(b, e)), GetProblemSegment(intrinsic, new Segment(c, d))));

            goals.Add(new GeometricCongruentSegments(ad, ae));
        }
    }
}