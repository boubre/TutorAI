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
            //problems.Add(new Page1Col2Prob3(true, false));         // GTG
            problems.Add(new Page2Col1Prob2(true, false));         // GTG
            //problems.Add(new Page2Col2Prob2(true, false));         // GTG

            // problems.Add(new Page1Col1Prob5(true, false));


            //problems.Add(new Page1Col1Prob1(true, false));

            //problems.Add(new Page1Col2Prob1(true, false));
            //problems.Add(new Page1Col2Prob2(true, false));                           // AtomicRegion


            // problems.Add(new Page2Col2Prob1(true, false));                             // No solution


            //problems.Add(new PathologicalTester(true, false));

            //
            // Jurgensen
            //
            // problems.Add(new Page2prob15(true, false));    // GTG
            // problems.Add(new Page2prob17(true, false)); 

            return problems;
        }
    }
}
