using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GeometryTutorLib.FirstOrderLogic
{
    class ASTNode
    {
        string _name;
        List<ASTNode> _children;
        ASTNode _parent;

        public ASTNode(string name, ASTNode parent)
        {
            _name = name;
            _children = new List<ASTNode>();
            _parent = parent;
        }

        public ASTNode(string name)
        {
            _name = name;
            _children = new List<ASTNode>();
            _parent = null;
        }

        public string GetName()
        {
            return _name;
        }

        public void AddChild(ASTNode node)
        {
            _children.Add(node);
        }

        public ASTNode GetParent()
        {
            return _parent;
        }

        public void SetParent(ASTNode node)
        {
            _parent.DeleteChild(this);
            _parent = node;
            _parent.AddChild(this);
        }

        public int NumChildren()
        {
            return _children.Count();
        }

        public ASTNode GetChild(int num)
        {
            return _children[num];
        }

        public ASTNode GetLastChild()
        {
            return _children[_children.Count() - 1];
        }

        public void DeleteChild(ASTNode node)
        {
            _children.Remove(node);
        }

        private int tabSize = 3;
        protected void Indent(StringBuilder sb, int tabDepth)
        {
            for (int i = 0; i < tabSize * tabDepth; i++)
            {
                sb.Append(' ');
            }
        }

        public string Unparse()
        {
            StringBuilder sb = new StringBuilder();
            BuildUnparse(sb, 0);
            return sb.ToString();
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append(_name);
            sb.AppendLine();
            for (int i = 0; i < _children.Count(); i++)
            {
                _children[i].BuildUnparse(sb, tabDepth + 1);
            }
        }
    }
}