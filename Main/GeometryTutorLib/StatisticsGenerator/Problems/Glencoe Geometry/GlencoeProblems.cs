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

            problems.Add(new Page156Problem34(false));  // GTG
            problems.Add(new Page156Problem35(false));  // GTG
            problems.Add(new Page156Problem36(false));  // GTG
            problems.Add(new Page164Problem36(false));  // GTG
            problems.Add(new Page164Problem44(false));  // GTG
            problems.Add(new Page156Problem37(false));  // GTG
            problems.Add(new Page226Problem41(false));  // GTG
            problems.Add(new Page226Problem42(false));  // GTG
            problems.Add(new Page226Problem43(false));  // GTG
            problems.Add(new Page226Problem44(false));  // GTG
            problems.Add(new Page231Problem14(false));  // GTG
            problems.Add(new Page273Problem39(false));  // GTG
            problems.Add(new Page273Problem40(false));  // GTG


            problems.Add(new Page231Picture(false));  // Encoding of a point

            return problems;
        }
    }
}
