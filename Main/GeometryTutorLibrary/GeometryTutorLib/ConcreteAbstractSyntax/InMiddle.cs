using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class InMiddle : Descriptor
    {
        public ConcretePoint Point { get; private set; }
        public ConcreteSegment Segment { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public InMiddle(ConcretePoint p, ConcreteSegment segment)
        {
            this.Point = p;
            this.Segment = segment;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("InMiddle");
            sb.AppendLine();
            Point.BuildUnparse(sb, tabDepth + 1);
            Segment.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.Point.Equals(Point) && im.Segment.Equals(Segment);
        }
    }
}
