using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public abstract class AtomicRegion
    {
        public AtomicRegion()
        {
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            AtomicRegion thatAtom = obj as AtomicRegion;
            if (thatAtom == null) return false;

            return true;
        }
    }
}
