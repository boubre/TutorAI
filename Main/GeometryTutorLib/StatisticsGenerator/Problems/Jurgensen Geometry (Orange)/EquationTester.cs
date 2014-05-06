using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class EquationTester : TransversalsProblem
    {
        public EquationTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -2, 0); intrinsic.Add(a);
            Point m = new Point("M", 0, 0);  intrinsic.Add(m);
            Point b = new Point("B", 3, 0);  intrinsic.Add(b);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            Addition sum1 = new Addition(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, m)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(m, b)));
            GeometricSegmentEquation eq1 = new GeometricSegmentEquation(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)), sum1);

            Addition sum2 = new Addition(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(m, b)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, m)));
            GeometricSegmentEquation eq2 = new GeometricSegmentEquation(sum2, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)));

            System.Diagnostics.Debug.WriteLine(eq2.Equals(eq1));
        }
    }
}