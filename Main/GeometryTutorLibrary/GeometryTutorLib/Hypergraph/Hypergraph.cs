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

//            public int marked;
//            public bool computed;
            public bool pebbled;
//            public int numNegArgs;

            // For path creation; represents the predecessor set of nodes
            public List<int> pi;

            public HyperNode(GroundedClause gc, int i)
            {
                id = i;
                clause = gc;
//                marked = gc.GetPredecessors().Count;
//                computed = false;
                pebbled = false;
//                numNegArgs = 0;

                successorNodes = new List<int>();
                successorEdges = new List<HyperEdge>();
                predecessorNodes = new List<int>();
                predecessorEdges = new List<TransposeHyperEdge>();

                pi = new List<int>();
            }

            public override string ToString()
            {
                string retS = clause.ToString() + "\t\t\t\t= { ";

                retS += id + ", Pebbled(";
                retS += pebbled + "), ";
//                retS += marked + ", ";
//                retS += computed + ", ";
//                retS += val + ", ";
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
                retS += "}, PI = { ";
                foreach (int p in pi) { retS += p + ", "; }
                if (pi.Count != 0) retS = retS.Substring(0, retS.Length - 2);
                retS += " } }";


                return retS;
            }
        }

        public class HyperEdge
        {
            public List<int> sourceNodes;
            public List<int> pebbles; // Contains all source nodes that have been pebbled: for each source node, there is a 'standard edge' that must be pebbled

            public int targetNode;
            public bool visited;
            public int numNegArgs; // Number of false atoms in a clause; in this case, init to number of source nodes

            public HyperEdge(List<int> src, int target)
            {
                sourceNodes = src;
                pebbles = new List<int>(); // If empty, we assume all false (not pebbled)
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

                        // Add the direct algebraic predecessors of the deduced node to the 
                        Equation newEq = gc.Value as Equation;
                        Equation oldEq = groundChecked.ElementAt(newClauseGroundIndex) as Equation;
                        if (newEq != null && oldEq != null)
                        {
                            Utilities.AddUniqueList<int>(oldEq.directAlgebraicPredecessors, newEq.directAlgebraicPredecessors);
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

                        // Add the direct algebraic predecessors of the deduced node to the 
                        Equation newEq = gc.Value as Equation;
                        Equation oldEq = worklist.ElementAt(newClauseWorklistIndex) as Equation;
                        if (newEq != null && oldEq != null)
                        {
                            Utilities.AddUniqueList<int>(oldEq.directAlgebraicPredecessors, newEq.directAlgebraicPredecessors);
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

                // This clause will eventually be added to the checked list at this position
                clause.SetID(groundChecked.Count);

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
                    AddDeducedClausesToWorklist(worklist, ConcreteSegment.Instantiate(clause));
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
                    AddDeducedClausesToWorklist(worklist, Simplification.Instantiate(clause));
                    AddDeducedClausesToWorklist(worklist, ConcreteCongruent.Instantiate(clause));
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
            for (int n = 0; n < nodes.Length; n++)
            {
                List<KeyValuePair<List<GroundedClause>, GroundedClause>> succs = nodes[n].clause.GetSuccessors();

                // Convert all edge representations to integers
                foreach (KeyValuePair<List<GroundedClause>, GroundedClause> edge in succs)
                {
                    // Convert the nodes > ints
                    List<int> srcNodes = new List<int>();
                    foreach (GroundedClause src in edge.Key)
                    {
                        srcNodes.Add(src.graphId);
                        Utilities.AddUnique<int>(nodes[n].successorNodes, src.graphId);
                    }

                    //
                    // Create ONE hyperedge object and add to all appropriate nodes
                    //
                    // If this is the smallest valued source node, create edge and add to all source nodes
                    // Note: this is ok due to the creation of the node list first
                    if (n == srcNodes.Min())
                    {
                        HyperEdge edgeObj = new HyperEdge(srcNodes, edge.Value.graphId);

                        foreach (int srcNode in srcNodes)
                        {
                                nodes[srcNode].successorEdges.Add(edgeObj);
                        }

                        // Add to the overall list of hyperedges; unique addition should not be needed
                        Utilities.AddUnique<HyperEdge>(graphHyperedges, edgeObj);
                    }
                }

                List<KeyValuePair<GroundedClause, List<GroundedClause>>> preds = nodes[n].clause.GetPredecessors();

                // Convert all transpose edge representations to integers
                foreach (KeyValuePair<GroundedClause, List<GroundedClause>> transposeEdge in preds)
                {
                    List<int> targetNodes = new List<int>();
                    foreach (GroundedClause target in transposeEdge.Value)
                    {
                        targetNodes.Add(target.graphId);
                        Utilities.AddUnique<int>(nodes[n].predecessorNodes, target.graphId);
                    }

                    nodes[n].predecessorEdges.Add(new TransposeHyperEdge(transposeEdge.Key.graphId, targetNodes));
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

/*
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
*/

        //
        // Use Dowling-Gallier pebbling technique
        //
        public bool Pebble(List<int> src, List<int> goals)
        {
            // Pebble all figure-based clauses
            foreach (GroundedClause figureClause in grounds)
            {
                if (groundChecked.Contains(figureClause))
                {
                    Traverse(figureClause.graphId);
                }
            }


            // Pebble all start nodes
            foreach (int s in src)
            {
                Traverse(s);
            }

            foreach (int goal in goals)
            {
                if (!nodes[goal].pebbled)
                {
                    Debug.WriteLine("Did not successfully pebble: " + groundChecked.ElementAt(goal).ToString());
                }
                else
                {
                    Debug.WriteLine("Pebbled: " + groundChecked.ElementAt(goal).ToString());
                }
            }

            Debug.WriteLine("Nodes after pebbling.");
            for (int i = 0; i < nodes.Length; i++)
            {
                StringBuilder str = new StringBuilder();

                str.Append(nodes[i].id + ": pebbled(" + nodes[i].pebbled + ")");

                str.Append(i + ": preds(pi)(");
                foreach (int pred in nodes[i].pi)
                {
                    str.Append(pred + " ");
                }
                str.Append(")");
                Debug.WriteLine(str);
            }

            Debug.WriteLine("Edges after pebbling.");
            for (int i = 0; i < graphHyperedges.Count; i++)
            {
                StringBuilder str = new StringBuilder();
                str.Append(i + ": sources(");
                foreach (int pred in graphHyperedges.ElementAt(i).sourceNodes)
                {
                    str.Append(pred + " ");
                }
                str.Append(")");
                
                str.Append(i + ": pebbling("); 
                foreach (int pebbled in graphHyperedges.ElementAt(i).pebbles)
                {
                    str.Append(pebbled + " ");
                }
                str.Append( ")");
                Debug.WriteLine(str);
            }


            return true;
        }

        //
        // Given a node, pebble the reachable parts of the graph
        //
        public void Traverse(int currentNodeIndex)
        {
            // Has this node already been pebbled?
            if (nodes[currentNodeIndex].pebbled) return;

            // Pebble the node currentnode
            nodes[currentNodeIndex].pebbled = true;

            //
            // For all hyperedges leaving this node, mark a pebble along the arc
            //
            foreach (HyperEdge currentEdge in nodes[currentNodeIndex].successorEdges)
            {
                // Indicate the node has been pebbled by adding to the list of pebbled nodes; should not have to be a unique addition
                Utilities.AddUnique<int>(currentEdge.pebbles, currentNodeIndex);

                // Now, check to see if the target node if available to pebble: number of incoming nodes equates to the number of pebbles
                if (currentEdge.sourceNodes.Count == currentEdge.pebbles.Count)
                {
                    // Percolate this node through the graph recursively; recursion will pebble the target node
                    Traverse(currentEdge.targetNode);

                    // For paths, indicate that we have a predecessor
                    nodes[currentEdge.targetNode].pi.AddRange(currentEdge.sourceNodes);     // it is possible to have more than one edge pebble into the target; need to handle for ALL paths
                }
            }
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public bool AcquirePath(List<int> path, int source, int target)
        {
            bool pathFound = false;

            // Did we find our goal?
            if (source == target)
            {
                Utilities.AddUnique<int>(path, source);
                //Debug.WriteLine("Added: " + source);
                return true;
            }
            // Are there any predecessors to this node?
            else if (!nodes[target].pi.Any())
            {
                return false;
            }
            else
            {
                //
                // We must add all predecessor nodes to the list and pursue those
                // nodes up the graph to nodes without predecessors
                //
                foreach (int pred in nodes[target].pi)
                {
                    if (!path.Contains(pred))
                    {
                        Utilities.AddUnique<int>(path, pred);
                        //Debug.WriteLine("Added: " + pred);
                        pathFound |= AcquirePath(path, source, pred);
                    }
                }
                Utilities.AddUnique<int>(path, target);
                //Debug.WriteLine("Added: " + target);
            }

            return pathFound;
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public void PrintPath(int source, int target)
        {
            List<int> path = new List<int>();

            if (!AcquirePath(path, source, target))
            {
                Debug.WriteLine("No path found from " + source + " to " + target);
            }
            else
            {

                int[] pathArray = path.ToArray();
                Array.Sort<int>(pathArray);

                // for all predecessors nodes
                foreach (int node in pathArray)
                {
                    Debug.WriteLine(node + " " + nodes[node]);
                }
            }
        }

        //
        // Given start and end node-pairs, print the required nodes
        //
        public void PrintAllPathsToInteresting(int source)
        {
            const int NumGoals = 3;
            MaxHeap interestingGoals = FindInterestingGoalNodes();

            for (int goal = 0; goal < NumGoals; goal++)
            {
                int goalNode = (int)interestingGoals.ExtractMax().data;

                Debug.WriteLine("Interesting Goal (" + goalNode + ")" + nodes[goalNode].ToString());

                PrintPath(source, goalNode);
            }
        }

        //
        // Find n interesting goal nodes in the hypergraph
        //
        public MaxHeap FindInterestingGoalNodes()
        {
            // Order the interesting nodes by a scored (numeric) criteria defined by each class
            MaxHeap interestingGoals = new MaxHeap(nodes.Length);

            for (int n = 0, interestingNess = 0; n < nodes.Length; n++, interestingNess = 0)
            {
                // Equations are generally not interesting' congruences are;
                // Or, is this a source node (having no predecessors)?
                if (nodes[n].clause is ArithmeticNode || !nodes[n].clause.GetPredecessors().Any())
                {
                    interestingNess = 0;
                }
                else
                {
                    // If this node produces many things and is not a source node
                    if (nodes[n].clause.GetSuccessors().Count > 1)
                    {
                        interestingNess++;
                    }

                    // If this node requires many things to prove, it is interesting
                    if (nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count > 1)
                    {
                        interestingNess += (int)Math.Floor(nodes[n].clause.GetPredecessors().ElementAt(0).Value.Count / 2);
                    }
                }

                interestingGoals.Insert(new HeapNode<int>(n), interestingNess);
            }

            return interestingGoals;
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

                    StringBuilder str = new StringBuilder();

                    str.Append(inNodes + i + " " + groundChecked.ElementAt(i).ToString());

                    if (groundChecked.ElementAt(i) is Equation)
                    {
                        str.Append("  { ");
                        foreach (int v in ((Equation)groundChecked.ElementAt(i)).directAlgebraicPredecessors)
                        {
                            str.Append(v + " ");
                        }
                        str.Append(" } ");
                    }

                    Debug.WriteLine(str);
                }
            }

            int numAlgebraicNodes = 0;

            Debug.WriteLine("\nNodes: ");
            i = 0;
            foreach (HyperNode node in nodes)
            {
                StringBuilder str = new StringBuilder();

                str.Append(i + ": " + node.ToString());
                i++;

                if (!(node.clause is Equation)) Debug.WriteLine(str);

                if (node.clause is Equation) numAlgebraicNodes++;
            }
            int numGeometricNodes = nodes.Length - numAlgebraicNodes;
            Debug.WriteLine("\nNumber of Algebraic Nodes: " + numAlgebraicNodes);
            Debug.WriteLine("Number of Geometric Nodes: " + numGeometricNodes);
            Debug.WriteLine("Total Nodes: " + nodes.Length);

            Debug.WriteLine("\nEdges: ");
            i = 0;
            foreach (HyperEdge edge in graphHyperedges)
            {
                Debug.WriteLine(i + ": " + edge.ToString());
                i++;
            }
        }

        ////
        //// According to the paper, this procedure updates numNegArgs for every clause in the 
        //// clauselist corresponding to the positive literal current
        ////
        //private void Update(int nodeId)
        //{
        //    foreach (HyperEdge edge in graphHyperedges)
        //    {
        //        if (edge.sourceNodes.Contains(nodeId))
        //        {
        //            edge.numNegArgs--;
        //        }
        //    }
        //}

        ////
        //// Given a node, traverse the graph
        ////
        //public void OriginalTraverse(int currentNodeIndex)
        //{
        //    // If val of current is not already computed call traverse recursively
        //    if (nodes[currentNodeIndex].computed) return;

        //    // Take care of nodes initialized to true
        //    if (nodes[currentNodeIndex].pebbled)
        //    {
        //        nodes[currentNodeIndex].computed = true;

        //        // Update clause to have one fewer negative value
        //        Update(currentNodeIndex);

        //        return;
        //    }

        //    // For every clause number j, compute the value of the targets of all edges with source
        //    // current labeled j, as long as current.pebbled is not true
        //    //
        //    // Each hypernode has independent outgoing edges:
        //    // For all outgoing hyperedge 'groups' from this node,
        //    //   traverse depth-first to other nodes. 
        //    //
        //    List<HyperEdge> tagset = nodes[currentNodeIndex].successorEdges;

        //    for (int j = 0; j < tagset.Count && !nodes[currentNodeIndex].pebbled; j++)
        //    {
        //        HyperEdge arc = tagset[j];

        //        // Traverse Recursively for every arc labeled j
        //        // That is, for all nodes which are sources of this edge, traverse
        //        foreach (int arcSourceNode in arc.sourceNodes)
        //        {
        //            Debug.WriteLine("Considering Edge <" + currentNodeIndex + " : " + arcSourceNode + ", " + arc.targetNode + ">");

        //            // If arc not visited then call traverse on the target of the hyperedge
        //            if (!arc.visited)
        //            {
        //                nodes[currentNodeIndex].marked--;
        //                arc.visited = true;
        //                Traverse(arc.targetNode);
        //            }
        //            // If all arcs have been visited and target node has some unmarked outgoing edge then call traverse 
        //            else if (nodes[arc.targetNode].marked != 0 && nodes[currentNodeIndex].marked == 0)
        //            {
        //                Traverse(arc.targetNode);
        //            }
        //        }

        //        // If not already computed and all arguments for clause j are available, compute the truth
        //        // values of current
        //        if (!nodes[currentNodeIndex].computed)
        //        {
        //            if (arc.numNegArgs == 0)
        //            {
        //                // Update counter for every clause in the clauselist corresponding to current and set to true
        //                Update(currentNodeIndex);
        //                nodes[currentNodeIndex].pebbled = true;
        //            }
        //        }
        //    }

        //    nodes[currentNodeIndex].computed = true;
        //}

        //public bool AcquirePath(List<int> path, int source, int target)
        //{
        //    bool pathFound = false;

        //    if (source == target)
        //    {
        //        Utilities.AddUnique<int>(path, source);
        //        return true;
        //    }
        //    else if (!nodes[target].pi.Any())
        //    {
        //        Debug.WriteLine("No path exists from node " + source + " to " + target);
        //    }
        //    else
        //    {
        //        // for all predecessors nodes
        //        for (int i = 0; i < nodes[target].pi.Count && !pathFound; i++)
        //        {
        //            int pred = nodes[target].pi[i];
        //            if (!path.Contains(pred))
        //            {
        //                pathFound = AcquirePath(path, source, pred);
        //                Utilities.AddUnique<int>(path, pred);
        //            }
        //        }
        //        Utilities.AddUnique<int>(path, target);
        //    }
        //    return pathFound;
        //}
    }
}