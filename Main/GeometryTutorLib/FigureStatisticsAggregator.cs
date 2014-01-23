using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib
{
    public class FigureStatisticsAggregator
    {
        //ProblemAnalyzer.QueryFeatureVector easyProblemDifficulty = new ProblemAnalyzer.QueryFeatureVector();
        //ProblemAnalyzer.QueryFeatureVector mediumProblemDifficulty = new ProblemAnalyzer.QueryFeatureVector();
        //ProblemAnalyzer.QueryFeatureVector hardProblemDifficulty = new ProblemAnalyzer.QueryFeatureVector();

        int totalProblemsGenerated;
        int totalBookProblemsGenerated;
        int totalInterestingProblems;

        double averagePathLength;
        double averagePathWidth;
    }
}
