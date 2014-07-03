using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace LiveGeometry.AtomicRegionIdentifier
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public static class AtomicIdentifierMain
    {
        public static List<AtomicRegion> GetAtomicRegions(List<Point> figurePoints,
                                                          List<GeometryTutorLib.ConcreteAST.Circle> circles,
                                                          List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            //
            // Convert all circles to atomic regions identifying their chord / radii substructure.
            //
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                List<AtomicRegion> circleAtoms = circle.Atomize(figurePoints);

                // Make this circle the owner of the atomic regions.
                foreach (AtomicRegion atom in circleAtoms)
                {
                    atom.AddOwner(circle);
                }
                atoms.AddRange(circleAtoms);
            }

            // Make all of the polygons an atomic region.
            for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                {
                    ShapeAtomicRegion shapeAtom = new ShapeAtomicRegion(poly);
                    shapeAtom.AddOwner(poly);
                    atoms.Add(shapeAtom);
                }
            }

            //
            // Since circles were atomized, there may be duplicate atomic regions.
            //
            atoms = RemoveDuplicates(atoms);

            //
            // Combine all of the atomic regions together.
            //
            atoms = ComposeRegions(figurePoints, atoms);

            return atoms;
        }

        private static List<AtomicRegion> RemoveDuplicates(List<AtomicRegion> atoms)
        {
            List<AtomicRegion> unique = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atoms)
            {
                if (!unique.Contains(atom)) unique.Add(atom);
            }

            return unique;
        }

        //
        // Worklist algorithm to combine all shapes with atomized circles.
        //
        private static List<AtomicRegion> ComposeRegions(List<Point> figurePoints, List<AtomicRegion> atoms)
        {
            List<AtomicRegion> worklist = new List<AtomicRegion>(atoms);
            List<AtomicRegion> completed = new List<AtomicRegion>();

            // Take 1 atom from the list and compare against all others in the list.
            // the first time two atoms create more than the two atoms, add them to the worklist.
            // If no changes with this atom, add to compelted list.
            while (worklist.Any())
            {
                AtomicRegion current = worklist[0];
                worklist.RemoveAt(0);

                List<AtomicRegion> composed = new List<AtomicRegion>();
                foreach (AtomicRegion atom in worklist)
                {
                    List<AtomicRegion> toAdd = null;
                    List<AtomicRegion> toRemove = null;
                    AtomComposer.Compose(figurePoints, current, atom, out toAdd, out toRemove);

                    composed.AddRange(toAdd);
                    // Remove atoms.
                }

                worklist.AddRange(composed);
                completed.Add(current);
            }

            return completed;
        }
    }
}