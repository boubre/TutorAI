using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 197 Problem 35
    //
    public class Page197Problem35 : TransversalsProblem
    {
        public Page197Problem35(bool onoff) : base(onoff)
        {
            problemName = "Page 197 Problem 35";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 0, 5); intrinsic.Add(a);
            Point b = new Point("B", 3, 8); intrinsic.Add(b);
            Point c = new Point("C", 5, 0); intrinsic.Add(c);
            Point d = new Point("D", 9000, 5); intrinsic.Add(d);
            Point e = new Point("E", 0, 0); intrinsic.Add(e);
            Point f = new Point("F", 0, 30); intrinsic.Add(f);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment ad = new Segment(a, d); intrinsic.Add(ad);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(a);
            pts.Add(f);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(f, a, b)),
                                                   GetProblemAngle(intrinsic, new Angle(d, a, b)), "Given"));
            
            goals.Add(new Strengthened(GetProblemIntersection(intrinsic, new Segment(a, b), new Segment(a, c)),
                                       new Perpendicular(GetProblemIntersection(intrinsic, new Segment(a, b), new Segment(a, c)), "GOAL"),"GOAL"));
        }
    }
}