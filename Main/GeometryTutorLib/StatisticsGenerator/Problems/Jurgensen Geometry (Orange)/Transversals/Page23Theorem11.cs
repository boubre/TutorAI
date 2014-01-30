using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Midpoint Theorem
    //
    public class Page23Theorem11 : TransversalsProblem
    {
        public Page23Theorem11(bool onoff) : base(onoff)
        {
            problemName = "Midpoint Theorem";


            Point a = new Point("A", -3, 0);   intrinsic.Add(a);
            Point m = new Point("M", 0, 0);  intrinsic.Add(m);
            Point b = new Point("B", 3, 0);   intrinsic.Add(b);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(GetProblemInMiddle(intrinsic, m, GetProblemSegment(intrinsic, new Segment(a, b)))));

            Multiplication product1 = new Multiplication(new NumericValue(2), GetProblemSegment(intrinsic, new Segment(a, m)));
            goals.Add(new GeometricSegmentEquation(product1, GetProblemSegment(intrinsic, new Segment(a, b))));

            Multiplication product2 = new Multiplication(new NumericValue(2), GetProblemSegment(intrinsic, new Segment(m, b)));
            goals.Add(new GeometricSegmentEquation(product2, GetProblemSegment(intrinsic, new Segment(a, b))));
        }
    }
}