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
            Debug.Write("Problem #\t");
            Debug.Write("Name\t\t\t");
            Debug.Write("# Book Probs\t");
            Debug.Write("# Probs\t");
            Debug.Write("# of Int Probs\t");
            Debug.Write("Ave Width\t");
            Debug.Write("Ave Length\t");
            Debug.Write("Ave Deductive\t");
            Debug.Write("Time To Generate");
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

            Debug.WriteLine(output);
        
        }
    }
}