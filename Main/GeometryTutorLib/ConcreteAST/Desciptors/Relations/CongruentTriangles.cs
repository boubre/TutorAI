using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class CongruentTriangles : Congruent
    {
        private static readonly string CPCTC_NAME = "CPCTC";

        public Triangle ct1 { get; protected set; }
        public Triangle ct2 { get; protected set; }

        public CongruentTriangles(Triangle t1, Triangle t2, string just)
            : base()
        {
            ct1 = t1;
            ct2 = t2;
            justification = just;
        }

        public override bool Covers(GroundedClause gc)
        {
            if (gc is Triangle) return ct1.StructurallyEquals(gc) || ct2.StructurallyEquals(gc);

            return ct1.Covers(gc) || ct2.Covers(gc);
        }

        public override bool StructurallyEquals(Object c)
        {
            if (!(c is CongruentTriangles)) return false;

            CongruentTriangles cct = (CongruentTriangles)c;

            return ct1.StructurallyEquals(cct.ct1) && ct2.StructurallyEquals(cct.ct2) ||
                   ct1.StructurallyEquals(cct.ct2) && ct2.StructurallyEquals(cct.ct1);
        }

        public override bool Equals(Object c)
        {
            if (!(c is CongruentTriangles)) return false;

            CongruentTriangles cct = (CongruentTriangles)c;

            return (ct1.Equals(cct.ct1) && ct2.Equals(cct.ct2) || ct1.Equals(cct.ct2) && ct2.Equals(cct.ct1)) && base.Equals(c);
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
        // This congruence is given from the user, so handle any type of processing needed to prevent
        // 'reproving' facts implied by this congruence
        //
        public void ProcessGivens()
        {
            ct1.AddCongruentTriangle(ct2);
            ct2.AddCongruentTriangle(ct1);
        }

        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        private static List<GroundedClause> GenerateCPCTCSegments(List<Point> orderedTriOnePts,
                                                                  List<Point> orderedTriTwoPts)
        {
            List<GroundedClause> congSegments = new List<GroundedClause>();

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                Segment cs1 = new Segment(orderedTriOnePts.ElementAt(0), orderedTriOnePts.ElementAt(1));
                Segment cs2 = new Segment(orderedTriTwoPts.ElementAt(0), orderedTriTwoPts.ElementAt(1));
                GeometricCongruentSegments ccss = new GeometricCongruentSegments(cs1, cs2, CPCTC_NAME);

                congSegments.Add(ccss);

                // rotate the lists
                Point tmp = orderedTriOnePts.ElementAt(0);
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
        public static List<GroundedClause> GenerateCPCTCAngles(List<Point> orderedTriOnePts,
                                                               List<Point> orderedTriTwoPts)
        {
            List<GroundedClause> congAngles = new List<GroundedClause>();

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                GeometricCongruentAngles ccas = new GeometricCongruentAngles(new Angle(orderedTriOnePts),
                                                                             new Angle(orderedTriTwoPts), CPCTC_NAME);
                congAngles.Add(ccas);

                // rotate the lists
                Point tmp = orderedTriOnePts[0];
                orderedTriOnePts.RemoveAt(0);
                orderedTriOnePts.Add(tmp);

                tmp = orderedTriTwoPts[0];
                orderedTriTwoPts.RemoveAt(0);
                orderedTriTwoPts.Add(tmp);
            }

            return congAngles;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> GenerateCPCTC(CongruentTriangles ccts,
                                                         List<Point> orderedTriOnePts,
                                                         List<Point> orderedTriTwoPts)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(ccts);
            List<GroundedClause> congAngles = GenerateCPCTCAngles(orderedTriOnePts, orderedTriTwoPts);

            foreach (GeometricCongruentAngles ccas in congAngles)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccas));
            }

            List<GroundedClause> congSegments = GenerateCPCTCSegments(orderedTriOnePts, orderedTriTwoPts);
            foreach (GroundedClause ccss in congSegments)
            {
                newClauses.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccss));
            }

            return newClauses;
        }

        //
        // Generate all corresponding conngruent components.
        // Ensure vertices correpsond appropriately.
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            CongruentTriangles conTris = clause as CongruentTriangles;

            if (conTris == null) return newGrounded;

            List<Point> orderedTriOnePts = new List<Point>();
            List<Point> orderedTriTwoPts = new List<Point>();

            if (!conTris.VerifyCongruentTriangles(out orderedTriOnePts, out orderedTriTwoPts)) return newGrounded;

            return GenerateCPCTC(conTris, orderedTriOnePts, orderedTriTwoPts);
        }

        public bool VerifyCongruentTriangles()
        {
            List<Point> orderedTriOnePts = new List<Point>();
            List<Point> orderedTriTwoPts = new List<Point>();

            return VerifyCongruentTriangles(out orderedTriOnePts, out orderedTriTwoPts);
        }


        private bool VerifyCongruentTriangles(out List<Point> orderedTriOnePts, out List<Point> orderedTriTwoPts)
        {
            // Ensure the points are ordered appropriately.
            // CongruentTriangle ensures points are mapped.

            List<Angle> triOneAngles = ct1.GetAngles();
            List<Angle> triTwoAngles = ct2.GetAngles();

            orderedTriOnePts = new List<Point>();
            orderedTriTwoPts = new List<Point>();
            bool[] marked = new bool[3];

            //
            // Check angles correspondence
            //
            for (int i = 0; i < triOneAngles.Count; i++)
            {
                for (int j = 0; j < triTwoAngles.Count; j++)
                {
                    if (!marked[j])
                    {
                        if (Utilities.CompareValues(triOneAngles[i].measure, triTwoAngles[j].measure))
                        {
                            orderedTriOnePts.Add(triOneAngles[i].GetVertex());
                            orderedTriTwoPts.Add(triTwoAngles[j].GetVertex());
                            marked[j] = true;
                            break;
                        }
                    }
                }
            }

            // Similarity has failed 
            if (marked.Contains(false))
            {
                return false;
            }

            //
            // The established correspondence can now be verified with segment lengths
            //
            for (int v = 0; v < 2; v++)
            {
                double seg1Distance = Point.calcDistance(orderedTriOnePts[v], orderedTriOnePts[v + 1 < 3 ? v + 1 : 0]);
                double seg2Distance = Point.calcDistance(orderedTriTwoPts[v], orderedTriTwoPts[v + 1 < 3 ? v + 1 : 0]);

                if (!Utilities.CompareValues(seg1Distance, seg2Distance))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString() { return "Congruent(" + ct1.ToString() + ", " + ct2.ToString() + "): " + justification; }
    }
}
