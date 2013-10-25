using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A First Order Logic clause that describes a property about a geometric drawing.
    /// </summary>
    public abstract class GroundedClause
    {
        public GroundedClause()
        {
            justification = "";
            successors = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            predecessors = new List<KeyValuePair<GroundedClause, List<GroundedClause>>>();
            graphId = -1;
            multiplier = 1;
        }

        // The justification for when a node is deduced
        protected string justification;
        public string GetJustification() { return justification; }
        public void SetJustification(string j) { justification = j; }

        //
        // For the Hypergraph
        //
        private List<KeyValuePair<List<GroundedClause>, GroundedClause>> successors = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        private List<KeyValuePair<GroundedClause, List<GroundedClause>>> predecessors = new List<KeyValuePair<GroundedClause, List<GroundedClause>>>();
        public List<KeyValuePair<List<GroundedClause>, GroundedClause>> GetSuccessors() { return successors; }
        public List<KeyValuePair<GroundedClause, List<GroundedClause>>> GetPredecessors() { return predecessors; }

        public void AddSuccessorEdge(KeyValuePair<List<GroundedClause>, GroundedClause> edge) { successors.Add(edge); }
        public void AddPredecessorEdge(KeyValuePair<GroundedClause, List<GroundedClause>> edge) { predecessors.Add(edge); }

        public static void ConstructClauseLinks(List<GroundedClause> antecedent, GroundedClause consequent)
        {
            KeyValuePair<List<GroundedClause>, GroundedClause> succEdge = new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, consequent);

            foreach (GroundedClause ante in antecedent)
            {
                ante.AddSuccessorEdge(succEdge);
            }
            consequent.AddPredecessorEdge(new KeyValuePair<GroundedClause, List<GroundedClause>>(consequent, antecedent));
        }

        public int graphId { get; private set; }
        public void SetID(int id)
        {
            graphId = id;
        }

        //
        // For equation simplification
        //
        public int multiplier { get; set; }
        public virtual List<GroundedClause> CollectTerms()
        {
            return new List<GroundedClause>(Utilities.MakeList<GroundedClause>(this));
        }

        //
        // For subsitution and algebraic Simplifications
        //
        public virtual bool Contains(GroundedClause clause) { return false; }
        public virtual void Substitute(GroundedClause c1, GroundedClause c2) { }
        public virtual GroundedClause Flatten() { return this; }
        public virtual GroundedClause DeepCopy() { return (GroundedClause)this.MemberwiseClone(); }

        /// <summary>
        /// Create a formatted string that represents the tree structure of this clause and its children.
        /// </summary>
        /// <returns>A textual representation of the Clause and its subclauses</returns>
        public string Unparse()
        {
            StringBuilder sb = new StringBuilder();
//            BuildUnparse(sb, 0);
            return sb.ToString();
        }

        ///// <summary>
        ///// Build the Unparse() string.
        ///// </summary>
        ///// <param name="sb">A StringBuffer to append text to. This buffer should be passed to child nodes by calling their BuildUnparse() methods.</param>
        ///// <param name="tabDepth">The number of tabs to indent before appending text to the buffer.</param>
        //p abstract void BuildUnparse(StringBuilder sb, int tabDepth);

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

        public override abstract string ToString();
    }
}
