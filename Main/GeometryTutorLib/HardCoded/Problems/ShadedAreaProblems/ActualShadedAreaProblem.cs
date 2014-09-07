using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.TutorParser;

namespace GeometryTutorLib.GeometryTestbed
{
    public abstract class ActualShadedAreaProblem : ActualProblem
    {
        // Atomic regions
        public List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> goalRegions { get; protected set; }

        // Known values stated in the problem.
        public GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator known { get; protected set; }

        private double solutionArea;
        public void SetSolutionArea(double a) { solutionArea = a; }
        public double GetSolutionArea() { return solutionArea; }

        //
        // Statistics Generation
        //
        // All statistics returned from the analysis
        protected StatisticsGenerator.ShadedAreaFigureStatisticsAggregator figureStats;

        //
        // Aggregation variables for all <figure, given, goal> pairings.
        //
        public static int TotalPoints = 0;
        public static int TotalSegments = 0;
        public static int TotalInMiddle = 0;
        public static int TotalIntersections = 0;
        public static int TotalAngles = 0;
        public static int TotalTriangles = 0;
        public static int TotalQuadrilaterals = 0;
        public static int TotalCircles = 0;
        public static int TotalImplicitFacts = 0;
        public static int TotalExplicitFacts = 0;

        public static int TotalShapes = 0;
        public static int TotalRootShapes = 0;
        public static int TotalAtomicRegions = 0;

        public static int TotalInteresting = 0;
        public static int TotalComplete = 0;
        public static int TotalOriginalInteresting = 0;

        public static System.TimeSpan TotalTime = new System.TimeSpan();
        public static System.TimeSpan TotalImplicitTime = new System.TimeSpan();
        public static System.TimeSpan TotalDeductionTime = new System.TimeSpan();
        public static System.TimeSpan TotalSolverTime = new System.TimeSpan();

        public System.TimeSpan ImplicitTiming;
        public System.TimeSpan DeductionTiming;
        public System.TimeSpan SolverTiming;

        public int numInteresting = 0;

        public ActualShadedAreaProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
            known = new GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator();

            solutionArea = -1;
        }

        public override void Run()
        {
            if (!this.problemIsOn) return;

            // We have already parsed the figure; acquire the timing.
            ImplicitTiming = this.parser.implied.GetTiming();

            // Map the set of clauses from parser to one set of intrinsic clauses.
            ConstructIntrinsicSet();

            // Create the analyzer
            HardCodedShadedAreaMain analyzer = new HardCodedShadedAreaMain(intrinsic, given, known, goalRegions, this.parser.implied, GetSolutionArea());

            // Perform and time the analysis
            figureStats = analyzer.AnalyzeFigure();

            // Get the deduction and solving times.
            DeductionTiming = analyzer.GetDeductionTiming();
            SolverTiming = analyzer.GetSolverTiming();

            // Determine completeness
            this.isComplete = analyzer.IsComplete();

            //
            // Determine the number of interesting problems from this figure.
            //
            int interestingComputable = 0;
            int uninterestingComputable = 0;
            int interestingIncomputable = 0;
            int uninterestingIncomputable = 0;
            analyzer.GetComputableInterestingCount(out interestingComputable, out uninterestingComputable,
                                                   out interestingIncomputable, out uninterestingIncomputable);

            if (Utilities.SHADED_AREA_SOLVER_DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Interesting Computable: " + interestingComputable);
                System.Diagnostics.Debug.WriteLine("UNinteresting Computable: " + uninterestingComputable);
                System.Diagnostics.Debug.WriteLine("Interesting INcomputable: " + interestingIncomputable);
            }

            numInteresting = interestingComputable;

            //
            // Add to the cumulative statistics
            //
            ActualShadedAreaProblem.TotalTime = ActualShadedAreaProblem.TotalTime.Add(figureStats.stopwatch.Elapsed);
            ActualShadedAreaProblem.TotalImplicitTime = ActualShadedAreaProblem.TotalImplicitTime.Add(ImplicitTiming);
            ActualShadedAreaProblem.TotalDeductionTime = ActualShadedAreaProblem.TotalDeductionTime.Add(DeductionTiming);
            ActualShadedAreaProblem.TotalSolverTime = ActualShadedAreaProblem.TotalSolverTime.Add(SolverTiming);

            ActualShadedAreaProblem.TotalPoints += figureStats.numPoints;
            ActualShadedAreaProblem.TotalSegments += figureStats.numPoints;
            ActualShadedAreaProblem.TotalInMiddle += figureStats.numInMiddle;
            ActualShadedAreaProblem.TotalAngles += figureStats.numAngles;
            ActualShadedAreaProblem.TotalTriangles += figureStats.numTriangles;
            ActualShadedAreaProblem.TotalIntersections += figureStats.numIntersections;

            ActualShadedAreaProblem.TotalImplicitFacts += figureStats.totalImplicitFacts;
            ActualShadedAreaProblem.TotalExplicitFacts += figureStats.totalExplicitFacts;

            ActualShadedAreaProblem.TotalShapes += figureStats.numShapes;
            ActualShadedAreaProblem.TotalRootShapes += figureStats.numRootShapes;
            ActualShadedAreaProblem.TotalAtomicRegions += figureStats.numAtomicRegions;

            ActualShadedAreaProblem.TotalInteresting += numInteresting;

            if (isComplete) ActualShadedAreaProblem.TotalComplete++;
            if (figureStats.originalProblemInteresting) ActualShadedAreaProblem.TotalOriginalInteresting++;
        }

        public override string ToString()
        {
            string output = "";

            output += problemName + "\t\t";

            output += figureStats.totalImplicitFacts + "\t\t\t";
            output += figureStats.totalExplicitFacts + "\t\t   ";

            output += figureStats.numShapes + "\t";
            output += figureStats.numRootShapes + "\t\t";
            output += figureStats.numAtomicRegions + "\t\t\t";

            output += Utilities.TimeToString(ImplicitTiming) + "\t\t";
            output += Utilities.TimeToString(DeductionTiming) + "\t\t";
            output += Utilities.TimeToString(SolverTiming) + "\t";

            output += numInteresting + "\t\t";
            output += isComplete + "\t\t\t";

            output += figureStats.originalProblemInteresting;

            return output;
        }
    }
}