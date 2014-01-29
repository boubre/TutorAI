using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.StatisticsGenerator
{
    public static class IndianTextProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.Add(new IPage123Example5(true));   // GTG
            problems.Add(new IPage123Example6(true));   // GTG
            problems.Add(new IPage128Problem01(true));  // GTG  // Good to analyze
            problems.Add(new IPage128Problem03(true));  // GTG
            problems.Add(new IPage120Problem7(true));   // GTG
            problems.Add(new IPage120Problem8(true));   // GTG  Overlapping Right Triangles
            problems.Add(new JPage140Problem9(true));   // GTG
            problems.Add(new JPage140Problem6(true));   // GTG: Interleaving Triangles
            problems.Add(new JPage140Problem7(true));   // GTG
            problems.Add(new JPage141Problem11(true));  // GTG
            problems.Add(new JPage135Example4(true));   // GTG

            problems.Add(new IPage120Problem6(false));   // POINT encoding



            //problems.Add(new IPage149TopExercise(false)); // Requires implementing a new theorem
            
            // May omit these from study
            // Proportionality problems.Add(new JPage152Problem01());
            // Problems with proportionality problems.Add(new JPage153Problem09());

            return problems;
        }
    }
}
