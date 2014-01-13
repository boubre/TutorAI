using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class InterestingProblemCalculator
    {
        Hypergraph<GroundedClause, int> graph;
        List<GroundedClause> figure;
        List<GroundedClause> givens;

        public InterestingProblemCalculator(Hypergraph<GroundedClause, int> g, List<GroundedClause> f, List<GroundedClause> gs)
        {
            this.graph = g;
            this.figure = f;
            this.givens = gs;

            COVERAGE_WEIGHTS = new double[NUM_INTRINSIC];
            // Sum the factors
            double sum = 0;
            foreach (int i in COVERAGE_FACTOR) sum += i;
            // Divide for weights
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                COVERAGE_WEIGHTS[w] = COVERAGE_FACTOR[w] / sum;
            }
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

        private readonly int POINTS = 0;
        private readonly int SEGMENTS = 1;
        private readonly int ANGLES = 2;
        private readonly int INTERSECTION = 3;
        private readonly int TRIANGLES = 4;
        private readonly int IN_MIDDLES = 5;
        private readonly int NUM_INTRINSIC = 6;

        private readonly int[] COVERAGE_FACTOR = { 1, // Points
                                                   2, // Segments
                                                   3, // Angles
                                                   2, // Intersection
                                                   6, // Triangles
                                                   3, // InMiddle
                                                 };

        private double[] COVERAGE_WEIGHTS; // Init in constructor based on the coverage factors stated in the array above
        private readonly double MINIMUM_WEIGHTED_COVERAGE_FACTOR = 1; // Weighted %: [0, 1]

        //
        // A problem is defined as interesting if:
        //   1. It is minimal in its given information
        //   2. The problem implies all of the facts of the given figure; that is, if the set of all the facts of a figure are not in the source of the problem, then reject 
        //
        private bool IsInteresting(Problem problem)
        {
            // If there is no path, we define it as uninteresting
            if (!problem.path.Any()) return false;

            // Acquire the percentage of the individual components in the figure that are covered
            double[] problemCoverage = InterestingProblemCoverage(problem);

            // Calculate the actual coverage factor
            double finalCoverageFactor = 0;
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                finalCoverageFactor += COVERAGE_WEIGHTS[w] * problemCoverage[w];
            }

            if (Utilities.DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Weighted Coverage Factor: " + finalCoverageFactor);
            }
            return finalCoverageFactor > MINIMUM_WEIGHTED_COVERAGE_FACTOR ||
                   Utilities.CompareValues(finalCoverageFactor, MINIMUM_WEIGHTED_COVERAGE_FACTOR);
        }

        //
        // A problem is defined as interesting if:
        //   1. It is minimal in its given information
        //   2. The problem implies all of the facts of the given figure; that is, if the set of all the facts of a figure are not in the source of the problem, then reject 
        //
        // Returns a 
        private double[] InterestingProblemCoverage(Problem problem)
        {
            List<int> problemGivens = problem.givens;

            //
            // Collect all of the figure intrinsic covered nodes
            //
            List<int> intrinsicCollection = new List<int>();
            foreach (int src in problem.givens)
            {
                Utilities.AddUniqueList<int>(intrinsicCollection, graph.vertices[src].data.figureComponents);
            }

            // Sort is not required, but for debug is eaiser to digest
            intrinsicCollection.Sort();

            // DEBUG
            //System.Diagnostics.Debug.WriteLine("\n" + problem + "\nCovered Nodes: ");
            //foreach (int coveredNode in intrinsicCollection)
            //{
            //    System.Diagnostics.Debug.WriteLine("\t" + coveredNode);
            //}

            //
            // Calculate the 
            //
            int[] numCoveredNodes = new int[NUM_INTRINSIC];
            int[] numUncoveredNodes = new int[NUM_INTRINSIC];
            int totalCovered = 0;
            int totalUncovered = 0;
            foreach (GroundedClause gc in figure)
            {
//                System.Diagnostics.Debug.WriteLine("Checking: " + gc.ToString());
                if (intrinsicCollection.Contains(gc.clauseId))
                {
                    if (gc is Point) numCoveredNodes[POINTS]++;
                    else if (gc is Segment) numCoveredNodes[SEGMENTS]++;
                    else if (gc is Angle) numCoveredNodes[ANGLES]++;
                    else if (gc is Intersection) numCoveredNodes[INTERSECTION]++;
                    else if (gc is Triangle) numCoveredNodes[TRIANGLES]++;
                    else if (gc is InMiddle) numCoveredNodes[IN_MIDDLES]++;
                    totalCovered++;
                }
                else
                {
                    if (Utilities.DEBUG)
                    {
                        System.Diagnostics.Debug.WriteLine("Uncovered: " + gc.ToString());
                    }
                    if (gc is Point) numUncoveredNodes[POINTS]++;
                    else if (gc is Segment) numUncoveredNodes[SEGMENTS]++;
                    else if (gc is Angle) numUncoveredNodes[ANGLES]++;
                    else if (gc is Intersection) numUncoveredNodes[INTERSECTION]++;
                    else if (gc is Triangle) numUncoveredNodes[TRIANGLES]++;
                    else if (gc is InMiddle) numUncoveredNodes[IN_MIDDLES]++;
                    totalUncovered++;
                }
            }

            if (Utilities.DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Covered: ");
                System.Diagnostics.Debug.WriteLine("\tPoints\t\t\t" + numCoveredNodes[POINTS]);
                System.Diagnostics.Debug.WriteLine("\tSegments\t\t" + numCoveredNodes[SEGMENTS]);
                System.Diagnostics.Debug.WriteLine("\tAngles\t\t\t" + numCoveredNodes[ANGLES]);
                System.Diagnostics.Debug.WriteLine("\tIntersection\t" + numCoveredNodes[INTERSECTION]);
                System.Diagnostics.Debug.WriteLine("\tTriangles\t\t" + numCoveredNodes[TRIANGLES]);
                System.Diagnostics.Debug.WriteLine("\tInMiddles\t\t" + numCoveredNodes[IN_MIDDLES]);
                System.Diagnostics.Debug.WriteLine("\t\t\t\t\t" + totalCovered);

                System.Diagnostics.Debug.WriteLine("Uncovered: ");
                System.Diagnostics.Debug.WriteLine("\tPoints\t\t\t" + numUncoveredNodes[POINTS]);
                System.Diagnostics.Debug.WriteLine("\tSegments\t\t" + numUncoveredNodes[SEGMENTS]);
                System.Diagnostics.Debug.WriteLine("\tAngles\t\t\t" + numUncoveredNodes[ANGLES]);
                System.Diagnostics.Debug.WriteLine("\tIntersection\t" + numUncoveredNodes[INTERSECTION]);
                System.Diagnostics.Debug.WriteLine("\tTriangles\t\t" + numUncoveredNodes[TRIANGLES]);
                System.Diagnostics.Debug.WriteLine("\tInMiddles\t\t" + numUncoveredNodes[IN_MIDDLES]);
                System.Diagnostics.Debug.WriteLine("\t\t\t\t\t" + totalUncovered);
            }

            //
            // Calculate the coverage percentages
            //
            double[] percentageCovered = new double[NUM_INTRINSIC];
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                // If there are none of the particular node we have 'covered' them all
                if (numCoveredNodes[w] + numUncoveredNodes[w] == 0)
                {
                    percentageCovered[w] = 1;
                }
                else
                {
                    percentageCovered[w] = (double)(numCoveredNodes[w]) / (numCoveredNodes[w] + numUncoveredNodes[w]);
                }
            }

            return percentageCovered;
        }
    }
}
