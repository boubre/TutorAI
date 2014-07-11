using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class JurgensenProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            // Transversals
            problems.Add(new Page23Theorem11(true, true));                  // GTG
            problems.Add(new Page32ClassroomProblem11To14(true, false));     // GTG

            // Parallel
            problems.Add(new Page60Theorem22(true, false));                  // GTG
            problems.Add(new Page62Problem1(true, false));                   // GTG
            problems.Add(new Page62Problem2(true, false));                   // GTG
            problems.Add(new Page62Problems3To4(true, false));               // GTG if no AngleAdditionAxiom

            // Congruent Triangles
            problems.Add(new Page113Problem7(true, true));                  // GTG
            problems.Add(new Page134Problem6(true, true));                  // GTG
            problems.Add(new Page134Problem7(true, true));                  // GTG
            problems.Add(new Page135Problem21(true, false));                 // GTG
            problems.Add(new Page144ClassroomExercise01(true, false));       // GTG
            problems.Add(new Page144ClassroomExercise02(true, true));       // GTG
            problems.Add(new Page144ClassroomExercise03(true, false));       // GTG
            problems.Add(new Page144ClassroomExercise04(true, false));       // GTG 1:38 execute
            problems.Add(new Page144Problem01(true, true));                 // GTG
            problems.Add(new Page144Problem02(true, false));                 // GTG
            problems.Add(new Page145Problem03(true, false));                 // GTG
            problems.Add(new Page145Problem04(true, false));                 // GTG // Intersection
            problems.Add(new Page145Problem07(true, true));                 // GTG // Intersection
            problems.Add(new Page145Problem08(true, false));                 // GTG // Intersection
            problems.Add(new Page145Problem09(true, true));                 // GTG
            problems.Add(new Page145Problem10(true, false));                 // GTG
            problems.Add(new Page146Problem14(true, false));                 // GTG // Intersection
            problems.Add(new Page146Problem15(true, false));                 // GTG
            //problems.Add(new Page146Problem18(true, false));                 // GTG 1:20 execute
            problems.Add(new Page155Problem14(true, true));                 // GTG
            problems.Add(new Page175ClassroomExercise12(true, true));       // GTG
            problems.Add(new Page223Problem22(true, false));                 // GTG
            problems.Add(new Page223Problem23(true, false));                 // GTG
            problems.Add(new Page242Problem17(true, false));                 // GTG



            ////
            //// Quadrilaterals
            ////
            //problems.Add(new Page162Problem20(false, false));          //parallelogram
            //problems.Add(new Page166Problem01(false, false));          //parallelogram
            //problems.Add(new Page166Problem02(false, false));          //parallelogram
            //problems.Add(new Page166Problem03(false, false));          //parallelogram
            //problems.Add(new Page166Problem04(false, false));          //parallelogram
            //problems.Add(new Page166Problem05(false, false));          //parallelogram
            //problems.Add(new Page166Problem13(false, false));          //parallelogram
            //problems.Add(new Page170ClassroomExercise02(false, false)); //rhombus
            //problems.Add(new Page172Problem19(false, false));          //rhombus
            //problems.Add(new Page169Theorem413(false, false));          //rectangle
            //problems.Add(new Page171Problem17(false, false));           //rectangle
            ////problems.Add(new Page178SelfTest07(false, false));          //rectangle
            //problems.Add(new Page173Theorem415(false, false));          //trapezoid
            //problems.Add(new Page174Theorem416_1(false, false));          //trapezoid
            //problems.Add(new Page174Theorem416_2(false, false));           //trapezoid
            //problems.Add(new Page170ClassroomExercise05(false, false));      //square
            //problems.Add(new ExtraSquareProblem(false, false));                //square

            //
            //Circles
            //
            //problems.Add(new Page296Theorem7_1(false, false));
            //problems.Add(new Page296Theorem7_1_Test2(false, false));
            //problems.Add(new Page306Theorem7_4_1(true, false));
            //problems.Add(new Page306Theorem7_4_1_Semicircle(false, false));
            //problems.Add(new Page309Problem09(false, false));
            //problems.Add(new Page307Theorem7_5(false, false));
            //problems.Add(new Page312Theorem7_7_Semicircle(false, false)); //This problem will work, but since it involves both angles and arcs,
                                                                          //any theorem which tries to create an equation relating angle measure and 
                                                                          //arc measure must first be commented out in the instantiator until the
                                                                          //equation issue is resolved


            //problems.Add(new BackwardPage134Problem7(true, false)); 

            //problems.Add(new Page146Problem12(true, false));                 // How to solve this on paper? A theorem missing perhaps?
            //problems.Add(new Page146Problem17(true, false));                 // MAJOR Encoding issues  
            //problems.Add(new Page147Problem21(true, false));                 // Encoding
            //problems.Add(new Page147Problem22(true, false));                 // Encoding Issues
            //problems.Add(new Page146Problem13(true, false));                 // Endocing

            
            //problems.Add(new Page124Figure31(true, false)); // Classic Isosceles Test

            // problems.Add(new Page60Theorem22Extended(true, false)); Not a real problem
            //problems.Add(new Page147Problem20(false);     // LATER ; omit potentially
            //problems.Add(new Page175ClassroomExercise01to02(true, false));  OMIT
            // problems.Add(new Page175ClassroomExercise03to06(true, false)); OMIT
            //problems.Add(new Page175WrittenExercise1to4(true, false)); // OMIT
            //problems.Add(new Page223Problem24(true, false)); // OMIT can't encode goal
            //problems.Add(new Page223Problem25(true, false)); // OMIT can't encode goal
            // problems.Add(new Page223Problem26(true, false)); OMIT goal encoding
            // problems.Add(new Page223Problem27(true, false)); OMIT goal encoding
            // mislabel problems.Add(new Page223Problem32(true, false));
            //problems.Add(new Page229Problem03(true, false));
            //problems.Add(new Page229Problem05(true, false));
            //problems.Add(new Page229Problem07(true, false));
            //problems.Add(new Page229Problem08(true, false));  
            //problems.Add(new Page229Problem09(true, false)); 
            // problems.Add(new Page242Problem16(true, false)); Given Encoding
            //problems.Add(new Page242Problem21(true, false)); OMIT goal encoding
            //problems.Add(new Page243Problem15(true, false)); OMIT goal encoding
            //problems.Add(new Page243Problem16(true, false)); OMIT given encoding

            return problems;
        }
    }
}
