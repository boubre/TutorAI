using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace StatisticsGenerator
{
    public class ShadedAreaFigureStatisticsAggregator : FigureStatisticsAggregator
    {
        public int numShapes;
        public int numRootShapes;
        public int numAtomicRegions;

        public bool originalProblemInteresting;

        public ShadedAreaFigureStatisticsAggregator() : base()
        {
            numShapes = 0;
            numRootShapes = 0;
            numAtomicRegions = 0;

            originalProblemInteresting = false;
        }
    }
}
