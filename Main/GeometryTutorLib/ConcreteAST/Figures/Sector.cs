using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Sector : Figure
    {
        public Arc theArc { get; protected set; }

        public Sector(Arc a)
        {
            theArc = a;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.StructurallyEquals(sector.theArc);
        }

        public override bool Equals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.Equals(sector.theArc);
        }

        public override string ToString() { return "Sector(" + theArc + ")"; }
    }
}