using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Calculational_Logic
{
    public class AreaAnalysisMain
    {
        public static bool InvokeLogic(List<ConcreteAST.Figure> figure, List<ConcreteAST.Figure> givens)
        {
            //
            // Pass an empty set of goals
            //
            StatisticsGenerator.HardCodedFigureAnalyzerMain deducer = new StatisticsGenerator.HardCodedFigureAnalyzerMain(figure, givens, new List<ConcreteAST.Figure>());

            //
            // What we want are the generated problems in which the goal is something relevant to our area calculations:
            // information regarding measurements or relationships among measurements.
            //
            //deducer.AnalyzeFigure();


            //
            // To invoke the logic on the figures:
            //    1) Filter the figure to only polygons and circles.
            //    2) Find ALL regions.
            //    3) Determine, via set-based analysis, how all regions are formed.
            //    4) Can we find the area of all regions? (ALL regions? Is some enough?)
            //    5) Return something
            //

            //    1) Filter the figure to only polygons and circles.
            List<Figure> polygons = null;
            List<Circle> circles = null;

            FilterFigures(figure, out polygons, out circles);

            //    2) Find ALL regions.
            //    3) Determine, via set-based analysis, how all regions are formed.
            //    4) Can we find the area of all regions? (ALL regions? Is some enough?)
            //    5) Return something
            //
            return true;
        }

        private static void FilterFigures(List<Figure> diagram, out List<Figure> polygons, out List<Circle> circles)
        {
            polygons = new List<Figure>();
            circles = new List<Circle>();

            foreach(Figure f in diagram)
            {
                if (f is Circle) circles.Add(f as Circle);
                else if (f is Triangle) polygons.Add(f);
                else if (f is Quadrilateral) polygons.Add(f);
            }
        }
    }
}
