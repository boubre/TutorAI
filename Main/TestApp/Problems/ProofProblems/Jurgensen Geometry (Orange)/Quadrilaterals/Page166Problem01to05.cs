using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page166Problem01to05 : CongruentTrianglesProblem
    {
        // Geometry; Page 166 Problem 01 to 05
        // Demonstrates usage of:
        // 1. Definition of parallelogram
        // 2. If both pairs of opposite sides of a quad are congruent, then the quad is a parallelogram
        // 3. If one pair of opposite sides of a quad are congruent and parallel, then the quad is a parallelogram
        // 4. If diagonals of a quad bisect each other, then the quad is a parallelogram
        // 5. If both pairs of opposite angles of a quad are congruent, then the quad is a parallelogram
        public Page166Problem01to05(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 166 Problem 01 to 05";

            Point a = new Point("A", 6, 2); points.Add(a);
            Point c = new Point("C", 5, -2); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point s = new Point("S", -5, 2); points.Add(s);
            Point k = new Point("K", -6, -2); points.Add(k);

            Segment sa = new Segment(s, a); segments.Add(sa);
            Segment kc = new Segment(k, c); segments.Add(kc);
            Segment sk = new Segment(s, k); segments.Add(sk);
            Segment ac = new Segment(a, c); segments.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            List<Point> pts2 = new List<Point>();
            pts2.Add(s);
            pts2.Add(o);
            pts2.Add(c);
            collinear.Add(new Collinear(pts2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            //problem 01 - demonstrates (1)
            given.Add(new GeometricParallel(sa, kc));
            given.Add(new GeometricParallel(sk, ac));

            //problem 02 - demonstrates (2)
            given.Add(new GeometricCongruentSegments(sa, kc));
            given.Add(new GeometricCongruentSegments(sk, ac));

            //Combination of givens from Problem 01 and 02 demonstrates (3)

            //problem 04 - demonstrates (4)
            //Segment sc = (Segment)parser.Get(new Segment(s, c));
            //Segment ka = (Segment)parser.Get(new Segment(k, a));
            //given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(o, sc))));
            //given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(o, ka))));

            //problem 05 - demonstrates (5)
            //given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(s, k, c)), (Angle)parser.Get(new Angle(c, a, s))));
            //given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(k, c, a)), (Angle)parser.Get(new Angle(a, s, k))));

            Quadrilateral quad = (Quadrilateral) parser.Get(new Quadrilateral(sk, ac, sa, kc));
            goals.Add(new Strengthened(quad, new Parallelogram(quad)));
    }
    }
}
