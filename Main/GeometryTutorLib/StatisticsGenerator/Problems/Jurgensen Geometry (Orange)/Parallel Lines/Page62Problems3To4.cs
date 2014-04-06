﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 62 Problems 3-4
    //
    // Parallel Transversals
    //
    public class Page62Problems3To4 : ParallelLinesProblem
    {
        public Page62Problems3To4(bool onoff) : base(onoff)
        {
            problemName = "Page 62 Problems 3-4";

            Point a = new Point("A", -1, 3); intrinsic.Add(a);
            Point b = new Point("B", 4, 3);  intrinsic.Add(b);
            Point c = new Point("C", 0, 0);  intrinsic.Add(c);
            Point d = new Point("D", 5, 0);  intrinsic.Add(d);

            Point e = new Point("E", -3, 3);  intrinsic.Add(e);
            Point f = new Point("F", -2, 6); intrinsic.Add(f);
            Point g = new Point("G", 3, 6);  intrinsic.Add(g);
            Point h = new Point("H", 8, 3);  intrinsic.Add(h);
            Point i = new Point("I", 10, 0); intrinsic.Add(i);
            Point j = new Point("J", 7, -6);  intrinsic.Add(j);
            Point k = new Point("K", 1, -3);  intrinsic.Add(k);
            Point l = new Point("L", -5, 0); intrinsic.Add(l);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(a);
            pts.Add(c);
            pts.Add(k);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(a);
            pts.Add(b);
            pts.Add(h);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(b);
            pts.Add(d);
            pts.Add(j);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(c);
            pts.Add(d);
            pts.Add(i);
            Collinear coll4 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, d))));
            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, c)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, d))));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, a, f))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, e))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, a))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, c, d))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, d, i))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, c, k))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, d, j))));

            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(f, a, e))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, b, a))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(c, a, b))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(d, b, h))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, c, a))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(b, d, c))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(k, c, d))));
            goals.Add(new Supplementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, b, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(j, d, i))));
        }
    }
}