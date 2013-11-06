using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MidpointDefinition : Definition
    {
        private readonly static string NAME = "Definition of Midpoint";

        public MidpointDefinition() { }

        //
        // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
        // Midpoint(M, Segment(A, B)) -> AM + MB = AB
        // Midpoint(M, Segment(A, B)) -> AM = MB
        // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is ConcreteMidpoint)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            ConcreteMidpoint cm = (ConcreteMidpoint)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(cm);

            // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
            InMiddle im = new InMiddle(cm.midpoint, cm.segment, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, im));


            // CTA: BEGIN deletion

            //
            // Midpoint(M, Segment(A, B)) -> AM = MB
            //
            ConcreteSegment left = new ConcreteSegment(cm.segment.Point1, cm.midpoint);
            ConcreteSegment right = new ConcreteSegment(cm.midpoint, cm.segment.Point2);
            SegmentEquation strictEq = new SegmentEquation(left, right, NAME);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, strictEq));

            //
            // Midpoint(M, Segment(A, B)) -> AM + MB = AB
            //
            Addition sum = new Addition(left, right);
            SegmentEquation generalEq = new SegmentEquation(sum, cm.segment, NAME);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, generalEq));

            // CTA: END deletion

            //
            // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B))
            //
            ConcreteCongruentSegments ccss = new ConcreteCongruentSegments(left, right, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccss));

            return newGrounded;
        }
    }
}