using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class RightTriangle : Triangle
    {
        public override double CoordinatizedArea()
        {
            Segment hyp = this.GetHypotenuse();
            Segment other1, other2;            
            GetOtherSides(hyp, out other1, out other2);

            return other1.Length * other2.Length / 2.0;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Triangle> tris = Triangle.GetTrianglesFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Triangle tri in tris)
            {
                if (tri.isRightTriangle())
                {
                    RightTriangle rTri = new RightTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, rTri);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, rTri.points, rTri));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static RightTriangle ConstructDefaultRightTriangle()
        {
            int length = Figure.DefaultSideLength();
            int width = Figure.DefaultSideLength();

            Point topLeft = PointFactory.GeneratePoint(0, width);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment hypotenuse = new Segment(topLeft, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new RightTriangle(left, bottom, hypotenuse);
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
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            eqs.Add(new GeometricAngleEquation(this.rightAngle, new NumericValue(90)));

            List<Constraint> constraints = Constraint.MakeEquationsIntoConstraints(eqs);
            constraints.AddRange(Constraint.MakeCongruencesIntoConstraints(congs));

            constraints.Add(new FigureConstraint(this));

            return constraints;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            Segment other1, other2;

            this.GetOtherSides(this.GetHypotenuse(), out other1, out other2);

            List<Segment> variables = new List<Segment>();
            variables.Add(other1);
            variables.Add(other2);

            return variables;
        }
    }
}
