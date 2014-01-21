using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MedianDefinition : Definition
    {
        private readonly static string NAME = "Definition of Median";

        public MedianDefinition() { }

        //
        // This implements forward and Backward instantiation
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (c is Median) return InstantiateMedian(c as Median);

            if (c is SegmentBisector || c is Triangle) return InstantiateBisector(c);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // Median(Segment(V, C), Triangle(C, A, B)) -> Midpoint(V, Segment(B, A))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateMedian(Median median)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Which point is on the side of the triangle?
            Point vertexOnTriangle = median.theTriangle.GetVertexOn(median.medianSegment);
            Segment segmentCutByMedian = median.theTriangle.GetOppositeSide(vertexOnTriangle);
            Point midpt = segmentCutByMedian.FindIntersection(median.medianSegment);

            // This is to acquire the name of the midpoint, nothing more.
            if (midpt.Equals(median.medianSegment.Point1)) midpt = median.medianSegment.Point1;
            else if (midpt.Equals(median.medianSegment.Point2)) midpt = median.medianSegment.Point2;

            // Create the midpoint
            Midpoint newMidpoint = new Midpoint(midpt, segmentCutByMedian, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(median);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newMidpoint));

            return newGrounded;
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // SegmentBisector(Segment(V, C), Segment(B, A)), Triangle(A, B, C) -> Median(Segment(V, C), Triangle(A, B, C))
        //        
        private static List<Triangle> candidateTriangle = new List<Triangle>();
        private static List<SegmentBisector> candidateBisector = new List<SegmentBisector>();

        // Reset saved data for next problem
        public static void Clear()
        {
            candidateBisector.Clear();
            candidateTriangle.Clear();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateBisector(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Triangle)
            {
                Triangle tri = c as Triangle;

                foreach (SegmentBisector bisector in candidateBisector)
                {
                    newGrounded.AddRange(InstantiateToDef(tri, bisector));
                }

                candidateTriangle.Add(tri);
            }
            else if (c is SegmentBisector)
            {
                SegmentBisector bisector = c as SegmentBisector;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToDef(tri, bisector));
                }

                candidateBisector.Add(bisector);
            }

            return newGrounded;
        }

        //
        // Take the angle congruence and bisector and create the AngleBisector relation
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToDef(Triangle tri, SegmentBisector sb)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The Bisector cannot be a side of the triangle.
            if (tri.CoincidesWithASide(sb.bisector) != null) return newGrounded;

            // Acquire the intersection segment that coincides with the base of the triangle
            Segment triangleBaseCandidate = sb.bisected.OtherSegment(sb.bisector);
            Segment triangleBase = tri.CoincidesWithASide(triangleBaseCandidate);
            if (triangleBase == null) return newGrounded;

            // The candidate base and the actual triangle side must equate exactly
            if (!triangleBase.HasSubSegment(triangleBaseCandidate) || !triangleBaseCandidate.HasSubSegment(triangleBase)) return newGrounded;

            // The point opposite the base of the triangle must be within the endpoints of the bisector
            Point oppPoint = tri.OtherPoint(triangleBase);
            if (!sb.bisector.PointIsOnAndBetweenEndpoints(oppPoint)) return newGrounded;

            // -> Median(Segment(V, C), Triangle(A, B, C))
            Median newMedian = new Median(sb.bisector, tri, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(sb);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newMedian));
            return newGrounded;
        }
    }
}