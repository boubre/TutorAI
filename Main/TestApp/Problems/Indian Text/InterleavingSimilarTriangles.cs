using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    public class InterleavingSimilarTriangles : ActualProblem
    {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\
            //
            //
        public InterleavingSimilarTriangles() : base()
        {
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

            given.Add(new GeometricCongruentTriangles(leftOne, rightOne, "Given"));
        }
    }
}