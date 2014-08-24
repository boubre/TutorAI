using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class ShadedAreaProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            //
            // Testing
            //
            //// problems.Add(new ThreeCircleTwoOverlapTester(true, false)); // GTG
            //// problems.Add(new ThreeCirclePathologicalTester(true, false)); GTG
            //// problems.Add(new RegularPolygonCrushingTester(true, false));
            //// problems.Add(new BasicInteriorPolygonTester2(true, false));
            //// problems.Add(new BasicInteriorPolygonTester(true, false));
            //// problems.Add(new BasicPolygonTester(true, false));
            //// problems.Add(new ContainmentTester(true, false));
            //// problems.Add(new CircCircCircRegionTester(true, false));
            //// problems.Add(new FilamentTester(true, false));
            //// problems.Add(new TesterForCircleTriangle(true, false));
          
            //
            // Class X
            //
            //problems.Add(new Page1Col1Prob1(true, false));      // GTGDemo
            // problems.Add(new Page1Col1Prob2(true, false));     // GTGDemo
            // problems.Add(new Page1Col1Prob3(true, false));     // GTGDemo
            // problems.Add(new Page1Col1Prob5(true, false));     // GTGDemo
            // problems.Add(new Page1Col2Prob1(true, false));     // GTGDemo
            // problems.Add(new Page1Col2Prob2(true, false));     // GTGDemo (Need to fix Implied Calculator to recognize ALL constructed polygons for deduction engine)
            // problems.Add(new Page1Col2Prob3(true, false));     // GTGDemo
            // problems.Add(new Page1Col2Prob4(true, false));     // GTGDemo: Good circle example
            // problems.Add(new Page2Col1Prob1(true, false));     // GTGDemo: Implied polygon id problem.
            // problems.Add(new Page2Col1Prob2(true, false));     // GTGDemo
            // problems.Add(new Page2Col1Prob3(true, false));     // GTGDemo
            // problems.Add(new Page2Col2Prob1(true, false));     // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // problems.Add(new Page2Col2Prob2(true, false));     // GTGDemo: Redundant to demo; Same Deduc Engine Problem

            //
            // Jurgensen
            //
            // problems.Add(new Page2Prob15(true, false));       // GTGDemo: Redundant
            // problems.Add(new Page2Prob16(true, false));       // GTG
            // problems.Add(new Page2Prob17(true, false));       // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // problems.Add(new Page2Prob18(true, false));       // GTG
            // problems.Add(new Page2Prob19(true, false));       // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // problems.Add(new Page3Prob21(true, false));       // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // // problems.Add(new Page3Prob22(true, false));    // Too long to execute
            // problems.Add(new Page3Prob23(true, false));       // GTG2Demo: Interior Tangent Circles
            // problems.Add(new Page3Prob24(true, false));       // Atomic Region ID redundancy problem; LONG solving...
            
            // // problems.Add(new Page2Prob28(true, false));    // Atomic Region Finder Problems; Arc equations needed (as well as work to calculate trapezoid area))  
            // problems.Add(new Page4Prob7(true, false));        // GTGDemo
            // problems.Add(new Page4Prob8(true, false));        // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // problems.Add(new Page4Prob13(true, false));       // GTGDemo
            // problems.Add(new Page4Prob14(true, false));    // Encoding: Points (need triangle similarity to solve?)
            //problems.Add(new Page4Prob15(true, false));        // GTG
            // problems.Add(new Page4prob17(true, false));       // GTG
            //problems.Add(new Page4prob18(true, false));        //Works, but uses unproven semicircle clause
            // // problems.Add(new Page4Prob19(true, false));    // Encoding

            //
            // McDougall
            //
            // problems.Add(new Page5Row1Prob24(true, false));   // GTGDemo
            // problems.Add(new Page5Row1Prob25(true, false));   // GTGDemo
            // problems.Add(new Page5Row1Prob26(true, false));   // GTGDemo
            // problems.Add(new Page5Row2Prob27(true, false));   // GTGDemo
            // problems.Add(new Page5Row2Prob28(true, false));   // GTGDemo
            // problems.Add(new Page5Row3Prob2(true, false));    // GTGDemo
            // problems.Add(new Page5Row3Prob3(true, false));    // GTGDemo
            // problems.Add(new Page5Row3Prob4(true, false));    // GTGDemo
            // problems.Add(new Page5Row4Prob24(true, false));   // GTGDemo
            // problems.Add(new Page5Row4Prob25(true, false));   // GTG
            // // problems.Add(new Page5Row5Prob17(true, false));   // Can't Deduce a Square
            // problems.Add(new Page6Prob26(true, false));       // GTG
            // problems.Add(new Page6Prob27(true, false));       // GTG
            // problems.Add(new Page6Prob29(true, false));       // GTG
            //problems.Add(new Page6Prob30(true, false));        // GTG
            // problems.Add(new Page6Prob31(true, false));       // GTG
            //problems.Add(new Page6Prob32a(true, false));       // GTG

            

            //
            // Singapore
            //
            // problems.Add(new Page205(true, false));  // GTGDemo
            // // problems.Add(new Page207(true, false));        // Takes a long time; eliminate supplementary from parallelograms to speed up ; Angle equation problem.
            // problems.Add(new Page208(true, false));  // GTGDemo
            // // problems.Add(new Page209(true, false));        // GTG (Long deduction engine)
            // // problems.Add(new Page210(true, false));        // Incomputable ; LONG execution need angle addition axiom ; Don't include.
            // // problems.Add(new Page199(true, false));        // Problems with deduction.

            //
            // Glencoe
            //
            // problems.Add(new Page7Row6(true, false));           // Works, but slow parse time (~3 minutes) (faster if RelationsOfCongruentAnglesAreCongruent is off)
            // problems.Add(new Page7Prob15(true, false));         // GTG
            // problems.Add(new Page7Prob16(true, false));         // Execution time very long, gets stuck in area solution generator
            // // problems.Add(new Page7Prob17(true, false));      // Execution time very long, gets stuck in area solution generator
            // problems.Add(new Page7Prob27(true, false));         // works if RelationsOfCongruentAnglesAreCongruent is turned off (otherwise execution is too long)
            // problems.Add(new Page7Prob28(true, false));         // Not working - Encoding issue
            // problems.Add(new Page8Prob18(true, false));         // GTG
            // problems.Add(new Page8Prob14(true, false));         // GTG
            // //  problems.Add(new Page8Prob21(true, false));      // Atomic region issue
            // problems.Add(new Page8Row5Prob40(true, false));     // GTGDemo: Redundant to demo; Same Deduc Engine Problem
            // // problems.Add(new Page9Prob8(true, false));          // Encoding
            // problems.Add(new Page9Prob13(true, false));         // GTGDemo
            // problems.Add(new Page9Prob16(true, false));         // GTGDemo
            // problems.Add(new Page9Prob33(true, false));         // GTGDemo
            // problems.Add(new Page10Prob35(true, false));        // GTGDemo
            // problems.Add(new Page10Prob36(true, false));        // GTGDemo
            // // problems.Add(new Page10Prob8(true, false));         // Encoding
            // problems.Add(new Page10Prob9(true, false));         // GTGDemo
            // problems.Add(new Page10Prob10(true, false));        // GTGDemo
            // // problems.Add(new Page10Prob17(true, false));        // No solution: Deduction Problem
            // problems.Add(new Page10Prob18(true, false));        // GTGDemo
            // // problems.Add(new Page10Prob16(true, false));        // Ambiguous / impossible problem

            //
            // ACT Practice
            //
            //problems.Add(new TwoCircleInteriorTangent(true, false)); // GTGDemo
            //problems.Add(new CircCircRegionTester(true, false)); // No solution found
            //problems.Add(new FailingCircCircRegionTester(true, false)); // No solution found

            return problems;
        }
    }
}
