using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Book I Page 149 Exercise at the top of the page; it is not numbered.
    //
    public class IPage149TopExercise : ActualProblem
    {
        public IPage149TopExercise(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 149 Exercise at Top of Page";
            numberOfOriginalTextProblems = 1;

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
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(d);
            pts.Add(c);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(d);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new Midpoint(e, GetProblemSegment(intrinsic, new Segment(a, b)), "Given"));
            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(e, d)), bc, "Given"));
            given.Add(new GeometricParallel(GetProblemSegment(intrinsic, new Segment(c, m)), GetProblemSegment(intrinsic, new Segment(b, a)), "Given"));
        }
    }
}