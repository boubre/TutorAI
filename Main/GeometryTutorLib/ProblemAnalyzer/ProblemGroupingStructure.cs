using System;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // The intent of this class is to facilitate storing groups of problems.
    // So, if a set of problems are alike in some manner, they are easily accessible
    // using feature vector access
    //
    public class ProblemGroupingStructure
    {
        // To access node value information; mapping problem values back to the Geometric Graph
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph;

        // The list of equivalent problem sets as defined by the query vector
        List<ProblemPartition> partitions;

        private int totalProblems;

        public ProblemGroupingStructure(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> g)
        {
            graph = g;
            partitions = new List<ProblemPartition>();
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
                foreach (ProblemPartition partition in partitions)
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
                    ProblemPartition newPartition = new ProblemPartition(graph);
                    newPartition.Add(problem);
                    partitions.Add(newPartition);
                }
            }
        }

        // For debugging purposes
        public void DumpPartitions(QueryFeatureVector query)
        {
            for (int p = 0; p < partitions.Count; p++)
            {
                //System.Diagnostics.Debug.WriteLine("Partition (" + (p + 1) + ") contains " + partitions[p].Size() + " problems.\n" + partitions[p].ToString());
            }

            System.Diagnostics.Debug.WriteLine("Using query " + query.ToString() + ", there are " + partitions.Count + " partitions with " + totalProblems + " problems.\n");
        }
    }
}