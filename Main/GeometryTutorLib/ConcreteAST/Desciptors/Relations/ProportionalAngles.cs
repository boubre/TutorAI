using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class ProportionalAngles : Descriptor
    {
        public Angle smallerAngle { get; protected set; }
        public Angle largerAngle { get; protected set; }
        public KeyValuePair<int, int> proportion { get; protected set; }
        public double dictatedProportion { get; protected set; }

        public override bool Covers(GroundedClause gc)
        {
            return largerAngle.Covers(gc) || smallerAngle.Covers(gc);
        }

        public ProportionalAngles(Angle angle1, Angle angle2, string just) : base()
        {
            smallerAngle = angle1.measure < angle2.measure ? angle1 : angle2;
            largerAngle = angle1.measure < angle2.measure ? angle2 : angle1;

            justification = just;

            proportion = Utilities.RationalRatio(angle1.measure, angle2.measure);

            // Non-rational ratios which may have arisen due to dual congruenceg implying proportionality
            if (proportion.Key == -1 && proportion.Value == -1)
            {
                dictatedProportion = angle1.measure / angle2.measure < 1 ? angle2.measure / angle1.measure : angle1.measure / angle2.measure;
            }
            else dictatedProportion = (double)(proportion.Key) / proportion.Value;

            if (Utilities.CompareValues(dictatedProportion, 1))
            {
                throw new Exception("An angle proportion should not be 1:1 " + this.ToString());
            }

            // Reinit the multipliers to basic values
            smallerAngle.multiplier = 1;
            largerAngle.multiplier = 1;
        }

        // Return the number of shared angles in both congruences
        public int SharesNumClauses(CongruentAngles thatCas)
        {
            int numShared = smallerAngle.Equals(thatCas.ca1) || smallerAngle.Equals(thatCas.ca2) ? 1 : 0;
            numShared += largerAngle.Equals(thatCas.ca1) || largerAngle.Equals(thatCas.ca2) ? 1 : 0;

            return numShared;
        }

        public bool LinksTriangles(Triangle ct1, Triangle ct2)
        {
            return (ct1.HasAngle(smallerAngle) && ct2.HasAngle(largerAngle)) ||
                   (ct1.HasAngle(largerAngle) && ct2.HasAngle(smallerAngle));
        }

        //
        // Compare the numeric proportion between the relations
        //
        public bool ProportionallyEquals(ProportionalAngles that)
        {
            if (this.proportion.Key == -1 && this.proportion.Value == -1)
            {
                return Utilities.CompareValues(this.dictatedProportion, that.dictatedProportion);
            }

            return this.proportion.Key == that.proportion.Key && this.proportion.Value == that.proportion.Value;
        }

        public override bool StructurallyEquals(Object obj)
        {
            ProportionalAngles p = obj as ProportionalAngles;
            if (p == null) return false;
            return smallerAngle.StructurallyEquals(p.smallerAngle) && largerAngle.StructurallyEquals(p.largerAngle);
        }

        public override bool Equals(Object obj)
        {
            ProportionalAngles p = obj as ProportionalAngles;
            if (p == null) return false;
            return smallerAngle.Equals(p.smallerAngle) && largerAngle.Equals(p.largerAngle) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // Return the shared angle in both congruences
        public Angle AngleShared(CongruentAngles thatCas)
        {
            if (SharesNumClauses(thatCas) != 1) return null;

            return smallerAngle.Equals(thatCas.ca1) || smallerAngle.Equals(thatCas.ca2) ? smallerAngle : largerAngle;
        }

        // Return the shared angle in both congruences
        public Angle OtherAngle(Angle thatAngle)
        {
            if (smallerAngle.Equals(thatAngle)) return largerAngle;
            if (largerAngle.Equals(thatAngle)) return smallerAngle;

            return null;
        }

        public override string ToString()
        {
            return "Proportional(" + largerAngle.ToString() + " < " + dictatedProportion + " > " + smallerAngle.ToString() + "): " + justification;
        }

        //
        // Convert an equation to a proportion: 2AM = MC -> Proportional(Angle(A, M), Angle(M, C))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(clause is AngleEquation)) return newGrounded;

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
                        AlgebraicProportionalAngles prop = new AlgebraicProportionalAngles((Angle)flattened.lhsExps[0].DeepCopy(),
                                                                                               (Angle)flattened.rhsExps[0].DeepCopy(), "Atomic Equations are Proportional");

                        List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
                        newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, prop));
                    }
                }
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveProportion(ProportionalAngles pss, CongruentAngles conAngs)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these proportions come from the other?
            if (pss.HasRelationPredecessor(conAngs) || conAngs.HasRelationPredecessor(pss)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(pss);
            antecedent.Add(conAngs);

            //
            // Create the consequent clause
            //
            Angle shared = pss.AngleShared(conAngs);

            AlgebraicProportionalAngles newPS = new AlgebraicProportionalAngles(pss.OtherAngle(shared), conAngs.OtherAngle(shared), "Proportional / Congruence Transitivity");

            // Update relationship among the congruence pairs to limit cyclic information generation
            //newPS.AddPredecessor(pss);
            //newPS.AddPredecessor(conAngs);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newPS));

            return newGrounded;
        }

        //
        // Convert a proportion to an equation: Proportional(Angle(A, M), Angle(M, C)) -> 2AM = MC
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateProportion(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(clause is ProportionalAngles)) return newGrounded;

            ProportionalAngles propAngs = clause as ProportionalAngles;

            // Do not generate equations based on 'forced' proportions
            if (propAngs.proportion.Key == -1 || propAngs.proportion.Value == -1) return newGrounded;

            // Create a product on the left hand side
            Multiplication productLHS = new Multiplication(new NumericValue(propAngs.proportion.Key), propAngs.smallerAngle.DeepCopy());

            // Create a product on the right hand side, if it applies.
            GroundedClause rhs = propAngs.largerAngle.DeepCopy();
            if (propAngs.proportion.Value > 1)
            {
                rhs = new Multiplication(new NumericValue(propAngs.proportion.Key), rhs);
            }

            //
            // Create the equation 
            //
            Equation newEquation = null;
            if (propAngs is AlgebraicProportionalAngles)
            {
                newEquation = new AlgebraicAngleEquation(productLHS, rhs, "Defintion of Proportional Angles");
            }
            else if (propAngs is GeometricProportionalAngles)
            {
                newEquation = new GeometricAngleEquation(productLHS, rhs, "Defintion of Proportional Angles");
            }

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(propAngs);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEquation));

            return newGrounded;
        }
    }
}
