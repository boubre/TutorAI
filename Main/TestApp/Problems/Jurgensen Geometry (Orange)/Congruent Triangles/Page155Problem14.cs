using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 155 Problem 14
    //
    public class Page155Problem14 : CongruentTrianglesProblem
    {
        public Page155Problem14() : base()
        {
            Point r = new Point("R", 2, 5); intrinsic.Add(r);
            Point s = new Point("S", 0, 0); intrinsic.Add(s);
            Point t = new Point("T", 3.2, 2); intrinsic.Add(t);
            Point z = new Point("Z", 0.8, 2); intrinsic.Add(z);
            Point w = new Point("W", 4, 0); intrinsic.Add(w);

            Point x = new Point("X", 2, 1.25); intrinsic.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(z);
            pts.Add(s);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(t);
            pts.Add(w);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(z);
            pts.Add(x);
            pts.Add(w);
            Collinear coll3 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(t);
            pts.Add(x);
            pts.Add(s);
            Collinear coll4 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateSegmentClauses(coll3));
            intrinsic.AddRange(GenerateSegmentClauses(coll4));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(r, z)),
                                                     GetProblemSegment(intrinsic, new Segment(r, t)), "Given"));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, new Segment(z, s)),
                                                     GetProblemSegment(intrinsic, new Segment(t, w)), "Given"));


            //Segment rs = new Segment(r, s);
            //Segment rt = new Segment(r, t);
            //Segment st = new Segment(s, t);

            //Segment rz = new Segment(r, z);
            //Segment rw = new Segment(r, w);
            //Segment zw = new Segment(z, w);

            //Angle rst = new Angle(r, s, t);
            //Angle str = new Angle(s, t, r);
            //Angle trs = new Angle(t, r, s);

            //Angle rzw = new Angle(r, z, w);
            //Angle zwr = new Angle(z, w, r);
            //Angle wrz = new Angle(w, r, z);

            //Triangle left = new Triangle(rs, rt, st, "Intrinsic");
            //Triangle right = new Triangle(rz, zw, rw, "Intrinsic");

            //Segment zx = new Segment(z, x);
            //Segment sx = new Segment(s, x);
            //Segment zs = new Segment(z, s);

            //Segment xw = new Segment(x, w);
            //Segment tx = new Segment(t, x);
            //Segment tw = new Segment(t, w);

            //Angle zxs = new Angle(z, x, s);
            //Angle xsz = new Angle(x, s, z);
            //Angle szx = new Angle(s, z, x);

            //Angle txw = new Angle(t, x, w);
            //Angle xwt = new Angle(x, w, t);
            //Angle wtx = new Angle(w, t, x);

            //Triangle leftSmall = new Triangle(zs, sx, zx, "Intrinsic");
            //Triangle rightSmall = new Triangle(tx, xw, tw, "Intrinsic");

            //Intersection i1 = new Intersection(x, st, zw, "Intrinsic");
            //Intersection i2 = new Intersection(z, rs, zw, "Intrinsic");
            //Intersection i3 = new Intersection(t, st, rw, "Intrinsic");
            ////Intersection i4 = new Intersection(t, bc, cp, "Intrinsic");
            ////Intersection i5 = new Intersection(t, cq, ac, "Intrinsic");

            //InMiddle im1 = new InMiddle(x, st, "Intrinsic");
            //InMiddle im2 = new InMiddle(x, zw, "Intrinsic");
            //InMiddle im3 = new InMiddle(z, rs, "Intrinsic");
            //InMiddle im4 = new InMiddle(t, rw, "Intrinsic");

            //intrinsic.Add(r);
            //intrinsic.Add(s);
            //intrinsic.Add(t);
            //intrinsic.Add(z);
            //intrinsic.Add(w);

            //intrinsic.Add(rs);
            //intrinsic.Add(rt);
            //intrinsic.Add(st);
            //intrinsic.Add(rz);
            //intrinsic.Add(rw);
            //intrinsic.Add(zw);

            //intrinsic.Add(rst);
            //intrinsic.Add(str);
            //intrinsic.Add(trs);
            //intrinsic.Add(rzw);
            //intrinsic.Add(zwr);
            //intrinsic.Add(wrz);

            //intrinsic.Add(left);
            //intrinsic.Add(right);

            //intrinsic.Add(zx);
            //intrinsic.Add(sx);
            //intrinsic.Add(zs);
            //intrinsic.Add(xw);
            //intrinsic.Add(tx);
            //intrinsic.Add(tw);

            //intrinsic.Add(zxs);
            //intrinsic.Add(xsz);
            //intrinsic.Add(szx);
            //intrinsic.Add(txw);
            //intrinsic.Add(xwt);
            //intrinsic.Add(wtx);

            //intrinsic.Add(leftSmall);
            //intrinsic.Add(rightSmall);

            //intrinsic.Add(im1);
            //intrinsic.Add(im2);
            //intrinsic.Add(im3);
            //intrinsic.Add(im4);

            //intrinsic.Add(i1);
            //intrinsic.Add(i2);
            //intrinsic.Add(i3);
        }
    }
}