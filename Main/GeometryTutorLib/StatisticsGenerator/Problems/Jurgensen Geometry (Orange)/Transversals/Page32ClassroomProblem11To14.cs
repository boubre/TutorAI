using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 32 Classroom Problems 11-14
    //
    public class Page32ClassroomProblem11To14 : TransversalsProblem
    {
        public Page32ClassroomProblem11To14(bool onoff) : base(onoff)
        {
            problemName = "Page 32 Classroom Problems 11-14";


            Point o = new Point("O", 0, 0);   intrinsic.Add(o);
            Point m = new Point("M", -4, 3);  intrinsic.Add(m);
            Point j = new Point("J", 4, -3);   intrinsic.Add(j);
            Point h = new Point("H", -1, -4); intrinsic.Add(h);
            Point l = new Point("L", 1, 4);   intrinsic.Add(l);
            Point g = new Point("G", -4, 0);  intrinsic.Add(g);
            Point k = new Point("K", 5, 0);   intrinsic.Add(k);

            List<Point> pts = new List<Point>();
            pts.Add(m);
            pts.Add(o);
            pts.Add(j);
            Collinear coll1 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(o);
            pts.Add(h);
            Collinear coll2 = new Collinear(pts);

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(o);
            pts.Add(g);
            Collinear coll3 = new Collinear(pts);

            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
            intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll3));
            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, o, h)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, o, k))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, o, m)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(k, o, j))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(m, o, k)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(g, o, j))));
            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(l, o, g)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(h, o, k))));
        }
    }
}