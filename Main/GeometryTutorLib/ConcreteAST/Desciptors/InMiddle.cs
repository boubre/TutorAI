using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class InMiddle : Descriptor
    {
        public Point point { get; private set; }
        public Segment segment { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public InMiddle(Point p, Segment segment, string just) : base()
        {
            this.point = p;
            this.segment = segment;
            justification = just;
        }

        //
        // Can this relationship can strengthened to a Midpoint?
        //
        public Strengthened CanBeStrengthened()
        {
            if (Utilities.CompareValues(Point.calcDistance(point, segment.Point1), Point.calcDistance(point, segment.Point2)))
            {
                return new Strengthened(this, new Midpoint(point, segment, "Precomputed"), "Precomputed");
            }

            return null;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Midpoint midpoint = gc as Midpoint;
            if (midpoint == null) return false;

            return this.point.StructurallyEquals(midpoint.midpoint) && this.segment.StructurallyEquals(midpoint.segment);
        }

        public override bool Covers(GroundedClause gc)
        {
            return point.Covers(gc) || segment.Covers(gc);
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("InMiddle");
            sb.AppendLine();
            point.BuildUnparse(sb, tabDepth + 1);
            segment.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.point.StructurallyEquals(point) && im.segment.StructurallyEquals(segment);
        }

        public override bool Equals(Object obj)
        {
            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.point.Equals(point) && im.segment.Equals(segment);
        }

        public override string ToString()
        {
            return "InMiddle(" + point.ToString() + ", " + segment.ToString() + "): " + justification;
        }
    }
}
