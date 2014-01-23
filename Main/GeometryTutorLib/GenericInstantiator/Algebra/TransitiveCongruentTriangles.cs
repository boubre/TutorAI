using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TransitiveCongruentTriangles : GenericRule
    {
        private static readonly string NAME = "Transitivity of Congruent Triangles";

        // Congruences imply equations: AB \cong CD -> AB = CD
        private static List<GeometricCongruentTriangles> candidateCongruentTriangles = new List<GeometricCongruentTriangles>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateCongruentTriangles.Clear();
        }

        //
        // Implements transitivity with equations
        // Congruent(Triangle(A, B, C), Triangle(D, E, F)), Congruent(Triangle(L, M, N), Triangle(D, E, F)) -> Congruent(Triangle(A, B, C), Triangle(L, M, N))
        //
        // This includes CongruentSegments and CongruentAngles
        //
        // Generation of new equations is restricted to the following rules; let G be Geometric and A algebriac
        //     G + G -> A
        //     G + A -> A
        //     A + A -X> A  <- Not allowed
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            CongruentTriangles cts = clause as CongruentTriangles;
            if (cts == null) return newGrounded;

            foreach (GeometricCongruentTriangles gcts in candidateCongruentTriangles)
            {
                newGrounded.AddRange(InstantiateTransitive(gcts, cts));
            }

            if (cts.IsGeometric()) candidateCongruentTriangles.Add(cts as GeometricCongruentTriangles);

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateTransitive(GeometricCongruentTriangles gcts, CongruentTriangles cts)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Dictionary<Point, Point> firstTriangleCorrespondence = gcts.HasTriangle(cts.ct1);
            Dictionary<Point, Point> secondTriangleCorrespondence = gcts.HasTriangle(cts.ct2);

            // Same Congruence
            if (firstTriangleCorrespondence != null && secondTriangleCorrespondence != null) return newGrounded;

            // No relationship between congruences
            if (firstTriangleCorrespondence == null && secondTriangleCorrespondence == null) return newGrounded;

            // Acquiring the triangle that links the congruences
            Triangle linkTriangle = firstTriangleCorrespondence != null ? cts.ct1 : cts.ct2;
            List<Point> linkPts = linkTriangle.GetPoints();

            Dictionary<Point, Point> otherCorrGCTSpts = gcts.OtherTriangle(linkTriangle);
            Dictionary<Point, Point> otherCorrCTSpts = cts.OtherTriangle(linkTriangle);

            // Link the other triangles together in a new congruence
            Dictionary<Point, Point> newCorrespondence = new Dictionary<Point,Point>();
            foreach (Point linkPt in linkPts)
            {
                Point otherGpt;
                if (!otherCorrGCTSpts.TryGetValue(linkPt, out otherGpt)) throw new ArgumentException("Something strange happened in Triangle correspondence.");

                Point otherCpt;
                if (!otherCorrCTSpts.TryGetValue(linkPt, out otherCpt)) throw new ArgumentException("Something strange happened in Triangle correspondence.");

                newCorrespondence.Add(otherGpt, otherCpt);
            }

            List<Point> triOne = new List<Point>(); 
            List<Point> triTwo = new List<Point>();
            foreach (KeyValuePair<Point, Point> pair in newCorrespondence)
            {
                triOne.Add(pair.Key);
                triTwo.Add(pair.Value);
            }

            //
            // Create the new congruence
            //
            AlgebraicCongruentTriangles acts = new AlgebraicCongruentTriangles(new Triangle(triOne), new Triangle(triTwo), NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(gcts);
            antecedent.Add(cts);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, acts));

            return newGrounded;
        }
    }
}