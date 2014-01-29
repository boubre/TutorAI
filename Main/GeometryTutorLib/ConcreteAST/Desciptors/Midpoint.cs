using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class Midpoint : InMiddle
    {
        //public Point midpoint { get; private set; }
        //public Segment segment { get; private set; }

        public Midpoint(Point mid, Segment seg, string just) : base(mid, seg, just) { }
        public Midpoint(InMiddle im, string just) : base(im.point, im.segment, just) { }

        public override bool StructurallyEquals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;

            return point.StructurallyEquals(midptObj.point) && segment.StructurallyEquals(midptObj.segment);
        }

        public override bool Covers(GroundedClause gc)
        {
            if (gc is Point) return point.Equals(gc as Point) || segment.Covers(gc);
            else if (gc is Segment) return segment.Covers(gc);

            InMiddle im = gc as InMiddle;
            if (im == null) return false;
            return point.Covers(im.point) && segment.Covers(im.segment);
        }

        public override bool Equals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;
            return point.Equals(midptObj.point) && segment.Equals(midptObj.segment) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Midpoint(" + point.ToString() + ", " + segment.ToString() + "): " + justification;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}
