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

            problems.Add(new Page156Problem34(true, false));  // GTG
            problems.Add(new Page156Problem35(true, true));  // GTG
            problems.Add(new Page156Problem36(true, false));  // GTG
            problems.Add(new Page164Problem36(true, false));  // GTG
            problems.Add(new Page164Problem44(true, false));  // GTG
            problems.Add(new Page156Problem37(true, false));  // GTG
            problems.Add(new Page226Problem41(true, false));  // GTG
            problems.Add(new Page226Problem42(true, true));  // GTG
            problems.Add(new Page226Problem43(true, true));  // GTG
            problems.Add(new Page226Problem44(true, false));  // GTG
            problems.Add(new Page231Problem14(true, true));  // GTG
            problems.Add(new Page273Problem39(true, false));  // GTG
            problems.Add(new Page273Problem40(true, true));  // GTG


//            problems.Add(new Page231Picture(true, false));  // Encoding of a point

            return problems;
        }
    }
}
