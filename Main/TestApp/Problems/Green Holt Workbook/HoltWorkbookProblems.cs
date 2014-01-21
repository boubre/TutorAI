using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Testbed
{
    public static class HoltWorkbookProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new Page13Problem10(false));  // Needs definition of perpendicular
            problems.Add(new Page17Problem9(false));   // Supplementary hypergraph construction problem / intersections
            problems.Add(new Page23Problem9(false));   // GTG
            problems.Add(new Page24Problem7(false));   // GTG
            problems.Add(new Page25Problem8(false));   // Good problem: Intersection -> Parallel -> Congruent angles prob?
            problems.Add(new Page26Problem2(false));   // GTG

            return problems;
        }
    }
}
