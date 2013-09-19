using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public class FigureList : Node
    {
        public Figure Head { get; private set; }
        public FigureList Tail { get; private set; }

        public FigureList(Figure head, FigureList tail)
        {
            Head = head;
            Tail = tail;
        }

        public Boolean HasTail()
        {
            return Tail != null;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("FigureList");
            sb.AppendLine();
            Head.BuildUnparse(sb, tabDepth + 1);
            if (HasTail()) Tail.BuildUnparse(sb, tabDepth);
        }
    }
}
