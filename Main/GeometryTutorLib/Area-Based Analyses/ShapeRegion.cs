﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A set of atomic regions (one or more) that possibly make up a basic shape.
    /// </summary>
    public class ShapeRegion : Region
    {
        //
        // The inherited list of atomic regions union to form a shape (we can calculate its area).
        //
        public Figure shape { get; private set; }

        public ShapeRegion(List<AtomicRegion> ats) : base(ats)
        {
        }

        public ShapeRegion(AtomicRegion atom) : this(Utilities.MakeList<AtomicRegion>(atom)) { }

        public ShapeRegion(List<AtomicRegion> atoms, Figure theShape) : base(atoms)
        {
            shape = theShape;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            ShapeRegion thatRegion = obj as ShapeRegion;
            if (thatRegion == null) return false;

            if (!shape.Equals(thatRegion.shape)) return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string str = "{ " + shape.ToString() + " = { ";

            for (int a = 0; a < atoms.Count; a++)
            {
                str += atoms[a].ToString();
                if (a < atoms.Count - 1) str += ", ";
            }

            return str + " } }";
        }
    }
}
