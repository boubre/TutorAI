using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericAbstractSyntax;

namespace GeometryTutorLib.Hypergraph
{
    public class Hypergraph
    {
        public class HyperEdge
        {
            public List<GroundedClause> sourceNodes;
            GroundedClause targetNode;
        }

        //// Implements a k-ary tree
        //private class TreeNode
        //{
        //    public List<TreeNode> children { get; private set; }
        //    public TreeNode parent { get; private set; }
        //    public GroundedClause value { get; private set; }
        //    public TreeNode()
        //    {
        //        children = new List<TreeNode>();
        //        parent = null;
        //        value = null;
        //    }
        //    public TreeNode(GroundedClause v, TreeNode par)
        //    {
        //        children = new List<TreeNode>();
        //        parent = par;
        //        value = v;
        //    }
        //}

        // List of ground clauses
        List<ConcreteAbstractSyntax.GroundedClause> grounds;

        // Clauses that have gone through the instantiation process
        List<GroundedClause> groundChecked;

        // Clauses that have gone through the instantiation process
        List<GroundedClause> sourceNodes;
        List<GroundedClause> leafNodes;

        // A list of paths from leaves to source nodes
        //        List<TreeNode> paths;

        public Hypergraph(List<ConcreteAbstractSyntax.GroundedClause> grs)
        {
            grounds = grs;
            groundChecked = new List<GroundedClause>();
            sourceNodes = new List<GroundedClause>();
            leafNodes = new List<GroundedClause>();
            //            paths = new List<TreeNode>();
        }

        private void AddDeducedClausesToWorklist(List<GroundedClause> worklist, List<GroundedClause> newVals)
        {
            foreach (GroundedClause gc in newVals)
            {
                if (!groundChecked.Contains(gc) && !worklist.Contains(gc))
                {
                    worklist.Add(gc);
                }
            }
        }

