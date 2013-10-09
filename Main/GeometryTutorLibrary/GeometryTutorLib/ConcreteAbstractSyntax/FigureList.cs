using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A List of Figures on a Drawing. The Head is a Figure and the Tail is a reference to the next Figure (ie the tail is a FigureList).
    /// </summary>
    public class FigureList : Node
    {
        public Figure Head { get; private set; }
        public FigureList Tail { get; private set; }

        /// <summary>
        /// Create a new FigureList.
        /// </summary>
        /// <param name="head">A Figure on the Drawing</param>
        /// <param name="tail">The next FigureList in the sequence. Should be null if this is the last Figure.</param>
        public FigureList(Figure head, FigureList tail)
        {
            Head = head;
            Tail = tail;
        }

        /// <summary>
        /// Tests to see if this FigureList has a valid tail.
        /// </summary>
        /// <returns>TRUE if the figure has a tail, FALSE if the tail is null (which implies this is the last Figure in the list)</returns>
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
