using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TransitiveSubstitution : GenericRule
    {
        private static readonly string NAME = "Transitive Substitution";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_SUBSTITUTION);

        // Congruences imply equations: AB \cong CD -> AB = CD
        private static List<GeometricCongruentSegments> geoCongSegments = new List<GeometricCongruentSegments>();
        private static List<GeometricCongruentAngles> geoCongAngles = new List<GeometricCongruentAngles>();

        // These are transitively deduced congruences
        private static List<AlgebraicCongruentSegments> algCongSegments = new List<AlgebraicCongruentSegments>();
        private static List<AlgebraicCongruentAngles> algCongAngles = new List<AlgebraicCongruentAngles>();

        // Old segment equations
        private static List<GeometricSegmentEquation> geoSegmentEqs = new List<GeometricSegmentEquation>();
        private static List<AlgebraicSegmentEquation> algSegmentEqs = new List<AlgebraicSegmentEquation>();

        // Old angle measure equations
        private static List<AlgebraicAngleEquation> algAngleEqs = new List<AlgebraicAngleEquation>();
        private static List<GeometricAngleEquation> geoAngleEqs = new List<GeometricAngleEquation>();

        // Old Proportional Segment Expressions
        private static List<GeometricProportionalSegments> geoPropSegs = new List<GeometricProportionalSegments>();
        private static List<AlgebraicProportionalSegments> algPropSegs = new List<AlgebraicProportionalSegments>();

        // Old Proportional Angle Expressions
        private static List<GeometricProportionalAngles> geoPropAngs = new List<GeometricProportionalAngles>();
        private static List<AlgebraicProportionalAngles> algPropAngs = new List<AlgebraicProportionalAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            geoCongSegments.Clear();
            geoCongAngles.Clear();

            algCongSegments.Clear();
            algCongAngles.Clear();

            geoSegmentEqs.Clear();
            algSegmentEqs.Clear();

            algAngleEqs.Clear();
            geoAngleEqs.Clear();

            geoPropSegs.Clear();
            algPropSegs.Clear();

            geoPropAngs.Clear();
            algPropAngs.Clear();
        }

        //
        // Implements transitivity with equations
        // Equation(A, B), Equation(B, C) -> Equation(A, C)
        //
        // This includes CongruentSegments and CongruentAngles
        //
        // Generation of new equations is restricted to the following rules; let G be Geometric and A algebriac
        //     G + G -> A
        //     G + A -> A
        //     A + A -X> A  <- Not allowed
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have an equation or congruence?
            if (!(clause is Equation) && !(clause is Congruent) && !(clause is ProportionalSegments)) return newGrounded;

            // Has this clause been generated before?
            // Since generated clauses will eventually be instantiated as well, this will reach a fixed point and stop.
            // Uniqueness of clauses needs to be handled by the class calling this
            if (ClauseHasBeenDeduced(clause)) return newGrounded;

            // A reflexive expression provides no information of interest or consequence.
            if (clause.IsReflexive()) return newGrounded;

            //
            // Process the clause
            //
            if (clause is SegmentEquation)
            {
                newGrounded.AddRange(HandleNewSegmentEquation(clause as SegmentEquation));
            }
            else if (clause is AngleEquation)
            {
                newGrounded.AddRange(HandleNewAngleEquation(clause as AngleEquation));
            }
            else if (clause is CongruentAngles)
            {
                newGrounded.AddRange(HandleNewCongruentAngles(clause as CongruentAngles));
            }
            else if (clause is CongruentSegments)
            {
                newGrounded.AddRange(HandleNewCongruentSegments(clause as CongruentSegments));
            }
            else if (clause is ProportionalSegments)
            {
                // Avoid using proportional segments that should really be congruent (they are deduced from similar triangles which are, in fact, congruent)
                if (Utilities.CompareValues((clause as ProportionalSegments).dictatedProportion, 1)) return newGrounded;

                newGrounded.AddRange(HandleNewProportionalSegments(clause as ProportionalSegments));
            }

            // Add the new clause to the right list for later combining
            AddToAppropriateList(clause);

            // Add predecessors
            MarkPredecessors(newGrounded);

            return newGrounded;
        }

        //
        // Add predecessors to the equations, congruence relationships, etc.
        //
        private static void MarkPredecessors(List<EdgeAggregator> edges)
        {
            foreach (EdgeAggregator edge in edges)
            {
                foreach (GroundedClause predNode in edge.antecedent)
                {
                    edge.consequent.AddRelationPredecessor(predNode);
                    edge.consequent.AddRelationPredecessors(predNode.relationPredecessors);
                }
            }
        }

        // Sets all of the deduced nodes to be purely algebraic: A + A -> A
        private static List<EdgeAggregator> MakePurelyAlgebraic(List<EdgeAggregator> edges)
        {
            foreach (EdgeAggregator edge in edges)
            {
                edge.consequent.MakePurelyAlgebraic();
            }

            return edges;
        }

        //
        // For generation of transitive congruent segments
        //
        private static List<EdgeAggregator> CreateCongruentSegments(CongruentSegments css, CongruentSegments geoCongSeg)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

