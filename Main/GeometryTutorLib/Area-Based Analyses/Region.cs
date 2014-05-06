using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A set of atomic regions (one or more) that possibly make up a basic shape.
    /// </summary>
    public class Region
    {
        public List<AtomicRegion> atoms { get; private set; }

        public Region()
        {
            atoms = new List<AtomicRegion>();
        }

        public Region(AtomicRegion a)
        {
            atoms = new List<AtomicRegion>();
            atoms.Add(a);
        }

        public Region(List<AtomicRegion> ats)
        {
            atoms = new List<AtomicRegion>(ats);
        }

        public bool AddAtomicRegion(AtomicRegion atom)
        {
            if (atoms.Contains(atom)) return false;

            atoms.Add(atom);

            return false;
        }
    }
}
