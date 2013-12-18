using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class Midpoint : Descriptor
    {
        public Point midpoint { get; private set; }
        public Segment segment { get; private set; }

        public Midpoint(Point mid, Segment seg, string just) : base()
        {
            midpoint = mid;
            segment = seg;
            justification = just;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;

            return midpoint.StructurallyEquals(midptObj.midpoint) && segment.StructurallyEquals(midptObj.segment);
        }

        public override bool Equals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;
            return midpoint.Equals(midptObj.midpoint) && segment.Equals(midptObj.segment) && base.Equals(obj);
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
