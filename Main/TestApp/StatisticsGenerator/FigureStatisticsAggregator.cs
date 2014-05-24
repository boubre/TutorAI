using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace StatisticsGenerator
{
    public class FigureStatisticsAggregator
    {
        // k-G Counts
        public const int MAX_K = 2;
        public int[] kGcardinalities = new int[MAX_K + 1];

        public bool isComplete;

        // Figure Properties (implicit facts)
        public int totalProperties;
        public int numInMiddle;
        public int numPoints;
        public int numSegments;
        public int numIntersections;
        public int numTriangles;
        public int numAngles;

        // Later
        public int numQuadrilaterals;
        public int numCircles;

        public int totalExplicitFacts;

        //
        public int totalProblemsGenerated;
        public int totalBackwardProblemsGenerated;
        public int totalBookProblemsGenerated;
        public int totalStrictInterestingProblems;
        public int totalInterestingProblems;
        public int numInterestingPartitions;

        public double averageProblemLength;
        public double averageProblemWidth;
        public double averageProblemDeductiveSteps;

        public double strictAverageProblemLength;
        public double strictAverageProblemWidth;
        public double strictAverageProblemDeductiveSteps;

        // A list of <goal type, number of problems>
        public List<KeyValuePair<GeometryTutorLib.ConcreteAST.GroundedClause, int>> goalPartitionSummary;
        public List<int> sourcePartitionSummary;
        public Dictionary<int, int> difficultyPartitionSummary; // Pair is: <upperBound value, number of problems>
        public Dictionary<int, int> interestingPartitionSummary; // Pair is: <upperBound value (% as int), number of problems>

        public Dictionary<int, int> strictDifficultyPartitionSummary; // Pair is: <upperBound value, number of problems>
        public Dictionary<int, int> strictInterestingPartitionSummary; // Pair is: <upperBound value (% as int), number of problems>

        // For timing the processing of each figure
        public GeometryTutorLib.Stopwatch stopwatch;

        public FigureStatisticsAggregator()
        {
            totalProperties = 0;
            numInMiddle = 0;
            numPoints = 0;
            numSegments = 0;
            numIntersections = 0;
            numTriangles = 0;
            numAngles = 0;
            numQuadrilaterals = 0;
            numCircles = 0;

            totalExplicitFacts = 0;

            totalProblemsGenerated = -1;
            totalBackwardProblemsGenerated = -1;
            totalBookProblemsGenerated = -1;
            totalStrictInterestingProblems = -1;
            totalInterestingProblems = -1;
            numInterestingPartitions = -1;
            averageProblemDeductiveSteps = -1;
            averageProblemLength = -1;
            averageProblemWidth = -1;
            strictAverageProblemDeductiveSteps = -1;
            strictAverageProblemLength = -1;
            strictAverageProblemWidth = -1;

            goalPartitionSummary = new List<KeyValuePair<GeometryTutorLib.ConcreteAST.GroundedClause, int>>();
            sourcePartitionSummary = new List<int>();
            difficultyPartitionSummary = new Dictionary<int, int>();
            interestingPartitionSummary = new Dictionary<int, int>();
            strictDifficultyPartitionSummary = new Dictionary<int, int>();
            strictInterestingPartitionSummary = new Dictionary<int, int>();
 
            stopwatch = new GeometryTutorLib.Stopwatch();
        }
    }
}
