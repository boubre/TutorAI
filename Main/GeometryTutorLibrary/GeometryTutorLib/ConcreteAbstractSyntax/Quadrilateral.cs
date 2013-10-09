using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents a Quadrilateral.
    /// </summary>
    /// <remarks>
    /// This class is currently unimplemented.
    /// </remarks>
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
