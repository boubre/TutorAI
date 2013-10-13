using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes colinear segments
    /// </summary>
    public class Colinear : Descriptor
    {
        public ConcreteSegment Segment1 { get; private set; }
        public ConcreteSegment Segment2 { get; private set; }

        /// <summary>
        /// Create a new Colinear statement
        /// </summary>
        /// <param name="s1">A segment.</param>
        /// <param name="s2">A colinear segment.</param>
        public Colinear(ConcreteSegment s1, ConcreteSegment s2)
        {
            this.Segment1 = s1;
            this.Segment2 = s2;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Colinear");
            sb.AppendLine();
            Segment1.BuildUnparse(sb, tabDepth + 1);
            Segment2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            Colinear c = obj as Colinear;
            if (c == null) return false;
            return c.Segment1.Equals(Segment1) && c.Segment2.Equals(Segment2);
        }
    }
}
