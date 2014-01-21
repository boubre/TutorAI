using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 159 Problem 37
    //
    public class Page159Problem37 : ParallelLinesProblem
    {
        public Page159Problem37(bool onoff) : base(onoff)
        {
            problemName = "Page 159 Problem 37";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 1, 5); intrinsic.Add(a);
            Point b = new Point("B", 2, 4); intrinsic.Add(b);
            Point c = new Point("C", 0, 3); intrinsic.Add(c);
            Point d = new Point("D", 0, 0); intrinsic.Add(d);
            Point e = new Point("E", 3, 9); intrinsic.Add(e);
            Point f = new Point("F", 4.5, 9); intrinsic.Add(f);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(a);
            pts.Add(e);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(d);
            pts2.Add(b);
            pts2.Add(f);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));
        }
    }
}