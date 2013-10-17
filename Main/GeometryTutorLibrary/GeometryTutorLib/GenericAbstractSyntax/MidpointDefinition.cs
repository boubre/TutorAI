using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
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
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            if (!(c is ConcreteMidpoint)) return new List<GroundedClause>();

            ConcreteMidpoint cm = (ConcreteMidpoint)c;
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
            InMiddle im = new InMiddle(cm.midpoint, cm.segment, NAME);
            newGrounded.Add(im);
            im.AddPredecessor(Utilities.MakeList<GroundedClause>(cm));
            cm.AddSuccessor(im);

            //
            // Midpoint(M, Segment(A, B)) -> AM = MB
            //
            ConcreteSegment left = new ConcreteSegment(cm.segment.Point1, cm.midpoint);
            ConcreteSegment right = new ConcreteSegment(cm.midpoint, cm.segment.Point2);
            SegmentEquation strictEq = new SegmentEquation(left, right, NAME);

            newGrounded.Add(strictEq);
            strictEq.AddPredecessor(Utilities.MakeList<GroundedClause>(cm));
            cm.AddSuccessor(strictEq);

            //
            // Midpoint(M, Segment(A, B)) -> AM + MB = AB
            //
            Addition sum = new Addition(left, right);
            SegmentEquation generalEq = new SegmentEquation(sum, cm.segment, NAME);

            newGrounded.Add(generalEq);
            generalEq.AddPredecessor(Utilities.MakeList<GroundedClause>(cm));
            cm.AddSuccessor(generalEq);

            //
            // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B))
            //
            ConcreteCongruentSegments ccss = new ConcreteCongruentSegments(left, right, NAME);
            newGrounded.Add(ccss);
            ccss.AddPredecessor(Utilities.MakeList<GroundedClause>(cm));
            cm.AddSuccessor(ccss);

            return newGrounded;
        }
    }
}