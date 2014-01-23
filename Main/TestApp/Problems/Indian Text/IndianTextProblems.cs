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

            problems.Add(new IPage123Example5(false));   // GTG
            problems.Add(new IPage123Example6(false));   // GTG
            problems.Add(new IPage128Problem01(false));  // GTG  // Good to analyze
            problems.Add(new IPage128Problem03(false));  // GTG
            problems.Add(new IPage120Problem7(false));   // GTG
            problems.Add(new IPage120Problem8(false));   // GTG  Overlapping Right Triangles
            problems.Add(new JPage140Problem9(false));   // GTG
            problems.Add(new JPage140Problem7(false));   // GTG
            problems.Add(new JPage141Problem11(false));  // GTG
            problems.Add(new JPage135Example4(false));   // GTG

            problems.Add(new IPage120Problem6(false));   // POINT encoding
            problems.Add(new JPage140Problem6(false));   // OUt of Memory: Interleaving Triangles



            //problems.Add(new IPage149TopExercise(false)); // Requires implementing a new theorem
            
            // May omit these from study
            // Proportionality problems.Add(new JPage152Problem01());
            // Problems with proportionality problems.Add(new JPage153Problem09());

            return problems;
        }
    }
}
