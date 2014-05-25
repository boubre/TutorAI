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

        //
        // The list of atomic regions is broken into two lists: (1) those that make shapes, (2) those that do not.
        //

        // If this region defines one or many shapes, we have them here.
        public List<Figure> shapes { get; private set; }

        // If this region defines one or many shapes, we have them here.
        public List<AtomicRegion> nonShapes { get; private set; }

        public Region(List<AtomicRegion> ats)
        {
            atoms = new List<AtomicRegion>(ats);
            shapes = new List<Figure>();
            nonShapes = new List<AtomicRegion>();

            IdentifyShapes();
        }

        public Region(AtomicRegion atom) : this(Utilities.MakeList<AtomicRegion>(atom)) { }

        private void IdentifyShapes()
        {

        }

        public bool HasAtom(AtomicRegion atom)
        {
            return atoms.Contains(atom);
        }

        public int NumShapesDefineRegion()
        {
            return shapes.Count;
        }

        public bool DefinedByAllShapes()
        {
            return !nonShapes.Any();
        }

        public bool DefinesSingleShape()
        {
            return !nonShapes.Any() && shapes.Count == 1;
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
    }
}
