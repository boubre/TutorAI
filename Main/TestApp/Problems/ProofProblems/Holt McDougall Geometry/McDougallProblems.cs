using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class McDougallProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page168Problem34(false, false));   // GTG
            problems.Add(new Page159Problem37(false, false));   // GTG
            problems.Add(new Page159Problem41(false, false));   // GTG
            problems.Add(new Page160Problem42(false, false));   // GTG
            problems.Add(new Page166Problem25(false, false));   // GTG on limited givens
            problems.Add(new Page168Problem35(false, false));   // GTG
            problems.Add(new Page284Problem17(false, true));   // GTG 
            problems.Add(new Page284Problem18(false, false));   // GTG 
            problems.Add(new Page284Example44(false, true));   // GTG
            problems.Add(new Page284Exameple45(false, false));  // GTG
            problems.Add(new Page284Example46(false, true));   // GTG
            problems.Add(new Page285Problem21(false, true));   // GTG
            problems.Add(new Page285Problem22(false, true));   // GTG
            problems.Add(new Page285Problem23(false, false));   // GTG
            problems.Add(new Page286Problem8(false, false));    // GTG; multiple solutions possible
            problems.Add(new Page286Problem9(false, false));    // GTG
            problems.Add(new Page301Problem50(false, false));   // GTG
            problems.Add(new Page301Problem51(false, true));   // GTG
            problems.Add(new Page301Problem52(false, true));   // GTG
            problems.Add(new Page316Problem42(false, false));   // GTG
            problems.Add(new Page316Problem43(false, false));   // GTG
            problems.Add(new Page316Problem44(false, false));   // GTG





            //problems.Add(new Page168Problem36(false, false));  // Endocing  
            //problems.Add(new Page168Problem37(false, false));  // Endocing  
            //problems.Add(new Page197Problem37(false, false));  // Quite algebraic
            //problems.Add(new Page160Problem43(false, false));  // Encoding; not for this implementation round
            // problems.Add(new Page301Problem42(false, false));  Can't deduce geometrically; coordinate-based needed

            return problems;
        }
    }
}
