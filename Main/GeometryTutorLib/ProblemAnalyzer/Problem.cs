using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // A problem is defined as a the sub-hypergraph from a set of source nodes to a goal node
    //
    public class Problem
    {
        // Problem Statement
        public List<int> givens { get; private set; }
        public int goal { get; private set; }

        // Path from start of problem to end of problem
        public List<int> suppressedGivens { get; private set; }

        // Path from start of problem to end of problem
        public List<int> path { get; private set; }
        public List<PebblerHyperEdge> edges { get; private set; }
        public DiGraph<int> graph { get; private set; }

        public void AddEdge(PebblerHyperEdge edge)
        {
            // Add to the graph
            graph.AddHyperEdge(edge.sourceNodes, edge.targetNode);

            edges.Add(edge);
        }

        // For backward problem generation
        public Problem()
        {
            givens = new List<int>();
            goal = -1;

            path = new List<int>();
            edges = new List<PebblerHyperEdge>();
            suppressedGivens = new List<int>();

            graph = new DiGraph<int>();
        }

        public Problem(List<int> gs, int gl)
        {
            givens = gs;
            goal = gl;

            path = new List<int>();
            edges = new List<PebblerHyperEdge>();
            suppressedGivens = new List<int>();

            graph = new DiGraph<int>();
            graph.AddHyperEdge(gs, gl);
        }

        public Problem(Problem thatProblem)
        {
            givens = new List<int>(thatProblem.givens);
            goal = thatProblem.goal;

            path = new List<int>(thatProblem.path);
            edges = new List<PebblerHyperEdge>(thatProblem.edges);
            suppressedGivens = new List<int>(thatProblem.suppressedGivens);

            graph = new DiGraph<int>(thatProblem.graph);
        }

        public bool ContainsGoalEdge(int targetNode)
        {
            foreach (PebblerHyperEdge edge in edges)
            {
                if (edge.targetNode == targetNode) return true;
            }

            return false;
        }

        public bool ContainsCycle()
        {
            return graph.ContainsCycle();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //
        // Just a simple hashing mechanism
        //
        public long GetHashKey()
        {
            long key = 1;

            key *= givens[0];

            if (path.Any()) key *= path[0];

            key *= goal;

            return key;
        }

        public bool InSource(int n)
        {
            return givens.Contains(n);
        }

        public bool InPath(int n)
        {
            return path.Contains(n);
        }

        public bool HasGoal(int n)
        {
            return goal == n;
        }

        //
        // Add to an existent problem by removing the target and appending the new sources
        //
        // This problem                       { This Givens } { This Path } -> This Goal
        // The new problem is of the form:    { New Givens } { Path: emptyset } -> Goal
        //                       Combined:    { New Givens  U  This Givens \minus This Goal} {This Path  U  This Goal } -> Goal
        //
        public void CombineWithEdge(List<int> newSources, int target)
        {
            // If this is the first node in the sequence, return the other problem
            if (goal == -1)
            {
                givens.AddRange(newSources);
                goal = target;
                return;
            }

            // degenerate the target node by removing the new target from the old sources
            this.givens.Remove(target);

            // Add all the new sources to the degenerated old sources; do so uniquely
            Utilities.AddUniqueList<int>(this.givens, newSources);

            // Combine all the paths of the old and the new problems together; do so uniquely
            // Utilities.AddUniqueList<int>(newProblem.path, problemToInsert.path);

            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
            Utilities.AddUnique<int>(this.path, target);

            // Now, if there exists a node in the path AND in the givens, remove it from the path. (Why not from givens?)
            foreach (int src in this.givens)
            {
                this.path.Remove(src);
            }
        }


        //
        // Create a new problem by removing the target and appending the new sources
        //
        // This problem                       { This Givens } { This Path } -> This Goal
        // The new problem is of the form:    { New Givens } { Path: emptyset } -> Goal
        //                       Combined:    { New Givens  U  This Givens \minus This Goal} {This Path  U  This Goal } -> Goal
        //
        public Problem CombineAndCreateNewBackwardProblem(Problem problemToInsert)
        {
            // If this is the first node in the sequence, return the other problem
            if (goal == -1) return new Problem(problemToInsert);

            // Make a copy of this (old) problem.
            Problem newProblem = new Problem(this);

            // degenerate the target node by removing the new target from the old sources
            newProblem.givens.Remove(problemToInsert.goal);

            // Add all the new sources to the degenerated old sources; do so uniquely
            Utilities.AddUniqueList<int>(newProblem.givens, problemToInsert.givens);

            // Combine all the paths of the old and the new problems together; do so uniquely
            // Utilities.AddUniqueList<int>(newProblem.path, problemToInsert.path);

            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
            Utilities.AddUnique<int>(newProblem.path, problemToInsert.goal);

            // Now, if there exists a node in the path AND in the givens, remove it from the path
            foreach (int src in newProblem.givens)
            {
                newProblem.path.Remove(src);
            }

//System.Diagnostics.Debug.WriteLine("Combining --------------------------\n" + "\t" + this + "\t" + problemToInsert + "\n = \t" + newProblem + "\n-----------------------");

            return newProblem;
        }

        //
        // Create a new problem by removing the target and appending the new sources
        //
        public Problem CombineAndCreateNewProblem(Problem problemToInsert)
        {
            // Make a copy of this (old) problem.
            Problem newProblem = new Problem(this);

            // degenerate the target node by removing the new target from the old sources
            newProblem.givens.Remove(problemToInsert.goal);

            // Add all the new sources to the degenerated old sources; do so uniquely
            Utilities.AddUniqueList<int>(newProblem.givens, problemToInsert.givens);

            // Combine all the paths of the old and the new problems together; do so uniquely
            Utilities.AddUniqueList<int>(newProblem.path, problemToInsert.path);

            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
            Utilities.AddUnique<int>(newProblem.path, problemToInsert.goal);

            // Now, if there exists a node in the path AND in the givens, remove it from the path
            foreach (int src in newProblem.givens)
            {
                if (newProblem.path.Contains(src))
                {
                    newProblem.path.Remove(src);
                }
            }

//System.Diagnostics.Debug.WriteLine("Combining --------------------------\n" + this +  problemToInsert + " = " + newProblem + "\n-----------------------");

            return newProblem;
        }

        //
        // Problems are equal only if the givens, goal, and paths are the same
        //
        public override bool Equals(object obj)
        {
            Problem thatProblem = obj as Problem;
            if (thatProblem == null) return false;

            if (this.goal != thatProblem.goal) return false;

            if (this.givens.Count != thatProblem.givens.Count) return false;

            if (this.path.Count != thatProblem.path.Count) return false;

            // Union the sets; if the union is the same size as the original, they are the same
            List<int> union = new List<int>(this.givens);
            Utilities.AddUniqueList<int>(union, thatProblem.givens);
            if (union.Count != this.givens.Count) return false;

            union = new List<int>(this.path);
            Utilities.AddUniqueList<int>(union, thatProblem.path);
            if (union.Count != this.path.Count) return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Problem: { ");
            foreach (int g in givens)
            {
                str.Append(g + " ");
            }
            str.Append("} -> " + goal);

            str.Append("   Path: { ");
            foreach (int g in givens)
            {
                str.Append(g + " ");
            }
            str.Append("}, { ");
            foreach (int p in path)
            {
                str.Append(p + " ");
            }
            str.Append("} -> " + goal);

            return str.ToString();
        }

        public string ConstructProblemAndSolution(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph)
        {
            // Determine the suppressed nodes in the graph and break
            // the givens into those that must be explicitly stated to the user and those that are implicit.
            foreach (int g in givens)
            {
                ConcreteAST.GroundedClause clause = graph.vertices[g].data;
                if (clause.IsAxiomatic() || clause.IsIntrinsic())
                {
                    suppressedGivens.Add(g);
                }
            }
            suppressedGivens.ForEach(s => givens.Remove(s));

            //
            // Sort the givens and path for ease in readability
            //
            givens.Sort();
            suppressedGivens.Sort();
            path.Sort();

            StringBuilder str = new StringBuilder();

            str.AppendLine("Source: ");
            foreach (int g in givens)
            {
                str.AppendLine("\t (" + g + ")" + graph.GetNode(g).ToString());
            }
            str.AppendLine("Suppressed Source: ");
            foreach (int s in suppressedGivens)
            {
                str.AppendLine("\t (" + s + ")" + graph.GetNode(s).ToString());
            }
            str.AppendLine("HyperEdges:");
            foreach (PebblerHyperEdge edge in edges)
            {
                str.AppendLine("\t" + edge.ToString());
            }
            str.AppendLine("  Path:");
            foreach (int p in path)
            {
                str.AppendLine("\t (" + p + ")" + graph.GetNode(p).ToString());
            }

            str.Append("  -> Goal: (" + goal + ")" + graph.GetNode(goal).ToString());

            return str.ToString();
        }

        public string EdgeAndSCCDump()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("HyperEdges:");
            foreach (PebblerHyperEdge edge in edges)
            {
                str.AppendLine("\t" + edge.ToString());
            }

            str.AppendLine(this.graph.GetStronglyConnectedComponentDump());

            return str.ToString();
        }
    }
}
