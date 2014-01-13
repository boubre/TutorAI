using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 155 Problem 14
    //
    public class Backup : ActualProblem
    {
        public Backup() : base()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 3);
            Point c = new Point("C", 4, 0);
            Point d = new Point("D", 0, 0);

            Segment cd = new Segment(c, d);
            Segment ad = new Segment(a, d);
            Segment bc = new Segment(b, c);
            Segment bd = new Segment(b, d);
            Segment ac = new Segment(a, c);

            Segment am = new Segment(a, m);
            Segment mb = new Segment(m, b);
            Segment mc = new Segment(m, c);
            Segment md = new Segment(m, d);

            InMiddle im1 = new InMiddle(m, ac, "Intrinsic");
            InMiddle im2 = new InMiddle(m, bd, "Intrinsic");

            Triangle rightOne = new Triangle(ad, cd, ac, "Intrinsic");

            Triangle isoOne = new Triangle(am, md, ad, "Intrinsic");
            Triangle isoTwo = new Triangle(mb, mc, bc, "Intrinsic");

            Triangle bottomIso = new Triangle(mc, md, cd, "Intrinsic");

            Intersection inter = new Intersection(m, ac, bd, "Intrinsic");

            Intersection inter10 = new Intersection(m, am, bd, "Intrinsic");
            Intersection inter11 = new Intersection(m, mc, bd, "Intrinsic");
            Intersection inter12 = new Intersection(m, mb, ac, "Intrinsic");
            Intersection inter13 = new Intersection(m, md, ac, "Intrinsic");
            Intersection inter14 = new Intersection(m, am, mb, "Intrinsic");
            Intersection inter15 = new Intersection(m, mb, mc, "Intrinsic");
            Intersection inter16 = new Intersection(m, mc, md, "Intrinsic");
            Intersection inter17 = new Intersection(m, md, am, "Intrinsic");

            Intersection inter2 = new Intersection(a, ac, ad, "Intrinsic");
            Intersection inter3 = new Intersection(b, bc, bd, "Intrinsic");
            Intersection inter4 = new Intersection(c, bc, ac, "Intrinsic");
            Intersection inter5 = new Intersection(c, cd, bc, "Intrinsic");
            Intersection inter6 = new Intersection(c, cd, ac, "Intrinsic");

            Intersection inter7 = new Intersection(d, ad, cd, "Intrinsic");
            Intersection inter8 = new Intersection(d, ad, bd, "Intrinsic");
            Intersection inter9 = new Intersection(d, cd, bd, "Intrinsic");

            intrinsic.Add(a);
            intrinsic.Add(m);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(cd);
            intrinsic.Add(ad);
            intrinsic.Add(bc);
            intrinsic.Add(bd);
            intrinsic.Add(ac);
            intrinsic.Add(am);
            intrinsic.Add(mb);
            intrinsic.Add(mc);
            intrinsic.Add(md);
            intrinsic.Add(im1);
            intrinsic.Add(im2);
            intrinsic.Add(rightOne);
            intrinsic.Add(isoOne);
            intrinsic.Add(isoTwo);
            intrinsic.Add(bottomIso);
            intrinsic.Add(inter);
            intrinsic.Add(inter2);
            intrinsic.Add(inter3);
            intrinsic.Add(inter4);
            intrinsic.Add(inter5);
            intrinsic.Add(inter6);
            intrinsic.Add(inter7);
            intrinsic.Add(inter8);
            intrinsic.Add(inter9);
            intrinsic.Add(inter10);
            intrinsic.Add(inter11);
            intrinsic.Add(inter12);
            intrinsic.Add(inter13);
            intrinsic.Add(inter14);
            intrinsic.Add(inter15);
            intrinsic.Add(inter16);
            intrinsic.Add(inter17);

            given.Add(new Midpoint(m, ac, "Given"));
            given.Add(new Midpoint(m, bd, "Given"));
            given.Add(new RightTriangle(bc, cd, bd, "Given"));
        }
    }
}