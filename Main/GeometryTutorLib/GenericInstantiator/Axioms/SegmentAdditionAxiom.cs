using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SegmentAdditionAxiom : Axiom
    {
        private static readonly string NAME = "Segment Addition Axiom";

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is InMiddle)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            InMiddle im = (InMiddle)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new  List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Segment s1 = new Segment(im.segment.Point1, im.point);
            Segment s2 = new Segment(im.point, im.segment.Point2);
            Addition sum = new Addition(s1, s2);
            GeometricSegmentEquation eq = new GeometricSegmentEquation(sum, im.segment, NAME);
            eq.MakeAxiomatic();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(im);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq));
           

            return newGrounded;
        }
    }
}