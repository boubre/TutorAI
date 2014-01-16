using System;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // The intent of this class is to facilitate storing groups of problems.
    // So, if a set of problems are alike in some manner, they are easily accessible
    // using feature vector access
    //
    public class PartitionedProblemSpace
    {
        // To access node value information; mapping problem values back to the Geometric Graph
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;

        // The list of equivalent problem sets as defined by the query vector
        List<ProblemEquivalenceClass> partitions;

        private int totalProblems;

        public PartitionedProblemSpace(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> g)
        {
            graph = g;
            partitions = new List<ProblemEquivalenceClass>();
            totalProblems = 0;
        }

        public void ConstructPartitions(List<Problem> problems, QueryFeatureVector query)
        {
            totalProblems += problems.Count;
            //
            // For each problem, add to the appropriate partition based on the query vector
            //
            foreach (Problem problem in problems)
            {
                bool added = false;
                foreach (ProblemEquivalenceClass partition in partitions)
                {
                    if (partition.IsStrictlyIsomorphic(problem, query))
                    {
                        partition.Add(problem);
                        added = true;
                        break; // If this problem is in several partitions, we should delete this; this is more applicable with non-strict isomorphism
                    }
                }
                // If this problem was not added into a partition, create a new partition
                if (!added)
                {
                    ProblemEquivalenceClass newPartition = new ProblemEquivalenceClass(graph);
                    newPartition.Add(problem);
                    partitions.Add(newPartition);
                }
            }
        }

        //
        // Validate that we have generated all of the original problems from the text (based strictly on givens and goals)
        //
        public List<Problem> ValidateOriginalProblems(List<ConcreteAST.GroundedClause> givens, List<ConcreteAST.GroundedClause> goals)
        {
            // Acquire the indices of the givens
            List<int> givenIndices = new List<int>();
            foreach (ConcreteAST.GroundedClause given in givens)
            {
                givenIndices.Add(Utilities.StructuralIndex(graph, given));
            }

            //Acquire the indices of the goals
            List<int> goalIndices = new List<int>();
            foreach (ConcreteAST.GroundedClause goal in goals)
            {
                int goalIndex = Utilities.StructuralIndex(graph, goal);
                if (goalIndex == -1) throw new Exception("FATAL ERROR: Goal Node not deduced!!!" + goal);
                goalIndices.Add(goalIndex);
            }

            //
            // Search through all the partitiions in the space for matching problems
            // We specifically seek problems with the same givens so we can then check the goals
            //
            List<Problem> validatedProblems = new List<Problem>();
            foreach (ProblemEquivalenceClass partition in partitions)
            {
                foreach (Problem problem in partition.elements)
                {
                    // Does this problem have the same goals?
                    if (Utilities.EqualSets<int>(problem.givens, givenIndices))
                    {
                        // Chcek the goal indices
                        if (goalIndices.Contains(problem.goal))
                        {
                            // Success, we generated a book problem directly
                            // We ensured previously that there does not exist more than one problem with the same given set and goal node;
                            // therefore, we need not track that this problem is matched with a different problem.
                            validatedProblems.Add(problem);
                        }
                    }
                }
            }
            return validatedProblems;
        }

        // For debugging purposes
        public void DumpPartitions(QueryFeatureVector query)
        {
            for (int p = 0; p < partitions.Count; p++)
            {
                System.Diagnostics.Debug.WriteLine("Partition (" + (p + 1) + ") contains " + partitions[p].Size() + " problems.\n" + partitions[p].ToString());
            }

            System.Diagnostics.Debug.WriteLine("Using query " + query.ToString() + ", there are " + partitions.Count + " partitions with " + totalProblems + " problems.\n");
            System.Diagnostics.Debug.WriteLine("Using query " + query.ToString() + ", there are " + partitions.Count + " partitions with " + totalProblems + " problems.\n");
        }
    }
}