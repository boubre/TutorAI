﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class ShapeAtomicRegion : AtomicRegion
    {
        public Figure shape { get; private set; }
        
        //public ShapeAtomicRegion() : base()
        //{
        //}

        public ShapeAtomicRegion(Figure f) : base()
        {
            shape = f;
            ownedPoints.AddRange(f.ownedPoints);
        }


        ////
        //// Can we put together this atom with the given atomic region?
        //// Does a sequence of sides match?
        ////
        //public override bool Stitch(AtomicRegion thatAtom)
        //{
        //    if (thatAtom is NonShapeAtomicRegion) return thatAtom.Stitch(this);

        //    //
        //    // This is ShapeAtomic + ShapeAtomic
        //    //

        //    return false;
        //}

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            ShapeAtomicRegion thatAtom = obj as ShapeAtomicRegion;
            if (thatAtom == null) return false;

            return shape.StructurallyEquals(thatAtom.shape) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "ShapeAtom: (" + shape.ToString() + ")";
        }
    }
}
