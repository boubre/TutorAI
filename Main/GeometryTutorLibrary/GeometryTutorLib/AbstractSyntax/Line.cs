using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class Line : Figure
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }
        public LineType Type { get; private set; }

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
