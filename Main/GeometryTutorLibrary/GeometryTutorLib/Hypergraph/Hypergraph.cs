using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericAbstractSyntax;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Create the dependence structure of Concrete Nodes, using the figure input to deduce (Instantiate) clauses.
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Hypergraph
    {     
        private readonly static int UNMARKED_NODE = -1;

        private class HyperNode
        {
            public int id;
            public GroundedClause clause;

            public List<int> successorNodes;
            public List<int> predecessorNodes;

            public List<HyperEdge> successorEdges;
            public List<TransposeHyperEdge> predecessorEdges;

            public int marked;
            public bool computed;
            public bool val;
            public int numNegArgs;
            
            public HyperNode(GroundedClause gc, int i)
            {
                id = i;
                clause = gc;
                marked = UNMARKED_NODE;
                computed = false;
                val = false;
                numNegArgs = 0;

                successorNodes = new List<int>();
                successorEdges = new List<HyperEdge>();
                predecessorNodes = new List<int>();
                predecessorEdges = new List<TransposeHyperEdge>();
            }

            public override string ToString()
            {
                string retS = clause.ToString() + "= { ";

                retS += id + ", ";
                retS += marked + ", ";
                retS += computed + ", ";
                retS += val + ", ";
                retS += "SuccN={";
                foreach (int n in successorNodes) retS += n + ",";
                if (successorNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
                retS += "}, SuccE = { ";
                foreach (HyperEdge edge in successorEdges) { retS += edge.ToString() + ", "; }
                if (successorEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
                retS += " } }, PredN={";
                foreach (int n in predecessorNodes) retS += n + ",";
                if (predecessorNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
                retS += "}, PredE = { ";
                foreach (TransposeHyperEdge edge in predecessorEdges) { retS += edge.ToString() + ", "; }
                if (predecessorEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
                retS += " }";

                return retS;
            }
        }

        public class HyperEdge
        {
            public List<int> sourceNodes;
            public int targetNode;
            public bool visited;
            public int numNegArgs; // Number of false atoms in a clause; in this case, init to number of source nodes

            public HyperEdge(List<int> src, int target)
            {
                sourceNodes = src;
                targetNode = target;
                visited = false;
                numNegArgs = src.Count;
            }

            // The source nodes and target must be the same for equality.
            public override bool Equals(object obj)
            {
                HyperEdge thatEdge = obj as HyperEdge;
                if (thatEdge == null) return false;
                foreach (int src in sourceNodes)
                {
                    if (!thatEdge.sourceNodes.Contains(src)) return false;
                }
                return targetNode == thatEdge.targetNode;
            }

            public override string ToString()
            {
                String retS = " { ";
                foreach (int node in sourceNodes)
                {
                    retS += node + ",";
                }
                if (sourceNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
                retS += " } -> " + targetNode;
                return retS;
            }
        }

        public class TransposeHyperEdge
        {
            public List<int> targetNodes;
            public int source;
            public bool visited;

            public TransposeHyperEdge(int src, List<int> targets)
            {
                targetNodes = targets;
                source = src;
                visited = false;
            }

            public override string ToString()
            {
                String retS = source + " -> { ";
                foreach (int node in targetNodes)
                {
                    retS += node + ",";
                }
                if (targetNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
                retS += " } ";
                return retS;
            }
        }

        // A list of all hyperedges
        private List<HyperEdge> graphHyperedges;

        // List of ground clauses
        List<ConcreteAbstractSyntax.GroundedClause> grounds;

        // Clauses that have gone through the instantiation process
        List<GroundedClause> groundChecked;

        // Clauses that have gone through the instantiation process
        List<GroundedClause> sourceNodes;
        List<GroundedClause> leafNodes;


        private HyperNode[] nodes;

        public Hypergraph(List<ConcreteAbstractSyntax.GroundedClause> grs)
        {
            grounds = grs;
            groundChecked = new List<GroundedClause>();
            sourceNodes = new List<GroundedClause>();
            leafNodes = new List<GroundedClause>();
            graphHyperedges = new List<HyperEdge>();
        }

        //
        // Add all new deduced clauses to the worklist if they have not been deduced before.
        // If the given clause has been deduced before, update the hyperedges that were generated previously
        //
        private void AddDeducedClausesToWorklist(List<GroundedClause> worklist,
                                                 List<KeyValuePair<List<GroundedClause>, GroundedClause>> newVals)
        {
            foreach (KeyValuePair<List<GroundedClause>, GroundedClause> gc in newVals)
            {
                int newClauseGroundIndex = groundChecked.IndexOf(gc.Value);
                int newClauseWorklistIndex = worklist.IndexOf(gc.Value);

                // If this node has not been deduced previously
                if (newClauseGroundIndex == -1 && newClauseWorklistIndex == -1)
                {
                    worklist.Add(gc.Value);
                }
                else
                {
                    if (newClauseGroundIndex != -1)
                    {
                        Debug.WriteLine("Has been previously deduced (ground): " + gc.Value.ToString());

                        // Change all (sources -> target) pairs to be (sources -> previously-deduced)
                        foreach (GroundedClause c in gc.Key)
                        {
                            // Remove the successor node
                            c.GetSuccessors().Remove(gc);
                            c.AddSuccessorEdge(new KeyValuePair<List<GroundedClause>, GroundedClause>(gc.Key, groundChecked.ElementAt(newClauseGroundIndex)));

                            // Remove the predecessor node
                            KeyValuePair<GroundedClause, List<GroundedClause>> pred = new KeyValuePair<GroundedClause, List<GroundedClause>>(gc.Value, gc.Key);
                            c.GetPredecessors().Remove(pred);
                            c.AddPredecessorEdge(new KeyValuePair<GroundedClause, List<GroundedClause>>(groundChecked.ElementAt(newClauseGroundIndex), gc.Key));
                        }
                    }

                    if (newClauseWorklistIndex != -1)
                    {
                        Debug.WriteLine("Has been previously deduced (worklist): " + gc.Value.ToString());

                        // Change all (sources -> target) pairs to be (sources -> previously-deduced)
                        foreach (GroundedClause c in gc.Key)
                        {
                            // Remove the successor node
                            c.GetSuccessors().Remove(gc);
                            c.AddSuccessorEdge(new KeyValuePair<List<GroundedClause>, GroundedClause>(gc.Key, worklist.ElementAt(newClauseWorklistIndex)));

                            // Remove the predecessor node
                            KeyValuePair<GroundedClause, List<GroundedClause>> pred = new KeyValuePair<GroundedClause, List<GroundedClause>>(gc.Value, gc.Key);
                            c.GetPredecessors().Remove(pred);
                            c.AddPredecessorEdge(new KeyValuePair<GroundedClause, List<GroundedClause>>(worklist.ElementAt(newClauseWorklistIndex), gc.Key));
                        }
                    }
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
                    // In order to avoid redundancy, after a substitution, we should perform a simplification
                    //List<KeyValuePair<List<GroundedClause>, GroundedClause>> returnedEqs= Substitution.Instantiate(clause);

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
        }

        //
        // Find the index of the grounded node
        //
        private int NodeAtIndex(GroundedClause n)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Equals(n)) return i;
            }
            return -1;
        }

        public void ConstructGraphRepresentation()
        {
            IdentifyAllSourceNodes();
            IdentifyAllLeafNodes();

            //
            // Construct the nodes of the graph
            //
            nodes = new HyperNode[groundChecked.Count];

            for (int i = 0; i < nodes.Length; i++)
            {
                                     // (clause                   ; local id)
                nodes[i] = new HyperNode(groundChecked.ElementAt(i), i);
            }

            //
            // Construct the hyperedges of the graph
            //
            foreach (HyperNode node in nodes)
            {
                List<KeyValuePair<List<GroundedClause>, GroundedClause>> succs = node.clause.GetSuccessors();

                // Convert all edge representations to integers
                foreach(KeyValuePair<List<GroundedClause>, GroundedClause> edge in succs)
                {
                    List<int> srcNodes = new List<int>();
                    foreach (GroundedClause src in edge.Key)
                    {
                        srcNodes.Add(src.graphId);
                        Utilities.AddUnique<int>(node.successorNodes, src.graphId);
                    }

                    HyperEdge edgeObj = new HyperEdge(srcNodes, edge.Value.graphId);
                    node.successorEdges.Add(edgeObj);

                    Utilities.AddUnique<HyperEdge>(graphHyperedges, edgeObj);
                }

                List<KeyValuePair<GroundedClause, List<GroundedClause>>> preds = node.clause.GetPredecessors();

                // Convert all transpose edge representations to integers
                foreach (KeyValuePair<GroundedClause, List<GroundedClause>> transposeEdge in preds)
                {
                    List<int> targetNodes = new List<int>();
                    foreach (GroundedClause target in transposeEdge.Value)
                    {
                        targetNodes.Add(target.graphId);
                        Utilities.AddUnique<int>(node.predecessorNodes, target.graphId);
                    }

                    node.predecessorEdges.Add(new TransposeHyperEdge(transposeEdge.Key.graphId, targetNodes));
                }
            }
        }

        // Finds all source nodes (nodes that have no predecessor).
        private void IdentifyAllSourceNodes()
        {
            foreach (GroundedClause clause in groundChecked)
            {
                // Omit nodes with no preds and no succs
                if (!clause.GetPredecessors().Any() && clause.GetSuccessors().Any()) sourceNodes.Add(clause);
            }
        }

        // Finds all source nodes (nodes that have no successor).
        private void IdentifyAllLeafNodes()
        {
            foreach (GroundedClause clause in groundChecked)
            {
                // Omit nodes with no preds and no succs
                if (!clause.GetSuccessors().Any() && clause.GetPredecessors().Any()) leafNodes.Add(clause);
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
        public void ConstructDualPaths(List<int> src, List<int> goal)
        {
            List<int> reachableNodes = new List<int>();
            foreach (int s in src)
            {
                Traverse(s);
                reachableNodes.Add(s);
            }

            //
            // Create a reachable set of <nodes, edges> (reachable subset of components of the original)
            //
            List<HyperEdge> reachableEdges = new List<HyperEdge>();
            foreach (HyperEdge edge in graphHyperedges)
            {
                if (edge.visited)
                {
                    reachableEdges.Add(edge);
                    Utilities.AddUnique<int>(reachableNodes, edge.targetNode);
                }
            }

            //PathGraph p = new PathGraph(nodes.Count, reachableEdges, reachableNodes);
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
            //Traverse(src.ElementAt(0));
            foreach (int s in src)
            {
                Traverse(s);
            }

            foreach (int goal in goals)
            {
                if (!nodes[goal].computed)
                {
                    Debug.WriteLine("Did not successfully compute: " + groundChecked.ElementAt(goal).ToString());
                    //return false;
                }
                else
                {
                    Debug.WriteLine("Computed: " + groundChecked.ElementAt(goal).ToString());
                }
            }

            Debug.WriteLine("Nodes after pebbling.");
            for (int i = 0; i < nodes.Length; i++)
            {
                Debug.WriteLine(nodes[i].id + ": computed(" + nodes[i].computed + ") marked ("
                                            + nodes[i].marked + ") NumNegArgs(" + nodes[i].numNegArgs + ")");
            }

            Debug.WriteLine("Edges after pebbling.");
            for (int i = 0; i < graphHyperedges.Count; i++)
            {
                Debug.WriteLine(i + ": visited(" + graphHyperedges.ElementAt(i).visited + ") numNegArgs(" + graphHyperedges.ElementAt(i).numNegArgs + ")");
            }

            return true;
        }

        //
        // According to the paper, this procedure updates numNegArgs for every clause in the 
        // clauselist corresponding to the positive literal current
        //
        private void Update(int nodeId)
        {
            foreach (HyperEdge edge in graphHyperedges)
            {
                if (edge.sourceNodes.Contains(nodeId))
                {
                    edge.numNegArgs--;
                }
            }
        }

        //
        // Given a node, traverse the graph
        //
        public void Traverse(int currentNodeIndex)
        {
            // If val of current is not already computed call traverse recursively
            if (nodes[currentNodeIndex].computed) return;

            // Take care of nodes initialized to true
            if (nodes[currentNodeIndex].val)
            {
                nodes[currentNodeIndex].computed = true;
                
                // Update clause to have one fewer negative value
                Update(currentNodeIndex);

                return;
            }

            // For every clause number j, compute the value of the targets of all edges with source
            // current labeled j, as long as current.val is not true
            //
            // Each hypernode has independent outgoing edges:
            // For all outgoing hyperedge 'groups' from this node,
            //   traverse depth-first to other nodes. 
            //
            List<HyperEdge> tagset = nodes[currentNodeIndex].successorEdges;

            for (int j = 0; j < tagset.Count && !nodes[currentNodeIndex].val; j++)
            {
                HyperEdge arc = tagset[j];

                // Traverse Recursively for every arc labeled j
                // That is, for all nodes which are sources of this edge, traverse
                foreach (int arcSourceNode in arc.sourceNodes)
                {
                    Debug.WriteLine("Considering Edge <" + currentNodeIndex + " : " + arcSourceNode + ", " + arc.targetNode + ">");

                    // If arc not visited then call traverse on the target of the hyperedge
                    if (!arc.visited)
                    {
                        nodes[currentNodeIndex].marked--;
                        arc.visited = true;
                        Traverse(arc.targetNode);
                    }
                    // If all arcs have been visited and target node has some unmarked outgoing edge then call traverse 
                    else if (nodes[arc.targetNode].marked != 0 && nodes[currentNodeIndex].marked == 0)
                    {
                        Traverse(arc.targetNode);
                    }
                }

                // If not already computed and all arguments for clause j are available, compute the truth
                // values of current
                if (!nodes[currentNodeIndex].computed)
                {
                    if (arc.numNegArgs == 0)
                    {
                        // Update counter for every clause in the clauselist corresponding to current and set to true
                        Update(currentNodeIndex);
                        nodes[currentNodeIndex].val = true;
                     }
                 }
            }

            nodes[currentNodeIndex].computed = true;
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("Initial Set of Clauses:\n");

            int i = 0;
            foreach (GroundedClause gc in grounds)
            {
                if (!(gc is ConcretePoint))
                {
                    Debug.WriteLine(i + " " + gc.ToString());
                    i++;
                }
            }

            Debug.WriteLine("\n\nDeduced Clauses:\n");
            for (i = 0; i < groundChecked.Count; i++)
            {
                if (!grounds.Contains(groundChecked.ElementAt(i)))
                {
                    string inNodes = "{ ";
                    foreach (GroundedClause gc in groundChecked.ElementAt(i).GetPredecessors().ElementAt(0).Value)
                    {
                        inNodes += gc.graphId + " ";
                    }
                    inNodes += "} ";

                    Debug.WriteLine(inNodes + i + " " + groundChecked.ElementAt(i).ToString() + "\n");
                }
            }

            Debug.WriteLine("\nNodes: ");
            i = 0;
            foreach (HyperNode node in nodes)
            {
                Debug.WriteLine(i + ": " + node.ToString());
                i++;
            }

            Debug.WriteLine("\nEdges: ");
            i = 0;
            foreach (HyperEdge edge in graphHyperedges)
            {
                Debug.WriteLine(i + ": " + edge.ToString());
                i++;
            }
        }
    }
}