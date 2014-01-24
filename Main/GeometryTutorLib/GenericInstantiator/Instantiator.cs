using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAST;

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
        // Main instantiation function for all figures stated in the given list; worklist technique to construct the graph
        //
        public Hypergraph<GroundedClause, int> Instantiate(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
        {
            // The worklist initialized to initial set of ground clauses from the figure
            List<GroundedClause> worklist = new List<GroundedClause>(figure);
            worklist.AddRange(givens);

            // Add all initial elements to the graph
            figure.ForEach(g => { graph.AddNode(g); g.SetID(graph.Size()); } );
            givens.ForEach(g => { graph.AddNode(g); g.SetID(graph.Size()); } );

            // Indicate all figure-based information is intrinsic; this needs to verified with the UI
            figure.ForEach(f => f.MakeIntrinsic());

            // For problem generation, indicate that all given information is intrinsic
            givens.ForEach(g => g.MakeGiven());

            // Calculates Coverage of the figure
            HandleAllFigureIntrinsicFacts(figure);

            HandleAllGivens(figure, givens);

            //
            // Process all new clauses until the worklist is empty
            //
            while (worklist.Any())
            {
                // Acquire the first element from the list for processing
                GroundedClause clause = worklist[0];
                worklist.RemoveAt(0);

                if (Utilities.DEBUG) Debug.WriteLine("Working on: " + clause.clauseId + " " + clause.ToString());

                //
                // Apply the clause to all applicable instantiators
                //
                if (clause is Angle)
                {
                    // A list of all problem angles
                    Angle.Record(clause);

                    if (clause is RightAngle)
                    {
                        HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    }
                    HandleDeducedClauses(worklist, ExteriorAngleEqualSumRemoteAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleAdditionAxiom.Instantiate(clause));

                    //HandleDeducedClauses(worklist, ConcreteAngle.Instantiate(null, clause));
                    //HandleDeducedClauses(worklist, AngleBisector.Instantiate(clause));

                    HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));
                }
                else if (clause is Segment)
                {
                    HandleDeducedClauses(worklist, Segment.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                }
                else if (clause is InMiddle)
                {
                    HandleDeducedClauses(worklist, SegmentAdditionAxiom.Instantiate(clause));
                }
                else if (clause is Intersection)
                {
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));

                    if (clause is PerpendicularBisector)
                    {
                        HandleDeducedClauses(worklist, PerpendicularBisectorDefinition.Instantiate(clause));
                    }
                    else if (clause is Perpendicular)
                    {
                        HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));
                        HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                    }
                    else
                    {
                        HandleDeducedClauses(worklist, VerticalAnglesTheorem.Instantiate(clause));
                        HandleDeducedClauses(worklist, AltIntCongruentAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, SameSideSuppleAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                        HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, CorrespondingAnglesOfParallelLines.Instantiate(clause));
                        HandleDeducedClauses(worklist, CongruentCorrespondingAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, CongruentAdjacentAnglesImplyPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                        HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, ParallelImplyAltIntCongruentAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, ParallelImplySameSideInteriorSupplementary.Instantiate(clause));
                        HandleDeducedClauses(worklist, Intersection.InstantiateSupplementary(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                    }
                }
                else if (clause is Complementary)
                {
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                }
                else if (clause is Altitude)
                {
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                }
                else if (clause is AngleBisector)
                {
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                }
                else if (clause is Supplementary)
                {
                    HandleDeducedClauses(worklist, SameSideSuppleAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                }
                else if (clause is Equation)
                {
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, ProportionalSegments.InstantiateEquation(clause));
                    if (clause is AngleEquation)
                    {
                        HandleDeducedClauses(worklist, AnglesOfEqualMeasureAreCongruent.Instantiate(clause));
                        HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                    }

                    // If a geometric equation was constructed, it may not have been checked for proportionality
                    if ((clause as Equation).IsGeometric())
                    {
                        HandleDeducedClauses(worklist, ProportionalAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, ProportionalSegments.InstantiateEquation(clause));
                    }
                }
                else if (clause is Midpoint)
                {
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointTheorem.Instantiate(clause));
                }
                else if (clause is Collinear)
                {
                    HandleDeducedClauses(worklist, StraightAngleDefinition.Instantiate(clause));
                }
                else if (clause is Median)
                {
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                }
                else if (clause is SegmentBisector)
                {
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                }
                else if (clause is Parallel)
                {
                    HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                    HandleDeducedClauses(worklist, CorrespondingAnglesOfParallelLines.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, ParallelImplyAltIntCongruentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, ParallelImplySameSideInteriorSupplementary.Instantiate(clause));
                }
                else if (clause is ProportionalSegments)
                {
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSSSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, ProportionalSegments.InstantiateProportion(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                }
                else if (clause is ProportionalAngles)
                {
                    HandleDeducedClauses(worklist, ProportionalAngles.InstantiateProportion(clause));
                }
                else if (clause is CongruentTriangles)
                {
                    HandleDeducedClauses(worklist, CongruentTriangles.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveCongruentTriangles.Instantiate(clause));
                }
                else if (clause is CongruentAngles)
                {
                    HandleDeducedClauses(worklist, TwoPairsCongruentAnglesImplyThirdPairCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleAdditionAxiom.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAnglesInTriangleImplyCongruentSides.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AASimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltIntCongruentAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, CongruentCorrespondingAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAdjacentAnglesImplyPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                }
                else if (clause is CongruentSegments)
                {
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, CongruentSidesInTriangleImplyCongruentSegments.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSegmentsImplyProportionalSegmentsDefinition.Instantiate(clause));
                }
                else if (clause is Triangle)
                {
                    //HandleDeducedClauses(worklist, SumAnglesInTriangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, Angle.InstantiateReflexiveAngles(clause));
                    HandleDeducedClauses(worklist, Triangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ExteriorAngleEqualSumRemoteAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAnglesInTriangleImplyCongruentSides.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSidesInTriangleImplyCongruentSegments.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSSSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AASimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                    HandleDeducedClauses(worklist, AcuteAnglesInRightTriangleComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, TwoPairsCongruentAnglesImplyThirdPairCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSegmentsImplyProportionalSegmentsDefinition.Instantiate(clause));
                    
                    if (clause is IsoscelesTriangle)
                    {
                        HandleDeducedClauses(worklist, IsoscelesTriangleTheorem.Instantiate(clause));

                        // CTA: Needs to worl with Equilateral Triangles as well
                        HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                    }

                    if (clause is EquilateralTriangle)
                    {
                        HandleDeducedClauses(worklist, EquilateralTriangleHasSixtyDegreeAngles.Instantiate(clause));
                    }
                }
                else if (clause is Strengthened)
                {
                    HandleDeducedClauses(worklist, IsoscelesTriangleTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, AcuteAnglesInRightTriangleComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                    HandleDeducedClauses(worklist, EquilateralTriangleHasSixtyDegreeAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));

                    // For strengthening an intersection to a perpendicular
                    HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));

                    // InMiddle Strengthened to Midpoint
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));

                    // Right Angle
                    HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                }
            }

            return graph;
        }

        //
        // Add all new deduced clauses to the worklist if they have not been deduced before.
        // If the given clause has been deduced before, update the hyperedges that were generated previously
        //
        // Forward Instantiation does not permit any cycles in the resultant graph. We may deduce 
        //
        private void HandleDeducedClauses(List<GroundedClause> worklist,
                                          List<KeyValuePair<List<GroundedClause>, GroundedClause>> newVals)
        {
            foreach (KeyValuePair<List<GroundedClause>, GroundedClause> newEdge in newVals)
            {
                //Debug.WriteLine(newEdge.Value.clauseId + "(" + graph.Size() + ")" + ": " + newEdge.Value);


                GroundedClause graphNode = graph.GetNode(newEdge.Value);

                //
                // If the node is not in the graph already?
                //
                if (graphNode == null)
                {
                    // This node is not in the graph so add it; this should succeed
                    if (graph.AddNode(newEdge.Value))
                    {
                        newEdge.Value.SetID(graph.Size());
                    }

                    // Also add to the worklist
                    worklist.Add(newEdge.Value);

                    AddForwardEdge(newEdge.Key, newEdge.Value, 0); // 0: Annotation to be handled later
                }

                //
                // If the node is in the graph.
                //
                else
                {
                    AddForwardEdge(newEdge.Key, graphNode, 0); // 0: Annotation to be handled later
                }
            }
        }

        private void AddForwardEdge(List<GroundedClause> antecedent, GroundedClause consequent, int annotation)
        {
            // Add the hyperedge to the hypergraph
            graph.AddForwardEdge(antecedent, consequent, annotation); // 0: Annotation to be handled later

            // Create the linkage between the antecedent and consequent for coverage
            foreach (GroundedClause ante in antecedent)
            {
                consequent.AddComponent(ante.clauseId);
                consequent.AddComponentList(ante.figureComponents);
            }
        }

        //
        // Constructs the coverage relationship amongst all figure components
        //
        private static void HandleAllFigureIntrinsicFacts(List<GroundedClause> figure)
        {
            for (int f1 = 0; f1 < figure.Count; f1++)
            {
                for (int f2 = 0; f2 < figure.Count; f2++)
                {
                    if (f1 != f2)
                    {
                        if (figure[f1].Covers(figure[f2])) figure[f1].AddComponent(figure[f2].clauseId);
                    }
                }
            }
        }

        //
        // Preprocess the given clauses
        //
        private static void HandleAllGivens(List<GroundedClause> figure, List<GroundedClause> givens)
        {
            //
            // Link the boolean facts to the intrinsic facts through coverage (implication)
            //
            foreach (GroundedClause given in givens)
            {
                foreach (GroundedClause component in figure)
                {
                    if (given.Covers(component))
                    {
                        given.AddComponent(component.clauseId);
                        given.AddComponentList(component.figureComponents);
                    }
                }
            }

            //
            // Are any of the givens congruent relationships?
            //
            foreach (GroundedClause clause in givens)
            {
                if (clause is CongruentTriangles)
                {
                    CongruentTriangles conTris = clause as CongruentTriangles;
                    if (conTris.VerifyCongruentTriangles())
                    {
                        // indicate that these triangles are congruent to prevent future 'reproving' congruent
                        conTris.ProcessGivens();
                    }
                    // This is not a valid congruence.
                    else
                    {
                        // Remove?
                    }
                }
                //else if (clause is SimilarTriangles)
                //{
                //    SimilarTriangles simTris = clause as SimilarTriangles;
                //    if (simTris.VerifyCongruentTriangles())
                //    {
                //        // indicate that these triangles are congruent to prevent future 'reproving' congruent
                //        simTris.ProcessGivens();
                //    }
                //    // This is not a valid similarity.
                //    else
                //    {
                //        // Remove?
                //    }
                //}
            }
        }

        //
        // Clear all Instantiation Theorems / Axioms / Definitions
        //
        public static void Clear()
        {
            //
            // Algebra
            //
            RelationTransitiveSubstitution.Clear();
            TransitiveSubstitution.Clear();
            Angle.Clear();
            TransitiveCongruentTriangles.Clear();

            //
            // Axioms
            //
            AASimilarity.Clear();
            AngleAdditionAxiom.Clear();
            ASA.Clear();
            CongruentCorrespondingAnglesImplyParallel.Clear();
            CorrespondingAnglesOfParallelLines.Clear();
            SASCongruence.Clear();
            SSS.Clear();
            AnglesOfEqualMeasureAreCongruent.Clear();

            //
            // Definitions
            //
            CongruentSegmentsImplyProportionalSegmentsDefinition.Clear();
            IsoscelesTriangleDefinition.Clear();
            RightTriangleDefinition.Clear();
            SegmentBisectorDefinition.Clear();
            MedianDefinition.Clear();
            AltitudeDefinition.Clear();
            PerpendicularDefinition.Clear();
            ComplementaryDefinition.Clear();
            AngleBisectorDefinition.Clear();
            RightAngleDefinition.Clear();

            //
            // Theorems
            //
            AAS.Clear();
            AltIntCongruentAnglesImplyParallel.Clear();
            AltitudeOfRightTrianglesImpliesSimilar.Clear();
            AngleBisectorIsPerpendicularBisectorInIsosceles.Clear();
            CongruentAdjacentAnglesImplyPerpendicular.Clear();
            CongruentAnglesInTriangleImplyCongruentSides.Clear();
            CongruentSidesInTriangleImplyCongruentSegments.Clear();
            ExteriorAngleEqualSumRemoteAngles.Clear();
            HypotenuseLeg.Clear();
            ParallelImplyAltIntCongruentAngles.Clear();
            ParallelImplySameSideInteriorSupplementary.Clear();
            PerpendicularImplyCongruentAdjacentAngles.Clear();
            RelationsOfCongruentAnglesAreCongruent.Clear();
            SameSideSuppleAnglesImplyParallel.Clear();
            SASSimilarity.Clear();
            SSSSimilarity.Clear();
            TransversalPerpendicularToParallelImplyBothPerpendicular.Clear();
            TriangleProportionality.Clear();
            TwoPairsCongruentAnglesImplyThirdPairCongruent.Clear();
        }
    }
}