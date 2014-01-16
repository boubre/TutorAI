using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Testbed
{
    public static class JurgensenProblems
    {
        public static List<ActualProblem> GetProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            // Transversals
            problems.Add(new Page23Theorem11(false));
            problems.Add(new Page32ClassroomProblem11To14(false));

            // Parallel
            problems.Add(new Page60Theorem22(false));
            problems.Add(new Page60Theorem22Extended(false));
            
            
            problems.Add(new Page62Problem1(false));
            problems.Add(new Page62Problem2(false));
            problems.Add(new Page62Problems3To4(false));


            // Congruent Triangles
            problems.Add(new Page113Problem7(false));
            problems.Add(new Page124Figure31(false)); // Classic Isosceles Test
            problems.Add(new Page134Problem6(true)); //Original problem is there...!
            problems.Add(new Page134Problem7(true));
            problems.Add(new Page135Problem21(false));
            problems.Add(new Page144ClassroomExercise01(false));
            problems.Add(new Page144ClassroomExercise02(false));
            problems.Add(new Page144ClassroomExercise03(false));
            problems.Add(new Page144ClassroomExercise04(false));
            problems.Add(new Page144Problem01(false));
            problems.Add(new Page144Problem02(false));
            problems.Add(new Page145Problem03(false));
            problems.Add(new Page145Problem04(false));
            problems.Add(new Page145Problem07(false));
            problems.Add(new Page145Problem08(false));
            problems.Add(new Page145Problem09(false));
            problems.Add(new Page145Problem10(false));

			// BIG Hypergraph construction problems     problems.Add(new Page146Problem12(false));

            problems.Add(new Page146Problem13(false));
            problems.Add(new Page146Problem14(false));
            problems.Add(new Page146Problem15(false));
            problems.Add(new Page146Problem17(false));
            problems.Add(new Page146Problem18(false));
            problems.Add(new Page147Problem20(false));
            problems.Add(new Page147Problem21(false));
            problems.Add(new Page147Problem22(false));
            problems.Add(new Page155Problem14(false));
            problems.Add(new Page175ClassroomExercise01to02(false));
            problems.Add(new Page175ClassroomExercise03to06(false));
            problems.Add(new Page175ClassroomExercise07to12(false));
            problems.Add(new Page175WrittenExercise1to4(false));

            problems.Add(new Page223Problem22(false));
            problems.Add(new Page223Problem23(false));
            problems.Add(new Page223Problem24(false));
            problems.Add(new Page223Problem25(false));
            problems.Add(new Page223Problem26(false));
            problems.Add(new Page223Problem27(false));


            // mislabel problems.Add(new Page223Problem32(false));
            //problems.Add(new Page229Problem03(false));
            //problems.Add(new Page229Problem05(false));
            //problems.Add(new Page229Problem07(false));
            //problems.Add(new Page229Problem08(false));  
            //problems.Add(new Page229Problem09(false)); 

            return problems;
        }
    }
}
