using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes a property associated with two segments that are perpendicular. 
    /// Not based on actual angles of intersection, currently
    /// </summary>
    public class Perpendicular : Descriptor
    {
        public ConcreteSegment segment1 { get; private set; }
        public ConcreteSegment segment2 { get; private set; }

        /// <summary>
        /// Create a new Perpendicular statement
        /// </summary>
        /// <param name="segment1">First segment</param>
        /// <param name="segment2">Second segment</param>
        public Perpendicular(ConcreteSegment segment1, ConcreteSegment segment2, string just)
            : base()
        {
            this.segment1 = segment1;
            this.segment2 = segment2;
            justification = just;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }


        public override bool Equals(Object obj)
        {
            Perpendicular p = obj as Perpendicular;
            if (p == null) return false;
            return (segment1.Equals(p.segment1) && segment2.Equals(p.segment2)) || (segment1.Equals(p.segment2) && segment2.Equals(p.segment1));
        }

        public override string ToString()
        {
            return "Perpendicular(" + segment1.ToString() + ", " + segment2.ToString() + "): " + justification;
        }
    }
}