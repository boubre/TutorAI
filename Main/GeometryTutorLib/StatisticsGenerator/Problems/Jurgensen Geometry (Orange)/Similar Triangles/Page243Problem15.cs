using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 243 Problem 15
    //
    public class Page243Problem15 : CongruentTrianglesProblem
    {
        public Page243Problem15(bool onoff) : base(onoff)
        {
            problemName = "Page 243 Problem 15";


            Point d = new Point("D", 4, 24); intrinsic.Add(d);
            Point f = new Point("F", 3, 18); intrinsic.Add(f);
            Point h = new Point("H", 1, 6); intrinsic.Add(h);
            Point e = new Point("E", 5, 24); intrinsic.Add(e);
            Point g = new Point("G", 8, 18); intrinsic.Add(g);
            Point j = new Point("J", 14, 6); intrinsic.Add(j);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment hj = new Segment(h, j); intrinsic.Add(hj);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(f);
            pts.Add(h);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(e);
            pts2.Add(g);
            pts2.Add(j);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, de), GetProblemSegment(intrinsic, fg), "Given"));
            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, hj), GetProblemSegment(intrinsic, fg), "Given"));
        }
    }
}