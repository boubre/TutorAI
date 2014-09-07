using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Rectangle : Parallelogram
    {
        public override double CoordinatizedArea()
        {
            return this.orderedSides[0].Length * this.orderedSides[1].Length;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Quadrilateral> quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                if (quad.VerifyRectangle())
                {
                    Rectangle rect = new Rectangle(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, rect);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, rect.points, rect));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Quadrilateral ConstructDefaultRectangle()
        {
            int length = Figure.DefaultSideLength();
            int width = Figure.DefaultSideLength();

            // Ensure we don't have a square.
            while (width == length)
            {
                length = Figure.DefaultSideLength();
                width = Figure.DefaultSideLength();
            }

            Point topLeft = PointFactory.GeneratePoint(0, width);
            Point topRight = PointFactory.GeneratePoint(length, width);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Rectangle(left, right, top, bottom);
        }

        //
        // Construct and return all constraints.
        //
        public override List<Constraint> GetConstraints()
        {
            List<Equation> eqs = new List<Equation>();
            List<Congruent> congs = new List<Congruent>();

            //
            // Acquire the 'midpoint' equations from the polygon class.
            //
            GetGranularMidpointEquations(out eqs, out congs);

            //
            // Create all relationship equations among the sides of the rectangle.
            //
            congs.Add(new GeometricCongruentSegments(orderedSides[0], orderedSides[2]));
            congs.Add(new GeometricCongruentSegments(orderedSides[1], orderedSides[3]));

            //
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            for (int a1 = 0; a1 < angles.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < angles.Count; a2++)
                {
                    eqs.Add(new GeometricAngleEquation(angles[a1], angles[a2]));
                }
            }

            List<Constraint> constraints = Constraint.MakeEquationsIntoConstraints(eqs);
            constraints.AddRange(Constraint.MakeCongruencesIntoConstraints(congs));

            return constraints;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            List<Segment> variables = new List<Segment>();

            variables.Add(orderedSides[0]);
            variables.Add(orderedSides[1]);

            return variables;
        }
    }
}
