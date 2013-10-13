using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes congruent figures
    /// </summary>
    public class Congruent : Descriptor
    {
        public ConcreteFigure Figure1 { get; private set; }
        public ConcreteFigure Figure2 { get; private set; }

        /// <summary>
        /// Create a new Congruent statement
        /// </summary>
        /// <param name="f1">A figure.</param>
        /// <param name="f2">A congruent figure.</param>
        public Congruent(ConcreteFigure f1, ConcreteFigure f2)
        {
            this.Figure1 = f1;
            this.Figure2 = f2;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Congruent");
            sb.AppendLine();
            Figure1.BuildUnparse(sb, tabDepth + 1);
            Figure2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            Congruent c = obj as Congruent;
            if (c == null) return false;
            return c.Figure1.Equals(Figure1) && c.Figure2.Equals(Figure2);
        }
    }
}
