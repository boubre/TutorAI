using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class Substitution : GenericRule
    {
        private static readonly string NAME = "Substitution";
        private static List<SegmentEquation> segmentCandEqs = new List<SegmentEquation>();
        private static List<AngleMeasureEquation> angleCandEqs = new List<AngleMeasureEquation>();

        //
        // Implements transitivity with equations
        // Equation(A, B), Equation(B, C) -> Equation(A, C)
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            // Do we have an equation?
            if (!(c is Equation)) return new List<GroundedClause>();

            // Has this clause been generated before?
            // Since generated clauses will eventually be instantiated as well, this will reach a fixed point and stop.
            // Uniqueness of clauses needs to be handled by the class calling this
            if (segmentCandEqs.Contains(c as SegmentEquation) || segmentCandEqs.Contains(c as SegmentEquation)) return new List<GroundedClause>();

            //
            // Do we have enough information for unification?
            //
            if (c is SegmentEquation && !segmentCandEqs.Any())
            {
                segmentCandEqs.Add((SegmentEquation)c);
                return new List<GroundedClause>();
            }
            else if (c is AngleMeasureEquation && !angleCandEqs.Any())
            {
                angleCandEqs.Add((AngleMeasureEquation)c);
                return new List<GroundedClause>();
            }

            //
            // Does this equation have one side which is isolated (an atomic expression)?
            //
            int atomicSide = ((Equation)c).OneSideAtomic();
            Equation eq = (Equation)c;
            GroundedClause atomicExp = null;
            GroundedClause otherSide = null;
            switch(atomicSide)
            {
                case Equation.LEFT_ATOMIC:
                    atomicExp = eq.lhs;
                    otherSide = eq.rhs;
                    break;

                case Equation.RIGHT_ATOMIC:
                    atomicExp = eq.rhs;
                    otherSide = eq.lhs;
                    break;

                case Equation.BOTH_ATOMIC:
                    // Choose both sides
                    break;

                case Equation.NONE_ATOMIC:
                    // If neither side of this new equation are atomic, for simplicty,
                    // we do not perform a substitution
                    return new List<GroundedClause>();
            }

            //
            // For each old equation, does the atomic expression appear in the equation
            //
            List<GroundedClause> newGrounded = new List<GroundedClause>();
            if (c is SegmentEquation)
            {
                foreach (SegmentEquation se in segmentCandEqs)
                {
                    GroundedClause cl = null;
                    if (atomicExp != null)
                    {
                        // Check to see if the other stored equation is dually atomic as
                        // we will want to substitute the old eq into the new one
                        int seAtomic = se.OneSideAtomic();
                        if (seAtomic != Equation.BOTH_ATOMIC)
                        {
                            // Simple sub of new equation into old
                            cl = PerformSegmentSubstitution(se, (SegmentEquation)eq, atomicExp, otherSide);
                            if (cl != null) newGrounded.Add(cl);
                        }
                        else if (seAtomic == Equation.BOTH_ATOMIC)
                        {
                            // Dual sub of old equation into new
                            cl = PerformSegmentSubstitution((SegmentEquation)eq, se, se.lhs, se.rhs);
                            if (cl != null) newGrounded.Add(cl);
                            cl = PerformSegmentSubstitution((SegmentEquation)eq, se, se.rhs, se.lhs);
                            if (cl != null) newGrounded.Add(cl);
                        }
                    }
                    // We have both sides atomic; try to sub in the other side
                    else
                    {
                        cl = PerformSegmentSubstitution(se, (SegmentEquation)eq, eq.lhs, eq.rhs);
                        if (cl != null) newGrounded.Add(cl);
                        cl = PerformSegmentSubstitution(se, (SegmentEquation)eq, eq.rhs, eq.lhs);
                        if (cl != null) newGrounded.Add(cl);
                    }
                }

                segmentCandEqs.Add((SegmentEquation)eq);
            }
            else if (c is AngleMeasureEquation)
            {
                foreach (AngleMeasureEquation ae in angleCandEqs)
                {
                    GroundedClause cl = null;
                    if (atomicExp != null)
                    {
                        // Check to see if the other stored equation is dually atomic as
                        // we will want to substitute the old eq into the new one
                        int seAtomic = ae.OneSideAtomic();
                        if (seAtomic != Equation.BOTH_ATOMIC)
                        {
                            // Simple sub of new equation into old
                            cl = PerformAngleSubstitution(ae, (AngleMeasureEquation)eq, atomicExp, otherSide);
                            if (cl != null) newGrounded.Add(cl);
                        }
                        else if (seAtomic == Equation.BOTH_ATOMIC)
                        {
                            // Dual sub of old equation into new
                            cl = PerformAngleSubstitution((AngleMeasureEquation)eq, ae, ae.lhs, ae.rhs);
                            if (cl != null) newGrounded.Add(cl);
                            cl = PerformAngleSubstitution((AngleMeasureEquation)eq, ae, ae.rhs, ae.lhs);
                            if (cl != null) newGrounded.Add(cl);
                        }
                    }
                    // We have both sides atomic; try to sub in the other side
                    else
                    {
                        cl = PerformAngleSubstitution(ae, (AngleMeasureEquation)eq, eq.lhs, eq.rhs);
                        if (cl != null) newGrounded.Add(cl);
                        cl = PerformAngleSubstitution(ae, (AngleMeasureEquation)eq, eq.rhs, eq.lhs);
                        if (cl != null) newGrounded.Add(cl);
                    }
                }

                angleCandEqs.Add((AngleMeasureEquation)eq);
            }

            return newGrounded;
        }

        private static GroundedClause PerformSegmentSubstitution(SegmentEquation eq, SegmentEquation subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            if (eq.Contains(toFind))
            {
                // Make a copy
                SegmentEquation newSE = new SegmentEquation(eq.lhs, eq.rhs, NAME);

                // Substitute into the copy
                newSE.Substitute(toFind, toSub);
                newSE.AddSubstitutionLevel();

                List<GroundedClause> preds = new List<GroundedClause>();
                preds.Add(eq);
                preds.Add(subbedEq);
                newSE.AddPredecessor(preds);
                eq.AddSuccessor(newSE);
                subbedEq.AddSuccessor(newSE);

                return newSE;
            }
            return null;
        }

        private static GroundedClause PerformAngleSubstitution(AngleMeasureEquation eq, AngleMeasureEquation subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            if (eq.Contains(toFind))
            {
                // Make a copy
                AngleMeasureEquation newAE = new AngleMeasureEquation(eq.lhs, eq.rhs, NAME);

                // Substitute into the copy
                newAE.Substitute(toFind, toSub);
                newAE.AddSubstitutionLevel();

                List<GroundedClause> preds = new List<GroundedClause>();
                preds.Add(eq);
                preds.Add(subbedEq);
                newAE.AddPredecessor(preds);
                eq.AddSuccessor(newAE);
                subbedEq.AddSuccessor(newAE);

                return newAE;
            }
            return null;
        }
    }
}