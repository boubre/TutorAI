using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Parallelogram : Quadrilateral
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Quadrilateral> quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                if (quad.VerifyParallelogram())
                {
                    IsoscelesTrapezoid para = new IsoscelesTrapezoid(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, para);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, para.points, para));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Parallelogram ConstructDefaultParallelogram()
        {
            int baseLength = Figure.DefaultSideLength();
            int height = Figure.DefaultSideLength();
            int offset = Figure.SmallIntegerValue();

            Point topLeft = PointFactory.GeneratePoint(offset, height);
            Point topRight = PointFactory.GeneratePoint(offset + baseLength, height);
            Point bottomRight = PointFactory.GeneratePoint(baseLength, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Parallelogram(left, right, top, bottom);
        }
    }
}
