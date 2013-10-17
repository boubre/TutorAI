using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class SegmentAdditionAxiom : Axiom
    {
        private static readonly string NAME = "Segment Addition Axiom";

        public SegmentAdditionAxiom() { }

        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            if (!(c is InMiddle)) return new List<GroundedClause>();

            InMiddle im = (InMiddle)c;
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            ConcreteSegment s1 = new ConcreteSegment(im.segment.Point1, im.point);
            ConcreteSegment s2 = new ConcreteSegment(im.point, im.segment.Point2);
            Addition sum = new Addition(s1, s2);
            SegmentEquation eq = new SegmentEquation(sum, im.segment, NAME);

            newGrounded.Add(eq);
            eq.AddPredecessor(Utilities.MakeList<GroundedClause>(im));
            im.AddSuccessor(eq);

            return newGrounded;
        }
    }
}