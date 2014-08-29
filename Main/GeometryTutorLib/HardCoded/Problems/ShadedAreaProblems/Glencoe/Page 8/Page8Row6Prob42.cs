using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row6Prob42 : ActualShadedAreaProblem
    {
        public Page8Row6Prob42(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 6, 6); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);

            Point q = new Point("Q", 1.5, 6); points.Add(q);
            Point r = new Point("R", 1.5, 4.5); points.Add(r);
            Point s = new Point("S", 1.5, 3); points.Add(s);
            Point t = new Point("T", 1.5, 1.5); points.Add(t);
            Point u = new Point("U", 1.5, 0); points.Add(u);

            Point v = new Point("V", 4.5, 6); points.Add(v);
            Point w = new Point("W", 4.5, 4.5); points.Add(w);
            Point x = new Point("X", 4.5, 3); points.Add(x);
            Point y = new Point("Y", 4.5, 1.5); points.Add(y);
            Point z = new Point("Z", 4.5, 0); points.Add(z);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            //Segment qr = new Segment(q, r); segments.Add(qr);
            //Segment tu = new Segment(t, u); segments.Add(tu);
            //Segment vw = new Segment(v, w); segments.Add(vw);
            //Segment yz = new Segment(y, z); segments.Add(yz);

            List<Point> pts = new List<Point>();
            pts.Add(b); 
            pts.Add(q); 
            pts.Add(v); 
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a); 
            pts.Add(u); 
            pts.Add(z); 
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q); 
            pts.Add(r);
            pts.Add(s); 
            pts.Add(t); 
            pts.Add(u);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(v); 
            pts.Add(w); 
            pts.Add(x); 
            pts.Add(y); 
            pts.Add(z);
            collinear.Add(new Collinear(pts));

            Circle tLeft = new Circle(r, 1.5);
            Circle bLeft = new Circle(t, 1.5);
            Circle tRight = new Circle(w, 1.5);
            Circle bRight = new Circle(y, 1.5);

            circles.Add(tLeft);
            circles.Add(bLeft);
            circles.Add(tRight);
            circles.Add(bRight);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 6);

            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Segment ad = (Segment)parser.Get(new Segment(a, d));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, ad));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(q, tLeft, bc));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(u, bLeft, ad));
            CircleSegmentIntersection cInter3 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(v, tRight, bc));
            CircleSegmentIntersection cInter4 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(z, bRight, ad));

            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));
            given.Add(new Strengthened(cInter3, new Tangent(cInter3)));
            given.Add(new Strengthened(cInter4, new Tangent(cInter4)));

            given.Add(new Strengthened(quad, new Square(quad)));

            given.Add(new GeometricCongruentCircles(tLeft, bLeft));
            given.Add(new GeometricCongruentCircles(tLeft, tRight));
            given.Add(new GeometricCongruentCircles(tLeft, bRight));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.1, 0.1));
            wanted.Add(new Point("", 0.1, 3));
            wanted.Add(new Point("", 0.1, 5.9));
            wanted.Add(new Point("", 3, 5.9));
            wanted.Add(new Point("", 3, 3));
            wanted.Add(new Point("", 3, 0.1));
            wanted.Add(new Point("", 5.9, 0.1));
            wanted.Add(new Point("", 5.9, 3));
            wanted.Add(new Point("", 5.9, 5.9));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(36 - (9 * System.Math.PI));

            problemName = "Glencoe Page 8 Row 6 Problem 42";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
