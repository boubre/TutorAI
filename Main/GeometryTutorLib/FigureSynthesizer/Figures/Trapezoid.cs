﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public partial class Trapezoid : Quadrilateral
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Quadrilateral> quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                if (quad.VerifyTrapezoid())
                {
                    Trapezoid trap = new Trapezoid(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, trap);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, trap.points, trap));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Trapezoid ConstructDefaultTrapezoid()
        {
            int smallBase = Figure.DefaultSideLength();
            int largeBase = Figure.DefaultSideLength();

            int height = Figure.DefaultSideLength();
            int offset = Figure.SmallIntegerValue();

            // Acquire a non-equal side.
            while (smallBase >= largeBase)
            {
                smallBase = Figure.DefaultSideLength();
                largeBase = Figure.DefaultSideLength();
            }

            Point topLeft = PointFactory.GeneratePoint(offset, height);
            Point topRight = PointFactory.GeneratePoint(offset + smallBase, height);
            Point bottomRight = PointFactory.GeneratePoint(largeBase, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Trapezoid(left, right, top, bottom);
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            throw new NotImplementedException();
        }
    }
}
