using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Quadrilateral : Polygon
    {
        //
        // Acquire the list of all possible quadrilaterals 
        //
        public static List<Quadrilateral> GetQuadrilateralsFromPoints(List<Point> points)
        {
            List<Quadrilateral> quads = new List<Quadrilateral>();

            for (int p1 = 0; p1 < points.Count - 3; p1++)
            {
                for (int p2 = p1 + 1; p2 < points.Count - 2; p2++)
                {
                    for (int p3 = p2 + 1; p3 < points.Count - 1; p3++)
                    {
                        // Disallow collinearity of the first 3 points.
                        if (!points[p1].Collinear(points[p2], points[p3]))
                        {
                            for (int p4 = p3 + 1; p4 < points.Count; p4++)
                            {
                                if (!points[p4].Collinear(points[p2], points[p1]) &&
                                    !points[p4].Collinear(points[p2], points[p3]) &&
                                    !points[p4].Collinear(points[p1], points[p3]))
                                {
                                    quads.Add(Quadrilateral.MakeQuadrilateral(points[p1], points[p2], points[p3], points[p4]));
                                }
                            }
                        }
                    }
                }
            }

            return quads;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Quadrilateral ConstructDefaultQuadrilateral()
        {
            return null;
        }
    }
}
