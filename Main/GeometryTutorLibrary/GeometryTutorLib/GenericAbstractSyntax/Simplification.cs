using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using System.Diagnostics;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class Simplification : GenericRule
    {
        private static readonly string NAME = "Simplification";

        //
        // Given an equation, simplify algebraically using the following notions:
        //     A + A = B  -> 2A = B
        //     A + B = B + C -> A = C
        //     A + B = 2B + C -> A = B + C
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            Equation eq = c as Equation;

            // Do we have an equation?
            if (eq == null) return newGrounded;

            // Is the equation 0 = 0?
            if (eq.lhs.Equals(new NumericValue(0)) && eq.rhs.Equals(new NumericValue(0))) return newGrounded;

            //
            // Ideally, flattening would:
            // Remove all subtractions -> adding a negative instead
            // Distribute subtraction or multiplication over addition
            //
            // Flatten the equation so that each side is a sum of atomic expressions
            Equation copyEq = (Equation)eq.DeepCopy();
            FlatEquation flattened = new FlatEquation(copyEq.lhs.CollectTerms(), copyEq.rhs.CollectTerms());

            Debug.WriteLine("Equation prior to simplification: " + flattened.ToString());

            // Combine terms only on each side (do not cross =)
            FlatEquation combined = CombineLikeTerms(flattened);

            Debug.WriteLine("Equation after like terms combined on both sides: " + combined);

            // Combine terms across the equal sign
            FlatEquation across = CombineLikeTermsAcrossEqual(combined);

            Debug.WriteLine("Equation after simplifying both sides: " + across);

            //
            // Inflate the equation
            //
            Equation inflated = null;
            GroundedClause singleLeftExp = InflateEntireSide(across.lhsExps);
            GroundedClause singleRightExp = InflateEntireSide(across.rhsExps);
            if (eq is SegmentEquation)
            {
                inflated = new SegmentEquation(singleLeftExp, singleRightExp, NAME);
            }
            else if (eq is AngleMeasureEquation)
            {
                inflated = new SegmentEquation(InflateEntireSide(across.lhsExps), InflateEntireSide(across.rhsExps), NAME);
            }

            // Did we actually perform any simplification? If not, do not generate a new equation.
            if (!inflated.Equals(eq))
            {
                // Simplified equations should inherit the algebraic predecessors of the the original equation as well as the original node
                Utilities.AddUniqueList<int>(inflated.directAlgebraicPredecessors, eq.directAlgebraicPredecessors);
                Utilities.AddUnique<int>(inflated.directAlgebraicPredecessors, eq.graphId);

                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(eq);
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, inflated));
                GroundedClause.ConstructClauseLinks(antecedent, inflated);
            }

            return newGrounded;
        }

        //
        // Inflate a single term possibly creating a subtraction and / or multiplication node
        //
        private static GroundedClause InflateTerm(GroundedClause clause)
        {
            GroundedClause newClause = null;

            // This may not be necessary....
            // If the multiplier is non-unit (not 1), create a multiplication node
            if (Math.Abs(clause.multiplier) != 1)
            {
                newClause = new Multiplication(new NumericValue(Math.Abs(clause.multiplier)), clause);
            }

            // If the multiplier is negative, convert to subtraction
            if (clause.multiplier < 0)
            {
                newClause = new Subtraction(new NumericValue(0), newClause == null ? clause : newClause);
            }

            // Reset multiplier accordingly
            clause.multiplier = 1;

            return newClause == null ? clause : newClause;
        }

        //
        // Inflate am entire flattened side of an equation
        //
        private static GroundedClause InflateEntireSide(List<GroundedClause> side)
        {
            GroundedClause singleExp;

            if (side.Count <= 1)
            {
                singleExp = InflateTerm(side.ElementAt(0));
            }
            else
            {
                singleExp = new Addition(InflateTerm(side.ElementAt(0)), InflateTerm(side.ElementAt(1)));
                for (int i = 2; i < side.Count; i++)
                {
                    singleExp = new Addition(singleExp, InflateTerm(side.ElementAt(i)));
                }
            }

            return singleExp;
        }

        private static FlatEquation CombineLikeTerms(FlatEquation eq)
        {
            return new FlatEquation(CombineSideLikeTerms(eq.lhsExps), CombineSideLikeTerms(eq.rhsExps));
        }

        private static List<GroundedClause> CombineSideLikeTerms(List<GroundedClause> sideExps)
        {
            if (sideExps.Count == 0) return sideExps;

            if (sideExps.Count == 1)
            {
                return Utilities.MakeList<GroundedClause>(sideExps.ElementAt(0).DeepCopy());
            }

            // The new simplified side of the equation
            List<GroundedClause> simp = new List<GroundedClause>();

            // To collect all constants
            List<NumericValue> constants = new List<NumericValue>();

            // To avoid checking nodes we have already combined
            bool[] checkedExp = new bool[sideExps.Count]; // Auto-init to false
            for (int i = 0; i < sideExps.Count; i++)
            {
                if (!checkedExp[i])
                {
                    GroundedClause iExp = sideExps.ElementAt(i);

                    // Collect all constants specially
                    if (iExp is NumericValue)
                    {
                        constants.Add((NumericValue)iExp);
                    }
                    else
                    {
                        // Collect all like terms on this side
                        List<GroundedClause> likeTerms = new List<GroundedClause>();
                        for (int j = i + 1; j < sideExps.Count; j++)
                        {
                            // If same node, add to the list
                            if (sideExps.ElementAt(i).Equals(sideExps.ElementAt(j)))
                            {
                                likeTerms.Add(sideExps.ElementAt(j));
                                checkedExp[j] = true;
                            }
                        }

                        // Combine all the terms together into one node
                        GroundedClause copyExp = iExp.DeepCopy();
                        foreach (GroundedClause term in likeTerms)
                        {
                            copyExp.multiplier += term.multiplier;
                        }

                        simp.Add(copyExp);
                    }
                }

                checkedExp[i] = true;
            }

            //
            // Combine all the constants together
            //
            if (constants.Any())
            {
                int sum = 0;
                foreach (NumericValue constant in constants)
                {
                    sum += constant.value;
                }
                simp.Add(new NumericValue(sum));
            }

            return simp;
        }

        private static FlatEquation CombineLikeTermsAcrossEqual(FlatEquation eq)
        {
            // The new simplified side of the equation
            List<GroundedClause> leftSimp = new List<GroundedClause>();
            List<GroundedClause> rightSimp = new List<GroundedClause>();

            bool[] rightCheckedExp = new bool[eq.rhsExps.Count];
            foreach (GroundedClause lExp in eq.lhsExps)
            {
                int rightExpIndex = eq.rhsExps.IndexOf(lExp);

                //
                // Left expression has like term on the right?
                //
                if (rightExpIndex == -1) // it doesn't have a like term
                {
                    if (!(lExp is NumericValue))
                    {
                        leftSimp.Add(lExp); // No need to copy since it's a copy already
                    }
                    //
                    // Check the special case of a numeric value
                    //
                    else
                    {
                        // Normally the constant is at the end of the equation; so start looking there.
                        for (int j = eq.rhsExps.Count - 1; j >= 0; j--)
                        {
                            NumericValue rhsNumeric = eq.rhsExps.ElementAt(j) as NumericValue;
                            if (rhsNumeric != null)
                            {
                                rightCheckedExp[j] = true;

                                // Seek to keep positive values for the resultant, simplified expression
                                if (((NumericValue)lExp).value - rhsNumeric.value > 0)
                                {
                                    leftSimp.Add(new NumericValue(((NumericValue)lExp).value - rhsNumeric.value));
                                }
                                else if (rhsNumeric.value - ((NumericValue)lExp).value > 0)
                                {
                                    rightSimp.Add(new NumericValue(rhsNumeric.value - ((NumericValue)lExp).value));
                                }
                                else // Cancelation of the terms
                                {
                                }
                                // There's only one constant so break out.
                                break;
                            }
                        }
                    }

                }
                // Expression matches
                else
                {
                    rightCheckedExp[rightExpIndex] = true;
                    GroundedClause rExp = eq.rhsExps.ElementAt(rightExpIndex);
                    GroundedClause copyExp = lExp.DeepCopy(); // arbitrary if left or right

                    // Seek to keep positive values for the resultant, simplified expression
                    if (lExp.multiplier - rExp.multiplier > 0)
                    {
                        copyExp.multiplier = lExp.multiplier - rExp.multiplier;
                        leftSimp.Add(copyExp);
                    }
                    else if (rExp.multiplier - lExp.multiplier > 0)
                    {
                        copyExp.multiplier = rExp.multiplier - lExp.multiplier;
                        rightSimp.Add(copyExp);
                    }
                    else // Cancelation of the terms
                    {
                    }
                }
            }

            // Pick up all the expressions on the right hand side which were not like terms of those on the left
            for (int i = 0; i < eq.rhsExps.Count; i++)
            {
                if (!rightCheckedExp[i])
                {
                    rightSimp.Add(eq.rhsExps.ElementAt(i));
                }
            }

            //
            // Now check coefficients: both sides all have coefficients that evenly divide the other side.
            //
            if (leftSimp.Any() && rightSimp.Any())
            {
                // Calculate the gcd
                int gcd = leftSimp.ElementAt(0).multiplier;
                for (int i = 1; i < leftSimp.Count; i++)
                {
                    gcd = Utilities.GCD(gcd, leftSimp.ElementAt(i).multiplier);
                }
                foreach (GroundedClause rExp in rightSimp)
                {
                    gcd = Utilities.GCD(gcd, rExp.multiplier);
                }

                if (gcd != 1)
                {
                    // Divide all expressions by the gcd
                    foreach (GroundedClause lExp in leftSimp)
                    {
                        lExp.multiplier /= gcd;
                    }
                    foreach (GroundedClause rExp in rightSimp)
                    {
                        rExp.multiplier /= gcd;
                    }
                }
            }

            // Check for extreme case in which one side has no elements; in this case, add a zero
            if (!leftSimp.Any()) leftSimp.Add(new NumericValue(0));
            if (!rightSimp.Any()) rightSimp.Add(new NumericValue(0));

            return new FlatEquation(leftSimp, rightSimp);
        }
    }
}