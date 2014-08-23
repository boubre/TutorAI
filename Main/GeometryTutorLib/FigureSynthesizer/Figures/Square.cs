using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Square : Rhombus
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Quadrilateral> quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                if (quad.VerifySquare())
                {
                    Square square = new Square(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, square);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, square.points, square));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        //
        // Construct and return all constraints.
        //
        public override List<Constraint> GetConstraints()
        {
            List<Constraint> constraints = new List<Constraint>();

            //
            // Acquire the 'midpoint' equations from the polygon class.
            //
            constraints.AddRange(Constraint.MakeEquationsIntoConstraints(GetGranularMidpointEquations()));

            //
            // Create all relationship equations among the sides of the square.
            //
            for (int s1 = 0; s1 < orderedSides.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < orderedSides.Count; s2++)
                {
                    constraints.Add(new CongruenceConstraint(new GeometricCongruentSegments(orderedSides[s1], orderedSides[s2])));
                }
            }

            //
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            for (int a1 = 0; a1 < angles.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < angles.Count; a2++)
                {
                    constraints.Add(new EquationConstraint(new GeometricAngleEquation(angles[a1], angles[a2])));
                }
            }

            return constraints;
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Square ConstructDefaultSquare()
        {
            int length = Figure.DefaultSideLength();

            Point topLeft = PointFactory.GeneratePoint(0, length);
            Point topRight = PointFactory.GeneratePoint(length, length);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Square(left, right, top, bottom);
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            List<Segment> variables = new List<Segment>();

            variables.Add(orderedSides[0]);
            
            return variables;
        }
    }
}
