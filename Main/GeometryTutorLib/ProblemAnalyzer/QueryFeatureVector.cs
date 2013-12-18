using System;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class QueryFeatureVector
    {
        public int minLength { get; private set; }
        public int maxLength { get; private set; }
        public int minWidth { get; private set; }
        public int maxWidth { get; private set; }

        public bool sourceIsomorphism { get; private set; }
        public bool pathIsomorphism { get; private set; }
        public bool goalIsomorphism { get; private set; }

        public QueryFeatureVector()
        {
            minLength = 2;
            maxLength = 4;

            minWidth = 1;
            maxWidth = 4;

            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = true;
        }

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
