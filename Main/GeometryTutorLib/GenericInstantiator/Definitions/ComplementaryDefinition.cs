using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ComplementaryDefinition : Definition
    {
        private readonly static string NAME = "Definition of Complementary";

        //
        // This implements forward and Backward instantiation
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            if (clause is Complementary) return InstantiateFromComplementary(clause as Complementary);

            if (clause is RightAngle || clause is Strengthened || clause is AngleEquation)
            {
                return InstantiateToComplementary(clause);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        public static void Clear()
        {
            candidateAngleEquations.Clear();
            candidateRightAngles.Clear();
            candidateStrengthened.Clear();
        }

        private static List<AngleEquation> candidateAngleEquations = new List<AngleEquation>();
        private static List<RightAngle> candidateRightAngles = new List<RightAngle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToComplementary(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is AngleEquation)
            {
                AngleEquation eq = clause as AngleEquation;

                //
                // Filter only acceptable equations; one side has two values, the other one
                //
                // Check basic size of the sides
                int atomicity = eq.GetAtomicity();
                if (atomicity == Equation.BOTH_ATOMIC || atomicity == Equation.NONE_ATOMIC) return newGrounded;
                
                // Now check more involved cardinalities of each side
                KeyValuePair<int, int> cards = eq.GetCardinalities();
                if (!(cards.Key == 1 && cards.Value == 2) && !(cards.Key == 2 && cards.Value == 1)) return newGrounded;
                List<GroundedClause> terms = cards.Key == 2 ? eq.lhs.CollectTerms() : eq.rhs.CollectTerms();
                List<GroundedClause> singleton = cards.Key == 1 ? eq.lhs.CollectTerms() : eq.rhs.CollectTerms();

                // Coefficients need to be 1
                foreach (GroundedClause gc in terms)
                {
                    if (gc.multiplier != 1) return newGrounded;
                }

                // Require the constant to be 90
                NumericValue numeral = singleton[0] as NumericValue;
                if (numeral == null || Utilities.CompareValues(numeral.value, 90)) return newGrounded;

                // Require adjacent angles
                Angle angle1 = terms[0] as Angle;
                Angle angle2 = terms[1] as Angle;
                if (angle1.IsAdjacentTo(angle2) == null) return newGrounded;

                foreach (RightAngle ra in candidateRightAngles)
                {
                    newGrounded.AddRange(InstantiateToComplementary(eq, ra, ra));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToComplementary(eq, streng.strengthened as RightAngle, streng));
                }

                candidateAngleEquations.Add(eq);
            }
            else if (clause is RightAngle)
            {
                RightAngle newRa = clause as RightAngle;

                foreach (AngleEquation eq in candidateAngleEquations)
                {
                    newGrounded.AddRange(InstantiateToComplementary(eq, newRa, newRa));
                }

                candidateRightAngles.Add(newRa);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is RightAngle)) return newGrounded;

                foreach (AngleEquation eq in candidateAngleEquations)
                {
                    newGrounded.AddRange(InstantiateToComplementary(eq, newStreng.strengthened as RightAngle, newStreng));
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        // 
        // Complementary(Angle(A, B, C), Angle(D, E, F)) -> Angle(A, B, C) + Angle(D, E, F) = 90
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromComplementary(Complementary comp)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(comp);

            GeometricAngleEquation angEq = new GeometricAngleEquation(new Addition(comp.angle1, comp.angle2), new NumericValue(90), NAME);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, angEq));

            return newGrounded;
        }

        // 
        // RightAngle(A, B, C), Angle(A, B, X) + Angle(X, B, C) = 90 -> Complementary(Angle(A, B, X), Angle(X, B, C))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToComplementary(AngleEquation eq, RightAngle ra, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Acquire the two angles from the equation
            //
            KeyValuePair<int, int> cards = eq.GetCardinalities();
            List<GroundedClause> terms = cards.Key == 2 ? eq.lhs.CollectTerms() : eq.rhs.CollectTerms();
            List<GroundedClause> singleton = cards.Key == 1 ? eq.lhs.CollectTerms() : eq.rhs.CollectTerms();

            Angle angle1 = terms[0] as Angle;
            Angle angle2 = terms[1] as Angle;

            // Create the resultant angle to compare to the input right angle
            Segment shared = angle1.IsAdjacentTo(angle2);
            if (!ra.HasSegment(angle1.OtherRayEquates(shared)) || !ra.HasSegment(angle2.OtherRayEquates(shared))) return newGrounded;

            // Success, we have correspondence
            Complementary comp = new Complementary(angle1, angle2, NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, comp));

            return newGrounded;
        }
    }
}