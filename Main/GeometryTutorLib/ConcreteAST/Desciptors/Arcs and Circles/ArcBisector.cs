using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    //public class ArcBisector : Bisector
    //{
    //    public Intersection bisected { get; private set; }
    //    public Arc bisector { get; private set; }

    //    public ArcBisector(Intersection b, Arc bisec) : base()
    //    {
    //        bisected = b;
    //        bisector = bisec;
    //    }

    //    public override bool Covers(GroundedClause gc)
    //    {
    //        return bisected.Covers(gc) || bisector.Covers(gc);
    //    }

    //    public override int GetHashCode()
    //    {
    //        //Change this if the object is no longer immutable!!!
    //        return base.GetHashCode();
    //    }

    //    // ArcBisector has a specific order associated with the intersection Arcs.
    //    public override bool StructurallyEquals(Object obj)
    //    {
    //        ArcBisector b = obj as ArcBisector;
    //        if (b == null) return false;

    //        // The bisector Arc
    //        if (!bisector.StructurallyEquals(b.bisector)) return false;

    //        // The intersection points
    //        if (!bisected.intersect.StructurallyEquals(b.bisected.intersect)) return false;

    //        // The bisected Arcs
    //        return bisected.OtherArc(bisector).StructurallyEquals(b.bisected.OtherArc(b.bisector));
    //    }

    //    public override bool Equals(Object obj)
    //    {
    //        ArcBisector b = obj as ArcBisector;
    //        if (b == null) return false;
    //        return bisector.Equals(b.bisector) && bisected.Equals(b.bisected) && base.Equals(obj);
    //    }

    //    public override string ToString()
    //    {
    //        return "ArcBisector(" + bisector.ToString() + " Bisects(" + bisected.OtherArc(bisector) + ") at " + bisected.intersect + ")";
    //    }
    //}
}
