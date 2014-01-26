using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.StatisticsGenerator
{
    public static class HoltWorkbookProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new Page13Problem10(false));  // GTG
            problems.Add(new Page23Problem9(false));   // GTG
            problems.Add(new Page24Problem7(false));   // GTG
            problems.Add(new Page25Problem8(false));   // GTG
            problems.Add(new Page26Problem2(false));   // GTG
            problems.Add(new Page17Problem9(false));   // GTG

            return problems;
        }
    }
}
