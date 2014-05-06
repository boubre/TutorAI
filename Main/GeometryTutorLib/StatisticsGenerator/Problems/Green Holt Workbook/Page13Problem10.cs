using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 13 Problem 10
    //
    public class Page13Problem10 : TransversalsProblem
    {
        public Page13Problem10(bool onoff, bool complete) : base(onoff, complete)
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
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new AngleBisector(new Angle(h, k, j), ik));

            goals.Add(new Strengthened(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(i, k, j)), new RightAngle(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(i, k, j)))));
        }
    }
}