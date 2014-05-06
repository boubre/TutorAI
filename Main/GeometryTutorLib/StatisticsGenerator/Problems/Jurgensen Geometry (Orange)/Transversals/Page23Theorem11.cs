using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Midpoint Theorem
    //
    public class Page23Theorem11 : TransversalsProblem
    {
        public Page23Theorem11(bool onoff, bool complete) : base(onoff, complete)
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

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, m, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)))));

            Multiplication product1 = new Multiplication(new NumericValue(2), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, m)));
            goals.Add(new GeometricSegmentEquation(product1, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b))));

            Multiplication product2 = new Multiplication(new NumericValue(2), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(m, b)));
            goals.Add(new GeometricSegmentEquation(product2, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b))));
        }
    }
}