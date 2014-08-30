using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Circle : Figure
    {
        public override double CoordinatizedArea() { return radius * radius * Math.PI;  }

        public override bool CoordinateCongruent(Figure that)
        {
            Circle thatCirc = that as Circle;
            if (thatCirc == null) return false;

            return Utilities.CompareValues(this.radius, thatCirc.radius);
        }

        public static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static List<FigSynthProblem> AppendShape(List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static Circle ConstructDefaultCircle()
        {
            int radius = Figure.DefaultSideLength() / 2;

            return new Circle(origin, radius);
        }

        //
        // Specify how many points are required to atomize each circle.
        //
        public static Dictionary<Circle, int> AcquireCircleGranularity(List<Circle> circles)
        {
            Dictionary<Circle, int> granularity = new Dictionary<Circle,int>();

            if (!circles.Any()) return granularity;

            circles = new List<Circle>(circles.OrderBy(c => c.radius));

            // Construct the granularity for when we construct arcs.
            double currRadius = circles[0].radius;
            int gran = 1;

            foreach (Circle circle in circles)
            {
                if (circle.radius > currRadius) gran++;
                granularity[circle] = gran;
            }

            return granularity;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables() { throw new NotImplementedException(); }
    }
}