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
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Median || clause is Strengthened) newGrounded.AddRange(InstantiateFromMedian(clause));

            if (clause is SegmentBisector || clause is Triangle || clause is Strengthened) newGrounded.AddRange(InstantiateToMedian(clause));

            return newGrounded;
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // Median(Segment(V, C), Triangle(C, A, B)) -> Midpoint(V, Segment(B, A))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromMedian(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Median)
            {
                newGrounded.AddRange(InstantiateFromMedian(clause as Median, clause));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Median)) return newGrounded;

                newGrounded.AddRange(InstantiateFromMedian(streng.strengthened as Median, clause));
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromMedian(Median median, GroundedClause original)
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
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
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
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        // Reset saved data for next problem
        public static void Clear()
        {
            candidateBisector.Clear();
            candidateTriangle.Clear();
            candidateStrengthened.Clear();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToMedian(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Triangle)
            {
                Triangle tri = clause as Triangle;

                foreach (SegmentBisector sb in candidateBisector)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, sb, sb));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, streng.strengthened as SegmentBisector, streng));
                }

                candidateTriangle.Add(tri);
            }
            else if (clause is SegmentBisector)
            {
                SegmentBisector sb = clause as SegmentBisector;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, sb, sb));
                }

                candidateBisector.Add(sb);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is SegmentBisector)) return newGrounded;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, streng.strengthened as SegmentBisector, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // Take the angle congruence and bisector and create the AngleBisector relation
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToMedian(Triangle tri, SegmentBisector sb, GroundedClause original)
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
            antecedent.Add(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newMedian));

            return newGrounded;
        }
    }
}