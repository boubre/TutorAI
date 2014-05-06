using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 90 Problem 23
    //
    public class Page90Problem23 : CongruentTrianglesProblem
    {
        public Page90Problem23(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 90 Problem 23";


            Point e = new Point("E", 0, 0); intrinsic.Add(e);
            Point f = new Point("F", 4, -3); intrinsic.Add(f);
            Point g = new Point("G", 4, 3); intrinsic.Add(g);
            Point h = new Point("H", 7, 0); intrinsic.Add(h);
            Point j = new Point("J", 4, 0); intrinsic.Add(j);

            Segment eg = new Segment(e, g); intrinsic.Add(eg);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment hf = new Segment(h, f); intrinsic.Add(hf);
            Segment fe = new Segment(f, e); intrinsic.Add(fe);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(j);
            pts.Add(h);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(j);
            pts.Add(f);
            Collinear coll2 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentTriangles(new Triangle(g, h, j), new Triangle(f, h, j)));

            goals.Add(new GeometricCongruentSegments(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(e, f)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(e, g))));
        }
    }
}