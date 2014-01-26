using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GeometryTestbed
{
    public class MainProgram
    {
        private static List<GeometryTutorLib.StatisticsGenerator.ActualProblem> ConstructAllHardCodedProblems()
        {
            List<GeometryTutorLib.StatisticsGenerator.ActualProblem> problems = new List<GeometryTutorLib.StatisticsGenerator.ActualProblem>();

            problems.AddRange(GeometryTutorLib.StatisticsGenerator.JurgensenProblems.GetProblems());
            problems.AddRange(GeometryTutorLib.StatisticsGenerator.GlencoeProblems.GetProblems());
            problems.AddRange(GeometryTutorLib.StatisticsGenerator.IndianTextProblems.GetProblems());
            problems.AddRange(GeometryTutorLib.StatisticsGenerator.HoltWorkbookProblems.GetProblems());
            problems.AddRange(GeometryTutorLib.StatisticsGenerator.McDougallProblems.GetProblems());
            problems.AddRange(GeometryTutorLib.StatisticsGenerator.McDougallWorkbookProblems.GetProblems());

            return problems;
        }

        private static void DumpStatisticsHeader()
        {
            string header = "";
            header += "Problem #\t";
            header += "Name\t\t\t";
            header += "# Book Probs\t";
            header += "# Probs\t";
            header += "# of Int Probs\t";
            header += "Ave Width\t";
            header += "Ave Length\t";
            header += "Ave Deductive\t";
            header += "Time To Generate\t";
            header += "# Goal Partitions\t";
            header += "# Src Partitions\t\t";
            header += "Difficulty Partitions: ";
            List<int> difficultyPartitions = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            header += "0-" + difficultyPartitions[0] + "\t";
            for (int i = 0; i < difficultyPartitions.Count - 1; i++)
            {
                header += (difficultyPartitions[i] + 1) + "-" + difficultyPartitions[i + 1] + "\t";
            }
            header += ">=" + (difficultyPartitions[difficultyPartitions.Count - 1] + 1) + "\t";

            Debug.WriteLine(header);
        }

        static void Main(string[] args)
        {
            List<GeometryTutorLib.StatisticsGenerator.ActualProblem> problems = ConstructAllHardCodedProblems();

            DumpStatisticsHeader();

            int problemCount = 0;
            foreach (GeometryTutorLib.StatisticsGenerator.ActualProblem problem in problems)
            {
                if (problem.problemIsOn) // We may turn problems on / off: check on
                {
                    problem.Run();

                    Debug.Write(++problemCount + "\t\t");
                    Debug.Write(problem.ToString() + "\n");
                }
            }

            DumpAggregateTotals(problemCount);
        }

        private static void DumpAggregateTotals(int numFigures)
        {
            string output = "";

            output += "----------------------------------------- Summary -----------------------------------------\n";

            output += numFigures + "\t\t\t\t\t\t\t";
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalOriginalBookProblems + "\t";
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalGoals + "\t";
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalProblemsGenerated + "\t";
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalInterestingProblems + "\t";

            output += System.String.Format("{0:N2}\t", GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalProblemWidth / GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalProblemLength / GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalDeducedSteps / GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalInterestingProblems);

            output += System.String.Format("{0:00}:{1:00}.{2:00}",
                                           GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalTime.Minutes,
                                           GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalTime.Seconds,
                                           GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalTime.Milliseconds / 10);

            // Queries
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalGoalPartitions + "\t";
            output += GeometryTutorLib.StatisticsGenerator.ActualProblem.TotalSourcePartitions +"\t";

            // Query: Difficulty Partitioning
            foreach (int numProbs in GeometryTutorLib.StatisticsGenerator.ActualProblem.totalDifficulty)
            {
                output += numProbs + "\t";
            }

            Debug.WriteLine(output);
        
        }
    }
}