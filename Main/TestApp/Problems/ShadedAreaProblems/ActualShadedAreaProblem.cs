using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    public abstract class ActualShadedAreaProblem : ActualProblem
    {
        // Atomic regions
        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> goalRegions { get; protected set; }


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
        public static int TotalTotalProperties = 0;
        public static int TotalExplicitFacts = 0;

        public static System.TimeSpan TotalTime = new System.TimeSpan();

        public ActualShadedAreaProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>();
        }

        public override void Run()
        {
            if (!this.problemIsOn) return;



            // Map the set of clauses from parser to one set of intrinsic clauses.
            ConstructIntrinsicSet();

            // Create the analyzer
            StatisticsGenerator.HardCodedShadedAreaMain analyzer = new StatisticsGenerator.HardCodedShadedAreaMain(intrinsic, goalRegions, this.parser.implied);

            // Perform and time the analysis
            figureStats = analyzer.AnalyzeFigure();

            //
            // If we know it's complete, keep that overridden completeness.
            // Otherwise, determine completeness through analysis of the nodes in the hypergraph.
            //
            if (!this.isComplete) this.isComplete = figureStats.isComplete;

            System.Diagnostics.Debug.WriteLine("Resultant Complete: " + this.isComplete +"\n");


            // Calculate the final numbers: counts of the k-G Strictly interesting problems.
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            for (int k = 1; k <= StatisticsGenerator.FigureStatisticsAggregator.MAX_K; k++)
            {
                str.Append(figureStats.kGcardinalities[k] + "\t");
            }

            if (this.isComplete)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\complete.txt", true))
                //{
                //    file.WriteLine(str);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\completeTime.txt", true))
                //{
                //    file.WriteLine(figureStats.stopwatch.Elapsed);
                //}
            }
            else
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\interesting.txt", true))
                //{
                //    file.WriteLine(str);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\interestingTime.txt", true))
                //{
                //    file.WriteLine(figureStats.stopwatch.Elapsed);
                //}
            }

            // Add to the cumulative statistics
            ActualShadedAreaProblem.TotalTime = ActualShadedAreaProblem.TotalTime.Add(figureStats.stopwatch.Elapsed);

            ActualShadedAreaProblem.TotalPoints += figureStats.numPoints;
            ActualShadedAreaProblem.TotalSegments += figureStats.numPoints;
            ActualShadedAreaProblem.TotalInMiddle += figureStats.numInMiddle;
            ActualShadedAreaProblem.TotalAngles += figureStats.numAngles;
            ActualShadedAreaProblem.TotalTriangles += figureStats.numTriangles;
            ActualShadedAreaProblem.TotalIntersections += figureStats.numIntersections;
            ActualShadedAreaProblem.TotalTotalProperties += figureStats.totalProperties;

            ActualShadedAreaProblem.TotalExplicitFacts += figureStats.totalExplicitFacts;
        }

        public override string ToString()
        {
            string statsString = "";

            //
            // Totals and Averages
            //
            statsString += this.problemName + ":\t";

            statsString += figureStats.numPoints + "\t";
            statsString += figureStats.numSegments + "\t";
            statsString += figureStats.numInMiddle + "\t";
            statsString += figureStats.numIntersections + "\t";
            statsString += figureStats.numAngles + "\t";
            statsString += figureStats.numTriangles + "\t";
            statsString += figureStats.totalProperties + "\t";

            statsString += figureStats.totalExplicitFacts + "\t";

            statsString += this.goals.Count + "\t";

            // Format and display the elapsed time for this problem
            statsString += System.String.Format("{0:00}:{1:00}.{2:00}",
                                                figureStats.stopwatch.Elapsed.Minutes,
                                                figureStats.stopwatch.Elapsed.Seconds, figureStats.stopwatch.Elapsed.Milliseconds / 10);

            return statsString;
        }
    }
}