﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class SegmentBisector : Bisector
    {
        public Intersection bisected { get; private set; }
        public Segment bisector { get; private set; }

        public SegmentBisector(Intersection b, Segment bisec, string just) : base(just)
        {
            bisected = b;
            bisector = bisec;
        }

        public override bool Covers(GroundedClause gc)
        {
            return bisected.Covers(gc) || bisector.Covers(gc);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // SegmentBisector has a specific order associated with the intersection segments.
        public override bool StructurallyEquals(Object obj)
        {
            SegmentBisector b = obj as SegmentBisector;
            if (b == null) return false;

            // The bisector segment
            if (!bisector.StructurallyEquals(b.bisector)) return false;

            // The intersection points
            if (!bisected.intersect.StructurallyEquals(b.bisected.intersect)) return false;

            // The bisected segments
            return bisected.OtherSegment(bisector).StructurallyEquals(b.bisected.OtherSegment(b.bisector));
        }

        public override bool Equals(Object obj)
        {
            SegmentBisector b = obj as SegmentBisector;
            if (b == null) return false;
            return bisector.Equals(b.bisector) && bisected.Equals(b.bisected) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "SegmentBisector(" + bisector.ToString() + " Bisects(" + bisected.OtherSegment(bisector) + ") at " + bisected.intersect + ")";
        }
    }
}
