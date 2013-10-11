using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes two lines as having equal length
    /// </summary>
    public class Equal : Descriptor
    {
        public ConcreteSegment Segment1 { get; private set; }
        public ConcreteSegment Segment2 { get; private set; }

        /// <summary>
        /// Create a new Equal descriptor
        /// </summary>
        /// <param name="s1">A segment</param>
        /// <param name="s2">A segment with the same length as the previous segment</param>
        public Equal(ConcreteSegment s1, ConcreteSegment s2)
        {
            this.Segment1 = s1;
            this.Segment2 = s2;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Equal");
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
            Equal eq = obj as Equal;
            if (eq == null) return false;
            return eq.Segment1.Equals(Segment1) && eq.Segment1.Equals(Segment1);
        }
    }
}
