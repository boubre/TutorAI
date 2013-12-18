using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A First Order Logic clause that describes a property about a geometric drawing.
    /// </summary>
    public abstract class GroundedClause
    {
        // A unique integer identifier (from the hypergraph)
        public int clauseId { get; private set; }
        public void SetID(int id) { clauseId = id; }

        // Intrinsic as defined theoretically: characteristics of a figure that cannot be proven.
        private bool intrinsic;
        public bool IsIntrinsic() { return intrinsic; }
        public void MakeIntrinsic() { intrinsic = true; }

        // Contains all predecessors
        public List<int> generalPredecessors { get; private set; }
        // Contains only Relation-based predecessors
        public List<int> relationPredecessors { get; private set; }

        public bool HasRelationPredecessor(GroundedClause gc) { return relationPredecessors.Contains(gc.clauseId); }
        public bool HasGeneralPredecessor(GroundedClause gc) { return generalPredecessors.Contains(gc.clauseId) || relationPredecessors.Contains(gc.clauseId); }

        public void AddRelationPredecessor(GroundedClause gc)
        {
            if (gc.clauseId == -1)
            {
                Debug.WriteLine("ERROR: id is -1: " + gc.ToString());
            }
            Utilities.AddUnique(relationPredecessors, gc.clauseId);
        }
        public void AddGeneralPredecessor(GroundedClause gc)
        {
            if (gc.clauseId == -1)
            {
                Debug.WriteLine("ERROR: id is -1: " + gc.ToString());
            }
            Utilities.AddUnique(generalPredecessors, gc.clauseId);
        }
        public void AddRelationPredecessors(List<int> preds)
        {
            foreach (int pred in preds)
            {
                if (pred == -1)
                {
                    Debug.WriteLine("ERROR: id is -1: " + pred);
                }
                Utilities.AddUnique(relationPredecessors, pred);
            }
        }
        public void AddGeneralPredecessors(List<int> preds)
        {
            foreach (int pred in preds)
            {
                if (pred == -1)
                {
                    Debug.WriteLine("ERROR: id is -1: " + pred);
                }
                Utilities.AddUnique(generalPredecessors, pred);
            }
        }

        private bool axiomatic;
        public bool IsAxiomatic() { return axiomatic; }
        public void MakeAxiomatic() { axiomatic = true; }
        public virtual bool IsAlgebraic() { return false; } // Bydefault we will say a node is geometric
        public virtual bool IsGeometric() { return true; }  //  and not algebraic
        public virtual bool IsReflexive() { return false; }
        public virtual bool Strengthened() { return false; }

        public GroundedClause()
        {
            justification = "";
            multiplier = 1;
            clauseId = -1;
            axiomatic = false;
            generalPredecessors = new List<int>();
            relationPredecessors = new List<int>();
        }

        // The justification for when a node is deduced
        protected string justification;
        public string GetJustification() { return justification; }
        public void SetJustification(string j) { justification = j; }

        //
        // For equation simplification
        //
        public int multiplier { get; set; }
        public virtual List<GroundedClause> CollectTerms()
        {
            return new List<GroundedClause>(Utilities.MakeList<GroundedClause>(this));
        }

        public override bool Equals(object obj)
        {
            GroundedClause that = obj as GroundedClause;
            if (that == null) return false;
            return multiplier == that.multiplier; // && clauseId == that.clauseId;
        }

        public virtual bool StructurallyEquals(object obj) { return false; }

        //
        // For subsitution and algebraic Simplifications
        //
        public virtual bool Contains(GroundedClause clause) { return false; }
        public virtual void Substitute(GroundedClause c1, GroundedClause c2) { }
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

        public override int GetHashCode() { return base.GetHashCode(); }

        public override abstract string ToString();
    }
}
