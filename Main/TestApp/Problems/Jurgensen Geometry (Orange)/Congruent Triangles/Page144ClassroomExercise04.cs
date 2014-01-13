using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 144 CE #3
    //
    public class Page144ClassroomExercise04 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise04() : base()
        {
            Point d = new Point("D", 0, 12); intrinsic.Add(d);
            Point c = new Point("C", 9, 0); intrinsic.Add(c);
            Point p = new Point("P", 9, 12); intrinsic.Add(p);
            Point e = new Point("E", 14, 12); intrinsic.Add(e);
            Point q = new Point("Q", 19, 12); intrinsic.Add(q);
            Point g = new Point("G", 19, 24); intrinsic.Add(g);
            Point f = new Point("F", 28, 12); intrinsic.Add(f);

            Segment cd = new Segment(c, d); intrinsic.Add(cd);
            Segment cp = new Segment(c, p); intrinsic.Add(cp);
            Segment gq = new Segment(g, q); intrinsic.Add(gq);
            Segment fg = new Segment(f, g); intrinsic.Add(fg);
            
            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(g);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(p);
            pts.Add(e);
            pts.Add(q);
            pts.Add(f);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(cd, fg, "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(c, e)), GetProblemSegment(intrinsic, new Segment(e, g)), "Given"));
            given.Add(new RightAngle(c, p, e, "Given"));
            given.Add(new RightAngle(e, q, g, "Given"));
        }
    }
}