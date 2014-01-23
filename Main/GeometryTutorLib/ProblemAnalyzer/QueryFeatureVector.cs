using System;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class QueryFeatureVector
    {
        public int minLength { get; private set; }
        public int maxLength { get; private set; }
        public int minWidth { get; private set; }
        public int maxWidth { get; private set; }

        // The number of desired goals from the original figure statement
        public int numberOfOriginalGivens { get; private set; }

        public bool sourceIsomorphism { get; private set; }
        public bool pathIsomorphism { get; private set; }
        public bool goalIsomorphism { get; private set; }

        public QueryFeatureVector(int numGivens)
        {
            minLength = 2;
            maxLength = 4;

            minWidth = 1;
            maxWidth = 4;

            // Default to 100% of the givens
            numberOfOriginalGivens = numGivens;

            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = true;
        }

        //public QueryFeatureVector(int maxLeng)
        //{
        //    maxLength = maxLeng;

        //    minWidth = 1;
        //    maxWidth = 4;

        //    // Default to 100% of the givens
        //    numberOfOriginalGivens = numGivens;

        //    sourceIsomorphism = false;
        //    pathIsomorphism = false;
        //    goalIsomorphism = false;
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
