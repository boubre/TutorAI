﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A First Order Logic clause that describes a property about a geometric drawing.
    /// </summary>
    public abstract class GroundedClause : IEquatable<GroundedClause>
    {
        /// <summary>
        /// Create a formatted string that represents the tree structure of this clause and its children.
        /// </summary>
        /// <returns>A textual representation of the Clause and its subclauses</returns>
        public string Unparse()
        {
            StringBuilder sb = new StringBuilder();
            BuildUnparse(sb, 0);
            return sb.ToString();
        }

        /// <summary>
        /// Build the Unparse() string.
        /// </summary>
        /// <param name="sb">A StringBuffer to append text to. This buffer should be passed to child nodes by calling their BuildUnparse() methods.</param>
        /// <param name="tabDepth">The number of tabs to indent before appending text to the buffer.</param>
        internal abstract void BuildUnparse(StringBuilder sb, int tabDepth);

        private int tabSize = 3;
        /// <summary>
        /// Appends tabs to the given StringBuffer.
        /// </summary>
        /// <param name="sb">The StringBuffer to append tabs to.</param>
        /// <param name="tabDepth">The number of tabs to append.</param>
        protected void Indent(StringBuilder sb, int tabDepth)
        {
            for (int i = 0; i < tabSize * tabDepth; i++)
            {
                sb.Append(' ');
            }
        }

        public override string ToString()
        {
            return Unparse();
        }

        public abstract bool Equals(GroundedClause other);
    }
}