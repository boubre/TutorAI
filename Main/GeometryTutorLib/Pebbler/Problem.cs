using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // A problem is defined as a the sub-hypergraph from a set of source nodes to goal nodes
    //
    public class Problem
    {
        // Problem Statement
        public List<int> givens { get; private set; }
        public int goal { get; private set; }
        int problemId;

        // Path from start of problem to end of problem
        public List<int> path { get; private set; } 
        List<PebblerHyperEdge> edges;

        public Problem(List<int> gs, int gl, int id)
        {
            givens = gs;
            goal = gl;
            problemId = id;

            path = new List<int>();
            edges = new List<PebblerHyperEdge>();
        }

        public Problem(Problem thatProblem)
        {
            givens = new List<int>(thatProblem.givens);
            goal = thatProblem.goal;
            problemId = thatProblem.problemId;

            path = new List<int>(thatProblem.path);
            edges = new List<PebblerHyperEdge>(thatProblem.edges);
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

            foreach (int given in this.givens)
            {
                if (!thatProblem.givens.Contains(given)) return false;
            }

            foreach (int node in this.path)
            {
                if (!thatProblem.path.Contains(node)) return false;
            }

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
            str.AppendLine("} -> " + goal);

            return str.ToString();
        }
    }
}
