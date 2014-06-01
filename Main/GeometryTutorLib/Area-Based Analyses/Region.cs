using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A set of atomic regions (one or more) that possibly make up a basic shape.
    /// </summary>
    public class Region
    {
        // All the individual atomic regions.
        public List<AtomicRegion> atoms { get; private set; }

        public List<Point> ownedPoints { get; protected set; }

        public Region(List<AtomicRegion> ats)
        {
            atoms = new List<AtomicRegion>(ats);
            ownedPoints = new List<Point>();

            foreach (AtomicRegion atom in ats)
            {
                ownedPoints.AddRange(atom.ownedPoints);
            }
        }

        public Region(AtomicRegion atom) : this(Utilities.MakeList<AtomicRegion>(atom)) { }

        public override int GetHashCode() { return base.GetHashCode(); }

        public bool HasAtom(AtomicRegion atom)
        {
            return atoms.Contains(atom);
        }

        //
        // Create a region; check if the atoms make a single shape.
        // A single shape can be formed only if atoms are of the following combinations:
        //    (1) sectors + sectors = Circle
        //    (2) Weird region + 
        //
        public static Region MakeRegion(List<AtomicRegion> atoms, List<Circle> circles, List<Polygon>[] polygons, List<Sector> minorSectors, List<Sector> majorSectors)
        {
            List<Point> atomOwnedPoints = new List<Point>();

            foreach (AtomicRegion atom in atoms)
            {
                atomOwnedPoints.AddRange(atom.ownedPoints);
            }

            foreach (Circle circle in circles)
            {
                if (circle.DefinedByAtoms(atomOwnedPoints)) return new ShapeRegion(atoms, circle);
            }
            
            foreach (Sector sector in minorSectors)
            {
                if (sector.DefinedByAtoms(atomOwnedPoints)) return new ShapeRegion(atoms, sector);
            }

            foreach (Sector sector in majorSectors)
            {
                if (sector.DefinedByAtoms(atomOwnedPoints)) return new ShapeRegion(atoms, sector);
            }

            for (int p = 0; p < polygons.Length; p++)
            {
                foreach (Polygon poly in polygons[p])
                {
                    if (poly.DefinedByAtoms(atomOwnedPoints)) return new ShapeRegion(atoms, poly);
                }
            }

            return new Region(atoms);
            
            //// We try to stitch together all atoms into one.
            //AtomicRegion stitched = StitchTogetherAtoms(atoms);

            //// Stitching failed: just return an (abnormal) region.
            //if (stitched == null) return new Region(atoms);

            //// If we were able to stitch together into a single atom, check if it is a known shape.

        }

        //private static AtomicRegion StitchTogetherAtoms(List<AtomicRegion> atoms)
        //{
        //    // No stitching is needed.
        //    if (atoms.Count == 1) return atoms[0];

        //    // Fixed-point stitching onto a working atom.
        //    AtomicRegion working = atoms[0];

        //    bool progress = true;
        //    bool[] marked = new bool[atoms.Count];
        //    marked[0] = true;
        //    while (progress)
        //    {
        //        progress = false;
        //        for (int a = 1; a < atoms.Count; a++)
        //        {
        //            // Check all unmarked atoms to combine.
        //            if (!marked[a])
        //            {
        //                if (working.Stitch(atoms[a]))
        //                {
        //                    marked[a] = true;
        //                    progress = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return marked.Contains(false) ? null : working;
        //}

        public override bool Equals(Object obj)
        {
            Region thatRegion = obj as Region;
            if (thatRegion == null) return false;

            if (this.atoms.Count != thatRegion.atoms.Count) return false;

            foreach (AtomicRegion atom in atoms)
            {
                if (!thatRegion.HasAtom(atom)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            string str = "{ ";

            for (int a = 0; a < atoms.Count; a++)
            {
                str += atoms[a].ToString();
                if (a < atoms.Count - 1) str += ", ";
            }

            return str + " }";
        }
    }
}
