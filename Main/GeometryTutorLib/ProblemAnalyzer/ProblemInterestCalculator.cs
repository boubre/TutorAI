using System.Collections.Generic;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class ProblemInterestCalculator
    {

        Hypergraph<GroundedClause, int> graph;
        List<GroundedClause> figure;
        List<GroundedClause> givens;

        public ProblemInterestCalculator(Hypergraph<GroundedClause, int> g, List<GroundedClause> f, List<GroundedClause> gs)
        {
            this.graph = g;
            this.figure = f;
            this.givens = gs;
        }

        //
        // A problem is defined as interesting if:
        //   1. It is minimal in its given information
        //   2. The problem implies all of the facts of the given figure; that is, if the set of all the facts of a figure is not the source of the problem, then reject 
        //
        private bool IsInteresting(Problem problem)
        {
            List<int> problemGivens = problem.givens;

            //foreach ()
            //{
            //}

            return false;
        }

        //
        // Given a set of problems, determine which partition of problems meets the 'interesting' criteria
        //
        public List<Problem> DetermineInterestingProblems(List<Problem> problems)
        {
            List<Problem> interesting = new List<Problem>();

            foreach (Problem p in problems)
            {
                if (IsInteresting(p)) interesting.Add(p);
            }

            return interesting;
        }
    }
}
