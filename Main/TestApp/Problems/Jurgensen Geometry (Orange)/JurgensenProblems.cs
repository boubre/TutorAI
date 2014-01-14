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
            //problems.Add(new Page23Theorem11());
            //problems.Add(new Page32ClassroomProblem11To14());

            // Parallel
            //problems.Add(new Page60Theorem22());
            //problems.Add(new Page60Theorem22Extended());
            // PROBLEM WITH BACKWARD problems.Add(new Page62Problems1To6());

            // Congruent Triangles
            //problems.Add(new Page113Problem7());
            //problems.Add(new Page124Figure31()); // Classic Isosceles Test
            //problems.Add(new Page134Problem6());
            //problems.Add(new Page134Problem7());
            //problems.Add(new Page135Problem21());
            //problems.Add(new Page144ClassroomExercise01());
            //problems.Add(new Page144ClassroomExercise02());
            //problems.Add(new Page144ClassroomExercise03());
            //problems.Add(new Page144ClassroomExercise04());
            //problems.Add(new Page144Problem01());
            //problems.Add(new Page144Problem02());
            //problems.Add(new Page145Problem03());
            //problems.Add(new Page145Problem04());
            //problems.Add(new Page145Problem07());
            //problems.Add(new Page145Problem08());
            //problems.Add(new Page145Problem09());
            //problems.Add(new Page145Problem10());

			// BIG Hypergraph construction problems     problems.Add(new Page146Problem12());

            //problems.Add(new Page146Problem13());
            //problems.Add(new Page146Problem14());
            //problems.Add(new Page146Problem15());
            //problems.Add(new Page146Problem17());
            //problems.Add(new Page146Problem18());
            //problems.Add(new Page147Problem20());
            //problems.Add(new Page147Problem21());
            //problems.Add(new Page147Problem22());
            //problems.Add(new Page155Problem14());
            //problems.Add(new Page175ClassroomExercise01to02());
            //problems.Add(new Page175ClassroomExercise03to06());
            //problems.Add(new Page175ClassroomExercise07to12());
            //problems.Add(new Page175WrittenExercise1to4());

            //problems.Add(new Page223Problem22());
            //problems.Add(new Page223Problem23());
            //problems.Add(new Page223Problem24());
            //problems.Add(new Page223Problem25());
            //problems.Add(new Page223Problem26());
            //problems.Add(new Page223Problem27());

            return problems;
        }
    }
}
