using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.AtomicRegionIdentifier;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DrawingParserMain : AbstractParserMain
    {
        private Drawing drawing;

        /// <summary>
        /// Create a new Drawing Parser.
        /// </summary>
        /// <param name="drawing">The drawing to parse.</param>
        /// <param name="parseController">The parseController, used to add disambiguation dialogs.</param>
        public DrawingParserMain(Drawing drawing) : base()
        {
            this.drawing = drawing;
        }

        public void Parse()
        {
            //
            // From the Live Geometry UI, we parse those components of the figures
            //
            List<IFigure> ifigs = new List<IFigure>();
            drawing.Figures.ForEach<IFigure>(f => ifigs.Add(f));

            // Parsing of the UI-based components results in populated lists for:
            // (a) Named points
            // (b) Segments
            // (c) Polygons (includes triangles and quadrilaterals)
            // (d) Regular Polygons (in polygon structure)
            // (e) Circles
            DirectComponentsFromUI parser = new DirectComponentsFromUI(drawing, ifigs);

            //
            // Dump these clauses, if desired
            //
            if (GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
            {
                Debug.WriteLine(parser.ToString());
            }

            //
            // Calculate all of the implied components of the figure.
            //
            implied = new ImpliedComponentCalculator(parser.definedPoints, parser.definedSegments, parser.circles, parser.polygons);
            implied.ConstructAllImplied();

            //
            // Dump these clauses, if desired
            //
            if (GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
            {
                Debug.WriteLine(implied.ToString());
            }

            AtomicIdentifier atomicIdentifier = new AtomicIdentifier(implied);
            List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> atomicRegions =  atomicIdentifier.GetAtomicRegions();

            foreach (GeometryTutorLib.Area_Based_Analyses.AtomicRegion atom in atomicRegions)
            {
                Debug.WriteLine(atom.ToString());
            }
        }
    }
}