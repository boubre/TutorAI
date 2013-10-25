using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;   

namespace GeometryTutorLib
{
    public class BridgeUItoBackEnd
    {
        public static void AnalyzeFigure(List<ConcreteAbstractSyntax.GroundedClause> figure)
        {
            GeometryTutorLib.Hypergraph.Hypergraph graph = new GeometryTutorLib.Hypergraph.Hypergraph(figure);
            graph.ConstructGraph();
            graph.ConstructGraphRepresentation();
            graph.DebugDumpClauses();

            int[] srcArr = { 0, 1 };
            List<int> src = new List<int>(srcArr);
            int[] goalArr = { 5, 6, 7, 8, 9, 10 };
            List<int> goals = new List<int>(goalArr);

            graph.Pebble(src, goals);

            graph.PrintAllPathsToInteresting(1);

            //Debug.WriteLine("Path from 1 to 11:");
            //graph.PrintPath(1, 11);
            //Debug.WriteLine("Path from 1 to 14:");
            //graph.PrintPath(1, 14);
        }
    }
}
