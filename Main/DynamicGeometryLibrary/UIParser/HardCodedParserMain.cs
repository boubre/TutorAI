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
    public class HardCodedParserMain : AbstractParserMain
    {
        /// <summary>
        /// For hard-coded (old-style) tests of the system.
        /// </summary>
        public HardCodedParserMain(List<Point> points,
                                   List<GeometryTutorLib.ConcreteAST.Collinear> collinear,
                                   List<GeometryTutorLib.ConcreteAST.Segment> segments, // This is an optional list.
                                   List<GeometryTutorLib.ConcreteAST.Circle> circles,
                                   bool problemIsOn)
        {
            //
            // Calculate all of the implied components of the figure.
            //
            implied = new ImpliedComponentCalculator(points, collinear, segments, circles);
            implied.ConstructAllImplied();

            //
            // Dump these clauses, if desired
            //
            if (problemIsOn)
            {
                if (GeometryTutorLib.Utilities.CONSTRUCTION_DEBUG)
                {
                    Debug.WriteLine(implied.ToString());
                }
                
                AtomicIdentifier atomicIdentifier = new AtomicIdentifier(implied);
                List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> atomicRegions = atomicIdentifier.GetAtomicRegions();

                foreach (GeometryTutorLib.Area_Based_Analyses.AtomicRegion atom in atomicRegions)
                {
                    Debug.WriteLine(atom.ToString());
                }
            }
        }
    }
}