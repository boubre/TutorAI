using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricProportionalSegments : ProportionalSegments
    {
        public GeometricProportionalSegments(Segment segment1, Segment segment2) : base(segment1, segment2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override bool Equals(Object obj)
        {
            GeometricProportionalSegments aps = obj as GeometricProportionalSegments;
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
            return "GeometricProportional(" + largerSegment.ToString() + " < " + dictatedProportion + " > " + smallerSegment.ToString() + "): " + justification;
        }
    }
}
