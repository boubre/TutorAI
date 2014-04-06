using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.StatisticsGenerator
{
	//
	// Geometry; Page 223 Problem 23
	//
	public class Page223Problem23 : SimilarTrianglesProblem
	{
        public Page223Problem23(bool onoff) : base(onoff)
		{
            problemName = "Page 223 Problem 23";


            Point j = new Point("J", 0, 0); intrinsic.Add(j);
			Point i = new Point("I", 3, 3); intrinsic.Add(i);
			Point y = new Point("Y", 7, 7); intrinsic.Add(y);
			Point g = new Point("G", 6, 0); intrinsic.Add(g);
			Point z = new Point("Z", 7, 0); intrinsic.Add(z);

			Segment ig = new Segment(i, g); intrinsic.Add(ig);
            Segment yz = new Segment(y, z); intrinsic.Add(yz);

			List<Point> pts = new List<Point>();
			pts.Add(j);
			pts.Add(i);
			pts.Add(y);
			Collinear coll1 = new Collinear(pts);

			List<Point> pts2 = new List<Point>();
			pts2.Add(j);
			pts2.Add(g);
			pts2.Add(z);
			Collinear coll2 = new Collinear(pts2);

			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll1));
			intrinsic.AddRange(ClauseConstructor.GenerateSegmentClauses(coll2));
			intrinsic.AddRange(ClauseConstructor.GenerateAngleIntersectionPolygonClauses(intrinsic, onoff));

			given.Add(new GeometricCongruentAngles(ClauseConstructor.GetProblemAngle(intrinsic, new Angle(j, g, i)), ClauseConstructor.GetProblemAngle(intrinsic, new Angle(j, y, z))));

            goals.Add(new GeometricSimilarTriangles(ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(j, i, g)), ClauseConstructor.GetProblemTriangle(intrinsic, new Triangle(j, z, y))));
		}
	}
}