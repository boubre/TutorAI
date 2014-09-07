using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public partial class Triangle : Polygon
    {
        //
        // Acquire the list of all possible quadrilaterals 
        //
        public static List<Triangle> GetTrianglesFromPoints(List<Point> points)
        {
            List<Triangle> tris = new List<Triangle>();

            for (int p1 = 0; p1 < points.Count - 2; p1++)
            {
                for (int p2 = p1 + 1; p2 < points.Count - 1; p2++)
                {
                    for (int p3 = p2 + 1; p3 < points.Count; p3++)
                    {
                        if (!points[p1].Collinear(points[p2], points[p3]))
                        {
                            tris.Add(new Triangle(points[p1], points[p2], points[p3]));
                        }
                    }
                }
            }

            return tris;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Triangle ConstructDefaultTriangle()
        {
            Point other1 = PointFactory.GeneratePoint(Figure.GenerateIntValue(), 0);
            Point other2 = PointFactory.GeneratePoint(Figure.GenerateIntValue(), Figure.GenerateIntValue());

            return new Triangle(origin, other1, other2);
        }

        public override List<Segment> GetAreaVariables() { throw new NotImplementedException(); }
    }
}
