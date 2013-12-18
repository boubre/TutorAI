using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class SimilarTriangles : Descriptor
    {
        public Triangle st1 { get; private set; }
        public Triangle st2 { get; private set; }

        public SimilarTriangles(Triangle t1, Triangle t2, string just) : base()
        {
            st1 = t1;
            st2 = t2;
            justification = just;
        }

        public override bool IsReflexive() { return st1.StructurallyEquals(st2); }

        public Triangle OtherTriangle(Triangle that)
        {
            if (st1.Equals(that)) return st2;
            if (st2.Equals(that)) return st1;

            return null;
        }

        public Triangle SharedTriangle(SimilarTriangles sts)
        {
            if (st1.StructurallyEquals(sts.st1) && st1.StructurallyEquals(sts.st2)) return st1;
            if (st2.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2)) return st2;

            return null;
        }

        public int SharesNumTriangles(SimilarTriangles sts)
        {
            int shared = st1.StructurallyEquals(sts.st1) && st1.StructurallyEquals(sts.st2) ? 1 : 0;
            shared += st2.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2) ? 1 : 0;

            return shared;
        }

        public override bool StructurallyEquals(Object c)
        {
            SimilarTriangles sts = c as SimilarTriangles;
            if (sts == null) return false;
            return st1.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2) ||
                   st1.StructurallyEquals(sts.st2) && st2.StructurallyEquals(sts.st1);
        }

        public override bool Equals(Object c)
        {
            SimilarTriangles sts = c as SimilarTriangles;
            if (sts == null) return false;
            return (st1.Equals(sts.st1) && st2.Equals(sts.st2)) || (st1.Equals(sts.st2) && st2.Equals(sts.st1));
        }

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public override int GetHashCode()
        {
            //Change this if the objest is no longer immutable!!!
            return base.GetHashCode();
        }

        private static readonly string ANGLES_NAME = "Angles of Similar Triangles are Congruent";
        private static readonly string SEGMENTS_NAME = "Segments of Similar Triangles are Proportional";

        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateProportionalSegments(SimilarTriangles simTris,
                                                                                                             List<Point> orderedTriOnePts,
                                                                                                             List<Point> orderedTriTwoPts)
        {
            List<GroundedClause> propSegments = new List<GroundedClause>();

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                Segment cs1 = new Segment(orderedTriOnePts[0], orderedTriOnePts[1]);
                Segment cs2 = new Segment(orderedTriTwoPts[0], orderedTriTwoPts[1]);
                GeometricProportionalSegments prop = new GeometricProportionalSegments(cs1, cs2, SEGMENTS_NAME);

                propSegments.Add(prop);

                // rotate the lists
                Point tmp = orderedTriOnePts.ElementAt(0);
                orderedTriOnePts.RemoveAt(0);
                orderedTriOnePts.Add(tmp);

                tmp = orderedTriTwoPts.ElementAt(0);
                orderedTriTwoPts.RemoveAt(0);
                orderedTriTwoPts.Add(tmp);
            }

            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(simTris);
            foreach (GroundedClause prop in propSegments)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, prop));
            }

            return newClauses;
        }

        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateCongruentAngles(SimilarTriangles simTris,
                                                                                                       List<Point> orderedTriOnePts,
                                                                                                       List<Point> orderedTriTwoPts)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(simTris);
            List<GroundedClause> congAngles = CongruentTriangles.GenerateCPCTCAngles(orderedTriOnePts, orderedTriTwoPts);;

            foreach (GeometricCongruentAngles ccas in congAngles)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccas));
            }

            return newClauses;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateComponents(SimilarTriangles simTris,
                                                                                                  List<Point> orderedTriOnePts,
                                                                                                  List<Point> orderedTriTwoPts)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> angles = GenerateCongruentAngles(simTris, orderedTriOnePts, orderedTriTwoPts);
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> segments = GenerateProportionalSegments(simTris, orderedTriOnePts, orderedTriTwoPts);
            angles.AddRange(segments);
 
            return angles;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveSimilarTriangles(SimilarTriangles simTris1, SimilarTriangles simTris2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these congruences come from the other?
            // CTA: We don't need this anymore since use is restricted by class TransitiveSubstitution
            if (simTris1.HasRelationPredecessor(simTris2) || simTris2.HasRelationPredecessor(simTris1)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(simTris1);
            antecedent.Add(simTris2);

            //
            // Create the consequent clause
            //
            Triangle shared = simTris1.SharedTriangle(simTris2);

            AlgebraicSimilarTriangles newAP = new AlgebraicSimilarTriangles(simTris1.OtherTriangle(shared), simTris2.OtherTriangle(shared), "Transitivity");

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAP));

            return newGrounded;
        }

        public override string ToString() { return "Similar(" + st1.ToString() + ", " + st2.ToString() + "): " + justification; }
    }
}
