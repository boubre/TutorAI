using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    /// <summary>
    /// A Line, Ray, or Segment defined by 2 points.
    /// </summary>
    public class Line : Figure
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }
        public LineType Type { get; private set; }

        /// <summary>
        /// Create a new Line. 
        /// The first point is unbounded if the figure is a Line, and bounded if it is a Ray or Segment.
        /// The second point is unbounded if the figure is a Line or Ray, and bounded if it is a Segment.
        /// </summary>
        /// <param name="p1">A point on the line.</param>
        /// <param name="p2">A different point on the line.</param>
        /// <param name="type">The type of line this object represents.  Determines which points are bounded and unbounded.</param>
        public Line(Point p1, Point p2, LineType type)
        {
            Point1 = p1;
            Point2 = p2;
            Type = type;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Line [");
            sb.Append(Type.ToString());
            sb.Append(']');
            sb.AppendLine();
            Point1.BuildUnparse(sb, tabDepth + 1);
            Point2.BuildUnparse(sb, tabDepth + 1);
        }
    }
}
