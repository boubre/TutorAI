using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class AlgebraicProportionalSegments : ProportionalSegments
    {
        public AlgebraicProportionalSegments(Segment segment1, Segment segment2, string just) : base(segment1, segment2, just) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override bool Equals(Object obj)
        {
            AlgebraicProportionalSegments aps = obj as AlgebraicProportionalSegments;
            if (aps == null) return false;
            return base.Equals(aps);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "AlgebraicProportional(" + largerSegment.ToString() + " < " + dictatedProportion + " > " + smallerSegment.ToString() + "): " + justification;
        }
    }
}
