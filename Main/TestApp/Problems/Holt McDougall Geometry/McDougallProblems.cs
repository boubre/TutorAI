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

            problems.Add(new Page159Problem37(false));  // Encoding
            problems.Add(new Page159Problem41(false));  // Encoding
            problems.Add(new Page160Problem42(false));  // Encoding
            problems.Add(new Page160Problem43(false));  // Encoding

            problems.Add(new Page166Problem25(false));  // GTG
            problems.Add(new Page168Problem34(false));  // GTG
            problems.Add(new Page168Problem35(false));  // Infinite? Parallel?
            problems.Add(new Page168Problem36(false));  // Endocing  
            problems.Add(new Page168Problem37(false));  // Endocing  

            problems.Add(new Page197Problem35(false));  // FATAL GOAL NOT DEDUCED
            problems.Add(new Page197Problem36(false));  // FATAL GOAL NOT DEDUCED
            problems.Add(new Page197Problem37(false));   // GTG? Check with previous 2; related.

            return problems;
        }
    }
}
