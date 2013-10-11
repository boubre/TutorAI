using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A segment defined by two points.
    /// </summary>
    public class ConcreteSegment : ConcreteFigure
    {
        public ConcretePoint Point1 { get; private set; }
        public ConcretePoint Point2 { get; private set; }
        public double Length { get; private set; }

        /// <summary>
        /// Create a new ConcreteSegment. 
        /// </summary>
        /// <param name="p1">A point defining the segment.</param>
        /// <param name="p2">Another point defining the segment.</param>
        public ConcreteSegment(ConcretePoint p1, ConcretePoint p2)
        {
            Point1 = p1;
            Point2 = p2;
            Length = ConcretePoint.calcDistance(p1, p2);
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteSegment [l=");
            sb.Append(Length);
            sb.Append(']');
            sb.AppendLine();
            Point1.BuildUnparse(sb, tabDepth + 1);
            Point2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            ConcreteSegment segment = obj as ConcreteSegment;
            if (segment == null) return false;
            return segment.Point1.Equals(Point1) && segment.Point2.Equals(Point2);
        }
    }
}
