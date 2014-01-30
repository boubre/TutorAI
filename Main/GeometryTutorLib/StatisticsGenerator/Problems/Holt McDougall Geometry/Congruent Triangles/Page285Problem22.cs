using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 285 problem 22
    //
    public class Page285Problem22 : CongruentTrianglesProblem
    {
        public Page285Problem22(bool onoff) : base(onoff)
        {
            problemName = "Page 285 Problem 22";


            Point f = new Point("F", 0, 5); intrinsic.Add(f);
            Point g = new Point("G", 2.4, 1.8); intrinsic.Add(g);
            Point h = new Point("H", 0, 0); intrinsic.Add(h);
            Point k = new Point("K", -2.4, 1.8); intrinsic.Add(k);
            
            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            Segment fh = new Segment(f, h); intrinsic.Add(fh);
            Segment fk = new Segment(f, k); intrinsic.Add(fk);
            Segment gh = new Segment(g, h); intrinsic.Add(gh);
            Segment hk = new Segment(h, k); intrinsic.Add(hk);

            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(hk, gh));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(f, k, h))));
            given.Add(new RightAngle(GetProblemAngle(intrinsic, new Angle(f, g, h))));

            goals.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(k, f, h)), GetProblemAngle(intrinsic, new Angle(g, f, h))));
        }
    }
}