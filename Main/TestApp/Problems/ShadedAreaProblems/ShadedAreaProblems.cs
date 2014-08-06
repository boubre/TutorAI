using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class ShadedAreaProblems
    {
        public static List<ActualShadedAreaProblem> GetProblems()
        {
            List<ActualShadedAreaProblem> problems = new List<ActualShadedAreaProblem>();

            //
            // Testing
            //
            //problems.Add(new ThreeCircleTwoOverlapTester(true, false)); // GTG
            //problems.Add(new ThreeCirclePathologicalTester(true, false)); GTG
            //problems.Add(new RegularPolygonCrushingTester(true, false));
            //problems.Add(new BasicInteriorPolygonTester2(true, false));
            //problems.Add(new BasicInteriorPolygonTester(true, false));
            //problems.Add(new BasicPolygonTester(true, false));
            //problems.Add(new ContainmentTester(true, false));
            //problems.Add(new CircCircRegionTester(true, false));
            //problems.Add(new CircCircCircRegionTester(true, false));
            //problems.Add(new FilamentTester(true, false));

            //
            // Jurgensen
            //
            //problems.Add(new Page2Prob15(true, false));              // GTG
            


            //
            // Class X
            //
            // problems.Add(new Page1Col1Prob1(true, false));         // Infinite loop on solving
            // problems.Add(new Page1Col1Prob2(true, false));         // Arc equations....

            // problems.Add(new Page1Col1Prob3(true, false));        // Arc Equations

            // problems.Add(new Page1Col1Prob5(true, false));        // GTG: Pathological
            // problems.Add(new Page1Col2Prob1(true, false));        // GTG
            // problems.Add(new Page1Col2Prob2(true, false));        // GTG
            // problems.Add(new Page1Col2Prob3(true, false));        // GTG
            // problems.Add(new Page2Col1Prob1(true, false));        // GTG
            // problems.Add(new Page2Col1Prob2(true, false));        // GTG
            // problems.Add(new Page2Col2Prob2(true, false));        // GTG
            // problems.Add(new Page2Col1Prob3(true, false));        // GTG
            // problems.Add(new TesterForCircleTriangle(true, false));


            //problems.Add(new Page1Col2Prob1(true, false));
            //problems.Add(new Page1Col2Prob2(true, false));                           // AtomicRegion


            // problems.Add(new Page2Col2Prob1(true, false));                             // No solution


            //problems.Add(new PathologicalTester(true, false));

            //
            // Jurgensen
            //
            // problems.Add(new Page2Prob15(true, false));    // GTG
            // problems.Add(new Page2Prob17(true, false));    // GTG
            // problems.Add(new Page2Prob18(true, false));    // Arc equation
            // problems.Add(new Page2Prob19(true, false));    // GTG
            // problems.Add(new Page3Prob21(true, false));    // GTG: Takes a while.

            //
            // Singapore
            //
            // problems.Add(new Page205(true, false)); // GTG
            // problems.Add(new Page207(true, false));       // Takes a long time; eliminate supplementary from parallelograms to speed up ; Angle equation problem.
            // problems.Add(new Page208(true, false)); // GTG
            // problems.Add(new Page209(true, false));       // Problem with not calculating area; more deductions needed?
            // problems.Add(new Page210(true, false));       // Incomputable ; LONG execution need angle addition axiom ; Don't include.
            // problems.Add(new Page199(true, false));       // FUBAR

            return problems;
        }
    }
}
