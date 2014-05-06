﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
    //
    // Geometry; Page 37 Problem 2
    //
    public class Page37Problem2 : TransversalsProblem
    {
        public Page37Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 37 Problem 2";


            Point a = new Point("A", -4, 0); intrinsic.Add(a);
            Point b = new Point("B", 0, 0); intrinsic.Add(b);
            Point c = new Point("C", -4, 7); intrinsic.Add(c);

            Point x = new Point("X", 1, 3); intrinsic.Add(x);
            Point y = new Point("Y", 1, 0); intrinsic.Add(y);
            Point z = new Point("Z", 7, 0); intrinsic.Add(z);
            Point p = new Point("P", 5, 7); intrinsic.Add(p);

            Segment ab = new Segment(a, b); intrinsic.Add(ab);
            Segment bc = new Segment(b, c); intrinsic.Add(bc);
            Segment xy = new Segment(x, y); intrinsic.Add(xy);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);
            Segment yp = new Segment(y, p); intrinsic.Add(yp);

            intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

            given.Add(new Complementary(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, c)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, y, p))));
            Addition sum = new Addition(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(p, y, z)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(x, y, p)));
            given.Add(new GeometricAngleEquation(sum, new NumericValue(90)));

            goals.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(a, b, c)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(p, y, z))));
        }
    }
}