using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 134 #6
    //
    public class Page134Problem6 : CongruentTrianglesProblem
    {
        public Page134Problem6() : base()
        {
            Point l = new Point("L", -3, 4); intrinsic.Add(l);
            Point o = new Point("O", 1.5, 1); intrinsic.Add(o);
            Point j = new Point("J", 6, 4); intrinsic.Add(j);
            Point m = new Point("M", 0, 0); intrinsic.Add(m);
            Point n = new Point("N", 3, 0); intrinsic.Add(n);

     System.Diagnostics.Debug.WriteLine("Intersection: " + (new Segment(m, j).FindIntersection(new Segment(l, n)).ToString()));

            Segment lm = new Segment(l, m); intrinsic.Add(lm);
            Segment mn = new Segment(m, n); intrinsic.Add(mn);
            Segment nj = new Segment(n, j); intrinsic.Add(nj);

            List<Point> pts = new List<Point>();
            pts.Add(l);
            pts.Add(o);
            pts.Add(n);
            Collinear coll1 = new Collinear(pts, "Intrinsic");

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(o);
            pts.Add(j);
            Collinear coll2 = new Collinear(pts, "Intrinsic");

            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateSegmentClauses(coll2));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(o, m, n)),
                                                   GetProblemAngle(intrinsic, new Angle(o, n, m)), "Given"));

            given.Add(new GeometricCongruentAngles(GetProblemAngle(intrinsic, new Angle(l, m, o)),
                                                   GetProblemAngle(intrinsic, new Angle(j, n, o)), "Given"));
        }
    }
}