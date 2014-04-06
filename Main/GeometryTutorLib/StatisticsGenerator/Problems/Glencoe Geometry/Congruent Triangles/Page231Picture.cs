﻿using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 231 picture
    //
    public class Page231Picture : ParallelLinesProblem
    {
        public Page231Picture(bool onoff) : base(onoff)
        {
            problemName = "Page 231 Picture";

            Point f = new Point("F", 2, Math.Sqrt(12.0)); intrinsic.Add(f);
            Point g = new Point("G", 6, Math.Sqrt(12.0)); intrinsic.Add(g);   // Incorrect placement for problem scenario
            Point h = new Point("H", 4, 0); intrinsic.Add(h);
            Point j = new Point("J", 0, 0); intrinsic.Add(j);
            
            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment fh = new Segment(f, h); intrinsic.Add(fh);
            Segment fj = new Segment(f, j); intrinsic.Add(fj);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment hj = new Segment(h, j); intrinsic.Add(hj);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(fj, fh));
            given.Add(new GeometricCongruentSegments(fg, gh));
            given.Add(new GeometricCongruentSegments(fh, fg));

            goals.Add(new GeometricCongruentTriangles(new Triangle(j, f, h), new Triangle(f, g, h)));
        }
    }
}