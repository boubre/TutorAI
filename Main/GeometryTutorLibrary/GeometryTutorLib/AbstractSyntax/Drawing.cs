using System.Collections.Generic;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    /// <summary>
    /// Represents the root node of the AST.
    /// </summary>
    public class Drawing : Node
    {
        public FigureList Figures { get; private set; }

        /// <summary>
        /// Creates a new Drawing node given a <see cref="FigureList"/>.
        /// </summary>
        /// <param name="figures">The figures on the current Drawing</param>
        public Drawing(FigureList figures)
        {
            Figures = figures;
        }

        /// <summary>
        /// Iterates over all of the Figures in this Drawing's <see cref="FigureList"/>.
        /// </summary>
        /// <returns>An iterator that iterates over all of the Figures on this Drawing.</returns>
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
