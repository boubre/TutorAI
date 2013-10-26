using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public abstract class ConcreteCongruent : Descriptor
    {
        //
        // For transitivity of congruence statements
        //
        public int congruentId { get; private set; }
        public void SetId(int id) { congruentId = id; }
        public List<int> directCongruentPredecessors { get; private set; }
        public bool HasCongruencePredecessor(ConcreteCongruent cc) { return directCongruentPredecessors.Contains(cc.congruentId); }
        public void AddCongruencePredecessor(ConcreteCongruent cc) { directCongruentPredecessors.Add(cc.congruentId); }

        public virtual bool IsReflexive() { return false; }

        public ConcreteCongruent() : base()
        {
            congruentId = -1;
        }

        public virtual int SharesNumClauses(ConcreteCongruent thatCC)
        {
            return 0;
        }

        // Unification Candidates Congruence Pairs
        private static List<ConcreteCongruent> unifyCandCongruents = new List<ConcreteCongruent>();

        //
        // Given an equation, if both sides are atomic, then they are equal; generate the appropriate congruence clause
        // Otherwise, if we have two ConcreteCongruent pairs which create a transitive relationship, generate that relationship.
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause gc)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Handle equation
            //
            Equation eq = gc as Equation;

            if (eq != null)
            {
                newGrounded = InstantiateEquation(eq);
            }

            //
            // Handle congruence pair
            //
            else
            {
                ConcreteCongruent cc = gc as ConcreteCongruent;

                // Only add interesting congruences: non-reflexive statements
                if (cc != null && !cc.IsReflexive())
                {
                    newGrounded = InstantiateTransitive(cc);
                    unifyCandCongruents.Add(cc);
                }
            }

            return newGrounded;
        }

        //
        // Given an equation, if both sides are atomic, then they are equal; generate the appropriate congruence clause
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateEquation(Equation eq)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Both sides are single, atomic clauses
            if (eq.GetAtomicity() != Equation.BOTH_ATOMIC) return newGrounded;

            // Make sure the multiplier (constant coefficient) of each term is 1 for a valid congruence
            if (!(eq.lhs.multiplier == 1 && eq.rhs.multiplier == 1)) return newGrounded;

            //
            // Atomic expressions on both sides need to be segments or angles, respectively
            //
            ConcreteCongruent cc = null;
            if (eq is SegmentEquation && eq.lhs is ConcreteSegment && eq.rhs is ConcreteSegment)
            {
                cc = new ConcreteCongruentSegments((ConcreteSegment)eq.lhs, (ConcreteSegment)eq.rhs, "Segments of Equal Length are Congruent");
            }
            else if (eq is AngleMeasureEquation && eq.lhs is ConcreteAngle && eq.rhs is ConcreteAngle)
            {
                cc = new ConcreteCongruentAngles((ConcreteAngle)eq.lhs, (ConcreteAngle)eq.rhs, "Angles of Equal Measure are Congruent");
            }
            else
            {
                return newGrounded;
            }

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(eq);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cc));

            return newGrounded;
        }

        //
        // Compare the new Congruent statement with all others to acquire a new congruence pair due to transitivity
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateTransitive(ConcreteCongruent newCC)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            foreach (ConcreteCongruent cc in unifyCandCongruents)
            {
                // Do we have the same type of congruence statement (angle vs. segment)?
                if (newCC.GetType() == cc.GetType())
                {
                    int numSharedExps = cc.SharesNumClauses(newCC);
                    switch (numSharedExps)
                    {
                        case 0:
                            // Nothing is shared: do nothing
                            return newGrounded;

                        case 1:
                            // Expected case to create a new congruence relationship
                            return CreateTransitiveCongruence(cc, newCC);

                        case 2:
                            // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                            return newGrounded;
                
                        default:
                            throw new Exception("Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
                    }
                }
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveCongruence(ConcreteCongruent cc1,
                                                                                                           ConcreteCongruent cc2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these congruences come from the other?
            if (cc1.HasCongruencePredecessor(cc2) || cc2.HasCongruencePredecessor(cc1)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cc1);
            antecedent.Add(cc2);

            //
            // Create the consequent clause
            //
            ConcreteCongruent newCC = null;
            if (cc1 is ConcreteCongruentSegments)
            {
                ConcreteCongruentSegments ccss1 = cc1 as ConcreteCongruentSegments;
                ConcreteCongruentSegments ccss2 = cc2 as ConcreteCongruentSegments;

                ConcreteSegment shared = ccss1.SegmentShared(ccss2);

                newCC = new ConcreteCongruentSegments(ccss1.OtherSegment(shared), ccss2.OtherSegment(shared), "Transitivity");
            }
            else if (cc1 is ConcreteCongruentAngles)
            {
                ConcreteCongruentAngles ccas1 = cc1 as ConcreteCongruentAngles;
                ConcreteCongruentAngles ccas2 = cc2 as ConcreteCongruentAngles;

                ConcreteAngle shared = ccas1.SegmentShared(ccas2);

                newCC = new ConcreteCongruentAngles(ccas1.OtherAngle(shared), ccas2.OtherAngle(shared), "Transitivity");
            }

            // Update relationship among the congruence pairs to limit cyclic information generation
            newCC.AddCongruencePredecessor(cc1);
            newCC.AddCongruencePredecessor(cc2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newCC));

            return newGrounded;
        }
    }
}
