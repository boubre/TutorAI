using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public class IPage120Problem6 : CongruentTrianglesProblem
    {
        public IPage120Problem6(bool onoff) : base(onoff)
        {
            problemName = "Book I Page 120 Problem 6";
            numberOfOriginalTextProblems = 1;

            //Point a = new Point("A", 2.5, 0); intrinsic.Add(a);
            //Point b = new Point("B", 0, 0); intrinsic.Add(b);
            //Point c = new Point("C", 6, 0); intrinsic.Add(c);
            //Point d = new Point("D", 5, 0); intrinsic.Add(d);
            //Point e = new Point("E", 7, 0); intrinsic.Add(e);

            //System.Diagnostics.Debug.Write(new Segment(q, r).FindIntersection(new Segment(p, s)));

            //Segment ab = new Segment(a, b); intrinsic.Add(ab);
            //Segment ad = new Segment(a, d); intrinsic.Add(ad);
            //Segment ac = new Segment(a, c); intrinsic.Add(ac);
            //Segment ae = new Segment(a, e); intrinsic.Add(ae);
            //Segment ec = new Segment(e, c); intrinsic.Add(ec);
            //Segment de = new Segment(d, e); intrinsic.Add(de);

            //List<Point> pts = new List<Point>();
            //pts.Add(b);
            //pts.Add(d);
            //pts.Add(c);
            //Collinear coll1 = new Collinear(pts, "Intrinsic");

            //intrinsic.AddRange(GenerateSegmentClauses(coll1));
            //intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            //given.Add(new GeometricCongruentSegments(ac, ae, "Given"));
            //given.Add(new GeometricCongruentSegments(ab, ad, "Given"));
            //given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(b, a, d)), GetProblemAngle(intrinsic, new Angle(e, a, c)), "Given"));

            //goals.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(b, c)), de, "GOAL"));
        }
    }
}