using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Testbed
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
            problems.Add(new Page284Problem17(false));   // GTG 
            problems.Add(new Page284Problem18(false));   // GTG 
            problems.Add(new Page284Example44(false));   // GTG
            problems.Add(new Page284Exameple45(false));  // GTG
            problems.Add(new Page284Example46(false));   // GTG
            problems.Add(new Page285Problem21(false));   // GTG
            problems.Add(new Page285Problem22(false));   // GTG
            problems.Add(new Page285Problem23(false));   // GTG
            problems.Add(new Page166Problem25(false));   // Out of Memory; GTG on limited givens
            problems.Add(new Page286Problem8(false));    // GTG; multiple solutions possible
            problems.Add(new Page286Problem9(false));    // GTG


            problems.Add(new Page168Problem35(false));  // Out of Memory
            problems.Add(new Page168Problem36(false));  // Endocing  
            problems.Add(new Page168Problem37(false));  // Endocing  
            problems.Add(new Page197Problem37(false));  // Quite algebraic
            //problems.Add(new Page160Problem43(false));  // Encoding; not for this implementation round
            // problems.Add(new Page301Problem42(true));  Can't deduce geometrically; coordinate-based needed

            return problems;
        }
    }
}
