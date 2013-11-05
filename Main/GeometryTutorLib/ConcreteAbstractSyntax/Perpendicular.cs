using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class Perpendicular : Descriptor
    {
        public Intersection intersection { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public Perpendicular(Intersection inter, string just) : base()
        {
            this.intersection = inter;
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
            return intersection.Equals(p.intersection);
        }

        public override string ToString()
        {
            return "Perpendicular(" + intersection.ToString() + "): " + justification;
        }
    }
}
