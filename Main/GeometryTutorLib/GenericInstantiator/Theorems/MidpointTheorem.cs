using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MidpointTheorem : Theorem
    {
        private readonly static string NAME = "Midpoint Theorem";

        public MidpointTheorem() { }

        //
        // Midpoint(M, Segment(A, B)) -> 2AM = AB, 2BM = AB          A ------------- M ------------- B
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            if (clause is Midpoint) return InstantiateMidpointTheorem(clause, clause as Midpoint);

            if ((clause as Strengthened).strengthened is Midpoint) return InstantiateMidpointTheorem(clause, (clause as Strengthened).strengthened as Midpoint);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateMidpointTheorem(GroundedClause original, Midpoint midpt)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Construct 2AM
            Multiplication product1 = new Multiplication(new NumericValue(2), new Segment(midpt.midpoint, midpt.segment.Point1));
            // Construct 2BM
            Multiplication product2 = new Multiplication(new NumericValue(2), new Segment(midpt.midpoint, midpt.segment.Point2));

            // 2X = AB
            GeometricSegmentEquation newEq1 = new GeometricSegmentEquation(product1, midpt.segment, NAME);
            GeometricSegmentEquation newEq2 = new GeometricSegmentEquation(product2, midpt.segment, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEq1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEq2));

            return newGrounded;
        }
    }
}