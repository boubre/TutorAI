using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 80 Problem 9
    //
    public class BackwardPage80Problem9 : CongruentTrianglesProblem
    {
        public BackwardPage80Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 80 Problem 9";


            Point g = new Point("G", 0, 0); intrinsic.Add(g);
            Point h = new Point("H", 3, 0); intrinsic.Add(h);
            Point f = new Point("F", 10, 15); intrinsic.Add(f);
            Point i = new Point("I", 17, 0); intrinsic.Add(i);
            Point j = new Point("J", 20, 0); intrinsic.Add(j);

            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment fh = new Segment(f, h); intrinsic.Add(fh);
            Segment fi = new Segment(f, i); intrinsic.Add(fi);
            Segment fj = new Segment(f, j); intrinsic.Add(fj);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(i);
            pts.Add(j);
            Collinear coll1 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(fh, fi));
            given.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(h, g)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(i, j))));

            goals.Add(new GeometricCongruentSegments(fg, fj));

        }
    }
}