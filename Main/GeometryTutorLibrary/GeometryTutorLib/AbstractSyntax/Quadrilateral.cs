using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class Quadrilateral : Polygon
    {
        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Quadrilateral - Unimplemented");
            sb.AppendLine();
        }
    }
}