        public void ConstructGraph()
        {
            // The worklist initialized to ground clauses from the figure
            List<GroundedClause> worklist = new List<GroundedClause>(grounds);

            while (worklist.Any())
            {
                GroundedClause clause = worklist.ElementAt(0);
                worklist.Remove(clause);

                //
                // Construct a massive if-else chain based on what is applicable
                //
                if (clause is ConcretePoint)
                {
                }
                else if (clause is ConcreteAngle)
                {
                    //AddDeducedClausesToWorklist(worklist, ConcreteAngle.Instantiate(null, clause));
                    //AddDeducedClausesToWorklist(worklist, AngleBisector.Instantiate(clause));
                }
                else if (clause is ConcreteSegment)
                {
                    // AddDeducedClausesToWorklist(worklist, AngleBisector.Instantiate(clause));
                }
                else if (clause is InMiddle)
                {
                    AddDeducedClausesToWorklist(worklist, SegmentAdditionAxiom.Instantiate(clause));
                }
                else if (clause is EqualSegments)
                {
                    AddDeducedClausesToWorklist(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                }
                else if (clause is Intersection)
                {
                    AddDeducedClausesToWorklist(worklist, VerticalAnglesTheorem.Instantiate(clause));
                }
                else if (clause is Equation)
                {
                    AddDeducedClausesToWorklist(worklist, Substitution.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, Simplification.Instantiate(clause));
                }
                else if (clause is ConcreteMidpoint)
                {
                    AddDeducedClausesToWorklist(worklist, MidpointDefinition.Instantiate(clause));
                }
                else if (clause is ConcreteCollinear)
                {
                    AddDeducedClausesToWorklist(worklist, StraightAngleDefinition.Instantiate(clause));
                }
                else if (clause is ConcreteCongruentAngles)
                {
                    AddDeducedClausesToWorklist(worklist, SAS.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, ASA.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, AngleAdditionAxiom.Instantiate(clause));
                }
                else if (clause is ConcreteCongruentSegments)
                {
                    AddDeducedClausesToWorklist(worklist, SSS.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, SAS.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, ASA.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, HypotenuseLeg.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                }

                else if (clause is ConcreteTriangle)
                {
                    AddDeducedClausesToWorklist(worklist, SumAnglesInTriangle.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, ConcreteTriangle.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, SSS.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, SAS.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, ASA.Instantiate(clause));
                    //AddDeducedClausesToWorklist(worklist, HypotenuseLeg.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                }

                if (!(clause is ConcretePoint))
                {
                    clause.SetID(groundChecked.Count);
                    groundChecked.Add(clause);
                }
            }
            IdentifyAllSourceNodes();
            IdentifyAllLeafNodes();
        }

        // Finds all source nodes (nodes that have no predecessor).
        private void IdentifyAllSourceNodes()
        {
            foreach (GroundedClause clause in groundChecked)
            {
                if (!clause.GetPredecessors().Any()) sourceNodes.Add(clause);
            }
        }

        // Finds all source nodes (nodes that have no successor).
        private void IdentifyAllLeafNodes()
        {
            foreach (GroundedClause clause in groundChecked)
            {
                if (!clause.GetSuccessors().Any()) leafNodes.Add(clause);
            }
        }

        //
        // Constructs an integer representation of the powerset based on input value integer n
        // e.g. 2 -> { {}, {0}, {1}, {0, 1} }
        //
        private List<List<int>> ConstructPowerSetWithNoEmpty(int n)
        {
            if (n <= 0) return Utilities.MakeList<List<int>>(new List<int>());

            List<List<int>> powerset = ConstructPowerSetWithNoEmpty(n - 1);
            List<List<int>> newCopies = new List<List<int>>();

            foreach (List<int> intlist in powerset)
            {
                // Make a copy, add to copy, add to overall list
                List<int> copy = new List<int>(intlist);
                copy.Add(n - 1); // We are dealing with indices, subtract 1
                newCopies.Add(copy);
            }

            powerset.AddRange(newCopies);

            return powerset;
        }

        //
        // Construct a list of all possible pairs of source nodes mapped to leaf nodes
        //
        private List<KeyValuePair<List<int>, List<int>>> ConstructSourceAndGoalNodes()
        {
            List<List<int>> powersetSrc = ConstructPowerSetWithNoEmpty(sourceNodes.Count);
            List<List<int>> powersetLeaf = ConstructPowerSetWithNoEmpty(leafNodes.Count);
            List<KeyValuePair<List<int>, List<int>>> pairs = new List<KeyValuePair<List<int>, List<int>>>();

            foreach (List<int> srcNodeIndices in powersetSrc)
            {
                foreach (List<int> leafNodeIndices in powersetLeaf)
                {

                    pairs.Add(new KeyValuePair<List<int>, List<int>>(srcNodeIndices, leafNodeIndices));
                }
            }

            StringBuilder debugStr = new StringBuilder("{");
            foreach (List<int> srcNodeIndices in powersetSrc)
            {
                debugStr.Append(" { ");
                foreach (int i in srcNodeIndices)
                {
                    debugStr.Append(i + " ");
                }
                debugStr.AppendLine(" } ");
            }
            debugStr.AppendLine(" } ");
            Debug.WriteLine(debugStr.ToString());

            return pairs;
        }

        //
        // A path is defined as the hyperedges associated with a subset of source nodes leading to a subset of goal nodes.
        // At this time, the goal nodes are considered to be leaf nodes.
        //
        public void ConstructPath(List<int> src, List<int> goal)
        {
            StringBuilder debugStr = new StringBuilder();
            if (Pebble(src, goal))
            {
                debugStr.AppendLine("Path exists:");
                debugStr.AppendLine("SourceNode:");
                foreach (int srcNode in src)
                {
                    debugStr.AppendLine(groundChecked.ElementAt(srcNode).ToString());
                }

                debugStr.AppendLine("GoalNode:");
                foreach (int goalNode in goal)
                {
                    debugStr.AppendLine(groundChecked.ElementAt(goalNode).ToString());
                }
            }
            Debug.WriteLine(debugStr.ToString());
        }

        //
        // A path is defined as the hyperedges associated with a subset of source nodes leading to a subset of goal nodes.
        // At this time, the goal nodes are considered to be leaf nodes.
        //
        public void ConstructAllPaths()
        {
            List<KeyValuePair<List<int>, List<int>>> pairs = ConstructSourceAndGoalNodes();

            foreach (KeyValuePair<List<int>, List<int>> pair in pairs)
            {
                if (Pebble(pair.Key, pair.Value))
                {
                    Debug.WriteLine("Path exists:");
                    Debug.WriteLine("SourceNode:");
                    foreach (int srcNode in pair.Key)
                    {
                        Debug.WriteLine(groundChecked.ElementAt(srcNode).ToString());
                    }

                    Debug.WriteLine("GoalNode:");
                    foreach (int goalNode in pair.Value)
                    {
                        Debug.WriteLine(groundChecked.ElementAt(goalNode).ToString());
                    }

                    //Console.WriteLine("Path Requires:");
                    //for (int i = 0; i < ; i++)
                    //{
                    //    Console.WriteLine(groundChecked.ElementAt(goalNode).ToString());
                    //}

                }
            }
        }

        //
        // Use Dowling-Gallier pebbling technique
        //
        public bool Pebble(List<int> src, List<int> goals)
        {
            bool[] visited = new bool[groundChecked.Count]; // Auto-allocated to false;
            bool[] val = new bool[groundChecked.Count]; // Auto-allocated to false;
            int[] marked = new int[groundChecked.Count];
            bool[] computed = new bool[groundChecked.Count]; // Auto-allocated to false;

            for (int i = 0; i < marked.Length; i++)
            {
                marked[i] = groundChecked.ElementAt(i).GetSuccessors().Count;
            }

            foreach (int s in src)
            {
                Traverse(visited, val, marked, computed, s);
            }

            foreach (int goal in goals)
            {
                if (!computed[goal])
                {
                    Debug.WriteLine("Did not successfully compute: " + groundChecked.ElementAt(goal).ToString());
                    return false;
                }
                else
                {
                    Debug.WriteLine("Computed: " + groundChecked.ElementAt(goal).ToString());
                }
            }

            return true;
        }

        public void Traverse(bool[] visited, bool[] val, int[] marked, bool[] computed, int currentNodeIndex)
        {
            // Has this node already been computed?
            if (computed[currentNodeIndex]) return;

            List<GroundedClause> groundedSucc = groundChecked.ElementAt(currentNodeIndex).GetSuccessors();
            List<int> successors = new List<int>();

            // Convert clauses to integers
            foreach (GroundedClause gc in groundedSucc)
            {
                successors.Add(gc.graphId);
            }

            //
            // Visit each successor
            //
            foreach (int succ in successors)
            {
                if (!visited[succ])
                {
                    visited[succ] = true;
                    Traverse(visited, val, marked, computed, succ);
                }
                //                else if ()
            }

            computed[currentNodeIndex] = true;
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("Initial Set of Clauses:\n");

            foreach (GroundedClause g in grounds)
            {
                Debug.WriteLine(g.ToString());
            }

            Debug.WriteLine("\n\nDeduced Clauses:\n");
            foreach (GroundedClause g in groundChecked)
            {
                if (!grounds.Contains(g))
                {
                    Debug.WriteLine(g.ToString());
                }
            }
        }
    }
}