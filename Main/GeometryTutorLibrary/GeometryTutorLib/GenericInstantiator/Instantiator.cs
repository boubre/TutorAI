using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    //
    // Using a worklist technique, instantiate all nodes and construc the hypergraph on the fly.
    //
    public class Instantiator
    {
        // Contains all processed clauses and relationships amongst the clauses
        Hypergraph<GroundedClause, int> graph;

        public Instantiator()
        {
            graph = new Hypergraph<GroundedClause, int>();
        }

        //
        // Add all new deduced clauses to the worklist if they have not been deduced before.
        // If the given clause has been deduced before, update the hyperedges that were generated previously
        //
        private void HandleDeducedClauses(List<GroundedClause> worklist,
                                          List<KeyValuePair<List<GroundedClause>, GroundedClause>> newVals)
        {
            foreach (KeyValuePair<List<GroundedClause>, GroundedClause> gc in newVals)
            {
                //
                // Is this clause is in the worklist or graph? If not, add it.
                //
                int newClauseWorklistIndex = worklist.IndexOf(gc.Value);

                if (newClauseWorklistIndex == -1 && !graph.HasNode(gc.Value))
                {
                    worklist.Add(gc.Value);

                    if (gc.Value is Equation)
                    {
                        ((Equation)gc.Value).SetId(graph.Size());
                    }
                    else if (gc.Value is ConcreteCongruent)
                    {
                        ((ConcreteCongruent)gc.Value).SetId(graph.Size());
                    }

                    Debug.WriteLine(graph.Size() + ": " + gc.Value);
                }

                // Add the target node to the graph; hypergraph handles redundancy
                graph.AddNode(gc.Value);

                // Add the edge which deduced this clause
                graph.AddEdge(gc.Key, gc.Value, 0); // 0: Annotation to be handled later
            }
        }

        //
        // Main instantiation function for all figures stated in the given list
        //
        public Hypergraph<GroundedClause, int> Instantiate(List<GroundedClause> initGrounds)
        {
            // The worklist initialized to initial set of ground clauses from the figure
            List<GroundedClause> worklist = new List<GroundedClause>(initGrounds);

            //
            // Process all new clauses until the worklist is empty
            //
            while (worklist.Any())
            {
                // Acquire the first element from the list for processing
                GroundedClause clause = worklist.ElementAt(0);
                worklist.RemoveAt(0);

                // Add this working node to the graph
                if (!(clause is ConcretePoint))
                {
                    graph.AddNode(clause);
                }

                //
                // Apply the clause to all applicable instantiators
                //
                if (clause is ConcreteAngle)
                {
                    //HandleDeducedClauses(worklist, ConcreteAngle.Instantiate(null, clause));
                    //HandleDeducedClauses(worklist, AngleBisector.Instantiate(clause));
                }
                else if (clause is ConcreteSegment)
                {
                    HandleDeducedClauses(worklist, ConcreteSegment.Instantiate(clause));
                    // HandleDeducedClauses(worklist, AngleBisector.Instantiate(clause));
                }
                else if (clause is InMiddle)
                {
                    HandleDeducedClauses(worklist, SegmentAdditionAxiom.Instantiate(clause));
                }
                else if (clause is EqualSegments)
                {
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                }
                else if (clause is Intersection)
                {
                    HandleDeducedClauses(worklist, VerticalAnglesTheorem.Instantiate(clause));
                }
                else if (clause is Equation)
                {
                    // In order to avoid redundancy, after a substitution, we should perform a simplification
                    //List<KeyValuePair<List<GroundedClause>, GroundedClause>> returnedEqs= Substitution.Instantiate(clause);

                    HandleDeducedClauses(worklist, Substitution.Instantiate(clause));
                    HandleDeducedClauses(worklist, Simplification.Instantiate(clause));
                    HandleDeducedClauses(worklist, ConcreteCongruent.Instantiate(clause));
                }
                else if (clause is ConcreteMidpoint)
                {
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                }
                else if (clause is ConcreteCollinear)
                {
                    HandleDeducedClauses(worklist, StraightAngleDefinition.Instantiate(clause));
                }
                else if (clause is ConcreteCongruentAngles)
                {
                    HandleDeducedClauses(worklist, SAS.Instantiate(clause));
                    //HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleAdditionAxiom.Instantiate(clause));
                    HandleDeducedClauses(worklist, ConcreteCongruent.Instantiate(clause));
                }
                else if (clause is ConcreteCongruentSegments)
                {
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SAS.Instantiate(clause));
                    //HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    //HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ConcreteCongruent.Instantiate(clause));
                }
                else if (clause is ConcreteTriangle)
                {
                    HandleDeducedClauses(worklist, SumAnglesInTriangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, ConcreteTriangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SAS.Instantiate(clause));
                    //HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    //HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                }
            }

           return graph;
        }
    }
}