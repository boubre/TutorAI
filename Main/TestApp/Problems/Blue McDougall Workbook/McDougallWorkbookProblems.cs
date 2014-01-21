using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Testbed
{
    public static class McDougallWorkbookProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new Page48Problem23To31(false));  // Infinite Problem Generation
            problems.Add(new Page80Problem9(false));       // GTG
            problems.Add(new Page90Problem22(false));      // GTG
            problems.Add(new Page90Problem23(false));      // Intersection issue (look at last HYpergraph node)




            problems.Add(new Page37Problem2(false));      // Encoding
            //problems.Add(new Page41Problem15(false));    // Encoding
            //problems.Add(new Page42Problem16(false));   // Generation of intersections -> Lack of angles -> Not find angle ENCODING

            return problems;
        }
    }
}
