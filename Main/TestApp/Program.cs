using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using System.Diagnostics;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.Pebbler;

namespace Geometry_Testbed
{
    class Program
    {
        private static void TestSumAnglesInTriangle()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 3);

            Triangle t = new Triangle(a, m, b);

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(t);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestMidpointTheoremFigure()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 0);

            Segment ab = new Segment(a, b);
            Segment am = new Segment(a, m);
            Segment mb = new Segment(m, b);

            InMiddle im = new InMiddle(m, ab, "Intrinsic");
            Midpoint mid = new Midpoint(m, ab, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();
            intrinsic.Add(a);
            intrinsic.Add(m);
            intrinsic.Add(b);
            intrinsic.Add(ab);
            intrinsic.Add(am);
            intrinsic.Add(mb);
            intrinsic.Add(im);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(mid);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestMidpointTheorem()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 0);

            Segment am = new Segment(a, m);
            Segment ab = new Segment(a, b);
            Segment bm = new Segment(b, m);

            Midpoint mid = new Midpoint(m, ab, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(m);
            intrinsic.Add(b);
            intrinsic.Add(am);
            intrinsic.Add(ab);
            intrinsic.Add(bm);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(mid);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestFigureSix()
        {
            Point x = new Point("X", 0, 0);
            Point m = new Point("M", 2, 0);
            Point y = new Point("Y", 4, 0);
            Point t = new Point("T", -1, 2);
            Point n = new Point("N", 1, 2); 
            Point z = new Point("Z", -2, 2);

            Triangle ztn = new Triangle(z, t, n);
            Triangle tnm = new Triangle(t, n, m);
            Triangle nmy = new Triangle(n, m, y);
            Triangle txm = new Triangle(t, x, m);

            Segment zx = new Segment(z, x);
            Segment zy = new Segment(z, y);
            Segment xy = new Segment(x, y);

            Midpoint midT = new Midpoint(t, zx, "Given");
            Midpoint midN = new Midpoint(n, zy, "Given");
            Midpoint midM = new Midpoint(m, xy, "Given");

            GeometricCongruentSegments ccss1 = new GeometricCongruentSegments(new Segment(t, n), new Segment(x, m), "Given");
            GeometricCongruentSegments ccss2 = new GeometricCongruentSegments(new Segment(n, m), new Segment(t, x), "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(x);
            clauses.Add(m);
            clauses.Add(y);
            clauses.Add(t);
            clauses.Add(n);
            clauses.Add(z);
            clauses.Add(ztn);
            clauses.Add(tnm);
            clauses.Add(nmy);
            clauses.Add(txm);
            clauses.Add(zx);
            clauses.Add(zy);
            clauses.Add(xy);
            clauses.Add(midT);
            clauses.Add(midN);
            clauses.Add(midM);
            clauses.Add(ccss1);
            clauses.Add(ccss2);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestExteriorAngleSum()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 3);
            Point c = new Point("C", 4, 0);
            Point d = new Point("D", 0, 0);

            Triangle dmctriangle = new Triangle(d, m, c);
            Angle acdAngle = new Angle(a, c, d);
            Angle amdAngle = new Angle(a, m, d);
            Angle bmcAngle = new Angle(b, m, c);

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(c);
            clauses.Add(d);
            clauses.Add(dmctriangle);
            clauses.Add(acdAngle);
            clauses.Add(amdAngle);
            clauses.Add(bmcAngle);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestClassicOverlappingRightTriangleFigure()
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

            Triangle rightOne = new Triangle(ad, cd, ac, "Given");
            RightTriangle rightTwo = new RightTriangle(bc, cd, bd, "Given");

            Triangle isoOne = new Triangle(am, md, ad, "Given");
            Triangle isoTwo = new Triangle(mb, mc, bc, "Given");

            Triangle bottomIso = new Triangle(mc, md, cd, "Given");

            Intersection inter = new Intersection(m, ac, bd, "Given");

            Intersection inter10 = new Intersection(m, am, bd, "Given");
            Intersection inter11 = new Intersection(m, mc, bd, "Given");
            Intersection inter12 = new Intersection(m, mb, ac, "Given");
            Intersection inter13 = new Intersection(m, md, ac, "Given");
            Intersection inter14 = new Intersection(m, am, mb, "Given");
            Intersection inter15 = new Intersection(m, mb, mc, "Given");
            Intersection inter16 = new Intersection(m, mc, md, "Given");
            Intersection inter17 = new Intersection(m, md, am, "Given");

            Intersection inter2 = new Intersection(a, ac, ad, "Given");
            Intersection inter3 = new Intersection(b, bc, bd, "Given");
            Intersection inter4 = new Intersection(c, bc, ac, "Given");
            Intersection inter5 = new Intersection(c, cd, bc, "Given");
            Intersection inter6 = new Intersection(c, cd, ac, "Given");

            Intersection inter7 = new Intersection(d, ad, cd, "Given");
            Intersection inter8 = new Intersection(d, ad, bd, "Given");
            Intersection inter9 = new Intersection(d, cd, bd, "Given");

            Midpoint mid1 = new Midpoint(m, ac, "Given");
            Midpoint mid2 = new Midpoint(m, bd, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

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
            intrinsic.Add(rightOne);
            intrinsic.Add(rightTwo);
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

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(mid1);
            givens.Add(mid2);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        //      A
        //     /|\
        //    / | \
        //   /  |  \
        //  /___|___\
        //  B   M    C
        private static void TestIsoscelesTriangleFigure()
        {
            Point a = new Point("A", 2, 6);
            Point m = new Point("M", 2, 0);
            Point b = new Point("B", 0, 0);
            Point c = new Point("C", 4, 0);

            Segment ab = new Segment(a, b);
            Segment am = new Segment(a, m);
            Segment ac = new Segment(a, c);
            Segment bm = new Segment(b, m);
            Segment mc = new Segment(m, c);

            Segment bc = new Segment(b, c);

            IsoscelesTriangle original = new IsoscelesTriangle(ac, ab, bc, "Given");

            Triangle left = new Triangle(ab, bm, am, "Given");
            Triangle right = new Triangle(ac, mc, am, "Given");

            Intersection inter1 = new Intersection(a, ab, ac, "Given");
            Intersection inter2 = new Intersection(a, ab, am, "Given");
            Intersection inter3 = new Intersection(a, am, ac, "Given");

            Intersection inter4 = new Intersection(m, am, bc, "Given");
            Intersection inter5 = new Intersection(m, bm, mc, "Given");

            Intersection inter6 = new Intersection(b, ab, bm, "Given");
            Intersection inter7 = new Intersection(b, ab, bc, "Given");

            Intersection inter8 = new Intersection(c, ac, mc, "Given");
            Intersection inter9 = new Intersection(c, ac, bc, "Given");

            Angle bac = new Angle(b, a, c);

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(m);
            intrinsic.Add(b);
            intrinsic.Add(c);

            intrinsic.Add(ab);
            intrinsic.Add(am);
            intrinsic.Add(ac);
            intrinsic.Add(bm);
            intrinsic.Add(mc);
            intrinsic.Add(bc);

            intrinsic.Add(original);
            intrinsic.Add(left);
            intrinsic.Add(right);

            intrinsic.Add(inter1);
            intrinsic.Add(inter2);
            intrinsic.Add(inter3);
            intrinsic.Add(inter4);
            intrinsic.Add(inter5);
            intrinsic.Add(inter6);
            intrinsic.Add(inter7);
            intrinsic.Add(inter8);
            intrinsic.Add(inter9);

            intrinsic.Add(bac);

            List<GroundedClause> givens = new List<GroundedClause>();
            AngleBisector angBisector = new AngleBisector(bac, am, "Given");
            givens.Add(angBisector);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestIsoscelesTriangle()
        {
            Point a = new Point("A", -2, 0);
            Point m = new Point("M", 0, 0);
            Point b = new Point("B", 0, 3);
            Point c = new Point("C", 2, 0);

            Segment ab = new Segment(a, b);
            Segment bc = new Segment(b, c);
            Segment ac = new Segment(a, c);
            Segment mb = new Segment(m, b);
            Segment am = new Segment(a, m);
            Segment mc = new Segment(m, c);

            Midpoint mid = new Midpoint(m, ac, "Given");
            //Intersection inter = new Intersection(m, mb, ac, "Given");

            Triangle rightOne = new Triangle(ab, mb, am, "Given");
            rightOne.SetProvenToBeRight();
            Triangle rightTwo = new Triangle(bc, mb, mc, "Given");
            rightTwo.SetProvenToBeRight();
            Triangle iso = new Triangle(ab, bc, ac, "Given");


            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(c);
            clauses.Add(ab);
            clauses.Add(bc);
            clauses.Add(ac);
            clauses.Add(mb);
            clauses.Add(am);
            clauses.Add(mc);
            clauses.Add(mid);
            clauses.Add(iso);
            clauses.Add(rightOne);
            clauses.Add(rightTwo);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestParallel()
        {
            Point a = new Point("A", 0, 2);
            Point b = new Point("B", 6, 2);
            Point e = new Point("E", 0, 6);
            Point f = new Point("F", 6, 6);
            Point g = new Point("G", 0, 0);
            Point h = new Point("H", 12, 24);
            Segment ab = new Segment(a, b);
            Segment ef = new Segment(e, f);  //to be GeometricParallel with ab
            Segment gh = new Segment(g, h);

            GeometricParallel abef = new GeometricParallel(ab, ef, "Given");
            Intersection abgh = new Intersection(new Point("I1",1,2),ab,gh,"Given");
            Intersection efgh = new Intersection(new Point("I2",3, 6), ef, gh, "Given");

            List<GroundedClause> figure = new List<GroundedClause>();
            List<GroundedClause> givens = new List<GroundedClause>();

            figure.Add(a);
            figure.Add(b);
            figure.Add(e);
            figure.Add(f);
            figure.Add(g);
            figure.Add(h);

            figure.Add(ab); //to test paralell
            figure.Add(ef); //to test paralell
            figure.Add(abgh);
            figure.Add(efgh);
            givens.Add(abef); //to test paralell

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(figure, givens);
        }

        private static void TestAlternateInterior()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point c = new Point("C", 2, 2);
            Point d = new Point("D", 8, 2);
            Point m = new Point("M", 2, 0);
            Point n = new Point("N", 4, 2);
            Point p = new Point("P", 1, -1);
            Point q = new Point("Q", 5, 3);

            Segment ab = new Segment(a, b);
            Segment cd = new Segment(c, d);
            Segment pq = new Segment(p, q);

            Intersection top = new Intersection(m, ab, pq, "Intrinsic");
            Intersection bottom = new Intersection(n, cd, pq, "Intrinsic");

            Angle amn = new Angle(a, m, n);
            Angle dnm = new Angle(d, n, m);

            GeometricParallel gParallel = new GeometricParallel(ab, cd, "Given");
            // CongruentAngles cas = new CongruentAngles(amn, dnm, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(m);
            intrinsic.Add(n);
            intrinsic.Add(p);
            intrinsic.Add(q);

            intrinsic.Add(ab);
            intrinsic.Add(cd);
            intrinsic.Add(pq);

            intrinsic.Add(top);
            intrinsic.Add(bottom);

            intrinsic.Add(amn);
            intrinsic.Add(dnm);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(gParallel);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestCorrespondingAngleImplyParallel()
        {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\
            //
            //
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point c = new Point("C", 2, 2);
            Point d = new Point("D", 8, 2);
            Point m = new Point("M", 2, 0);
            Point n = new Point("N", 4, 2);
            Point p = new Point("P", 1, -1);
            Point q = new Point("Q", 5, 3);

            Segment ab = new Segment(a, b);
            Segment cd = new Segment(c, d);
            Segment pq = new Segment(p, q);

            Intersection top = new Intersection(m, ab, pq, "Intrinsic");
            Intersection bottom = new Intersection(n, cd, pq, "Intrinsic");

            //Angle amn = new Angle(a, m, n);
            //Angle cnq = new Angle(c, n, q);

            Angle pmb = new Angle(p, m, b);
            Angle pnd = new Angle(p, n, d);

            CongruentAngles cas = new CongruentAngles(pmb, pnd, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(m);
            intrinsic.Add(n);
            intrinsic.Add(p);
            intrinsic.Add(q);

            intrinsic.Add(ab);
            intrinsic.Add(cd);
            intrinsic.Add(pq);

            intrinsic.Add(top);
            intrinsic.Add(bottom);

            intrinsic.Add(pmb);
            intrinsic.Add(pnd);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(cas);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestSameSideSupplementary()
        {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\
            //
            //
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point c = new Point("C", 2, 2);
            Point d = new Point("D", 8, 2);
            Point m = new Point("M", 2, 0);
            Point n = new Point("N", 4, 2);
            Point p = new Point("P", 1, -1);
            Point q = new Point("Q", 5, 3);

            Segment ab = new Segment(a, b);
            Segment cd = new Segment(c, d);
            Segment pq = new Segment(p, q);

            Intersection top = new Intersection(m, ab, pq, "Intrinsic");
            Intersection bottom = new Intersection(n, cd, pq, "Intrinsic");

            Angle bmn = new Angle(b, m, n);
            Angle dnm = new Angle(d, n, m);

            Supplementary supp = new Supplementary(bmn, dnm, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(m);
            intrinsic.Add(n);
            intrinsic.Add(p);
            intrinsic.Add(q);

            intrinsic.Add(ab);
            intrinsic.Add(cd);
            intrinsic.Add(pq);

            intrinsic.Add(top);
            intrinsic.Add(bottom);

            intrinsic.Add(bmn);
            intrinsic.Add(dnm);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(supp);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestInterleavingSimilarTriangles()
        {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\
            //
            //
            Point a = new Point("A", 2, 12);
            Point d = new Point("D", 1.5, 9); // (1,3)
            Point e = new Point("E", 2.5, 9); // (3, 3)
            Point x = new Point("X", 2, 18 / 2.5); // (2, 2)
            Point b = new Point("B", 0, 0);
            Point c = new Point("C", 4, 0);

            Segment ab = new Segment(a, b);
            Segment ac = new Segment(a, c);
            Segment bc = new Segment(b, c);
            Segment be = new Segment(b, e);
            Segment bx = new Segment(b, x);

            Segment ad = new Segment(a, d);
            Segment cd = new Segment(c, d);
            Segment de = new Segment(d, e);
            Segment db = new Segment(d, b);
            Segment ae = new Segment(a, e);

            Segment ex = new Segment(e, x);
            Segment ec = new Segment(e, c);
            Segment xc = new Segment(x, c);
            Segment xd = new Segment(x, d);

            Triangle one = new Triangle(ad, de, ae, "Intrinsic");
            Triangle two = new Triangle(de, ex, xd, "Intrinsic");

            Triangle whole = new Triangle(ab, ac, bc, "Intrinsic");

            Triangle leftOne = new Triangle(ab, be, ae, "Intrinsic");
            Triangle leftTwo = new Triangle(db, de, be, "Intrinsic");

            Triangle rightOne = new Triangle(ac, ad, cd, "Intrinsic");
            Triangle rightTwo = new Triangle(ec, cd, de, "Intrinsic");

            Triangle bxd = new Triangle(bx, db, xd, "Intrinsic");
            Triangle cxe = new Triangle(xc, ex, ec, "Intrinsic");
            Triangle bxc = new Triangle(bx, xc, bc, "Intrinsic");

            Triangle bec = new Triangle(be, ec, bc, "Intrinsic");
            Triangle cdb = new Triangle(cd, db, bc, "Intrinsic");

            Intersection interLeft = new Intersection(d, ab, cd, "Intrinsic");
            Intersection interRight = new Intersection(e, ac, be, "Intrinsic");
            Intersection interMain = new Intersection(x, cd, be, "Intrinsic");

            Intersection i2 = new Intersection(b, be, ab, "Intrinsic");
            Intersection i3 = new Intersection(b, bc, ab, "Intrinsic");
            Intersection i4 = new Intersection(b, bc, be, "Intrinsic");

            Intersection i5 = new Intersection(c, ac, cd, "Intrinsic");
            Intersection i6 = new Intersection(c, ac, bc, "Intrinsic");
            Intersection i7 = new Intersection(c, cd, bc, "Intrinsic");

            Intersection i8 = new Intersection(d, cd, de, "Intrinsic");
            Intersection i9 = new Intersection(d, ab, de, "Intrinsic");
            Intersection i10 = new Intersection(e, de, ac, "Intrinsic");

            InMiddle im1 = new InMiddle(x, be, "Intrinsic");
            InMiddle im2 = new InMiddle(d, ab, "Intrinsic");
            InMiddle im3 = new InMiddle(e, ac, "Intrinsic");
            InMiddle im4 = new InMiddle(x, cd, "Intrinsic");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(d);
            intrinsic.Add(e);
            intrinsic.Add(x);
            intrinsic.Add(b);
            intrinsic.Add(c);

            intrinsic.Add(ab);
            intrinsic.Add(ac);
            intrinsic.Add(bc);
            intrinsic.Add(be);
            intrinsic.Add(bx);
            intrinsic.Add(ad);
            intrinsic.Add(cd);
            intrinsic.Add(de);
            intrinsic.Add(db);
            intrinsic.Add(ae);
            intrinsic.Add(ex);
            intrinsic.Add(ec);
            intrinsic.Add(xc);
            intrinsic.Add(xd);

            intrinsic.Add(one);
            intrinsic.Add(two);
            intrinsic.Add(whole);
            intrinsic.Add(leftOne);
            intrinsic.Add(leftTwo);
            intrinsic.Add(rightOne);
            intrinsic.Add(rightTwo);
            intrinsic.Add(bxc);
            intrinsic.Add(bxd);
            intrinsic.Add(cxe);
            intrinsic.Add(bec);
            intrinsic.Add(cdb);

            intrinsic.Add(im1);
            intrinsic.Add(im2);
            intrinsic.Add(im3);
            intrinsic.Add(im4);

            intrinsic.Add(interLeft);
            intrinsic.Add(interRight);
            intrinsic.Add(interMain);

            intrinsic.Add(i2);
            intrinsic.Add(i3);
            intrinsic.Add(i4);
            intrinsic.Add(i5);
            intrinsic.Add(i6);
            intrinsic.Add(i7);
            intrinsic.Add(i8);
            intrinsic.Add(i9);
            intrinsic.Add(i10);

            GeometricCongruentTriangles cts = new GeometricCongruentTriangles(leftOne, rightOne, "Given");

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(cts);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestCongruentAdjacentAngles()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point m = new Point("M", 2, 0);
            Point p = new Point("P", 2, -1);
            Point q = new Point("Q", 2, 3);

            Segment ab = new Segment(a, b);
            Segment pq = new Segment(p, q);

            Intersection inter = new Intersection(m, ab, pq, "Intrinsic");

            Angle amp = new Angle(a, m, p);
            Angle bmp = new Angle(b, m, p);

            CongruentAngles cas = new CongruentAngles(amp, bmp, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(m);
            intrinsic.Add(p);
            intrinsic.Add(q);

            intrinsic.Add(ab);
            intrinsic.Add(pq);

            intrinsic.Add(inter);

            intrinsic.Add(amp);
            intrinsic.Add(bmp);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(cas);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestPerpendicularImplyCongruentAdjacentAngles()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point m = new Point("M", 2, 0);
            Point p = new Point("P", 2, -1);
            Point q = new Point("Q", 2, 3);

            Segment ab = new Segment(a, b);
            Segment pq = new Segment(p, q);

            Intersection inter = new Intersection(m, ab, pq, "Intrinsic");

            Angle amp = new Angle(a, m, p);
            Angle bmp = new Angle(b, m, p);

            Perpendicular perp = new Perpendicular(inter, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(m);
            intrinsic.Add(p);
            intrinsic.Add(q);

            intrinsic.Add(ab);
            intrinsic.Add(pq);

            intrinsic.Add(inter);

            intrinsic.Add(amp);
            intrinsic.Add(bmp);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(perp);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestPerpendicularImplyComplementary()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 6, 0);
            Point c = new Point("C", 0, 4);
            Point d = new Point("D", 2, 2);

            Segment ab = new Segment(a, b);
            Segment ac = new Segment(a, c);
            Segment ad = new Segment(a, d);

            Intersection inter1 = new Intersection(a, ab, ac, "Intrinsic");
            Intersection inter2 = new Intersection(a, ab, ad, "Intrinsic");
            Intersection inter3 = new Intersection(a, ac, ad, "Intrinsic");

            Angle bad = new Angle(b, a, d);
            Angle dac = new Angle(d, a, c);

            Perpendicular perp = new Perpendicular(inter1, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);

            intrinsic.Add(ab);
            intrinsic.Add(ac);
            intrinsic.Add(ad);

            intrinsic.Add(inter1);
            intrinsic.Add(inter2);
            intrinsic.Add(inter3);

            intrinsic.Add(bad);
            intrinsic.Add(dac);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(perp);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestSupplementCongruence()
        {
            Point a = new Point("A", 5, 5);
            Point b = new Point("B", 1, 1);
            Point c = new Point("C", 4, 1);

            Point d = new Point("D", 14, 5);
            Point e = new Point("E", 10, 1);
            Point f = new Point("F", 13, 1);

            Angle abc = new Angle(a, b, c);
            Angle def = new Angle(d, e, f);

            GeometricCongruentAngles cas = new GeometricCongruentAngles(abc, def, "Given");

            Point m = new Point("M", 4, 4);
            Point n = new Point("N", 0, 0);
            Point p = new Point("P", -4, 0);

            Point r = new Point("R", -5, 5);
            Point s = new Point("S", -9, 1);
            Point t = new Point("T", -13, 1);

            Angle mnp = new Angle(m, n, p);
            Angle rst = new Angle(r, s, t);

            Supplementary supp1 = new Supplementary(abc, mnp, "Given");
            Supplementary supp2 = new Supplementary(def, rst, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);

            intrinsic.Add(d);
            intrinsic.Add(e);
            intrinsic.Add(f);

            intrinsic.Add(m);
            intrinsic.Add(n);
            intrinsic.Add(p);

            intrinsic.Add(r);
            intrinsic.Add(s);
            intrinsic.Add(t);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(cas);
            givens.Add(supp1);
            givens.Add(supp2);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestAngleBisectorTheorem()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 0, 6);
            Point c = new Point("C", 6, 0);

            Point d = new Point("D", 5, 5);

            Angle bac = new Angle(b, a, c);
            Segment bisector = new Segment(a, d);

            AngleBisector ab = new AngleBisector(bac, bisector, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);

            intrinsic.Add(bac);
            intrinsic.Add(bisector);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(ab);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestAngleBisectorIsPerpendicularBisector()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 3, 4);
            Point c = new Point("C", 6, 0);
            Point m = new Point("M", 3, 0);

            Segment bisector = new Segment(b, m);
            Segment leg1 = new Segment(a, b);
            Segment leg2 = new Segment(b, c);
            Segment baseSeg = new Segment(a, c);

            Intersection inter = new Intersection(m, bisector, baseSeg, "Intrinsic");

            Angle abc = new Angle(a, b, c);
            AngleBisector ab = new AngleBisector(abc, bisector, "Given");

            IsoscelesTriangle iso = new IsoscelesTriangle(leg1, leg2, baseSeg, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(m);
            intrinsic.Add(bisector);
            intrinsic.Add(leg1);
            intrinsic.Add(leg2);
            intrinsic.Add(baseSeg);

            intrinsic.Add(inter);
            intrinsic.Add(abc);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(ab);
            givens.Add(iso);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestSimpleIntersection()
        {
            Point p = new Point("P", 3, 3);
            Point s = new Point("S", 0, 0);
            Point t = new Point("T", 4, -4);
            Point r = new Point("R", -3, 3);
            Point q = new Point("Q", -4, -4);

            Segment pq = new Segment(p, q);
            Segment rt = new Segment(r, t);

            Segment rs = new Segment(r, s);
            Segment st = new Segment(s, t);
            Segment ps = new Segment(p, s);
            Segment sq = new Segment(s, q);

            Intersection inter = new Intersection(s, rt, pq, "Intrinsic");
            InMiddle im1 = new InMiddle(s, rt, "Intrinsic");
            InMiddle im2 = new InMiddle(s, pq, "Intrinsic");

            GeometricCongruentSegments css1 = new GeometricCongruentSegments(rs, ps, "Given");
            GeometricCongruentSegments css2 = new GeometricCongruentSegments(st, sq, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(p);
            intrinsic.Add(s);
            intrinsic.Add(t);
            intrinsic.Add(r);
            intrinsic.Add(q);
            intrinsic.Add(pq);
            intrinsic.Add(rt);
            intrinsic.Add(rs);
            intrinsic.Add(st);
            intrinsic.Add(ps);
            intrinsic.Add(sq);
            intrinsic.Add(im1);
            intrinsic.Add(im2);

            intrinsic.Add(inter);

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(css1);
            givens.Add(css2);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        // Orange book: Page 34: #33
        private static void TestRaysMakeATriangle()
        {
            Point a = new Point("A", -3, 0);
            Point b = new Point("B", 0, 4);
            Point c = new Point("C", 3, 0);
            Point x = new Point("X", -5, 0);
            Point y = new Point("Y", -6, -4);
            Point w = new Point("W", 6, 0);
            Point z = new Point("Z", 6, -4);

            Segment bz = new Segment(b, z);
            Segment by = new Segment(b, y);

            Segment xw = new Segment(x, w);

            Segment ab = new Segment(a, b);
            Segment ac = new Segment(a, c);
            Segment bc = new Segment(b, c);

            Angle bca = new Angle(b, c, a);
            Angle cab = new Angle(c, a, b);

            Triangle tri = new Triangle(ab, ac, bc, "Intrinsic");
            
            Intersection inter1 = new Intersection(a, by, xw, "Intrinsic");
            Intersection inter2 = new Intersection(c, bz, xw, "Intrinsic");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(x);
            intrinsic.Add(y);
            intrinsic.Add(w);
            intrinsic.Add(z);
            intrinsic.Add(bz);
            intrinsic.Add(by);
            intrinsic.Add(xw);
            intrinsic.Add(ab);
            intrinsic.Add(ac);
            intrinsic.Add(bc);

            intrinsic.Add(bca);
            intrinsic.Add(cab);

            intrinsic.Add(inter1);
            intrinsic.Add(inter2);

            intrinsic.Add(tri);

            GeometricCongruentAngles cas1 = new GeometricCongruentAngles(bca, cab, "Given");

            List<GroundedClause> givens = new List<GroundedClause>();
            givens.Add(cas1);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        static void Main(string[] args)
        {
         //TestIsoscelesTriangleFigure();
            //TestRaysMakeATriangle();
            //TestSimpleIntersection();
            //TestAngleBisectorIsPerpendicularBisector();
         //TestClassicOverlappingRightTriangleFigure();
            //TestIsoscelesTriangle();
            //IsolatedCongruenceEquationTest();
            //IsolatedEquationTest();
            //SSSTest5();
            //TestExteriorAngleSum();
            //HLTest1();
         TestMidpointTheoremFigure();
            //SASTest1();
            //TestSumAnglesInTriangle();
            //TestFigureSix();
            //TestAlternateInterior();
            //TestSameSideSupplementary();
         //TestInterleavingSimilarTriangles();
            //TestSimplification();
            //TestSimplificationConstants();
            //TestSimpleSubstitution();
            //TestCorrespondingAngles();
            //TestCorrespondingAngleImplyParallel();
            //TestCongruentAdjacentAngles();
            //TestPerpendicularImplyCongruentAdjacentAngles();
            //TestPerpendicularImplyComplementary();
            //TestSupplementCongruence();
            //TestMidpointTheorem();
            //TestAngleBisectorTheorem();
            //TestAcuteAnglesInRightTriangles();
            //TestThirdAnglesInTriangleCongruent();
        }

        private static void TestThirdAnglesInTriangleCongruent()
        {
            Point a = new Point("A", 0, 0);
            Point b = new Point("B", 0, 2);
            Point c = new Point("C", 4, 0);

            Segment ab = new Segment(a, b);
            Segment bc = new Segment(b, c);
            Segment ac = new Segment(a, c);

            Angle abc = new Angle(a, b, c);
            Angle bac = new Angle(b, a, c);

            Point d = new Point("D", 5, 0);
            Point e = new Point("E", 5, 2);
            Point f = new Point("F", 9, 0);

            Segment de = new Segment(d, e);
            Segment ef = new Segment(e, f);
            Segment df = new Segment(d, f);

            Angle def = new Angle(d, e, f);
            Angle edf = new Angle(e, d, f);

            Triangle first = new Triangle(ab, bc, ac, "Intrinsic");
            Triangle second = new Triangle(de, ef, df, "Intrinsic");

            CongruentAngles cas1 = new CongruentAngles(abc, def, "Given");
            CongruentAngles cas2 = new CongruentAngles(bac, edf, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(ab);
            intrinsic.Add(bc);
            intrinsic.Add(ac);
            intrinsic.Add(de);
            intrinsic.Add(ef);
            intrinsic.Add(df);

            intrinsic.Add(abc);
            intrinsic.Add(bac);

            intrinsic.Add(def);
            intrinsic.Add(edf);

            intrinsic.Add(first);
            intrinsic.Add(second);

            List<GroundedClause> givens = new List<GroundedClause>();

            givens.Add(cas1);
            givens.Add(cas2);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestAcuteAnglesInRightTriangles()
        {
            Point a = new Point("A", 0, 3);
            Point b = new Point("B", 4, 3);
            Point c = new Point("C", 4, 0);
            Point d = new Point("D", 0, 0);

            Segment cd = new Segment(c, d);
            Segment ad = new Segment(a, d);
            Segment bc = new Segment(b, c);
            Segment bd = new Segment(b, d);
            Segment ac = new Segment(a, c);

            RightTriangle rightOne = new RightTriangle(ad, cd, ac, "Given");
            RightTriangle rightTwo = new RightTriangle(bc, cd, bd, "Given");

            List<GroundedClause> intrinsic = new List<GroundedClause>();
            List<GroundedClause> givens = new List<GroundedClause>();

            intrinsic.Add(a);
            intrinsic.Add(b);
            intrinsic.Add(c);
            intrinsic.Add(d);
            intrinsic.Add(cd);
            intrinsic.Add(ad);
            intrinsic.Add(bc);
            intrinsic.Add(bd);
            intrinsic.Add(ac);

            intrinsic.Add(rightOne);
            intrinsic.Add(rightTwo);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(intrinsic, givens);
        }

        private static void TestCorrespondingAngles()
        {
            Point a = new Point("A", 0, 2);
            Point b = new Point("B", 6, 2);
            Point e = new Point("E", 0, 6);
            Point f = new Point("F", 6, 6);
            Point g = new Point("G", 0, 0);
            Point h = new Point("H", 12, 24);
            Point x = new Point("X", 1, 2);
            Point y = new Point("Y", 3, 6);

            Segment ab = new Segment(a, b);
            Segment ef = new Segment(e, f);  //to be GeometricParallel with ab
            Segment transversal = new Segment(g, h);

            GeometricParallel parallel = new GeometricParallel(ab, ef, "Given");
            Intersection abgh = new Intersection(x, ab, transversal, "Given");
            Intersection efgh = new Intersection(y, ef, transversal, "Given");

            List<GroundedClause> figure = new List<GroundedClause>();
            List<GroundedClause> givens = new List<GroundedClause>();

            figure.Add(a);
            figure.Add(b);
            figure.Add(e);
            figure.Add(f);
            figure.Add(g);
            figure.Add(h);
            figure.Add(x);
            figure.Add(y);

            figure.Add(ab);
            figure.Add(ef);
            figure.Add(transversal);
            figure.Add(abgh);
            figure.Add(efgh);
            givens.Add(parallel);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(figure, givens);
        }

        private static void IsolatedCongruenceEquationTest()
        {
            // ADC \cong BCD = 90
            // Triangle ADC

            Point a = new Point("A", 0, 3);
            Point b = new Point("B", 4, 3);
            Point c = new Point("C", 4, 0);
            Point d = new Point("D", 0, 0);

            Segment cd = new Segment(c, d);
            Segment ad = new Segment(a, d);
            Segment bc = new Segment(b, c);
            Segment bd = new Segment(b, d);
            Segment ac = new Segment(a, c);

            AlgebraicCongruentAngles aca = new AlgebraicCongruentAngles(new Angle(a, d, c), new Angle(b, c, d), "");

            Triangle rightOne = new Triangle(ad, cd, ac, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(b);
            clauses.Add(c);
            clauses.Add(d);
            clauses.Add(cd);
            clauses.Add(ad);
            clauses.Add(bc);
            clauses.Add(bd);
            clauses.Add(ac);
            clauses.Add(rightOne);
            clauses.Add(aca);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void IsolatedEquationTest()
        {
            // 2MC = AB
            // 2QF = AB
            Point A = new Point("A", 0, 0);
            Point B = new Point("B", 4, 0);
            Point M = new Point("M", 1, 1);
            Point C = new Point("C", 1, 3);
            Point Q = new Point("Q", 0, 0);
            Point F = new Point("F", 0, -2);

            Segment MC = new Segment(M, C);
            Segment AB = new Segment(A, B);
            Segment QF = new Segment(Q, F);

            Multiplication product1 = new Multiplication(new NumericValue(2), MC);
            Multiplication product2 = new Multiplication(new NumericValue(2), QF);

            Equation eq1 = new GeometricSegmentEquation(product1, AB);
            Equation eq2 = new GeometricSegmentEquation(product2, AB);

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(A);
            clauses.Add(B);
            clauses.Add(M);
            clauses.Add(C);
            clauses.Add(Q);
            clauses.Add(F);

            clauses.Add(MC);
            clauses.Add(AB);
            clauses.Add(QF);
            clauses.Add(eq1);
            clauses.Add(eq2);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void HLTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p12, p13);
            Triangle t1 = new Triangle(p11, p12, p13);
            t1.SetProvenToBeRight();

            Point p21 = new Point("D", 4, 0);
            Point p22 = new Point("E", 4, 2);
            Point p23 = new Point("F", 7, 0);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p22, p23);
            Triangle t2 = new Triangle(p21, p22, p23);
            t2.SetProvenToBeRight();

            //
            // Congruent Segments and Angle
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");

            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(p11);
            clauses.Add(p12);
            clauses.Add(p13);
            clauses.Add(s11);
            clauses.Add(s12);
            clauses.Add(t1);

            clauses.Add(p21);
            clauses.Add(p22);
            clauses.Add(p23);
            clauses.Add(s21);
            clauses.Add(s22);
            clauses.Add(t2);

            clauses.Add(ccs1);
            clauses.Add(ccs2);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestSimpleSubstitution()
        {
            Point a = new Point("A", 0, 3);
            Point b = new Point("B", 0, 0);
            Point c = new Point("C", 3, 0);
            Point d = new Point("D", 7, 3);
            Point e = new Point("E", 7, 0);
            Point f = new Point("F", 10, 0);

            Angle ang1 = new Angle(a, b, c);
            Angle ang2 = new Angle(d, e, f);

            AlgebraicAngleEquation ame1 = new AlgebraicAngleEquation(ang1, new NumericValue(90));
            AlgebraicAngleEquation ame2 = new AlgebraicAngleEquation(ang2, new NumericValue(90));

            TransitiveSubstitution.Instantiate(ame1);
            TransitiveSubstitution.Instantiate(ame2);
        }

        private static void TestSimplificationConstants()
        {
            Addition suml = new Addition(new NumericValue(0), new NumericValue(1));
            Addition sumr = new Addition(new NumericValue(9), new NumericValue(8));
            for (int i = 2; i < 10; i++)
            {
                suml = new Addition(suml, new NumericValue(i));
                sumr = new Addition(sumr, new NumericValue(10 - i));
            }

            AlgebraicSegmentEquation se = new AlgebraicSegmentEquation(sumr, sumr);

            //Simplification.Instantiate(se);
        }

        private static void TestSimplification()
        {
            Point a = new Point("A", 0, 3);
            Point m = new Point("M", 2, 1.5);
            Point b = new Point("B", 4, 3);
            Point c = new Point("C", 4, 0);
            Point d = new Point("D", 0, 0);

            Segment segmentAM = new Segment(a, m);
            Segment segmentMB = new Segment(m, b);
            Segment segmentAB = new Segment(a, b);

            NumericValue two = new NumericValue(2);

            Addition add = new Addition(segmentAM, segmentAM);
            Addition add2 = new Addition(segmentAM, segmentAM);
            Addition add3 = new Addition(add, add2);
            Multiplication product = new Multiplication(two, add3);

            AlgebraicSegmentEquation se = new AlgebraicSegmentEquation(add3, product);

           // Simplification.Instantiate(se);
        }

        private static void TestStraightAngleDefinition()
        {
            List<Point> pts = new List<Point>();
            string[] names = { "A", "B", "C", "D", "E", "F", "G", "H" };

            for (int i = 0; i < 6; i++)
            {
                pts.Add(new Point(names[i], i, 2 * i));
            }

            Collinear coll = new Collinear(pts, "Given");
        }

        private static void TestMidpointDefinition()
        {
            Point pt = new Point("M", 0, 0);
            Point end1 = new Point("A", -2, -2);
            Point end2 = new Point("C", 2, 2);
            Segment segment = new Segment(end1, end2);
            Midpoint mid = new Midpoint(pt, segment, "Given");
        }

        private static void TestAngleAdditionAxiom()
        {
            Point top = new Point("A", 0, 2);
            Point vertex = new Point("V", 0, 0);
            Point right = new Point("B", 2, 0);
            Angle firstQuad = new Angle(top, vertex, right);

            Point bottom = new Point("C", 0, -2);
            Point left = new Point("D", -2, 0);
            Angle thirdQuad = new Angle(left, vertex, bottom);

            Angle fourthQuad = new Angle(right, vertex, bottom);
        }

        private static void TestSegmentAdditionAxiom()
        {
            Point top = new Point("A", 0, 2);
            Point origin = new Point("V", 0, 0);
            Point right = new Point("B", 2, 0);
            Point bottom = new Point("C", 0, -2);
            Point left = new Point("D", -2, 0);

            Segment vertical = new Segment(top, bottom);
            Segment horizontal = new Segment(left, right);
            InMiddle mid1 = new InMiddle(origin, vertical, "Given");
            InMiddle mid2 = new InMiddle(origin, horizontal, "Given");
        }

        private static void TestVerticalAnglesTheorem()
        {
            Point top = new Point("A", 0, 2);
            Point origin = new Point("V", 0, 0);
            Point right = new Point("B", 2, 0);
            Point bottom = new Point("C", 0, -2);
            Point left = new Point("D", -2, 0);

            Segment vertical = new Segment(top, bottom);
            Segment horizontal = new Segment(left, right);
            Intersection inter = new Intersection(origin, vertical, horizontal, "Given");

            //Generate(inter);

            top = new Point("A", 2, 1);
            origin = new Point("V", 0, 0);
            right = new Point("B", 1, 2);
            bottom = new Point("C", -2, -1);
            left = new Point("D", -1, -2);

            Segment seg1 = new Segment(top, bottom);
            Segment seg2 = new Segment(left, right);
            inter = new Intersection(origin, seg1, seg2, "Given");

            //Generate(inter);
        }

        private static void TestIsoscelesTriangleDefinition()
        {
            Point a = new Point("A", -2, 0);
            Point b = new Point("B", 0, 2);
            Point c = new Point("C", 2, 0);
            Segment seg1 = new Segment(a, b);
            Segment seg2 = new Segment(b, c);

            Triangle tri = new Triangle(a, b, c);

            GeometricCongruentSegments ccss = new GeometricCongruentSegments(seg1, seg2, "Given");
        }

        private static void TestSubstitution()
        {
            Point a = new Point("A", -2, 0);
            Point b = new Point("B", 0, 2);
            Point c = new Point("C", 2, 0);
            Segment seg1 = new Segment(a, b);
            Segment seg2 = new Segment(b, c);
            AlgebraicSegmentEquation segEq = new AlgebraicSegmentEquation(seg1, seg2, "Given");

            //Generate(segEq);

            Point d = new Point("D", -2, -1);
            Point e = new Point("E", 0, -3);
            Point f = new Point("F", 2, -1);
            Segment seg3 = new Segment(d, e);
            Segment seg4 = new Segment(e, f);
            AlgebraicSegmentEquation segEq2 = new AlgebraicSegmentEquation(seg3, seg4, "Given");

            //Generate(segEq2);

            AlgebraicSegmentEquation segEq3 = new AlgebraicSegmentEquation(seg2, seg3, "Given");

            //Generate(segEq3);
        }

        //
        // Tests if the same triangle is congruent to itself
        //
        private static void SSSTest6()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p1 = new Point("A", 0, 0);
            Point p2 = new Point("B", 0, 2);
            Point p3 = new Point("C", 3, 0);

            Segment left = new Segment(p1, p2);
            Segment bottom = new Segment(p1, p3);
            Segment diagonal = new Segment(p2, p3);
            Triangle t1 = new Triangle(left, bottom, diagonal, "Given");

            //Generate(t1);

            Triangle t2 = new Triangle(left, bottom, diagonal, "Given");

            //Generate(t2);

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(left, left, "Given");
            //Generate(ccs1);

            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(bottom, bottom, "Given");
            //Generate(ccs2);

            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(diagonal, diagonal, "Given");
            //Generate(ccs3);
        }

        //
        // Tests two triangles in which two points are shared
        //
        private static void SSSTest5()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point shared1 = new Point("B", 0, 2);
            Point shared2 = new Point("C", 3, 0);

            Segment left = new Segment(p11, shared1);
            Segment bottom = new Segment(p11, shared2);
            Segment diagonal = new Segment(shared1, shared2);
            Triangle t1 = new Triangle(left, bottom, diagonal, "Given");

            Point pt = new Point("D", 3, 2);
            Segment top = new Segment(pt, shared1);
            Segment right = new Segment(pt, shared2);
            Triangle t2 = new Triangle(top, right, diagonal, "Given");

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(top, bottom, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(left, right, "Given");
            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(diagonal, diagonal, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();
            clauses.Add(p11);
            clauses.Add(shared1);
            clauses.Add(shared2);
            clauses.Add(left);
            clauses.Add(bottom);
            clauses.Add(diagonal);

            clauses.Add(t1);
            clauses.Add(pt);
            clauses.Add(top);
            clauses.Add(right);
            clauses.Add(t2);

            clauses.Add(t1);
            clauses.Add(t2);

            clauses.Add(ccs1);
            clauses.Add(ccs2);
            clauses.Add(ccs3);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);

        }

        //
        // Tests two triangles in which one point is shared
        //
        private static void SSSTest4()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point shared = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p11, shared);
            Segment s13 = new Segment(p12, shared);
            Triangle t1 = new Triangle(s11, s12, s13, "Given");

            Point p21 = new Point("D", 6, 0);
            Point p22 = new Point("E", 6, 2);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p21, shared);
            Segment s23 = new Segment(p22, shared);
            Triangle t2 = new Triangle(s21, s22, s23, "Given");

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");
            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(s13, s23, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();
            clauses.Add(p11);
            clauses.Add(p12);
            clauses.Add(shared);
            clauses.Add(s11);
            clauses.Add(s12);
            clauses.Add(s13);

            clauses.Add(p21);
            clauses.Add(p22);
            clauses.Add(s21);
            clauses.Add(s22);
            clauses.Add(s23);

            clauses.Add(ccs1);
            clauses.Add(ccs2);
            clauses.Add(ccs3);

            clauses.Add(t1);
            clauses.Add(t2);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void SSSTest3()
        {
            //
            // Tri ABC is congruent to Tri DEF ; order of points and segments changed
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p12, p11);
            Segment s12 = new Segment(p11, p13);
            Segment s13 = new Segment(p13, p12);
            Triangle t1 = new Triangle(s13, s11, s12, "Given");

            Point p21 = new Point("D", 6, 0);
            Point p22 = new Point("E", 6, 2);
            Point p23 = new Point("F", 3, 2);
            Segment s21 = new Segment(p22, p21);
            Segment s22 = new Segment(p21, p23);
            Segment s23 = new Segment(p23, p22);
            Triangle t2 = new Triangle(s22, s21, s23, "Given");

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s21, s11, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s23, "Given");
            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(s22, s13, "Given");
        }

        private static void SSSTest2()
        {
            //
            // Tri ABC is congruent to Tri DEF ; segments congruences seen first
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p11, p13);
            Segment s13 = new Segment(p12, p13);
            Triangle t1 = new Triangle(s11, s12, s13, "Given");

            Point p21 = new Point("D", 4, 0);
            Point p22 = new Point("E", 4, 2);
            Point p23 = new Point("F", 7, 0);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p21, p23);
            Segment s23 = new Segment(p22, p23);
            Triangle t2 = new Triangle(s21, s22, s23, "Given");

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");
            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(s13, s23, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();
            clauses.Add(p11);
            clauses.Add(p12);
            clauses.Add(p13);
            clauses.Add(s11);
            clauses.Add(s12);
            clauses.Add(s13);

            clauses.Add(p21);
            clauses.Add(p22);
            clauses.Add(p23);
            clauses.Add(s21);
            clauses.Add(s22);
            clauses.Add(s23);

            clauses.Add(ccs1);
            clauses.Add(ccs2);
            clauses.Add(ccs3);

            clauses.Add(t1);
            clauses.Add(t2);

            //GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void SSSTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p11, p13);
            Segment s13 = new Segment(p12, p13);
            Triangle t1 = new Triangle(s11, s12, s13, "Given");

            Point p21 = new Point("D", 4, 0);
            Point p22 = new Point("E", 4, 2);
            Point p23 = new Point("F", 7, 0);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p21, p23);
            Segment s23 = new Segment(p22, p23);
            Triangle t2 = new Triangle(s21, s22, s23, "Given");

            //
            // Congruent Segments
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");
            GeometricCongruentSegments ccs3 = new GeometricCongruentSegments(s13, s23, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();
            clauses.Add(p11);
            clauses.Add(p12);
            clauses.Add(p13);
            clauses.Add(s11);
            clauses.Add(s12);
            clauses.Add(s13);
            clauses.Add(t1);

            clauses.Add(p21);
            clauses.Add(p22);
            clauses.Add(p23);
            clauses.Add(s21);
            clauses.Add(s22);
            clauses.Add(s23);
            clauses.Add(t2);

            clauses.Add(ccs1);
            clauses.Add(ccs2);
            clauses.Add(ccs3);

            ////GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void SASTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p12, p13);
            Angle ang1 = new Angle(p11, p12, p13);
            Triangle t1 = new Triangle(p11, p12, p13);

            Point p21 = new Point("D", 4, 0);
            Point p22 = new Point("E", 4, 2);
            Point p23 = new Point("F", 7, 0);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p22, p23);
            Angle ang2 = new Angle(p21, p22, p23);
            Triangle t2 = new Triangle(p21, p22, p23);

            //
            // Congruent Segments and Angle
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");
            GeometricCongruentAngles cca = new GeometricCongruentAngles(ang1, ang2, "Given");
        }

        private static void SASTest2()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            Point p11 = new Point("A", 0, 0);
            Point p12 = new Point("B", 0, 2);
            Point p13 = new Point("C", 3, 0);
            Segment s11 = new Segment(p11, p12);
            Segment s12 = new Segment(p12, p13);
            Angle ang1 = new Angle(p11, p12, p13);
            Triangle t1 = new Triangle(p11, p12, p13);



            Point p21 = new Point("D", 4, 0);
            Point p22 = new Point("E", 4, 2);
            Point p23 = new Point("F", 7, 0);
            Segment s21 = new Segment(p21, p22);
            Segment s22 = new Segment(p22, p23);
            Angle ang2 = new Angle(p21, p22, p23);
            Triangle t2 = new Triangle(p21, p22, p23);

            //
            // Congruent Segments and Angle
            //
            GeometricCongruentSegments ccs1 = new GeometricCongruentSegments(s11, s21, "Given");
            GeometricCongruentSegments ccs2 = new GeometricCongruentSegments(s12, s22, "Given");
            GeometricCongruentAngles cca = new GeometricCongruentAngles(ang1, ang2, "Given");
        }
    }
}