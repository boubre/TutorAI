﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 284 example 4.6
    //
    public class Page284Example46 : CongruentTrianglesProblem
    {
        public Page284Example46(bool onoff) : base(onoff)
        {
            problemName = "Page 284 example 4.6";


            Point e = new Point("E", 1, 0); intrinsic.Add(e);
            Point f = new Point("F", 0, 8); intrinsic.Add(f);
            Point g = new Point("G", 5, 3); intrinsic.Add(g);
            Point h = new Point("H", 9, 6); intrinsic.Add(h);
            Point j = new Point("J", 10, -2); intrinsic.Add(j);

            Segment ef = new Segment(e, f); intrinsic.Add(ef);
            Segment hj = new Segment(h, j); intrinsic.Add(hj);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(g);
            pts.Add(j);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(e);
            pts2.Add(g);
            pts2.Add(h);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(g, f)), GetProblemSegment(intrinsic, new Segment(j, g)), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(e, g)), GetProblemSegment(intrinsic, new Segment(h, g)), "Given"));

            goals.Add(new GeometricCongruentSegments(ef, hj, "Given"));
        }
    }
}