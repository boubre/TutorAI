using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    /// <summary>
    /// Represents a Triangle, which consists of 3 lines.
    /// </summary>
    public class Triangle : Polygon
    {
        public Line LineA { get; private set; }
        public Line LineB { get; private set; }
        public Line LineC { get; private set; }
        public TriangleType Type { get; private set; }

        /// <summary>
        /// Create a new triangle bounded by the 3 given lines. The set of points that define these lines should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The line opposite point a</param>
        /// <param name="b">The line opposite point b</param>
        /// <param name="c">The line opposite point c</param>
        /// <param name="type"></param>
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
