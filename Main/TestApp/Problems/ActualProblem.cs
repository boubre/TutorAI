using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.StatisticsGenerator
{
    public abstract class ActualProblem
    {
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
        public static int TotalGoals = 0;
        public static int TotalProblemsGenerated = 0;
        public static int TotalBackwardProblemsGenerated = 0;
        public static int TotalStrictInterestingProblems = 0;
        public static int TotalInterestingProblems = 0;
        public static int TotalOriginalBookProblems = 0;

        public static double TotalProblemWidth = 0;
        public static double TotalProblemLength = 0;
        public static double TotalDeducedSteps = 0;

        public static double TotalStrictProblemWidth = 0;
        public static double TotalStrictProblemLength = 0;
        public static double TotalStrictDeducedSteps = 0;

        public static int TotalGoalPartitions = 0;
        public static int TotalSourcePartitions = 0;
        public static int[] totalDifficulty = new int[ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds().Count + 1];
        public static int[] totalInteresting = new int[ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds().Count + 1];
        public static int[] totalStrictDifficulty = new int[ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds().Count + 1];
        public static int[] totalStrictInteresting = new int[ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds().Count + 1];

        // Hard-coded intrinsic problem characteristics
        protected List<GroundedClause> intrinsic;

        // Boolean facts
        protected List<GroundedClause> given;

        // Boolean the book problem is attempting to prove; use in validation of figures / generated problems
        // One <figure, given> pair may contain multiple goals
        protected List<GroundedClause> goals;

        // All statistics returned from the analysis
        protected FigureStatisticsAggregator figureStats;

        // Formatted Labeling of the Problem
        protected string problemName;

        public bool problemIsOn { get; private set; }

        //
        // For constructing clauses consistent with the UI.
        //
        protected LiveGeometry.TutorParser.HardCodedParserMain parser;

        protected List<Point> points = new List<Point>();
        protected List<Collinear> collinear = new List<Collinear>();
        protected List<Segment> segments = new List<Segment>();
        protected List<Circle> circles = new List<Circle>();



        public const bool INCOMPLETE = false;
        public const bool COMPLETE = true;
        public bool isComplete;

        public ActualProblem(bool runOrNot, bool comp)
        {
            intrinsic = new List<GroundedClause>();
            given = new List<GroundedClause>();
            goals = new List<GroundedClause>();

            problemName = "TODO: NAME ME" + this.GetType();
            problemIsOn = runOrNot;

            isComplete = comp;
        }

        public void Run()
        {
            // Map the set of clauses from parser to one set of intrinsic clauses.
            ConstructIntrinsicSet();

            // Create the analyzer
            GeometryTutorLib.StatisticsGenerator.HardCodedFigureAnalyzerMain analyzer = new GeometryTutorLib.StatisticsGenerator.HardCodedFigureAnalyzerMain(intrinsic, given, goals);

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
            for (int k = 1; k <= FigureStatisticsAggregator.MAX_K; k++)
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
            ActualProblem.TotalTime = ActualProblem.TotalTime.Add(figureStats.stopwatch.Elapsed);

            ActualProblem.TotalPoints += figureStats.numPoints;
            ActualProblem.TotalSegments += figureStats.numPoints;
            ActualProblem.TotalInMiddle += figureStats.numInMiddle;
            ActualProblem.TotalAngles += figureStats.numAngles;
            ActualProblem.TotalTriangles += figureStats.numTriangles;
            ActualProblem.TotalIntersections += figureStats.numIntersections;
            ActualProblem.TotalTotalProperties += figureStats.totalProperties;

            ActualProblem.TotalExplicitFacts += figureStats.totalExplicitFacts;

            ActualProblem.TotalGoals += goals.Count;
            ActualProblem.TotalProblemsGenerated += figureStats.totalProblemsGenerated;
            ActualProblem.TotalBackwardProblemsGenerated += figureStats.totalBackwardProblemsGenerated;

            // Query: Interesting Partitioning
            int numProblemsInPartition;
            List<int> upperBounds = ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProblem.totalInteresting[i] += figureStats.interestingPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProblem.totalInteresting[upperBounds.Count] += figureStats.interestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;

            upperBounds = ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProblem.totalStrictInteresting[i] += figureStats.strictInterestingPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProblem.totalStrictInteresting[upperBounds.Count] += figureStats.strictInterestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;


            // Rest of Cumulative Stats
            ActualProblem.TotalStrictInterestingProblems += figureStats.totalStrictInterestingProblems;
            ActualProblem.TotalInterestingProblems += figureStats.totalInterestingProblems;
            ActualProblem.TotalOriginalBookProblems += goals.Count;

            // Averages
            ActualProblem.TotalDeducedSteps += figureStats.averageProblemDeductiveSteps * figureStats.totalInterestingProblems;
            ActualProblem.TotalProblemLength += figureStats.averageProblemLength * figureStats.totalInterestingProblems;
            ActualProblem.TotalProblemWidth += figureStats.averageProblemWidth * figureStats.totalInterestingProblems;
            ActualProblem.TotalStrictDeducedSteps += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemDeductiveSteps * figureStats.totalStrictInterestingProblems;
            ActualProblem.TotalStrictProblemLength += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemLength * figureStats.totalStrictInterestingProblems;
            ActualProblem.TotalStrictProblemWidth += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemWidth * figureStats.totalStrictInterestingProblems;

            // Queries
            ActualProblem.TotalGoalPartitions += figureStats.goalPartitionSummary.Count;
            ActualProblem.TotalSourcePartitions += figureStats.sourcePartitionSummary.Count;

            // Query: Difficulty Partitioning
            upperBounds = ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProblem.totalDifficulty[i] += figureStats.difficultyPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProblem.totalDifficulty[upperBounds.Count] += figureStats.difficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;

            upperBounds = ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProblem.totalStrictDifficulty[i] += figureStats.strictDifficultyPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProblem.totalStrictDifficulty[upperBounds.Count] += figureStats.strictDifficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
        }

        private void ConstructIntrinsicSet()
        {
            parser.implied.allEvidentPoints.ForEach(pt => intrinsic.Add(pt));
            parser.implied.segments.ForEach(seg => intrinsic.Add(seg));
            parser.implied.inMiddles.ForEach(im => intrinsic.Add(im));
            parser.implied.angles.ForEach(angle => intrinsic.Add(angle));
            parser.implied.ssIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.minorArcs.ForEach(arc => intrinsic.Add(arc));
            parser.implied.circles.ForEach(circ => intrinsic.Add(circ));
            parser.implied.csIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.ccIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.arcInMiddle.ForEach(im => intrinsic.Add(im));

            foreach (List<Polygon> polyList in parser.implied.polygons)
            {
                polyList.ForEach(poly => intrinsic.Add(poly));
            }
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
            statsString += figureStats.totalBookProblemsGenerated + "\t";
            statsString += figureStats.totalProblemsGenerated + "\t";
            statsString += figureStats.totalInterestingProblems + "\t";
            statsString += figureStats.totalStrictInterestingProblems + "\t";
            statsString += figureStats.totalBackwardProblemsGenerated + "\t";

            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemWidth);
            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemLength);
            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemDeductiveSteps);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemWidth);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemLength);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemDeductiveSteps);

            // Format and display the elapsed time for this problem
            statsString += System.String.Format("{0:00}:{1:00}.{2:00}",
                                                figureStats.stopwatch.Elapsed.Minutes,
                                                figureStats.stopwatch.Elapsed.Seconds, figureStats.stopwatch.Elapsed.Milliseconds / 10);

            //
            // Sample Query Output
            //
            // Goal Isomorphism
            //
            statsString += "\t\t" + figureStats.goalPartitionSummary.Count + "\t";

            // Source Isomorphism
            statsString += figureStats.sourcePartitionSummary.Count + "\t|\t";

            // Difficulty Partitioning
            int numProblemsInPartition;
            foreach (int upperBound in ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds())
            {
                statsString +=  figureStats.difficultyPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.difficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";

            foreach (int upperBound in ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds())
            {
                statsString += figureStats.strictDifficultyPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.strictDifficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";


            // Interesting Partitioning
            foreach (int upperBound in ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds())
            {
                statsString += figureStats.interestingPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.interestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";

            // Strict Interesting Partitioning
            foreach (int upperBound in ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds())
            {
                statsString += figureStats.strictInterestingPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.strictInterestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t";

            return statsString;
        }
    }
}