using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace GeometryTutorLib.StatisticsGenerator
{
    public class FigureStatisticsAggregator
    {
        public int totalProblemsGenerated;
        public int totalBackwardProblemsGenerated;
        public int totalBookProblemsGenerated;
        public int totalInterestingProblems;
        public int numInterestingPartitions;

        public double averageProblemLength;
        public double averageProblemWidth;
        public double averageProblemDeductiveSteps;

        // A list of <goal type, number of problems>
        public List<KeyValuePair<GeometryTutorLib.ConcreteAST.GroundedClause, int>> goalPartitionSummary;
        public List<int> sourcePartitionSummary;
        public Dictionary<int, int> difficultyPartitionSummary; // Pair is: <upperBound value, number of problems>

        // For timing the processing of each figure
        public GeometryTutorLib.Stopwatch stopwatch;

        public FigureStatisticsAggregator()
        {
            totalProblemsGenerated = -1;
            totalBackwardProblemsGenerated = -1;
            totalBookProblemsGenerated = -1;
            totalInterestingProblems = -1;
            numInterestingPartitions = -1;
            averageProblemDeductiveSteps = -1;
            averageProblemLength = -1;
            averageProblemWidth = -1;

            goalPartitionSummary = new List<KeyValuePair<ConcreteAST.GroundedClause, int>>();
            sourcePartitionSummary = new List<int>();
            difficultyPartitionSummary = new Dictionary<int, int>();
 
            stopwatch = new GeometryTutorLib.Stopwatch();
        }
    }
}
