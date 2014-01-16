using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Testbed
{
    public static class IndianTextProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new IPage123Example5(false));
            problems.Add(new IPage123Example6(false));
            problems.Add(new IPage128Problem01(false)); // Good to analyze
            problems.Add(new IPage128Problem03(false));


            problems.Add(new IPage120Problem8(false)); // Good to analyze: Overlapping Right Triangles
            problems.Add(new JPage140Problem6(false)); // Good to analyze: Interleaving Triangles; Missing coverage of an intersection FOR ALL problems
            //problems.Add(new JPage140Problem7(false)); // Encoding ISSUES

            //problems.Add(new IPage149TopExercise(true)); // Requires implementing a new theorem
            // May omit these from study
            // Proportionality problems.Add(new JPage152Problem01());
            // Problems with proportionality problems.Add(new JPage153Problem09());

            return problems;
        }
    }
}
