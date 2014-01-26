using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.StatisticsGenerator
{
    public static class GlencoeProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new Page156Problem34(true));  // GTG
            problems.Add(new Page156Problem35(true));  // GTG
            problems.Add(new Page156Problem36(true));  // GTG
            problems.Add(new Page164Problem36(true));  // GTG
            problems.Add(new Page164Problem44(true));  // GTG
            problems.Add(new Page156Problem37(true));  // GTG

            return problems;
        }
    }
}
