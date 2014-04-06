using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.StatisticsGenerator
{
    public static class JurgensenProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            // Transversals
            problems.Add(new Page23Theorem11(false));                  // GTG
            problems.Add(new Page32ClassroomProblem11To14(false));     // GTG

            // Parallel
            problems.Add(new Page60Theorem22(false));                  // GTG
            problems.Add(new Page62Problem1(false));                   // GTG
            problems.Add(new Page62Problem2(false));                   // GTG
            problems.Add(new Page62Problems3To4(false));               // GTG if no AngleAdditionAxiom

            // Congruent Triangles
            problems.Add(new Page113Problem7(false));                  // GTG
            problems.Add(new Page134Problem6(false));                  // GTG
            problems.Add(new Page134Problem7(false));                  // GTG
            problems.Add(new Page135Problem21(false));                 // GTG
            problems.Add(new Page144ClassroomExercise01(false));       // GTG
            problems.Add(new Page144ClassroomExercise02(false));       // GTG
            problems.Add(new Page144ClassroomExercise03(false));       // GTG
            problems.Add(new Page144ClassroomExercise04(false));       // GTG 1:38 execute
            problems.Add(new Page144Problem01(false));                 // GTG
            problems.Add(new Page144Problem02(false));                 // GTG
            problems.Add(new Page145Problem03(false));                 // GTG
            problems.Add(new Page145Problem04(false));                 // GTG // Intersection
            problems.Add(new Page145Problem07(false));                 // GTG // Intersection
            problems.Add(new Page145Problem08(false));                 // GTG // Intersection
            problems.Add(new Page145Problem09(false));                 // GTG
            problems.Add(new Page145Problem10(false));                 // GTG
            problems.Add(new Page146Problem14(false));                 // GTG // Intersection
            problems.Add(new Page146Problem15(false));                 // GTG
            problems.Add(new Page146Problem18(false));                 // GTG 1:20 execute
            problems.Add(new Page155Problem14(false));                 // GTG
            problems.Add(new Page175ClassroomExercise12(false));        // GTG
            problems.Add(new Page223Problem22(false));                 // GTG
            problems.Add(new Page223Problem23(false));                 // GTG
            problems.Add(new Page242Problem17(false));                 // GTG


            problems.Add(new CircleTester2(false));


            //problems.Add(new BackwardPage134Problem7(false)); 


            //problems.Add(new Page146Problem12(false));                 // How to solve this on paper? A theorem missing perhaps?
            //problems.Add(new Page146Problem17(false));                 // MAJOR Encoding issues  
            //problems.Add(new Page147Problem21(false));                 // Encoding
            //problems.Add(new Page147Problem22(false));                 // Encoding Issues
            //problems.Add(new Page146Problem13(false));                 // Endocing


            
            //problems.Add(new Page124Figure31(false)); // Classic Isosceles Test

            // problems.Add(new Page60Theorem22Extended(false)); Not a real problem
            //problems.Add(new Page147Problem20(false);     // LATER ; omit potentially
            //problems.Add(new Page175ClassroomExercise01to02(false));  OMIT
            // problems.Add(new Page175ClassroomExercise03to06(false)); OMIT
            //problems.Add(new Page175WrittenExercise1to4(false)); // OMIT
            //problems.Add(new Page223Problem24(false)); // OMIT can't encode goal
            //problems.Add(new Page223Problem25(false)); // OMIT can't encode goal
            // problems.Add(new Page223Problem26(false)); OMIT goal encoding
            // problems.Add(new Page223Problem27(false)); OMIT goal encoding
            // mislabel problems.Add(new Page223Problem32(false));
            //problems.Add(new Page229Problem03(false));
            //problems.Add(new Page229Problem05(false));
            //problems.Add(new Page229Problem07(false));
            //problems.Add(new Page229Problem08(false));  
            //problems.Add(new Page229Problem09(false)); 
            // problems.Add(new Page242Problem16(false)); Given Encoding
            //problems.Add(new Page242Problem21(false)); OMIT goal encoding
            //problems.Add(new Page243Problem15(false)); OMIT goal encoding
            //problems.Add(new Page243Problem16(false)); OMIT given encoding

            return problems;
        }
    }
}
