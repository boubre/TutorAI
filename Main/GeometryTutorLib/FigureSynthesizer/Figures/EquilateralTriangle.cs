using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class EquilateralTriangle : IsoscelesTriangle
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Triangle> tris = Triangle.GetTrianglesFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Triangle tri in tris)
            {
                if (tri.IsEquilateral())
                {
                    EquilateralTriangle eqTri = new EquilateralTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, eqTri);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, eqTri.points, eqTri));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static EquilateralTriangle ConstructDefaultEquilateralTriangle()
        {
            int sideLength = Figure.DefaultSideLength();

            Point top = Figure.GetPointByLengthAndAngleInStandardPosition(sideLength, 60);
            top = PointFactory.GeneratePoint(top.X, top.Y);

            Point bottomRight = PointFactory.GeneratePoint(sideLength, 0);

            Segment left = new Segment(origin, top);
            Segment right = new Segment(top, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new EquilateralTriangle(left, right, bottom);
        }
    }
}
