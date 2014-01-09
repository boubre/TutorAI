using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    // Isosceles Triangle Figure
    //
    //      A
    //     /|\
    //    / | \
    //   /  |  \
    //  /___|___\
    //  B   M    C
    //
    public class Page124Figure31 : CongruentTrianglesProblem
    {
        public Page124Figure31() : base()
        {
            Point a = new Point("A", 2, 6); intrinsic.Add(a);
            Point m = new Point("M", 2, 0); intrinsic.Add(m);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", 4, 0); intrinsic.Add(c);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment ac = new Segment(a, c); intrinsic.Add(ac);
            Segment am = new Segment(a, m); intrinsic.Add(am);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new IsoscelesTriangle(ac, ab, GetProblemSegment(intrinsic, new Segment(b, c)), "Given"));
            given.Add(new AngleBisector(GetProblemAngle(intrinsic, new Angle(b, a, c)), am, "Given"));

            //Segment ab = new Segment(a, b);
            //Segment am = new Segment(a, m);
            //Segment ac = new Segment(a, c);
            //Segment bm = new Segment(b, m);
            //Segment mc = new Segment(m, c);

            //Segment bc = new Segment(b, c);

            //Angle bac = new Angle(b, a, c);
            //Angle acb = new Angle(a, c, b);
            //Angle cba = new Angle(c, b, a);

            //Angle amb = new Angle(a, m, b);
            //Angle amc = new Angle(a, m, c);

            //Triangle left = new Triangle(ab, bm, am, "Intrinsic");
            //Triangle right = new Triangle(ac, mc, am, "Intrinsic");

            //Triangle actIso = new Triangle(ac, ab, bc, "Intrinsic");

            //Intersection inter1 = new Intersection(a, ab, ac, "Intrinsic");
            //Intersection inter2 = new Intersection(a, ab, am, "Intrinsic");
            //Intersection inter3 = new Intersection(a, am, ac, "Intrinsic");

            //Intersection inter4 = new Intersection(m, am, bc, "Intrinsic");
            //Intersection inter5 = new Intersection(m, bm, mc, "Intrinsic");

            //Intersection inter6 = new Intersection(b, ab, bm, "Intrinsic");
            //Intersection inter7 = new Intersection(b, ab, bc, "Intrinsic");

            //Intersection inter8 = new Intersection(c, ac, mc, "Intrinsic");
            //Intersection inter9 = new Intersection(c, ac, bc, "Intrinsic");

            //intrinsic.Add(a);
            //intrinsic.Add(m);
            //intrinsic.Add(b);
            //intrinsic.Add(c);

            //intrinsic.Add(ab);
            //intrinsic.Add(am);
            //intrinsic.Add(ac);
            //intrinsic.Add(bm);
            //intrinsic.Add(mc);
            //intrinsic.Add(bc);

            //intrinsic.Add(bac);
            //intrinsic.Add(acb);
            //intrinsic.Add(cba);
            //intrinsic.Add(amb);
            //intrinsic.Add(amc);

            //intrinsic.Add(left);
            //intrinsic.Add(right);
            //intrinsic.Add(actIso);

            //intrinsic.Add(inter1);
            //intrinsic.Add(inter2);
            //intrinsic.Add(inter3);
            //intrinsic.Add(inter4);
            //intrinsic.Add(inter5);
            //intrinsic.Add(inter6);
            //intrinsic.Add(inter7);
            //intrinsic.Add(inter8);
            //intrinsic.Add(inter9);
        }
    }
}