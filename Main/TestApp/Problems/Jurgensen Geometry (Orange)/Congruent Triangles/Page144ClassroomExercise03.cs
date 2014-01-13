using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace Geometry_Testbed
{
    //
    // Geometry; Page 144 CE #3
    //
    public class Page144ClassroomExercise03 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise03() : base()
        {
            Point b = new Point("B", 0, 3); intrinsic.Add(b);
            Point l = new Point("L", 3, 5); intrinsic.Add(l);
            Point a = new Point("A", 5, 3); intrinsic.Add(a);
            Point o = new Point("O", 2, 3); intrinsic.Add(o);
            Point j = new Point("J", 3, 1); intrinsic.Add(j);

            Segment bl = new Segment(b, l); intrinsic.Add(bl);
            Segment bj = new Segment(b, j); intrinsic.Add(bj);
            Segment al = new Segment(a, l); intrinsic.Add(al);
            Segment aj = new Segment(a, j); intrinsic.Add(aj);
            Segment oj = new Segment(o, j); intrinsic.Add(oj);
            Segment ol = new Segment(o, l); intrinsic.Add(ol);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            Collinear coll1 = new Collinear(pts, "Intrinsic");
            
            intrinsic.AddRange(GenerateSegmentClauses(coll1));
            intrinsic.AddRange(GenerateAngleIntersectionTriangleClauses(intrinsic));

            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, bl), GetProblemSegment(intrinsic, bj), "Given"));
            given.Add(new GeometricCongruentSegments(GetProblemSegment(intrinsic, oj), GetProblemSegment(intrinsic, ol), "Given"));
        }
    }
}