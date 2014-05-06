using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class SimpleRegionEquation
    {
        public Region target { get; private set; }
        public AreaArithmeticNode expr { get; private set; }

        public SimpleRegionEquation(Region t, AreaArithmeticNode arith) : base()
        {
            target = t;
            expr = arith;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        //public override void Substitute(GroundedClause toFind, GroundedClause toSub)
        //{
        //    if (target.Equals(toFind))
        //    {
        //        target = toSub.DeepCopy();
        //    }
        //    else
        //    {
        //        target.Substitute(toFind, toSub);
        //    }

        //    if (rhs.Equals(toFind))
        //    {
        //        rhs = toSub.DeepCopy();
        //    }
        //    else
        //    {
        //        rhs.Substitute(toFind, toSub);
        //    }
        //}

        //public override bool Contains(GroundedClause target)
        //{
        //    // If a composite node, check accordingly; this will return false if they are atomic
        //    return target.Contains(target) || rhs.Contains(target);
        //}

        //
        // Determines if the equation has one side being atomic; no compound expressions
        // returns -1 (left is atomic), 0 (neither atomic), 1 (right is atomic)
        // both atomic: 2
        //
        //public const int LEFT_ATOMIC = -1;
        //public const int NONE_ATOMIC = 0;
        //public const int RIGHT_ATOMIC = 1;
        //public const int BOTH_ATOMIC = 2;
        //public int GetAtomicity()
        //{
        //    bool leftIs = target is Angle || target is Segment || target is NumericValue;
        //    bool rightIs = rhs is Angle || rhs is Segment || rhs is NumericValue;

        //    if (leftIs && rightIs) return BOTH_ATOMIC;
        //    if (!leftIs && !rightIs) return NONE_ATOMIC;
        //    if (leftIs) return LEFT_ATOMIC;
        //    if (rightIs) return RIGHT_ATOMIC;

        //    return NONE_ATOMIC;
        //}

        //// Collect all the terms and return a count for both sides <left, right>
        //public KeyValuePair<int, int> GetCardinalities()
        //{
        //    List<GroundedClause> left = target.CollectTerms();
        //    List<GroundedClause> right = rhs.CollectTerms();
        //    return new KeyValuePair<int, int>(left.Count, right.Count);
        //}

        //
        // Equals checks that for both sides of this equation is the same as one entire side of the other equation
        //
        public override bool Equals(object obj)
        {
            SimpleRegionEquation thatEquation = obj as SimpleRegionEquation;
            if (thatEquation == null) return false;

            return true;

            //
            // Collect all basic terms on the left and right hand sides of both equations.
            //
            //List<GroundedClause> thistarget = target.CollectTerms();
            //List<GroundedClause> thisRHS = rhs.CollectTerms();

            //List<GroundedClause> thattarget = thatEquation.target.CollectTerms();
            //List<GroundedClause> thatRHS = thatEquation.rhs.CollectTerms();

            //// Check side length counts as a first step.
            //if (!(thistarget.Count == thattarget.Count && thisRHS.Count == thatRHS.Count) && !(thistarget.Count == thatRHS.Count && thisRHS.Count == thattarget.Count)) return false;

            //// Seek one side equal to one side and then the other equals the other.
            //// Cannot do this easily with a union / set interection set theoretic approach since an equation may have multiple instances of a value
            //// In theory, since we always deal with simplified equations, there should not be multiple instances of a particular value.
            //// So, this should work.

            //// Note operations like multiplication and substraction have been taken into account.
            //List<GroundedClause> uniontarget = new List<GroundedClause>(thistarget);
            //Utilities.AddUniqueList(uniontarget, thattarget);

            //List<GroundedClause> unionRHS = new List<GroundedClause>(thisRHS);
            //Utilities.AddUniqueList(unionRHS, thatRHS);

            //// Exact same sides means the union is the same as each list itself
            //if (uniontarget.Count == thistarget.Count && unionRHS.Count == thisRHS.Count) return true;

            //// Check the other combination of sides
            //uniontarget = new List<GroundedClause>(thistarget);
            //Utilities.AddUniqueList(uniontarget, thatRHS);

            //if (uniontarget.Count != thistarget.Count) return false;

            //unionRHS = new List<GroundedClause>(thisRHS);
            //Utilities.AddUniqueList(unionRHS, thattarget);

            //// Exact same sides means the union is the same as each list itself
            //return unionRHS.Count == thisRHS.Count;
        }
    }
}