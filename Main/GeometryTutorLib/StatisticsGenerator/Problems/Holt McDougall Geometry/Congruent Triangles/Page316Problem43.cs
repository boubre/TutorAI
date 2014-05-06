using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 316 problem 43
    //
    public class Page316Problem43 : CongruentTrianglesProblem
    {
        public Page316Problem43(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 316 Problem 43";


            Point f = new Point("F", 0, 5); intrinsic.Add(f);
            Point g = new Point("G", 10, 0); intrinsic.Add(g);
            Point h = new Point("H", 0, -5); intrinsic.Add(h);
            Point j = new Point("J", 5, 0); intrinsic.Add(j);

            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment fj = new Segment(f, j); intrinsic.Add(fj);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment gj = new Segment(g, j); intrinsic.Add(gj);
            Segment hj = new Segment(h, j); intrinsic.Add(hj);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new GeometricCongruentSegments(fj, hj));
            given.Add(new GeometricCongruentSegments(fg, gh));

            goals.Add(new AngleBisector(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(f, g, h)), gj));
        }
    }
}