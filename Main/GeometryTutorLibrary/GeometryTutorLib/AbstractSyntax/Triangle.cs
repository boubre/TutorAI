using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class Triangle : Polygon
    {
        public Line LineA { get; private set; }
        public Line LineB { get; private set; }
        public Line LineC { get; private set; }
        public TriangleType Type { get; private set; }

        public Triangle(Line a, Line b, Line c, TriangleType type)
        {
            LineA = a;
            LineB = b;
            LineC = c;
            Type = type;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Triangle [");
            sb.Append(Type.ToString());
            sb.Append(']');
            sb.AppendLine();
            LineA.BuildUnparse(sb, tabDepth + 1);
            LineB.BuildUnparse(sb, tabDepth + 1);
            LineC.BuildUnparse(sb, tabDepth + 1);
        }
    }
}
