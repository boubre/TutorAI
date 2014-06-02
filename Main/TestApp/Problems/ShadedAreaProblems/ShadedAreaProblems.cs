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

            //problems.Add(new CircCircRegionTester(true, false));
            //problems.Add(new RegionTester(true, false));
            //problems.Add(new PathologicalTester(false, false));

            return problems;
        }
    }
}
