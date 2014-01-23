using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RightAngleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Right Angle";
        private readonly static string NAME_TRANS = "Transitivity of Congruent Angles With a Right Angle";

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Strengthened streng = clause as Strengthened;

            // FROM or TO RightAngle as needed
            if (clause is RightAngle || (streng != null && streng.strengthened is RightAngle))
            {
                newGrounded.AddRange(InstantiateFromRightAngle(clause));
                newGrounded.AddRange(InstantiateToRightAngle(clause));
            }

            // TO RightAngle
            if (clause is CongruentAngles)
            {
                return InstantiateToRightAngle(clause);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromRightAngle(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            RightAngle ra = null;

            if (clause is Strengthened) ra = ((clause as Strengthened).strengthened) as RightAngle;
            else if (clause is RightAngle) ra = clause as RightAngle;
            else return newGrounded;

            // Strengthening may be something else
            if (ra == null) return newGrounded;

            GeometricAngleEquation angEq = new GeometricAngleEquation(ra, new NumericValue(90), NAME);

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(clause);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, angEq));

            return newGrounded;
        }

        public static void Clear()
        {
            candidateCongruentAngles.Clear();
            candidateRightAngles.Clear();
            candidateStrengthened.Clear();
        }

        private static List<CongruentAngles> candidateCongruentAngles = new List<CongruentAngles>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<RightAngle> candidateRightAngles = new List<RightAngle>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToRightAngle(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is CongruentAngles)
            {
                CongruentAngles cas = clause as CongruentAngles;

                // Not interested in reflexive relationships in this case
                if (cas.IsReflexive()) return newGrounded;

                foreach (RightAngle ra in candidateRightAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(ra, cas, ra));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(streng.strengthened as RightAngle, cas, streng));
                }

                candidateCongruentAngles.Add(clause as CongruentAngles);
            }
            else if (clause is RightAngle)
            {
                RightAngle ra = clause as RightAngle;

                foreach (CongruentAngles oldCas in candidateCongruentAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(ra, oldCas, ra));
                }

                candidateRightAngles.Add(ra);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                // Only intrerested in right angles
                if (!(streng.strengthened is RightAngle)) return newGrounded;

                foreach (CongruentAngles oldCas in candidateCongruentAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(streng.strengthened as RightAngle, oldCas, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // Implements 'transitivity' with right angles; that is, we may know two angles are congruent and if one is a right angle, the other is well
        //
        // Congruent(Angle(A, B, C), Angle(D, E, F), RightAngle(A, B, C) -> RightAngle(D, E, F)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToRightAngle(RightAngle ra, CongruentAngles cas, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The congruent must have the given angle in order to generate
            if (!cas.HasAngle(ra)) return newGrounded;

            Angle toBeRight = cas.OtherAngle(ra);
            Strengthened newRightAngle = new Strengthened(toBeRight, new RightAngle(toBeRight, NAME_TRANS), NAME_TRANS);

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
            antecedent.Add(cas);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newRightAngle));

            return newGrounded;
        }
    }
}