//            if (css.HasRelationPredecessor(geoCongSeg) || geoCongSeg.HasRelationPredecessor(css)) return newGrounded;

            int numSharedExps = css.SharesNumClauses(geoCongSeg);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return Congruent.CreateTransitiveCongruence(css, geoCongSeg);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:

                    throw new Exception("Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }

        //
        // For generation of transitive proportional segments
        //
        private static List<EdgeAggregator> CreateProportionalSegments(ProportionalSegments pss, CongruentSegments conSeg)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

//            if (pss.HasRelationPredecessor(conSeg) || conSeg.HasRelationPredecessor(pss)) return newGrounded;

            int numSharedExps = pss.SharesNumClauses(conSeg);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return ProportionalSegments.CreateTransitiveProportion(pss, conSeg);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:
                    throw new Exception("Proportional / Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }

        //
        // For generation of transitive proportional angles
        //
        private static List<EdgeAggregator> CreateProportionalAngles(ProportionalAngles pas, CongruentAngles conAng)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

//            if (pas.HasRelationPredecessor(conAng) || conAng.HasRelationPredecessor(pas)) return newGrounded;

            int numSharedExps = pas.SharesNumClauses(conAng);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return ProportionalAngles.CreateTransitiveProportion(pas, conAng);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:
                    throw new Exception("Proportional / Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }

        //
        // Substitute this new segment congruence into old segment equations
        //
        private static List<EdgeAggregator> CreateSegmentEquation(SegmentEquation segEq, CongruentSegments congSeg)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

//            if (segEq.HasRelationPredecessor(congSeg) || congSeg.HasRelationPredecessor(segEq)) return newGrounded;

            newEquationEdge = PerformEquationSubstitution(segEq, congSeg, congSeg.cs1, congSeg.cs2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(segEq, congSeg, congSeg.cs2, congSeg.cs1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }

        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewCongruentSegments(CongruentSegments congSegs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentSegments gcss in geoCongSegments)
            {
                newGrounded.AddRange(CreateCongruentSegments(gcss, congSegs));
            }

            // New equations? G + G -> A
            foreach (GeometricSegmentEquation gseqs in geoSegmentEqs)
            {
                newGrounded.AddRange(CreateSegmentEquation(gseqs, congSegs));
            }

            // New proportions? G + G -> A
            foreach (GeometricProportionalSegments gps in geoPropSegs)
            {
                newGrounded.AddRange(CreateProportionalSegments(gps, congSegs));
            }

            if (congSegs is GeometricCongruentSegments)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentSegments acss in algCongSegments)
                {
                    newGrounded.AddRange(CreateCongruentSegments(acss, congSegs));
                }

                // New equations? G + A -> A
                foreach (AlgebraicSegmentEquation aseqs in algSegmentEqs)
                {
                    newGrounded.AddRange(CreateSegmentEquation(aseqs, congSegs));
                }

                // New proportions? G + A -> A
                foreach (AlgebraicProportionalSegments aps in algPropSegs)
                {
                    newGrounded.AddRange(CreateProportionalSegments(aps, congSegs));
                }
            }

            //
            // NEW
            //
            else if (congSegs is AlgebraicCongruentSegments)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentSegments oldACSS in algCongSegments)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateCongruentSegments(oldACSS, congSegs)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicSegmentEquation aseqs in algSegmentEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentEquation(aseqs, congSegs)));
                }

                // New proportions? A + A -> A
                foreach (AlgebraicProportionalSegments aps in algPropSegs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalSegments(aps, congSegs)));
                }
            }

            return newGrounded;
        }

        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewProportionalSegments(ProportionalSegments propSegs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentSegments gcs in geoCongSegments)
            {
                newGrounded.AddRange(CreateProportionalSegments(propSegs, gcs));
            }

            if (propSegs is GeometricProportionalSegments)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentSegments acs in algCongSegments)
                {
                    newGrounded.AddRange(CreateProportionalSegments(propSegs, acs));
                }
            }

            else if (propSegs is AlgebraicProportionalSegments)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentSegments acs in algCongSegments)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalSegments(propSegs, acs)));
                }
            }

            return newGrounded;
        }

        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewProportionalAngles(ProportionalAngles propAngs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcas in geoCongAngles)
            {
                newGrounded.AddRange(CreateProportionalAngles(propAngs, gcas));
            }

            if (propAngs is GeometricProportionalAngles)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(CreateProportionalAngles(propAngs, acas));
                }
            }

            //
            // NEW
            //
            if (propAngs is AlgebraicProportionalAngles)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalAngles(propAngs, acas)));
                }
            }


            return newGrounded;
        }

        //
        // For generation of transitive congruent Angles
        //
        private static List<EdgeAggregator> CreateCongruentAngles(CongruentAngles css, CongruentAngles congAng)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

