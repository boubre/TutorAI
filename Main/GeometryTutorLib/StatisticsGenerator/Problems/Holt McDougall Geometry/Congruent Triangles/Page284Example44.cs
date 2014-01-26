using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 284 Example 4.4
    //
    public class Page284Example44 : CongruentTrianglesProblem
    {
        public Page284Example44(bool onoff) : base(onoff)
        {
            problemName = "Page 284 Example 4.4";


            Point d = new Point("D", 0, 4); intrinsic.Add(d);
            Point e = new Point("E", 3, 0); intrinsic.Add(e);
            Point f = new Point("F", 6, 0); intrinsic.Add(f);
            Point g = new Point("G", 12, 4); intrinsic.Add(g);
            Point h = new Point("H", 9, 0); intrinsic.Add(h);

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment df = new Segment(d, f); intrinsic.Add(df);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment fg = new Segment(f, g); intrinsic.Add(fg);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(h);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(f, e)), GetProblemSegment(intrinsic, new Segment(f, h)), "Given"));
            given.Add(new GeometricCongruentSegments(de, gh, "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(d, e, f)), GetProblemAngle(intrinsic, new Angle(g, h, f)), "Given"));

            goals.Add(new GeometricCongruentTriangles(new Triangle(d, e, f), new Triangle(g, h, f), "GOAL"));
        }
    }
}