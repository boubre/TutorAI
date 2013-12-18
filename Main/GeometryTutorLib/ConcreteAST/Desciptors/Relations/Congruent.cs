using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Congruent : Descriptor
    {
        ////
        //// A simple congruence relationship (segments, angles) may be used to prove a relationship
        //// regarding a containing figure (triangle, quadrilateral, etc). This same congruence should
        //// not be able to reprove the same congruence
        ////
        //public List<int> elevatedRelationships { get; private set; }
        //public bool WasElevated(int id) { return elevatedRelationships.Contains(id); }
        //public void AddElevated(int id) { elevatedRelationships.Add(id); }

        ////
        //// For transitivity of congruence statements
        ////
        //public List<int> predecessors { get; private set; }
        //public bool HasPredecessor(int id) { return predecessors.Contains(id); }
        //public void AddPredecessor(int id) { predecessors.Add(id); }

        public Congruent() : base()
        {
            //predecessors = new List<int>();
        }

        public virtual int SharesNumClauses(Congruent thatCC)
        {
            return 0;
        }

        // Unification Candidates Congruence Pairs
        private static List<Congruent> unifyCandCongruents = new List<Congruent>();

        //
        // Given an equation, if both sides are atomic, then they are equal; generate the appropriate congruence clause
        // Otherwise, if we have two ConcreteCongruent pairs which create a transitive relationship, generate that relationship.
        //
        //public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause gc)
        //{
        //    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

        //    //
        //    // Handle equation
        //    //
        //    Equation eq = gc as Equation;

        //    if (eq != null)
        //    {
        //        newGrounded = InstantiateEquation(eq);
        //    }

        //    //
        //    // Handle congruence pair
        //    //
        //    else
        //    {
        //        Congruent cc = gc as Congruent;

        //        // Only add interesting congruences: non-reflexive statements
        //        if (cc != null && !cc.IsReflexive())
        //        {
        //            newGrounded = InstantiateTransitive(cc);
        //            unifyCandCongruents.Add(cc);
        //        }
        //    }

        //    return newGrounded;
        //}

        //
        // Given an equation, if both sides are atomic, then they are equal; generate the appropriate congruence clause
        //
        //public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateEquation(Equation eq)
        //{
        //    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

        //    // Both sides are single, atomic clauses
        //    if (eq.GetAtomicity() != Equation.BOTH_ATOMIC) return newGrounded;

        //    // Make sure the multiplier (constant coefficient) of each term is 1 for a valid congruence
        //    if (!(eq.lhs.multiplier == 1 && eq.rhs.multiplier == 1)) return newGrounded;

        //    //
        //    // Atomic expressions on both sides need to be segments or angles, respectively
        //    //
        //    Congruent cc = null;
        //    if (eq is AlgebraicSegmentEquation && eq.lhs is Segment && eq.rhs is Segment)
        //    {
        //        cc = new GeometricCongruentSegments((Segment)eq.lhs, (Segment)eq.rhs, "Segments of Equal Length are Congruent");
        //    }
        //    else if (eq is AlgebraicAngleEquation && eq.lhs is Angle && eq.rhs is Angle)
        //    {
        //        cc = new GeometricCongruentAngles((Angle)eq.lhs, (Angle)eq.rhs, "Angles of Equal Measure are Congruent");
        //    }
        //    else
        //    {
        //        return newGrounded;
        //    }

        //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(eq);
        //    newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cc));

        //    return newGrounded;
        //}

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveCongruence(Congruent congruent1, Congruent congruent2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these congruences come from the other?
            // CTA: We don't need this anymore since use is restricted by class TransitiveSubstitution
            if (congruent1.HasGeneralPredecessor(congruent2) || congruent2.HasGeneralPredecessor(congruent1)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(congruent1);
            antecedent.Add(congruent2);

            //
            // Create the consequent clause
            //
            Congruent newCC = null;
            if (congruent1 is CongruentSegments)
            {
                CongruentSegments css1 = congruent1 as CongruentSegments;
                CongruentSegments css2 = congruent2 as CongruentSegments;

                Segment shared = css1.SegmentShared(css2);

                newCC = new AlgebraicCongruentSegments(css1.OtherSegment(shared), css2.OtherSegment(shared), "Transitivity");
            }
            else if (congruent1 is CongruentAngles)
            {
                CongruentAngles cas1 = congruent1 as CongruentAngles;
                CongruentAngles cas2 = congruent2 as CongruentAngles;

                Angle shared = cas1.AngleShared(cas2);

                newCC = new AlgebraicCongruentAngles(cas1.OtherAngle(shared), cas2.OtherAngle(shared), "Transitivity");
            }

            if (newCC == null)
            {
                System.Diagnostics.Debug.WriteLine("");
                throw new NullReferenceException("Unexpected Problem in Atomic substitution...");
            }

            // Update relationship among the congruence pairs to limit cyclic information generation
            //newCC.AddPredecessor(congruent1);
            //newCC.AddPredecessor(congruent2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newCC));

            return newGrounded;
        }
    }
}
