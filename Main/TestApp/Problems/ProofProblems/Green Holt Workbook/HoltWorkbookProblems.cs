using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class HoltWorkbookProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page13Problem10(false, false));  // GTG
            problems.Add(new Page23Problem9(false, true));   // GTG
            problems.Add(new Page24Problem7(false, true));   // GTG
            problems.Add(new Page25Problem8(false, false));   // GTG
            problems.Add(new Page26Problem2(false, true));   // GTG
            problems.Add(new Page17Problem9(false, false));   // GTG

            return problems;
        }
    }
}
