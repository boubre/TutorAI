using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class Parallel : Descriptor
    {
        public ConcreteSegment segment1 { get; private set; }
        public ConcreteSegment segment2 { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public Parallel(ConcreteSegment segment1, ConcreteSegment segment2, string just) : base()
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
            Parallel p = obj as Parallel;
            if (p == null) return false;
            return (segment1.Equals(p.segment1) && segment2.Equals(p.segment2)) || (segment1.Equals(p.segment2) && segment2.Equals(p.segment1));
        }

        public override string ToString()
        {
            return "Parallel(" + segment1.ToString() + ", " + segment2.ToString() + "): " + justification;
        }
    }
}
