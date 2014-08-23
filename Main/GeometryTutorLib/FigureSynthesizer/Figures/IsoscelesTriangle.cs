using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class IsoscelesTriangle : Triangle
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Triangle> tris = Triangle.GetTrianglesFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Triangle tri in tris)
            {
                if (tri.IsIsosceles())
                {
                    IsoscelesTriangle isoTri = new IsoscelesTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, isoTri);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, isoTri.points, isoTri));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static IsoscelesTriangle ConstructDefaultIsoscelesTriangle()
        {
            int baseLength = Figure.DefaultSideLength();
            int height = Figure.DefaultSideLength();

            Point top = PointFactory.GeneratePoint(baseLength / 2.0, height);
            Point bottomRight = PointFactory.GeneratePoint(baseLength, 0);

            Segment left = new Segment(origin, top);
            Segment right = new Segment(top, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new IsoscelesTriangle(left, right, bottom);
        }
    }
}
