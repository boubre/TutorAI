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

            problems.Add(new IPage119Problem2(false, true));;   // GTG
            problems.Add(new IPage119Problem3(false, false));   // GTG
            problems.Add(new IPage119Problem5(false, false));   // GTG
            problems.Add(new IPage123Example5(false, true));;   // GTG
            problems.Add(new IPage123Example6(false, false));   // GTG
            problems.Add(new IPage128Problem01(false, false));  // GTG  // Good to analyze
            problems.Add(new IPage128Problem03(false, false));  // GTG
            //problems.Add(new IPage120Problem6(false, false));   // GTG  ONLY with Angle Addition Axiom
            problems.Add(new IPage120Problem7(false, true));;   // GTG
            problems.Add(new IPage120Problem8(false, true));;   // GTG  Overlapping Right Triangles
            problems.Add(new JPage140Problem9(false, false));   // GTG
            problems.Add(new JPage140Problem6(false, true));;   // GTG: Interleaving Triangles
            problems.Add(new JPage140Problem7(false, false));   // GTG
            problems.Add(new JPage141Problem11(false, false));  // GTG
            problems.Add(new JPage135Example4(false, true));;   // GTG


            //problems.Add(new IPage149TopExercise(false, false)); // Requires implementing a new theorem
            // May omit these from study
            // Proportionality problems.Add(new JPage152Problem01());
            // Problems with proportionality problems.Add(new JPage153Problem09());

            return problems;
        }
    }
}
