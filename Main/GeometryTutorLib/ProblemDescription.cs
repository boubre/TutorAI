using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    /// <summary>
    /// A description of a problem for the back end to solve.
    /// This class is used to pass information from the front end to the back end.
    /// </summary>
    public class ProblemDescription
    {
        public List<GroundedClause> givens { get; set; }

        public List<Point> points { get; set; }
        public List<Collinear> collinear { get; set; }
        public List<Triangle> triangles { get; set; }
        public List<Segment> segments { get; set; }
        public List<InMiddle> inMiddles { get; set; }

        public ProblemDescription()
        {
        }
    }
}
