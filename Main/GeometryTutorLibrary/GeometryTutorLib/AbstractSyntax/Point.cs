using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class Point : Figure
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Point [");
            sb.Append(X);
            sb.Append(", ");
            sb.Append(Y);
            sb.Append(']');
            sb.AppendLine();
        }
    }
}
