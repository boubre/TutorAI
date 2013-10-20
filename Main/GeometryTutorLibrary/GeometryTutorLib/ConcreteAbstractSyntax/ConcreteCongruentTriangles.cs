using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ConcreteCongruentTriangles : ConcreteCongruent
    {
        private static readonly string CPCTC_NAME = "CPCTC";

        public ConcreteTriangle ct1 { get; private set; }
        public ConcreteTriangle ct2 { get; private set; }

        public ConcreteCongruentTriangles(ConcreteTriangle t1, ConcreteTriangle t2, string just) : base()
        {
            ct1 = t1;
            ct2 = t2;
            justification = just;
        }

        public override bool Equals(Object c)
        {
            if (!(c is ConcreteCongruentTriangles)) return false;

            ConcreteCongruentTriangles cct = (ConcreteCongruentTriangles)c;

            return ct1.Equals(cct.ct1) && ct2.Equals(cct.ct2) || ct1.Equals(cct.ct2) && ct2.Equals(cct.ct1);
        }

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }


        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        private static List<GroundedClause> GenerateCPCTCSegments(List<ConcretePoint> orderedTriOnePts,
                                                                  List<ConcretePoint> orderedTriTwoPts)
        {
            List<GroundedClause> congSegments = new List<GroundedClause>();

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                ConcreteSegment cs1 = new ConcreteSegment(orderedTriOnePts.ElementAt(0), orderedTriOnePts.ElementAt(1));
                ConcreteSegment cs2 = new ConcreteSegment(orderedTriTwoPts.ElementAt(0), orderedTriTwoPts.ElementAt(1));
                ConcreteCongruentSegments ccss = new ConcreteCongruentSegments(cs1, cs2, CPCTC_NAME);

                congSegments.Add(ccss);

                // rotate the lists
                ConcretePoint tmp = orderedTriOnePts.ElementAt(0);
                orderedTriOnePts.RemoveAt(0);
                orderedTriOnePts.Add(tmp);

                tmp = orderedTriTwoPts.ElementAt(0);
                orderedTriTwoPts.RemoveAt(0);
                orderedTriTwoPts.Add(tmp);
            }

            return congSegments;
        }

        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        private static List<GroundedClause> GenerateCPCTCAngles(List<ConcretePoint> orderedTriOnePts,
                                                                List<ConcretePoint> orderedTriTwoPts)
        {
            List<GroundedClause> congAngles = new List<GroundedClause>();

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                ConcreteCongruentAngles ccas = new ConcreteCongruentAngles(new ConcreteAngle(orderedTriOnePts),
                                                                           new ConcreteAngle(orderedTriTwoPts), CPCTC_NAME);
                congAngles.Add(ccas);

                // rotate the lists
                ConcretePoint tmp = orderedTriOnePts.ElementAt(0);
                orderedTriOnePts.RemoveAt(0);
                orderedTriOnePts.Add(tmp);

                tmp = orderedTriTwoPts.ElementAt(0);
                orderedTriTwoPts.RemoveAt(0);
                orderedTriTwoPts.Add(tmp);
            }

            return congAngles;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateCPCTC(ConcreteCongruentTriangles ccts,
                                                         List<ConcretePoint> orderedTriOnePts,
                                                         List<ConcretePoint> orderedTriTwoPts)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(ccts);
            List<GroundedClause> congAngles = GenerateCPCTCAngles(orderedTriOnePts, orderedTriTwoPts);

            foreach (ConcreteCongruentAngles ccas in congAngles)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccas));
                GroundedClause.ConstructClauseLinks(antecedent, ccas);
            }

            List<GroundedClause> congSegments = GenerateCPCTCSegments(orderedTriOnePts, orderedTriTwoPts);
            foreach (GroundedClause ccss in congSegments)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccss));
                GroundedClause.ConstructClauseLinks(antecedent, ccss);
            }

            return newClauses;
        }

        public override string ToString() { return "Congruent(" + ct1.ToString() + ", " + ct2.ToString() + "): " + justification; }
    }
}
