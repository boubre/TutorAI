using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 144 CE #1
    //
    public class Page144ClassroomExercise01: CongruentTrianglesProblem
    {
        public Page144ClassroomExercise01(bool onoff) : base(onoff)
        {
            problemName = "Page 144 Classroom Ex Problem 1";
            numberOfOriginalTextProblems = 1;

            Point a = new Point("A", 2, 6); intrinsic.Add(a);
            Point s = new Point("S", 3, 0); intrinsic.Add(s);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 5, 0); intrinsic.Add(c);

            Point d = new Point("D", 8, 7); intrinsic.Add(d);
            Point t = new Point("T", 9, 1); intrinsic.Add(t);
            Point e = new Point("E", 6, 1); intrinsic.Add(e);
            Point f = new Point("F", 11, 1); intrinsic.Add(f);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment a_s = new Segment(a, s); intrinsic.Add(a_s);    //couldn't name it "as" because "as" is a reserved word

            Segment de = new Segment(d, e); intrinsic.Add(de);
            Segment df = new Segment(d, f); intrinsic.Add(df);
            Segment dt = new Segment(d, t); intrinsic.Add(dt); 

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(s);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            List<Point> pts2 = new List<Point>();
            pts2.Add(e);
            pts2.Add(t);
            pts2.Add(f);
            Collinear coll2 = new Collinear(pts2, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ab), GetProblemSegment(intrinsic, de), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, ac), GetProblemSegment(intrinsic, df), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(s, c)), GetProblemSegment(intrinsic, new Segment(t, f)), "Given"));
            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, a, c)), GetProblemAngle(intrinsic, new Angle(e, d, f)), "Given"));

            goals.Add(new GeometricCongruentSegments(a_s, dt, "GOAL"));
        }
    }
}