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
            //problems.Add(new Page32Problem11To14());

            // Parallel
            // FAILS problems.Add(new Page62Problems1To6());

            // Congruent Triangles
            //problems.Add(new Page113Problem7());
            //problems.Add(new Page124Figure31()); // Classic Isosceles Test
            //problems.Add(new Page134Problem6());
            //problems.Add(new Page134Problem7());
            //problems.Add(new Page135Problem21());
            //problems.Add(new Page155Problem14());

            return problems;
        }
    }
}
