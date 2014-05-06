﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Book I Page 149 Exercise at the top of the page; it is not numbered.
    //
    public class IPage149TopExercise : ActualProblem
    {
        public IPage149TopExercise(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 149 Exercise at Top of Page";


            Point a = new Point("A", 2, 4); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);
            Point d = new Point("D", 5, 2); intrinsic.Add(d);
            Point e = new Point("E", 1, 2); intrinsic.Add(e);
            Point f = new Point("F", 3, 2); intrinsic.Add(f);
            Point m = new Point("M", 7, 6); intrinsic.Add(m);

            Segment bc = new Segment(b, c); intrinsic.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(d);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(d);
            Collinear coll4 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll4));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Midpoint(ClauseConstructor.GetProblemInMiddle(intrinsic, e, ClauseConstructor.GetProblemSegment(intrinsic, new Segment(a, b)))));
            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(e, d)), bc));
            given.Add(new GeometricParallel(ClauseConstructor.GetProblemSegment(intrinsic, new Segment(c, m)), ClauseConstructor.GetProblemSegment(intrinsic, new Segment(b, a))));
        }
    }
}