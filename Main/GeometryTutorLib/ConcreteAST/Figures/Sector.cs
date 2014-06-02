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

        private bool AreClockwise(Point v1, Point v2) { return -v1.X * v2.Y + v1.Y * v2.X > 0; }

        //
        // Point must be in the given circle and then, specifically in the specified angle
        //
        public override bool IsPointOwned(Point pt)
        {
            // Is the point in the sector's circle?
            if (!theArc.theCircle.IsPointOwned(pt)) return false;

            Point relPoint = Point.MakeVector(theArc.theCircle.center, pt);

            if (theArc is MinorArc)
            {
                return !AreClockwise(theArc.endpoint1, relPoint) && AreClockwise(theArc.endpoint2, relPoint);
            }

            // Negation of MinorArc
            if (theArc is MajorArc)
            {
                return AreClockwise(theArc.endpoint1, relPoint) || !AreClockwise(theArc.endpoint2, relPoint);
            }

            return false;
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