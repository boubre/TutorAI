using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
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
        public static Equation Simplify(Equation original)
        {
            // Do we have an equation?
            if (original == null) throw new ArgumentException();

            // Is the equation 0 = 0? This should be allowed at it indicates a tautology
            if (original.lhs.Equals(new NumericValue(0)) && original.rhs.Equals(new NumericValue(0)))
            {
                throw new ArgumentException("Should not have an equation that is 0 = 0: " + original.ToString());
            }

            //
            // Ideally, flattening would:
            // Remove all subtractions -> adding a negative instead
            // Distribute subtraction or multiplication over addition
            //
            // Flatten the equation so that each side is a sum of atomic expressions
            Equation copyEq = (Equation)original.DeepCopy();
            FlatEquation flattened = new FlatEquation(copyEq.lhs.CollectTerms(), copyEq.rhs.CollectTerms());

            //Debug.WriteLine("Equation prior to simplification: " + flattened.ToString());

            // Combine terms only on each side (do not cross =)
            FlatEquation combined = CombineLikeTerms(flattened);

            //Debug.WriteLine("Equation after like terms combined on both sides: " + combined);

            // Combine terms across the equal sign
            FlatEquation across = CombineLikeTermsAcrossEqual(combined);

             //Debug.WriteLine("Equation after simplifying both sides: " + across);

            //
            // Inflate the equation
            //
            Equation inflated = null;
            GroundedClause singleLeftExp = InflateEntireSide(across.lhsExps);
            GroundedClause singleRightExp = InflateEntireSide(across.rhsExps);
            if (original is AlgebraicSegmentEquation)
            {
                inflated = new AlgebraicSegmentEquation(singleLeftExp, singleRightExp);
            }
            else if (original is GeometricSegmentEquation)
            {
                inflated = new GeometricSegmentEquation(singleLeftExp, singleRightExp);
            }
            else if (original is AlgebraicAngleEquation)
            {
                inflated = new AlgebraicAngleEquation(singleLeftExp, singleRightExp);
            }
            else if (original is GeometricAngleEquation)
            {
                inflated = new GeometricAngleEquation(singleLeftExp, singleRightExp);
            }

            // If simplifying didn't do anything, return the original equation
            if (inflated.Equals(original))
            {
                return original;
            }

            // Simplified equations should inherit the algebraic predecessors of the the original equation as well as the original node
            //Utilities.AddUniqueList<int>(inflated.predecessors, original.predecessors);
            //Utilities.AddUnique<int>(inflated.predecessors, original.clauseId);

            return inflated;
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
                singleExp = InflateTerm(side[0]);
            }
            else
            {
                singleExp = new Addition(InflateTerm(side[0]), InflateTerm(side[1]));
                for (int i = 2; i < side.Count; i++)
                {
                    singleExp = new Addition(singleExp, InflateTerm(side[i]));
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
                    GroundedClause iExp = sideExps[i];

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
                            if (sideExps[i].StructurallyEquals(sideExps[j]))
                            {
                                likeTerms.Add(sideExps[j]);
                                checkedExp[j] = true;
                            }
                        }

                        // Combine all the terms together into one node
                        GroundedClause copyExp = iExp.DeepCopy(); // Note, iExp represents the 'first' like term
                        foreach (GroundedClause term in likeTerms)
                        {
                            copyExp.multiplier += term.multiplier;
                        }

                        // If everything cancels, add a 0 to the equation
                        if (copyExp.multiplier == 0)
                        {
                            constants.Add(new NumericValue(0));
                        }
                        else
                        {
                            simp.Add(copyExp);
                        }
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

            // The resultant constant values on the left / right sides
            KeyValuePair<int, int> constantPair = HandleConstants(eq);

            bool[] rightCheckedExp = new bool[eq.rhsExps.Count];
            foreach (GroundedClause lExp in eq.lhsExps)
            {
                if (!(lExp is NumericValue))
                {
                    int rightExpIndex = StructuralIndex(eq.rhsExps, lExp);

                    //
                    // Left expression has like term on the right?
                    //
                    // it doesn't have a like term
                    if (rightExpIndex == -1)
                    {
                        leftSimp.Add(lExp);
                    }
                    //
                    // Expression has like term on the right
                    //
                    else
                    {
                        rightCheckedExp[rightExpIndex] = true;
                        GroundedClause rExp = eq.rhsExps[rightExpIndex];
                        GroundedClause copyExp = lExp.DeepCopy();

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
            }

            // Pick up all the expressions on the right hand side which were not like terms of those on the left
            for (int i = 0; i < eq.rhsExps.Count; i++)
            {
                if (!rightCheckedExp[i] && !(eq.rhsExps[i] is NumericValue))
                {
                    rightSimp.Add(eq.rhsExps[i]);
                }
            }

            //
            // Add back the constant to the correct side
            //
            if (constantPair.Key != 0) leftSimp.Add(new NumericValue(constantPair.Key));
            if (constantPair.Value != 0) rightSimp.Add(new NumericValue(constantPair.Value));

            //
            // Now check coefficients: both sides all have coefficients that evenly divide the other side.
            //
            if (leftSimp.Any() && rightSimp.Any())
            {
                // Calculate the gcd
                int gcd = leftSimp[0].multiplier;
                for (int i = 1; i < leftSimp.Count; i++)
                {
                    gcd = Utilities.GCD(gcd, leftSimp[i].multiplier);
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

        //
        // Returns a positive constant on the LHS or RHS as appropriate.
        //
        private static KeyValuePair<int, int> HandleConstants(FlatEquation eq)
        {
            int lhs = CollectConstants(eq.lhsExps);
            int rhs = CollectConstants(eq.rhsExps);

            int simpLeft = lhs > rhs ? lhs - rhs : 0;
            int simpRight = rhs > lhs ? rhs - lhs : 0;

            return new KeyValuePair<int, int>(simpLeft, simpRight);
        }

        //
        // Sum the constants on one side of an equation
        //
        private static int CollectConstants(List<GroundedClause> side)
        {
            int result = 0;

            foreach (GroundedClause term in side)
            {
                if (term is NumericValue)
                {
                    result += term.multiplier * (term as NumericValue).value;
                }
            }

            return result;
        }

        private static int StructuralIndex(List<GroundedClause> side, GroundedClause term)
        {
            for (int r = 0; r < side.Count; r++)
            {
                if (side[r].StructurallyEquals(term))
                {
                    return r;
                }
            }
            return -1;
        }
    }
}