using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.AbstractSyntax
{
    public abstract class Node
    {
        public string Unparse()
        {
            StringBuilder sb = new StringBuilder();
            BuildUnparse(sb, 0);
            return sb.ToString();
        }

        internal abstract void BuildUnparse(StringBuilder sb, int tabDepth);

        private int tabSize = 3;
        protected void Indent(StringBuilder sb, int tabDepth)
        {
            for (int i = 0; i < tabSize * tabDepth; i++)
            {
                sb.Append(' ');
            }
        }
    }
}
