using System;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class QueryFeatureVector
    {
        public bool lengthPartitioning { get; private set; }
        public bool widthPartitioning { get; private set; }
        public bool deductiveStepsPartitioning { get; private set; }

        public bool rangedLengthPartitioning { get; private set; }
        public IntegerPartition lengthPartitions { get; private set; }
        public bool rangedWidthPartitioning { get; private set; }
        public IntegerPartition widthPartitions { get; private set; }
        public bool rangedDeductiveStepsPartitioning { get; private set; }
        public IntegerPartition stepsPartitions { get; private set; }

        public int minLength { get; private set; }
        public int maxLength { get; private set; }
        public int minWidth { get; private set; }
        public int maxWidth { get; private set; }

        // The number of desired goals from the original figure statement
        //public int numberOfOriginalGivens { get; private set; }

        public bool sourceIsomorphism { get; private set; }
        public bool pathIsomorphism { get; private set; }
        public bool goalIsomorphism { get; private set; }

        //
        // By default, we turn everything off and make the widths / lengths a large range of values
        //
        public QueryFeatureVector()
        {
            lengthPartitioning = false;
            rangedLengthPartitioning = false;
            widthPartitioning = false;
            rangedWidthPartitioning = false;
            deductiveStepsPartitioning = false;
            rangedDeductiveStepsPartitioning = false;

            lengthPartitions = new IntegerPartition();
            widthPartitions = new IntegerPartition();
            stepsPartitions = new IntegerPartition();

            // Default to 100% of the givens
            //numberOfOriginalGivens = numGivens;

            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = false;
        }

        public QueryFeatureVector(List<int> lengthParts, List<int> widthParts, List<int> stepParts)
        {
            lengthPartitioning = false;
            rangedLengthPartitioning = false;
            widthPartitioning = false;
            rangedWidthPartitioning = false;
            deductiveStepsPartitioning = false;
            rangedDeductiveStepsPartitioning = false;

            lengthPartitions = new IntegerPartition(lengthParts);
            widthPartitions = new IntegerPartition(widthParts);
            stepsPartitions = new IntegerPartition(stepParts);

            // Default to 100% of the givens
            //numberOfOriginalGivens = numGivens;

            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = false;
        }

        //
        // A wrapper class to represent integer partitioning of values:
        //     ubs[0] ubs[1] ubs[2] ubs[3]
        //  [     |     |      |      |     ]
        //
        // Note: this is an inequality check implementation; for example we use <= to compare with the upper bound, NOT <
        //
        public class IntegerPartition
        {
            private List<int> partitionUpperBounds;

            public IntegerPartition() { partitionUpperBounds = new List<int>(); }

            public IntegerPartition(List<int> ubs)
            {
                partitionUpperBounds = new List<int>(ubs);

                partitionUpperBounds.Sort();
            }

            public void SetPartitions(List<int> ubs)
            {
                partitionUpperBounds = new List<int>(ubs);

                partitionUpperBounds.Sort();
            }

            public int GetUpperBound(int index) { return partitionUpperBounds[index]; }
            public int Size() { return partitionUpperBounds.Count; }

            // Given the specific value, we reutrn the index; this facilitates comparing
            // indices to determine if two problems are in the same dictated partitions
            public int GetPartitionIndex(int value)
            {
                // General case with > 1 partition value
                for (int p = 0; p < partitionUpperBounds.Count; p++)
                {
                    if (value <= partitionUpperBounds[p]) return p;
                }

                // The value is greater than all given partitions (also handles case where only 1 partition value given)
                return partitionUpperBounds.Count;
            }
        }

        //
        // --------------------------- Factories for various query vectors -----------------------------------
        //
        public static QueryFeatureVector ConstructGoalIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.goalIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructSourceIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.sourceIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructSourceAndGoalIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.sourceIsomorphism = true;
            query.goalIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructDeductiveBasedIsomorphismQueryVector(List<int> partitions)
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.deductiveStepsPartitioning = true;
            query.rangedDeductiveStepsPartitioning = true;

            query.stepsPartitions.SetPartitions(partitions);

            return query;
        }

        public static QueryFeatureVector ConstructDeductiveGoalIsomorphismQueryVector(List<int> partitions)
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.deductiveStepsPartitioning = true;
            query.rangedDeductiveStepsPartitioning = true;

            query.goalIsomorphism = true;

            query.stepsPartitions.SetPartitions(partitions);

            return query;
        }

        //public static QueryFeatureVector ConstructWidthBasedQueryVector(int minWidth, int maxWidth)
        //{
        //    QueryFeatureVector query = new QueryFeatureVector();

        //    query.widthPartitioning = true;

        //    query.minWidth = minWidth;
        //    query.maxWidth = maxWidth;

        //    return query;
        //}

        //public static QueryFeatureVector ConstructLengthBasedQueryVector(int minLength, int maxLength)
        //{
        //    QueryFeatureVector query = new QueryFeatureVector();

        //    query.lengthPartitioning = true;

        //    query.minLength = minLength;
        //    query.maxLength = maxLength;

        //    return query;
        //}

        //public static QueryFeatureVector ConstructWidthLengthBasedQueryVector(int minLength, int maxLength, int minWidth, int maxWidth)
        //{
        //    QueryFeatureVector query = new QueryFeatureVector();

        //    query.widthPartitioning = true;
        //    query.lengthPartitioning = true;

        //    query.minLength = minLength;
        //    query.maxLength = maxLength;

        //    query.minWidth = minWidth;
        //    query.maxWidth = maxWidth;

        //    return query;
        //}

        public override string ToString()
        {
            string retS = "[ ";

            if (sourceIsomorphism) retS += "Source Isomorphism";
            if (pathIsomorphism) retS += "Path Isomorphism";
            if (goalIsomorphism) retS += "Goal Isomorphism";

            return retS + " ]";
        }
    }
}
