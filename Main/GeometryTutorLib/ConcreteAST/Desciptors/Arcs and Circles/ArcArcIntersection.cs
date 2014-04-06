using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArcArcIntersection : ArcIntersection
    {
        public Arc otherArc { get; protected set; }

        public ArcArcIntersection(Point p, Arc a, Arc thatArc) : base(p, a)
        {
            otherArc = thatArc;

            // Find the intersection points
            Point pt1, pt2;
            arc.theCircle.Intersection(otherArc.theCircle, out pt1, out pt2);
            intersection1 = pt1;
            intersection2 = pt2;
        }

        //
        // If the arcs intersect at a single point.
        //
        public override bool IsTangent()
        {
            return intersection1 != null && intersection2 == null;
        }

        //
        // If the segment starts on this arc and extends outward.
        //
        public override bool StandsOn()
        {
            return Arc.BetweenMinor(arc.endpoint1, otherArc) || Arc.BetweenMinor(arc.endpoint2, otherArc) ||
                   Arc.BetweenMinor(otherArc.endpoint1, arc) || Arc.BetweenMinor(otherArc.endpoint2, arc);
        }

        // If the arc / arc passes through this arc and extends outward.
        public override bool Crossing()
        {
            return !IsTangent() && !StandsOn();
        }

        public override bool StructurallyEquals(Object obj)
        {
            ArcArcIntersection inter = obj as ArcArcIntersection;
            if (inter == null) return false;
            return this.otherArc.StructurallyEquals(inter.otherArc) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.arc.StructurallyEquals(inter.arc);
        }

        public override bool Equals(Object obj)
        {
            ArcArcIntersection inter = obj as ArcArcIntersection;
            if (inter == null) return false;
            return this.otherArc.Equals(inter.otherArc) &&
                   this.intersect.Equals(inter.intersect) &&
                   this.arc.Equals(inter.arc);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "ArcArcIntersection(" + intersect.ToString() + ", " + arc.ToString() + ", " + otherArc.ToString() + ") " + justification;
        }
    }
}