using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class ConcreteMidpoint : GroundedClause
    {
        public ConcretePoint midpoint { get; private set; }
        public ConcreteSegment segment { get; private set; }

        public ConcreteMidpoint(ConcretePoint mid, ConcreteSegment seg, string just) : base()
        {
            midpoint = mid;
            segment = seg;
            justification = just;
        }

        public override bool Equals(Object obj)
        {
            ConcreteMidpoint midptObj = obj as ConcreteMidpoint;
            if (midptObj == null) return false;
            return midpoint.Equals(midptObj.midpoint) && segment.Equals(midptObj.segment);
        }

        public override string ToString()
        {
            return "Midpoint(" + midpoint.ToString() + ", " + segment.ToString() + "): " + justification;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}
