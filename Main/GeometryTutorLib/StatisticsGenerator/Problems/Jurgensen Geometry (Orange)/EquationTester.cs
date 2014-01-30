using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class EquationTester : TransversalsProblem
    {
        public EquationTester(bool onoff)
            : base(onoff)
        {
            Point a = new Point("A", -2, 0); intrinsic.Add(a);
            Point m = new Point("M", 0, 0);  intrinsic.Add(m);
            Point b = new Point("B", 3, 0);  intrinsic.Add(b);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            Addition sum1 = new Addition(GetProblemSegment(intrinsic, new Segment(a, m)), GetProblemSegment(intrinsic, new Segment(m, b)));
            GeometricSegmentEquation eq1 = new GeometricSegmentEquation(GetProblemSegment(intrinsic, new Segment(a, b)), sum1);

            Addition sum2 = new Addition(GetProblemSegment(intrinsic, new Segment(m, b)), GetProblemSegment(intrinsic, new Segment(a, m)));
            GeometricSegmentEquation eq2 = new GeometricSegmentEquation(sum2, GetProblemSegment(intrinsic, new Segment(a, b)));

            System.Diagnostics.Debug.WriteLine(eq2.Equals(eq1));
        }
    }
}