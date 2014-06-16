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

        public Region(AtomicRegion atom) : this(Utilities.MakeList<AtomicRegion>(atom)) { }
        public Region(List<AtomicRegion> ats)
        {
            atoms = new List<AtomicRegion>(ats);
            ownedPoints = new List<Point>();
            thisArea = -1;
        }

        // The area calculated externally, not an internally 
        private double thisArea;
        public void SetKnownArea(double a) { thisArea = a; }
        public double GetKnownArea() { return thisArea; }

        public bool IsAtomic() { return atoms.Count == 1; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public bool HasAtom(AtomicRegion atom)
        {
            return atoms.Contains(atom);
        }

        public bool RegionDefinedBy(List<AtomicRegion> thatAtoms)
        {
            return Utilities.EqualSets<AtomicRegion>(atoms, thatAtoms);
        }

        //
        // The area of this region can only be calculated if all of the atomic regions can be calculated.
        //
        public virtual bool IsComputableArea()
        {
            foreach (AtomicRegion atom in atoms)
            {
                if (!atom.IsComputableArea()) return false;
            }

            return true;
        }

        //
        // Get the numeric value of this area.
        //
        public virtual double GetArea(KnownMeasurementsAggregator known)
        {
            // Did we memoize this area?
            if (thisArea > 0) return thisArea;

            // Calculate the area if not memoized.
            double area = 0;

            foreach (AtomicRegion atom in atoms)
            {
                double currArea = atom.GetArea(known);

                if (currArea <= 0) return -1;

                area += currArea;
            }

            return area;
        }

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
