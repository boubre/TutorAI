using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularBisectorDefinition : Definition
    {
        private readonly static string NAME = "Definition of Perpendicular Bisector";

        //
        // PerpendicularBisector(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D))) ->
        //                         Perpendicular(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D))),
        //                         SegmentBisector(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D)))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            if (clause is PerpendicularBisector) return InstantiateFromPerpendicularBisector(clause, clause as PerpendicularBisector);

            if ((clause as Strengthened).strengthened is PerpendicularBisector)
            {
                return InstantiateFromPerpendicularBisector(clause, (clause as Strengthened).strengthened as PerpendicularBisector);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromPerpendicularBisector(GroundedClause original, PerpendicularBisector pb)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            Strengthened streng1 = new Strengthened(pb.originalInter, new Perpendicular(pb.originalInter, NAME), NAME);
            Strengthened streng2 = new Strengthened(pb.originalInter, new SegmentBisector(pb.originalInter, pb.bisector, NAME), NAME);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, streng1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, streng2));

            return newGrounded;
        }
    }
}