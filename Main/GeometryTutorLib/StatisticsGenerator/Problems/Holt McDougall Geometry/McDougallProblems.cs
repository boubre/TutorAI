using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.StatisticsGenerator
{
    public static class McDougallProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new Page168Problem34(false));   // GTG
            problems.Add(new Page159Problem37(false));   // GTG
            problems.Add(new Page159Problem41(false));   // GTG
            problems.Add(new Page160Problem42(false));   // GTG
            problems.Add(new Page166Problem25(false));   // GTG on limited givens
            problems.Add(new Page168Problem35(false));   // GTG
            problems.Add(new Page284Problem17(false));   // GTG 
            problems.Add(new Page284Problem18(false));   // GTG 
            problems.Add(new Page284Example44(false));   // GTG
            problems.Add(new Page284Exameple45(false));  // GTG
            problems.Add(new Page284Example46(false));   // GTG
            problems.Add(new Page285Problem21(false));   // GTG
            problems.Add(new Page285Problem22(false));   // GTG
            problems.Add(new Page285Problem23(false));   // GTG
            problems.Add(new Page286Problem8(false));    // GTG; multiple solutions possible
            problems.Add(new Page286Problem9(false));    // GTG
            problems.Add(new Page301Problem50(false));   // GTG
            problems.Add(new Page301Problem51(false));   // GTG
            problems.Add(new Page301Problem52(false));   // GTG
            problems.Add(new Page316Problem42(false));   // GTG
            problems.Add(new Page316Problem43(false));   // GTG
            problems.Add(new Page316Problem44(false));   // GTG





            problems.Add(new Page168Problem36(false));  // Endocing  
            problems.Add(new Page168Problem37(false));  // Endocing  
            problems.Add(new Page197Problem37(false));  // Quite algebraic
            //problems.Add(new Page160Problem43(false));  // Encoding; not for this implementation round
            // problems.Add(new Page301Problem42(false));  Can't deduce geometrically; coordinate-based needed

            return problems;
        }
    }
}
