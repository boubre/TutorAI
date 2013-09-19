using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class Drawing : Node
    {
        public FigureList Figures { get; private set; }

        public Drawing(FigureList figures)
        {
            Figures = figures;
        }

        public IEnumerable<Figure> GetFigures()
        {
            FigureList current = Figures;
            while (current.HasTail())
            {
                yield return current.Head;
                current = current.Tail;
            }
            yield return current.Head;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Drawing");
            sb.AppendLine();
            Figures.BuildUnparse(sb, tabDepth + 1);
        }
    }
}
