using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.TutorParser;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class FigSynthShadedAreaProblem : ActualShadedAreaProblem
    {
        public FigSynthShadedAreaProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
        }

        public void SetPoints(List<Point> pts) { this.points = pts; }
        public void SetSegments(List<Segment> segs) { this.segments = segs; }
        public void SetCollinear(List<Collinear> coll) { this.collinear = coll; }
        public void SetCircles(List<Circle> circs) { this.circles = circs; }
        public void SetWantedRegions(List<AtomicRegion> atoms) { this.goalRegions = atoms; }
        public void SetKnowns(Area_Based_Analyses.KnownMeasurementsAggregator k) { this.known = k; }
        public void SetGivens(List<GroundedClause> gs) { this.given = gs; }

        public void InvokeParser()
        {
            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, true);
        }

        public List<AtomicRegion> GetRemainingRegionsFromParser(FigSynthProblem problem)
        {
            // Acquire all the figures.
            List<Figure> figures = problem.CollectFigures();

            // Remove the outer-most figures.
            figures.RemoveAt(0);

            // Acquire the remaining atomic regions.
            return parser.implied.GetAtomicRegionsNotByFigures(figures);
        }

        public UIProblemDrawer.ProblemDescription MakeUIProblemDescription()
        {
            UIProblemDrawer.ProblemDescription desc = new UIProblemDrawer.ProblemDescription();

            desc.Points = this.points;
            desc.Segments = this.segments;
            desc.Circles = this.circles;
            desc.Regions = this.goalRegions;

            return desc;
        }
    }
}