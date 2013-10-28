using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.Hypergraph;

namespace GeometryTutorLib
{
    public class BridgeUItoBackEnd
    {
        public static void AnalyzeFigure(List<ConcreteAbstractSyntax.GroundedClause> figure)
        {
            GeometryTutorLib.GenericInstantiator.Instantiator instantiator = new GeometryTutorLib.GenericInstantiator.Instantiator();

            Hypergraph<GroundedClause, int> graph = instantiator.Instantiate(figure);

            graph.DebugDumpClauses();

            Pebbler.PebblerHypergraph<GroundedClause> pebblerGraph = graph.GetPebblerHypergraph();

            int[] srcArr = { 1 };
            List<int> src = new List<int>(srcArr);

            pebblerGraph.GenerateAllPaths(src);
        }
    }
}
