using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 160 Problem 43
    //
    public class Page160Problem43 : ParallelLinesProblem
    {
        public Page160Problem43(bool onoff) : base(onoff)
        {
            problemName = "Page 160 Problem 43";
            numberOfOriginalTextProblems = 1;

            Point t = new Point("T", 15.0 - 5.0 * Math.Sqrt(3.0), 0); intrinsic.Add(t);
            Point s = new Point("S", 10, 0); intrinsic.Add(s);
            Point f = new Point("F", 15, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(f); // This is the wrong coordinate pair......
            Point e = new Point("E", 15, 5.0 * Math.Sqrt(3.0)); intrinsic.Add(e);
            Point r = new Point("R", 20, 0); intrinsic.Add(r);

            Segment er = new Segment(e, r); intrinsic.Add(er);
            Segment fs = new Segment(f, s); intrinsic.Add(fs);
            Segment se = new Segment(s, e); intrinsic.Add(se);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(r);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(t);
            pts2.Add(f);
            pts2.Add(e);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(s, e, r)), GetProblemAngle(intrinsic, new Angle(s, r, e)), "Given"));
            given.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(r, s, f)), se, "Given"));

            //goals.Add(new GeometricAngleEquation(GetProblemAngle(intrinsic, new Angle(f, s, t)), new NumericValue(60), "GOAL")); 
        }
    }
}