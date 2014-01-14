using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class ProportionalSegments : Descriptor
    {
        public Segment smallerSegment { get; protected set; }
        public Segment largerSegment { get; protected set; }
        public KeyValuePair<int, int> proportion { get; protected set; }
        public double dictatedProportion { get; protected set; }

        public ProportionalSegments(Segment segment1, Segment segment2, string just) : base()
        {
            smallerSegment = segment1.Length < segment2.Length ? segment1 : segment2;
            largerSegment = segment1.Length < segment2.Length ? segment2 : segment1;

            justification = just;

            proportion = Utilities.RationalRatio(segment1.Length, segment2.Length);

            // A similar triangle may induce proportional segments even though the triangles are congruent
            //if (proportion.Key == 1 && proportion.Value == 1)
            //{
            //    throw new Exception("A segment proportion should not be 1:1 " + this.ToString());
            //}

            // Non-rational ratios which may have arisen due to dual congruenceg implying proportionality
            if (proportion.Key == -1 && proportion.Value == -1)
            {
                dictatedProportion = segment1.Length / segment2.Length < 1 ? segment2.Length / segment1.Length : segment1.Length / segment2.Length;
            }
            else dictatedProportion = (double)(proportion.Key) / proportion.Value;

            // A similar triangle may induce proportional segments even though the triangles are congruent
            //if (Utilities.CompareValues(dictatedProportion, 1))
            //{
            //    throw new Exception("A segment proportion should not be 1:1 " + this.ToString());
            //}

            // Reinit the multipliers to basic values
            smallerSegment.multiplier = 1;
            largerSegment.multiplier = 1;
        }

        public override bool Covers(GroundedClause gc)
        {
            return largerSegment.Covers(gc) || smallerSegment.Covers(gc);
        }

        // Return the number of shared segments in both congruences
        public int SharesNumClauses(CongruentSegments thatCS)
        {
            //CongruentSegments css = thatPS as CongruentSegments;

            //if (css == null) return 0;

            int numShared = smallerSegment.Equals(thatCS.cs1) || smallerSegment.Equals(thatCS.cs2) ? 1 : 0;
            numShared += largerSegment.Equals(thatCS.cs1) || largerSegment.Equals(thatCS.cs2) ? 1 : 0;

            return numShared;
        }

        public bool LinksTriangles(Triangle ct1, Triangle ct2)
        {
            return (ct1.HasSegment(smallerSegment) && ct2.HasSegment(largerSegment)) ||
                   (ct1.HasSegment(largerSegment) && ct2.HasSegment(smallerSegment));
        }

        //
        // Compare the numeric proportion between the relations
        //
        public bool ProportionallyEquals(ProportionalSegments that)
        {
            if (this.proportion.Key == -1 && this.proportion.Value == -1)
            {
                return Utilities.CompareValues(this.dictatedProportion, that.dictatedProportion);
            }

            return this.proportion.Key == that.proportion.Key && this.proportion.Value == that.proportion.Value;
        }

        public override bool StructurallyEquals(Object obj)
        {
            ProportionalSegments p = obj as ProportionalSegments;
            if (p == null) return false;
            return smallerSegment.StructurallyEquals(p.smallerSegment) && largerSegment.StructurallyEquals(p.largerSegment);
        }

        public override bool Equals(Object obj)
        {
            ProportionalSegments p = obj as ProportionalSegments;
            if (p == null) return false;
            return smallerSegment.Equals(p.smallerSegment) && largerSegment.Equals(p.largerSegment) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public bool IsDistinctFrom(ProportionalSegments thatProp)
        {
            if (this.smallerSegment.StructurallyEquals(thatProp.smallerSegment)) return false;
            if (this.largerSegment.StructurallyEquals(thatProp.largerSegment)) return false;

            return true;
        }

        // Return the shared segment in both congruences
        public Segment SegmentShared(CongruentSegments thatCC)
        {
            if (SharesNumClauses(thatCC) != 1) return null;

            return smallerSegment.Equals(thatCC.cs1) || smallerSegment.Equals(thatCC.cs2) ? smallerSegment : largerSegment;
        }

        // Return the shared segment in both congruences
        public Segment OtherSegment(Segment thatSegment)
        {
            if (smallerSegment.Equals(thatSegment)) return largerSegment;
            if (largerSegment.Equals(thatSegment)) return smallerSegment;

            return null;
        }

        public override string ToString()
        {
            return "Proportional(" + largerSegment.ToString() + " < " + dictatedProportion +  " > " + smallerSegment.ToString() + "): " + justification;
        }

        //
        // Convert an equation to a proportion: 2AM = MC -> Proportional(Segment(A, M), Segment(M, C))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateEquation(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(clause is SegmentEquation)) return newGrounded;

            Equation original = clause as Equation;

            Equation copyEq = (Equation)original.DeepCopy();
            FlatEquation flattened = new FlatEquation(copyEq.lhs.CollectTerms(), copyEq.rhs.CollectTerms());

            if (flattened.lhsExps.Count == 1 && flattened.rhsExps.Count == 1)
            {
                KeyValuePair<int, int> ratio = Utilities.RationalRatio(flattened.lhsExps[0].multiplier, flattened.rhsExps[0].multiplier);
                if (ratio.Key != -1)
                {
                    if (ratio.Key <= 2 && ratio.Value <= 2)
                    {
                        AlgebraicProportionalSegments prop = new AlgebraicProportionalSegments((Segment)flattened.lhsExps[0].DeepCopy(),
                                                                                               (Segment)flattened.rhsExps[0].DeepCopy(), "Atomic Equations are Proportional");

                        List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
                        newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, prop));
                    }
                }
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveProportion(ProportionalSegments pss, CongruentSegments conSegs)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these proportions come from the other?
            if (pss.HasRelationPredecessor(conSegs) || conSegs.HasRelationPredecessor(pss)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(pss);
            antecedent.Add(conSegs);

            //
            // Create the consequent clause
            //
            Segment shared = pss.SegmentShared(conSegs);

            AlgebraicProportionalSegments newPS = new AlgebraicProportionalSegments(pss.OtherSegment(shared), conSegs.OtherSegment(shared), "Proportional / Congruence Transitivity");

            // Update relationship among the congruence pairs to limit cyclic information generation
            //newPS.AddPredecessor(pss);
            //newPS.AddPredecessor(conSegs);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newPS));

            return newGrounded;
        }

        //
        // Convert a proportion to an equation: Proportional(Segment(A, M), Segment(M, C)) -> 2AM = MC
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateProportion(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(clause is ProportionalSegments)) return newGrounded;

            ProportionalSegments propSegs = clause as ProportionalSegments;

            // Do not generate equations based on 'forced' proportions
//            if (propSegs.proportion.Key == -1 || propSegs.proportion.Value == -1) return newGrounded;
            KeyValuePair<int, int> ratio = Utilities.RationalRatio(propSegs.dictatedProportion);
            if (ratio.Key == -1 || ratio.Value == -1) return newGrounded;

            // Avoid generating equation if this is a congruence
            if (ratio.Key == ratio.Value) return newGrounded;

            // Create a product on the left hand side, if it applies.
            GroundedClause lhs = propSegs.smallerSegment.DeepCopy();
            if (ratio.Key > 1)
            {
                lhs = new Multiplication(new NumericValue(ratio.Key), lhs);
            }

            // Create a product on the right hand side, if it applies.
            GroundedClause rhs = propSegs.largerSegment.DeepCopy();
            if (ratio.Value > 1)
            {
               rhs = new Multiplication(new NumericValue(ratio.Value), rhs);
            }

            //
            // Create the equation 
            //
            Equation newEquation = null;
            if (propSegs is AlgebraicProportionalSegments)
            {
                newEquation = new AlgebraicSegmentEquation(lhs, rhs, "Defintion of Proportional Segments");
            }
            else if (propSegs is GeometricProportionalSegments)
            {
                newEquation = new GeometricSegmentEquation(lhs, rhs, "Defintion of Proportional Segments");
            }

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(propSegs);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEquation));

            return newGrounded;
        }
    }
}
