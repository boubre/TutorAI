using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericInstantiator;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Provides functionality to create a hypergraph: adding nodes and edges
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Pebbler
    {
        private readonly static int UNMARKED_NODE = -1;

        private Hypergraph<GroundedClause, int> graph;

        public Pebbler(Hypergraph<GroundedClause, int> g)
        {
            graph = g;
        }

        //// Finds all source nodes (nodes that have no predecessor).
        //private void IdentifyAllSourceNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit nodes with no preds and no succs
        //        if (!clause.GetPredecessors().Any() && clause.GetSuccessors().Any()) sourceNodes.Add(clause);
        //    }
        //}

        //// Finds all source nodes (nodes that have no successor).
        //private void IdentifyAllLeafNodes()
        //{
        //    foreach (GroundedClause clause in groundChecked)
        //    {
        //        // Omit nodes with no preds and no succs
        //        if (!clause.GetSuccessors().Any() && clause.GetPredecessors().Any()) leafNodes.Add(clause);
        //    }
        //}



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