//            if (css.HasRelationPredecessor(congAng) || congAng.HasRelationPredecessor(css)) return newGrounded;

            int numSharedExps = css.SharesNumClauses(congAng);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return Congruent.CreateTransitiveCongruence(css, congAng);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:

                    throw new Exception("Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }

        //
        // Substitute this new angle congruence into old angle equations
        //
        private static List<EdgeAggregator> CreateAngleEquation(AngleEquation angEq, CongruentAngles congAng)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

//            if (angEq.HasRelationPredecessor(congAng) || congAng.HasRelationPredecessor(angEq)) return newGrounded;

            newEquationEdge = PerformEquationSubstitution(angEq, congAng, congAng.ca1, congAng.ca2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(angEq, congAng, congAng.ca2, congAng.ca1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }

        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Angles
        //
        private static List<EdgeAggregator> HandleNewCongruentAngles(CongruentAngles congAngs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcss in geoCongAngles)
            {
                newGrounded.AddRange(CreateCongruentAngles(gcss, congAngs));
            }

            // New equations? G + G -> A
            foreach (GeometricAngleEquation gseqs in geoAngleEqs)
            {
                newGrounded.AddRange(CreateAngleEquation(gseqs, congAngs));
            }

            // New proportions? G + G -> A
            foreach (GeometricProportionalAngles gpas in geoPropAngs)
            {
                newGrounded.AddRange(CreateProportionalAngles(gpas, congAngs));
            }

            if (congAngs is GeometricCongruentAngles)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acss in algCongAngles)
                {
                    newGrounded.AddRange(CreateCongruentAngles(acss, congAngs));
                }

                // New equations? G + A -> A
                foreach (AlgebraicAngleEquation aseqs in algAngleEqs)
                {
                    newGrounded.AddRange(CreateAngleEquation(aseqs, congAngs));
                }

                // New proportions? G + A -> A
                foreach (AlgebraicProportionalAngles apas in algPropAngs)
                {
                    newGrounded.AddRange(CreateProportionalAngles(apas, congAngs));
                }
            }

            //
            // NEW
            //
            else if (congAngs is AlgebraicCongruentAngles)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles oldACSS in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateCongruentAngles(oldACSS, congAngs)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicAngleEquation aseqs in algAngleEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleEquation(aseqs, congAngs)));
                }

                // New proportions? G + A -> A
                foreach (AlgebraicProportionalAngles apas in algPropAngs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalAngles(apas, congAngs)));
                }
            }

            return newGrounded;
        }

        //
        // Check equivalence of lists by verifying dual containment.
        //
        private static bool EqualLists(List<GroundedClause> list1, List<GroundedClause> list2)
        {
            foreach (GroundedClause val1 in list1)
            {
                if (!list2.Contains(val1)) return false;
            }

            foreach (GroundedClause val2 in list2)
            {
                if (!list1.Contains(val2)) return false;
            }

            return true;
        }

        // For cosntruction of the new equations
        private static readonly int SEGMENT_EQUATION = 0;
        private static readonly int ANGLE_EQUATION = 1;
        private static readonly int EQUATION_ERROR = 1;
        private static int GetEquationType(Equation eq)
        {
            if (eq is SegmentEquation) return SEGMENT_EQUATION;
            if (eq is AngleEquation) return ANGLE_EQUATION;
            return EQUATION_ERROR;
        }

        //
        // Given two news sides for an equation, create the equation. If possible, create a congruence or a supplementary / complementary relationship
        //
        private static GroundedClause ConstructNewEquation(int equationType, GroundedClause left, GroundedClause right)
        {
            //
            // Construct the new equation with a given left / right side
            //
            Equation newEq = null;
            if (equationType == SEGMENT_EQUATION)
            {
                newEq = new AlgebraicSegmentEquation(left, right);
            }
            else if (equationType == ANGLE_EQUATION)
            {
                newEq = new AlgebraicAngleEquation(left, right);
            }

            //
            // Simplify the equation
            //
            Equation simplified = Simplification.Simplify(newEq);

            //
            // Create a congruence relationship if it applies
            //
            GroundedClause newCongruence = HandleCongruence(simplified);
            if (newCongruence != null) return newCongruence;

            //
            // If a congruence was not established, create a complementary or supplementary relationship, if applicable
            //
            if (equationType == ANGLE_EQUATION)
            {
                GroundedClause newRelation = HandleAngleRelation(simplified);
                if (newRelation != null) return newRelation;
            }

            // Just return the simplified equation if nothing else could be deduced
            return simplified;
        }

        //
        // Given two equations, perform a direct, transitive substitution of one equation into the other (and vice versa)
        //
        private static List<EdgeAggregator> PerformEquationTransitiviteSubstitution(Equation eq1, Equation eq2)
        {
            List<GroundedClause> newRelations = new List<GroundedClause>();

            //
            // Collect the terms from each side of both equations
            //
            List<GroundedClause> lhsTermsEq1 = eq1.lhs.CollectTerms();
            List<GroundedClause> lhsTermsEq2 = eq2.lhs.CollectTerms();
            List<GroundedClause> rhsTermsEq1 = eq1.rhs.CollectTerms();
            List<GroundedClause> rhsTermsEq2 = eq2.rhs.CollectTerms();

            int equationType = GetEquationType(eq1);

            //
            // Construct the new equations using all possible combinations
            //
            if (EqualLists(lhsTermsEq1, lhsTermsEq2))
            {
                newRelations.Add(ConstructNewEquation(equationType, eq1.rhs, eq2.rhs));
            }
            if (EqualLists(lhsTermsEq1, rhsTermsEq2))
            {
                newRelations.Add(ConstructNewEquation(equationType, eq1.rhs, eq2.lhs));
            }
            if (EqualLists(rhsTermsEq1, lhsTermsEq2))
            {
                newRelations.Add(ConstructNewEquation(equationType, eq1.lhs, eq2.rhs));
            }
            if (EqualLists(rhsTermsEq1, rhsTermsEq2))
            {
                newRelations.Add(ConstructNewEquation(equationType, eq1.lhs, eq2.lhs));
            }

            //
            // Construct the hypergraph edges for all of the new relationships
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq1);
            antecedent.Add(eq2);

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            foreach (GroundedClause gc in newRelations)
            {
                newGrounded.Add(new EdgeAggregator(antecedent, gc, annotation));
            }

            return newGrounded;
        }

        //
        // Given two equations, if one equation is atomic, substitute into the other (and vice versa)
        //
        //private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> PerformEquationDirectSubstitution(Equation eq1, Equation eq2)
        //{
        //    //
        //    // Does the new equation have one side which is isolated (an atomic expression)?
        //    //
        //    int atomicSide = newEq.GetAtomicity();

        //    GroundedClause atomicExp = null;
        //    GroundedClause otherSide = null;
        //    switch (atomicSide)
        //    {
        //        case Equation.LEFT_ATOMIC:
        //            atomicExp = newEq.lhs;
        //            otherSide = newEq.rhs;
        //            break;

        //        case Equation.RIGHT_ATOMIC:
        //            atomicExp = newEq.rhs;
        //            otherSide = newEq.lhs;
        //            break;

        //        case Equation.BOTH_ATOMIC:
        //            // Choose both sides
        //            break;

        //        case Equation.NONE_ATOMIC:
        //            // If neither side of this new equation are atomic, for simplicty,
        //            // we do not perform a substitution
        //            return new List<EdgeAggregator>();
        //    }

        //    KeyValuePair<List<GroundedClause>, GroundedClause> cl;
        //    // One side of the equation is atomic
        //    if (atomicExp != null)
        //    {
        //        // Check to see if the old equation is dually atomic as
        //        // we will want to substitute the old eq into the new one
        //        int oldAtomic = oldEq.GetAtomicity();
        //        if (oldAtomic != Equation.BOTH_ATOMIC)
        //        {
        //            // Simple sub of new equation into old
        //            cl = PerformEquationSubstitution(oldEq, newEq, atomicExp, otherSide);
        //            if (cl.Value != null) newGrounded.Add(cl);

        //            //
        //            // In the case where we have a situation of the form: A + B = C
        //            //                                                    A + B = D  -> C = D
        //            //
        //            // Perform a non-atomic substitution
        //            cl = PerformNonAtomicEquationSubstitution(oldEq, newEq, otherSide, atomicExp);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //        }
        //        else if (oldAtomic == Equation.BOTH_ATOMIC)
        //        {
        //            // Dual sub of old equation into new
        //            cl = PerformEquationSubstitution(newEq, oldEq, oldEq.lhs, oldEq.rhs);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //            cl = PerformEquationSubstitution(newEq, oldEq, oldEq.rhs, oldEq.lhs);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //        }
        //    }
        //    // The new equation has both sides atomic; try to sub in the other side
        //    else
        //    {
        //        cl = PerformEquationSubstitution(oldEq, newEq, newEq.lhs, newEq.rhs);
        //        if (cl.Value != null) newGrounded.Add(cl);
        //        cl = PerformEquationSubstitution(oldEq, newEq, newEq.rhs, newEq.lhs);
        //        if (cl.Value != null) newGrounded.Add(cl);
        //    }

        //    return newGrounded;
        //}

        //
        // Given an old and new set of angle measure equations substitute if possible.
        //
        private static List<EdgeAggregator> CreateNewEquation(Equation oldEq, Equation newEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

//Debug.WriteLine("Considering combining: " + oldEq + " + " + newEq);

            // Avoid redundant equation generation
//            if (oldEq.HasRelationPredecessor(newEq) || newEq.HasRelationPredecessor(oldEq)) return newGrounded;

            // Determine if there is a direct, transitive relationship with the equations
            newGrounded.AddRange(PerformEquationTransitiviteSubstitution(oldEq, newEq));

            // If we have an atomic situation, substitute into the other equation
            //newGrounded.AddRange(PerformEquationDirectSubstitution(oldEq, newEq));

            return newGrounded;
        }

        //
        // Generate all new relationships from an Equation Containing Angle measurements
        //
        private static List<EdgeAggregator> HandleNewAngleEquation(AngleEquation newAngEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcas in geoCongAngles)
            {
                newGrounded.AddRange(CreateAngleEquation(newAngEq, gcas));
            }

            // New equations? G + G -> A
            foreach (GeometricAngleEquation gangs in geoAngleEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gangs, newAngEq));
            }

            if (newAngEq is GeometricAngleEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(CreateAngleEquation(newAngEq, acas));
                }

                // New equations? G + A -> A
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aangs, newAngEq));
                }
            }

            //
            // NEW
            //
            else if (newAngEq is AlgebraicAngleEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles oldACAS in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleEquation(newAngEq, oldACAS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aangs, newAngEq)));
                }
            }

            return newGrounded;
        }

        //
        // Generate all new relationships from an Equation Containing Segments
        //
        private static List<EdgeAggregator> HandleNewSegmentEquation(SegmentEquation newSegEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentSegments gcss in geoCongSegments)
            {
                newGrounded.AddRange(CreateSegmentEquation(newSegEq, gcss));
            }

            // New equations? G + G -> A
            foreach (GeometricSegmentEquation gSegs in geoSegmentEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gSegs, newSegEq));
            }

            if (newSegEq is GeometricSegmentEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentSegments acss in algCongSegments)
                {
                    newGrounded.AddRange(CreateSegmentEquation(newSegEq, acss));
                }

                // New equations? G + A -> A
                foreach (AlgebraicSegmentEquation aSegs in algSegmentEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aSegs, newSegEq));
                }
            }

            //
            // NEW
            //
            else if (newSegEq is AlgebraicSegmentEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentSegments oldACSS in algCongSegments)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentEquation(newSegEq, oldACSS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicSegmentEquation aSegs in algSegmentEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aSegs, newSegEq)));
                }
            }


            //
            // Combining TWO algebraic equations only if the result is a congruence: A + A -> Congruent
            //
            //else if (newSegEq is AlgebraicSegmentEquation)
            //{
            //    foreach (AlgebraicSegmentEquation asegs in algSegmentEqs)
            //    {
            //        List<KeyValuePair<List<GroundedClause>, GroundedClause>> newEquationList = CreateNewEquation(newSegEq, asegs);
            //        if (newEquationList.Any())
            //        {
            //            KeyValuePair<List<GroundedClause>, GroundedClause> newEq = newEquationList[0];

            //            if (newEq.Value is AlgebraicCongruentSegments)
            //            {
            //                newGrounded.AddRange(newEquationList);
            //            }
            //        }
            //    }
            //}
            return newGrounded;
        }

        //
        // Substitute some clause (subbedEq) into an equation (eq)
        //
        private static EdgeAggregator PerformEquationSubstitution(Equation eq, GroundedClause subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            //Debug.WriteLine("Substituting with " + eq.ToString() + " and " + subbedEq.ToString());

            if (!eq.Contains(toFind)) return null;

            //
            // Make a deep copy of the equation
            //
            Equation newEq = null;
            if (eq is SegmentEquation)
            {
                newEq = new AlgebraicSegmentEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }
            else if (eq is AngleEquation)
            {
                newEq = new AlgebraicAngleEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }

            // Substitute into the copy
            newEq.Substitute(toFind, toSub);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq);
            antecedent.Add(subbedEq);

            //
            // Simplify the equation
            //
            Equation simplified = Simplification.Simplify(newEq);

            // Create a congruence relationship if it applies
            GroundedClause newCongruence = HandleCongruence(simplified);
            if (newCongruence != null) return new EdgeAggregator(antecedent, newCongruence, annotation);

            // If a congruence was not established, create a complementary or supplementary relationship, if applicable
            if (simplified is AngleEquation && newCongruence == null)
            {
                GroundedClause newRelation = HandleAngleRelation(simplified);
                if (newRelation != null) return new EdgeAggregator(antecedent, newRelation, annotation);
            }

            return new EdgeAggregator(antecedent, simplified, annotation);
        }

        //
        // If both sides of the substituted equation are atomic and not Numeric Values, create a congruence relationship instead.
        //
        private static GroundedClause HandleCongruence(Equation simplified)
        {
            // Both sides must be atomic and multiplied by a factor of 1 for a proper congruence
            if (simplified.GetAtomicity() != Equation.BOTH_ATOMIC) return null;

            if (!simplified.IsProperCongruence()) return null;

            // Then create a congruence, whether it be angle or segment
            Congruent newCongruent = null;
            if (simplified is AlgebraicAngleEquation)
            {
                // Do not generate for lines; that is, 180^o angles
                //if (((Angle)simplified.lhs).IsStraightAngle() && ((Angle)simplified.lhs).IsStraightAngle()) return null;
                newCongruent = new AlgebraicCongruentAngles((Angle)simplified.lhs, (Angle)simplified.rhs);
            }
            else if (simplified is AlgebraicSegmentEquation)
            {
                newCongruent = new AlgebraicCongruentSegments((Segment)simplified.lhs, (Segment)simplified.rhs);
            }

            // There is no need to simplify a congruence, so just return
            return newCongruent;
        }

        //
        // If both sides of the substituted equation are atomic and not Numeric Values, create a congruence relationship instead.
        //
        private static GroundedClause HandleAngleRelation(Equation simplified)
        {
            // One side must be atomic
            int atomicity = simplified.GetAtomicity();
            if (atomicity == Equation.NONE_ATOMIC) return null;

            GroundedClause atomic = null;
            GroundedClause nonAtomic = null;
            if (atomicity == Equation.LEFT_ATOMIC)
            {
                atomic = simplified.lhs;
                nonAtomic = simplified.rhs;
            }
            else if (atomicity == Equation.RIGHT_ATOMIC)
            {
                atomic = simplified.rhs;
                nonAtomic = simplified.lhs;
            }
            else if (atomicity == Equation.BOTH_ATOMIC)
            {
                return HandleCollinearPerpendicular(simplified.lhs, simplified.rhs);
            }

            NumericValue atomicValue = atomic as NumericValue;
            if (atomicValue == null) return null;

            //
            // We need only consider special angles (90 or 180)
            //
            if (!Utilities.CompareValues(atomicValue.IntValue, 90) && !Utilities.CompareValues(atomicValue.IntValue, 180)) return null;

            List<GroundedClause> nonAtomicSide = nonAtomic.CollectTerms();

            // Check multiplier for all terms; it must be 1.
            foreach (GroundedClause gc in nonAtomicSide)
            {
                if (gc.multiplier != 1) return null;
            }

            //
            // Complementary or Supplementary
            //
            AnglePairRelation newRelation = null;
            if (nonAtomicSide.Count == 2)
            {
                if (Utilities.CompareValues(atomicValue.IntValue, 90))
                {
                    newRelation = new Complementary((Angle)nonAtomicSide[0], (Angle)nonAtomicSide[1]);
                }
                else if (Utilities.CompareValues(atomicValue.IntValue, 180))
                {
                    newRelation = new Supplementary((Angle)nonAtomicSide[0], (Angle)nonAtomicSide[1]);
                }
            }

            return newRelation;
        }

        //
        // Create a deduced collinear or perpendicular relationship
        //
        private static GroundedClause HandleCollinearPerpendicular(GroundedClause left, GroundedClause right)
        {
            NumericValue numeral = null;
            Angle angle = null;

            //
            // Split the sides
            //
            if (left is NumericValue)
            {
                numeral = left as NumericValue;
                angle = right as Angle;
            }
            else if (right is NumericValue)
            {
                numeral = right as NumericValue;
                angle = left as Angle;
            }

            if (numeral == null || angle == null) return null;

            //
            // Create the new relationships
            //
            Descriptor newDescriptor = null;
            //if (Utilities.CompareValues(numeral.value, 90))
            //{
            //    newDescriptor = new Perpendicular(angle.GetVertex(), angle.ray1, angle.ray2);
            //}
            //else
            if (Utilities.CompareValues(numeral.IntValue, 180))
            {
                List<Point> pts = new List<Point>();
                pts.Add(angle.A);
                pts.Add(angle.B);
                pts.Add(angle.C);
                newDescriptor = new Collinear(pts);
            }

            return newDescriptor;
        }

        //
        // This is pure transitivity where A + B = C , A + B = D -> C = D
        //
        private static EdgeAggregator PerformNonAtomicEquationSubstitution(Equation eq, GroundedClause subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            //Debug.WriteLine("Substituting with " + eq.ToString() + " and " + subbedEq.ToString());

            // If there is a deduction relationship between the given congruences, do not perform another substitution
            //  subbedEq.HasPredecessor(eq)
            //if (eq.HasGeneralPredecessor(subbedEq) || subbedEq.HasGeneralPredecessor(eq)) return new EdgeAggregator(null, null);

            //
            // Verify that the non-atomic sides to both equations are the exact same
            //
            GroundedClause nonAtomicOriginal = null;
            GroundedClause atomicOriginal = null;
            if (eq.GetAtomicity() == Equation.LEFT_ATOMIC)
            {
                nonAtomicOriginal = eq.rhs;
                atomicOriginal = eq.lhs;
            }
            else if (eq.GetAtomicity() == Equation.RIGHT_ATOMIC)
            {
                nonAtomicOriginal = eq.lhs;
                atomicOriginal = eq.rhs;
            }

            // We collect all the flattened terms
            List<GroundedClause> originalTerms = nonAtomicOriginal.CollectTerms();
            List<GroundedClause> subbedTerms = toFind.CollectTerms();

            // Now, the lists must be the same; we check for containment in both directions
            foreach (GroundedClause originalTerm in originalTerms)
            {
                if (!subbedTerms.Contains(originalTerm)) return null;
            }

            foreach (GroundedClause subbedTerm in subbedTerms)
            {
                if (!originalTerms.Contains(subbedTerm)) return null;
            }

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq);
            antecedent.Add(subbedEq);
            EdgeAggregator newEdge;

            //
            // Generate a simple equation or an algebraic congruent statement
            //
            if (atomicOriginal is NumericValue || toSub is NumericValue)
            {
                Equation newEquation = null;
                if (eq is AngleEquation)
                {
                    newEquation = new AlgebraicAngleEquation(atomicOriginal.DeepCopy(), toSub.DeepCopy());
                }
                else if (eq is SegmentEquation)
                {
                    newEquation = new AlgebraicSegmentEquation(atomicOriginal.DeepCopy(), toSub.DeepCopy());
                }

                if (newEquation == null)
                {
                    Debug.WriteLine("");
                    throw new NullReferenceException("Unexpected Problem in Non-atomic substitution (equation)...");
                }

                newEdge = new EdgeAggregator(antecedent, newEquation, annotation);
            }
            else
            {
                Congruent newCongruent = null;
                if (eq is AngleEquation)
                {
                    newCongruent = new AlgebraicCongruentAngles((Angle)atomicOriginal.DeepCopy(), (Angle)toSub.DeepCopy());
                }
                else if (eq is SegmentEquation)
                {
                    newCongruent = new AlgebraicCongruentSegments((Segment)atomicOriginal.DeepCopy(), (Segment)toSub.DeepCopy());
                }

                if (newCongruent == null)
                {
                    Debug.WriteLine("");
                    throw new NullReferenceException("Unexpected Problem in Non-atomic substitution (Congruence)...");
                }

                newEdge = new EdgeAggregator(antecedent, newCongruent, annotation);
            }

            return newEdge;
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static void AddToAppropriateList(GroundedClause c)
        {
            if (c is GeometricCongruentSegments)
            {
                geoCongSegments.Add(c as GeometricCongruentSegments);
            }
            else if (c is GeometricSegmentEquation)
            {
                geoSegmentEqs.Add(c as GeometricSegmentEquation);
            }
            else if (c is GeometricCongruentAngles)
            {
                geoCongAngles.Add(c as GeometricCongruentAngles);
            }
            else if (c is GeometricAngleEquation)
            {
                geoAngleEqs.Add(c as GeometricAngleEquation);
            }
            else if (c is AlgebraicSegmentEquation)
            {
                algSegmentEqs.Add(c as AlgebraicSegmentEquation);
            }
            else if (c is AlgebraicAngleEquation)
            {
                algAngleEqs.Add(c as AlgebraicAngleEquation);
            }
            else if (c is AlgebraicCongruentSegments)
            {
                algCongSegments.Add(c as AlgebraicCongruentSegments);
            }
            else if (c is AlgebraicCongruentAngles)
            {
                algCongAngles.Add(c as AlgebraicCongruentAngles);
            }
            else if (c is GeometricProportionalSegments)
            {
                geoPropSegs.Add(c as GeometricProportionalSegments);
            }
            else if (c is AlgebraicProportionalSegments)
            {
                algPropSegs.Add(c as AlgebraicProportionalSegments);
            }
            else if (c is GeometricProportionalAngles)
            {
                geoPropAngs.Add(c as GeometricProportionalAngles);
            }
            else if (c is AlgebraicProportionalAngles)
            {
                algPropAngs.Add(c as AlgebraicProportionalAngles);
            }
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static bool ClauseHasBeenDeduced(GroundedClause c)
        {
            if (c is GeometricCongruentSegments)
            {
                return geoCongSegments.Contains(c as GeometricCongruentSegments);
            }
            else if (c is GeometricSegmentEquation)
            {
                return geoSegmentEqs.Contains(c as GeometricSegmentEquation);
            }
            else if (c is GeometricCongruentAngles)
            {
                return geoCongAngles.Contains(c as GeometricCongruentAngles);
            }
            else if (c is GeometricAngleEquation)
            {
                return geoAngleEqs.Contains(c as GeometricAngleEquation);
            }
            else if (c is AlgebraicSegmentEquation)
            {
                return algSegmentEqs.Contains(c as AlgebraicSegmentEquation);
            }
            else if (c is AlgebraicAngleEquation)
            {
                return algAngleEqs.Contains(c as AlgebraicAngleEquation);
            }
            else if (c is AlgebraicCongruentSegments)
            {
                return algCongSegments.Contains(c as AlgebraicCongruentSegments);
            }
            else if (c is AlgebraicCongruentAngles)
            {
                return algCongAngles.Contains(c as AlgebraicCongruentAngles);
            }
            else if (c is GeometricProportionalSegments)
            {
                return geoPropSegs.Contains(c as GeometricProportionalSegments);
            }
            else if (c is AlgebraicProportionalSegments)
            {
                return algPropSegs.Contains(c as AlgebraicProportionalSegments);
            }
            else if (c is GeometricProportionalAngles)
            {
                return geoPropAngs.Contains(c as GeometricProportionalAngles);
            }
            else if (c is AlgebraicProportionalAngles)
            {
                return algPropAngs.Contains(c as AlgebraicProportionalAngles);
            }

            return false;
        }
    }
}