using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace StatisticsGenerator
{
    public class ShadedAreaFigureStatisticsAggregator : FigureStatisticsAggregator
    {
        public int numCalculableRegions;
        public int numIncalculableRegions;

        public ShadedAreaFigureStatisticsAggregator() : base()
        {
        }
    }
}
