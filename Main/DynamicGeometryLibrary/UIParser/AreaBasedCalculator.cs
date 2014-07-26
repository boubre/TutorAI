using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Prepare / perform all calculations required for the Shaded Area problem calculations.
    /// </summary>
    public class AreaBasedCalculator
    {
        //
        // Members specific to this class and the required calculations.
        //
        // All the shapes in a single list.
        private List<Figure> allFigures;

        // The forest representing the shape hierarchy of the given figure.
        private List<Figure> shapeForest;
        public List<Figure> GetShapeHierarchy() { return shapeForest; }

        // The list of polygons which were strengthened from the deductive system.
        private List<Strengthened> strengthenedPolys;

        //
        // Implied components...just split up for this class.
        //

        // All points we can see in a drawing.
        private List<GeometryTutorLib.ConcreteAST.Point> figurePoints;
        private List<Semicircle> semiCircles;
        private List<Sector> minorSectors;
        private List<Sector> majorSectors;
        private List<GeometryTutorLib.ConcreteAST.Circle> circles;

        // Some polygons may be strengthened
        private List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons;

        // Some atomic regions may therefore, be strengthened (we update owners as well).
        private List<AtomicRegion> atomicRegions;

        public List<AtomicRegion> GetUpdatedAtomicRegions() { return atomicRegions; }

        public AreaBasedCalculator(ImpliedComponentCalculator impliedCalc, List<Strengthened> strengNodes)
        {
            allFigures = new List<Figure>();

            figurePoints = impliedCalc.allFigurePoints;
            semiCircles = impliedCalc.semiCircles;
            majorSectors = impliedCalc.majorSectors;
            minorSectors = impliedCalc.minorSectors;
            circles = impliedCalc.circles;
            polygons = impliedCalc.polygons;
            atomicRegions = impliedCalc.atomicRegions;

            // Acquire ONLY the strengthened polygons.
            strengthenedPolys = FilterStrengthenedPolygons(strengNodes);
        }

        //
        // Take a list of Strengthened objects, acquire only strengthened polygon clauses.
        //
        private List<Strengthened> FilterStrengthenedPolygons(List<Strengthened> nodes)
        {
            List<Strengthened> polys = new List<Strengthened>();

            foreach (Strengthened streng in nodes)
            {
                if (streng.strengthened is GeometryTutorLib.ConcreteAST.Polygon)
                {
                    polys.Add(streng);
                }
            }

            return polys;
        }

        public void PrepareAreaBasedCalculations()
        {
            // Strengthen any polygons 
            StrengthenPolygons();

            // Put all the figures into a single list: sectors, circles, polygons
            ComposeAllShapesIntoSingleList();

            // Now that we have all polygons and circles, associate the atomic regions with those shapes (and vice versa)
            AssociateAtomicRegionsWithShapes(figurePoints);

            // Establish the strict hierarchy of shapes (independent of atomic regions).
            ConstructShapeHierarchy(allFigures);
        }

        //
        // Compose the list of circles, sectors, and polygons into a single list
        //
        private void ComposeAllShapesIntoSingleList()
        {
            List<Figure> tempList = new List<Figure>();

            circles.ForEach(c => tempList.Add(c));
            minorSectors.ForEach(s => tempList.Add(s));
            majorSectors.ForEach(s => tempList.Add(s));
            foreach (List<GeometryTutorLib.ConcreteAST.Polygon> polys in polygons)
            {
                polys.ForEach(p => tempList.Add(p));
            }

            // Add any shapes from atomic region identification
            foreach (AtomicRegion atom in atomicRegions)
            {
                if (atom is ShapeAtomicRegion) tempList.Add((atom as ShapeAtomicRegion).shape);
            }

            // Make sure the shapes are unique.
            foreach (Figure fig in tempList)
            {
                if (!GeometryTutorLib.Utilities.HasStructurally<Figure>(allFigures, fig))
                {
                    allFigures.Add(fig);
                }
            }
        }

        //
        // If the deduction system was able to strengthen a polygon, replace the old polygon with the strengthened version.
        //
        private void StrengthenPolygons()
        {
            foreach (Strengthened strengPoly in strengthenedPolys)
            {
                StrengthenPolygon(strengPoly);
            }
        }

        //
        // Apply the strengthening clause to the appropriate polygon.
        //
        private void StrengthenPolygon(Strengthened streng)
        {
            GeometryTutorLib.ConcreteAST.Polygon strengPoly = streng.strengthened as GeometryTutorLib.ConcreteAST.Polygon;

            int numVertices = (streng.strengthened as GeometryTutorLib.ConcreteAST.Polygon).points.Count;
            int polyIndex = GeometryTutorLib.ConcreteAST.Polygon.GetPolygonIndex(numVertices);

            for (int p = 0; p < polygons[polyIndex].Count; p++)
            {
                if (polygons[polyIndex][p].StructurallyEquals(streng.original))
                {
                    // Is this the strongest class for this particular shape?
                    // For example, Quadrilateral -> Trapezoid -> Isosceles Trapezoid
                    if (strengPoly.IsStrongerThan(polygons[polyIndex][p]))
                    {
                        polygons[polyIndex][p] = strengPoly;

                        //
                        // Update any shape atomic regions as well as owners
                        // Find All instances of owners / update all figureAtoms as well (and their owners)
                        //
                        foreach (AtomicRegion atom in atomicRegions)
                        {
                            ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;
                            if (shapeAtom != null)
                            {
                                if (shapeAtom.shape.StructurallyEquals(streng.original))
                                {
                                    shapeAtom.ReshapeForStrenghthening(streng.strengthened as Figure);
                                }
                            }
                        }
                    }

                    return;
                }
            }
        }

        //
        // Associate the atoms with the figures and vice versa.
        //
        private void AssociateAtomicRegionsWithShapes(List<Point> figurePoints)
        {
            // Kill the existent list (if any)
            foreach (AtomicRegion atom in atomicRegions)
            {
                atom.ClearOwners();
            }

            foreach (Figure fig in allFigures)
            {
                foreach (AtomicRegion atom in atomicRegions)
                {
                    if (fig.Contains(figurePoints, atom))
                    {
                        fig.AddAtomicRegion(atom);
                        atom.AddOwner(fig);
                    }
                }
            }
        }

        //
        // Establish a strict hierarchy of shapes: this results in a forest of shapes.
        // (This has nothing to do with atomic shapes).
        // Do so recursively.
        // Note: containment is a transitive relationship so marking eliminates redundant calculations.
        private void ConstructShapeHierarchy(List<Figure> shapes)
        {
            //
            // Find the current set of top-most shapes.
            //
            bool[] marked = new bool[shapes.Count];
            List<Figure> topShapes = new List<Figure>();
            for (int f1 = 0; f1 < shapes.Count; f1++)
            {
                if (!marked[f1])
                {
                    bool contained = false;
                    List<Figure> containedShapes = new List<Figure>();
                    for (int f2 = 0; f2 < shapes.Count; f2++)
                    {
                        if (f1 != f2)
                        {
                            if (shapes[f2].Contains(shapes[f1]))
                            {
                                contained = true;
                                break;
                            }
                            else if (shapes[f1].Contains(shapes[f2]))
                            {
                                marked[f2] = true;
                                containedShapes.Add(shapes[f2]);
                            }
                        }
                    }

                    // Save outer shape and start recursive construction.
                    if (!contained)
                    {
                        topShapes.Add(shapes[f1]);
                        ShapeHierarchyHelper(shapes[f1], containedShapes);
                    }
                }
            }

            //
            // Initialize the class object: forest of trees representing the class hierarchy.
            //
            shapeForest = topShapes;
        }

        //
        // Finds the top-most shapes and recursively constructs the contained relationships.
        //
        private void ShapeHierarchyHelper(Figure outer, List<Figure> inner)
        {
            //
            // Base Case: Have we already processed this 'outer' figure?
            //
            if (outer.HierarchyEstablished()) return;

            //
            // Base case: no contained shapes; this is a bottom-most shape (leaf-shape in hierarchy)
            //
            if (!inner.Any())
            {
                outer.MakeLeaf();
                return;
            }

            //
            // Find the current set of top-most shapes.
            //
            List<Figure> topShapes = new List<Figure>();
            for (int f1 = 0; f1 < inner.Count; f1++)
            {
                bool contained = false;
                for (int f2 = 0; f2 < inner.Count; f2++)
                {
                    if (f1 != f2)
                    {
                        if (inner[f2].Contains(inner[f1]))
                        {
                            contained = true;
                            break;
                        }
                    }
                }

                if (!contained) topShapes.Add(inner[f1]);
            }

            //
            // Construct the child hierarchies:
            //    for each top shape, find the contained shapes, recur.
            //
            for (int f1 = 0; f1 < inner.Count; f1++)
            {
                // Find contained shapes.
                List<Figure> containedShapes = new List<Figure>();
                for (int f2 = 0; f2 < inner.Count; f2++)
                {
                    if (f1 != f2)
                    {
                        if (inner[f1].Contains(inner[f2]))
                        {
                            containedShapes.Add(inner[f2]);
                        }
                    }
                }

                // Recur
                ShapeHierarchyHelper(inner[f1], containedShapes);
            }

            //
            // Construct the list of children to add to the 'outer' figure's hierarchy.
            //
            outer.SetChildren(topShapes);
        }
    }
}