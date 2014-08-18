using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Main routine for the search technique to synthesize figures.
    /// </summary>
    public static class FigureSynthesizerMain
    {
        //
        // Given a template: \alpha - \beta , etc.
        //
        public static void SynthesizeFromTemplate()
        {

        }

        //
        // Given a set of Figures, synthesize a new problem
        //
        public static void SynthesizeFromFigures(Dictionary<Figure, int> FigureMap)
        {
            if (FigureMap.Count > 3)
            {
                throw new ArgumentException("Can synthesize a figure with more than 3 Figures.");
            }

        }

        public static void SynthesizeFromTemplateAndFigures(Dictionary<Figure, int> FigureMap) // , /* Some template mechanism */)
        {
            if (FigureMap.Count > 3)
            {
                throw new ArgumentException("Can synthesize a figure with more than 3 Figures.");
            }

        }


        public static List<Figure> AlphaMinusBeta(Figure alphaOuter, Figure betaInner)
        {
            return alphaOuter.ComposeSubtraction(betaInner);
        }

        public static List<Figure> AlphaPlusBeta(Figure alphaOuter, Figure betaOuter)
        {
            return alphaOuter.ComposeAddition(betaOuter);
        }
    }
}