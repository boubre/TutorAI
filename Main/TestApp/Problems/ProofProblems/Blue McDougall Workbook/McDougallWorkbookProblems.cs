using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class McDougallWorkbookProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page37Problem2(false, false));         // GTG
            problems.Add(new Page41Problem15(false, false));        // GTG
            problems.Add(new Page42Problem16(false, false));        // GTG
            problems.Add(new Page48Problem23To31(false, false));    // GTG
            problems.Add(new Page66Problem16(false, true));        // GTG : 1(5) Subset of givens required to prove same result
            problems.Add(new Page68Problem13(false, false));        // GTG
            problems.Add(new Page69Problem14(false, true));        // GTG
            problems.Add(new Page72Problem17(false, true));        // GTG
            problems.Add(new Page73Problem8(false, false));         // GTG
            problems.Add(new Page73Problem9(false, true));         // GTG
            problems.Add(new Page74Problem14To16(false, false));    // 3(9) Uses a subset of the givens to prove the same result
            problems.Add(new Page75Problem17(false, false));        // GTG
            problems.Add(new Page75Problem18(false, false));        // GTG
            problems.Add(new Page76Problem7(false, true));         // GTG
            problems.Add(new Page76Problem4(false, false));         // GTG
            problems.Add(new Page76Problem8(false, true));         // GTG
            problems.Add(new Page77Problem11(false, false));        // GTG
            problems.Add(new Page78Problem12(false, true));        // GTG
            problems.Add(new Page78Problem13(false, true));        // GTG
            problems.Add(new Page79Problem7(false, false));         // GTG
            problems.Add(new Page79Problem8(false, true));         // GTG
            problems.Add(new Page80Problem9(false, true));          // GTG
            problems.Add(new Page80Problem10(false, false));        // GTG
            problems.Add(new Page90Problem22(false, true));        // GTG
            problems.Add(new Page90Problem23(false, false));        // GTG    BACKWARD only works if <= 2 givens

            return problems;
        }
    }
}
