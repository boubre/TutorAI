using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 13 Problem 10
    //
    public class Page13Problem10 : TransversalsProblem
    {
        public Page13Problem10(bool onoff) : base(onoff)
        {
            problemName = "Page 13 Problem 10";


            Point h = new Point("H", 4, 0); intrinsic.Add(h);
            Point i = new Point("I", 0, 5); intrinsic.Add(i);
            Point j = new Point("J", -4, 0); intrinsic.Add(j);
            Point k = new Point("K", 0, 0); intrinsic.Add(k);

            Segment ik = new Segment(i, k); intrinsic.Add(ik);

            List<Point> pts = new List<Point>();
            pts.Add(h);
            pts.Add(k);
            pts.Add(j);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new AngleBisector(new Angle(h, k, j), ik, "Given"));

            goals.Add(new Strengthened(GetProblemAngle(intrinsic, new Angle(i, k, j)), new RightAngle(GetProblemAngle(intrinsic, new Angle(i, k, j)), "GOAL"), "GOAL"));
        }
    }
}