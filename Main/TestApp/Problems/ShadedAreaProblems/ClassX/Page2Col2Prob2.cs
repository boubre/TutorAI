using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Col2Prob2 : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public Page2Col2Prob2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point p = new Point("P", -10.54, -6.72); points.Add(p);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", -12.5, 0); points.Add(r);
            Point q = new Point("Q", 12.5, 0); points.Add(q);

            Segment rp = new Segment(r, p); segments.Add(rp);
            Segment qp = new Segment(q, p); segments.Add(qp);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(o);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 12.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(qp, 24);
            known.AddSegmentLength(rp, 7);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -11.83, -3.2));
            wanted.Add(new Point("", 0, -10));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(406.87385231);
        }
    }
}