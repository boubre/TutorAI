using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class Parallel : Descriptor
    {
        public Segment segment1 { get; private set; }
        public Segment segment2 { get; private set; }

        public Parallel(Segment segment1, Segment segment2, string just) : base()
        {
            this.segment1 = segment1;
            this.segment2 = segment2;
            justification = just;

            if (!segment1.IsParallelWith(segment2))
            {
                throw new ArgumentException("Given lines are not parallel: " + segment1 + " ; " + segment2);
            }
        }

        // This should never be true, otherwuse they are coinciding
        public override bool IsReflexive() { return segment1.StructurallyEquals(segment2); }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public Segment OtherSegment(Segment that)
        {
            if (segment1.Equals(that)) return segment2;
            if (segment2.Equals(that)) return segment1;

            return null;
        }

        public Segment CoincidesWith(Segment that)
        {
            if (segment1.IsCollinearWith(that)) return segment1;
            if (segment2.IsCollinearWith(that)) return segment2;

            return null;
        }

        public Segment SharedSegment(Parallel thatParallel)
        {
            if (segment1.IsCollinearWith(thatParallel.segment1) && segment1.IsCollinearWith(thatParallel.segment2)) return segment1;
            if (segment2.IsCollinearWith(thatParallel.segment1) && segment2.IsCollinearWith(thatParallel.segment2)) return segment2;

            return null;
        }

        public int SharesNumClauses(Parallel thatParallel)
        {
            int shared = segment1.IsCollinearWith(thatParallel.segment1) && segment1.IsCollinearWith(thatParallel.segment2) ? 1 : 0;
            shared += segment2.IsCollinearWith(thatParallel.segment1) && segment2.IsCollinearWith(thatParallel.segment2) ? 1 : 0;

            return shared;
        }

        public override bool Covers(GroundedClause gc)
        {
            return segment1.Covers(gc) || segment2.Covers(gc);
        }

        public override bool StructurallyEquals(Object obj)
        {
            Parallel p = obj as Parallel;
            if (p == null) return false;
            return (segment1.StructurallyEquals(p.segment1) && segment2.StructurallyEquals(p.segment2)) ||
                   (segment1.StructurallyEquals(p.segment2) && segment2.StructurallyEquals(p.segment1));
        }

        public override bool Equals(Object obj)
        {
            Parallel p = obj as Parallel;
            if (p == null) return false;
            return (segment1.Equals(p.segment1) && segment2.Equals(p.segment2)) ||
                   (segment1.Equals(p.segment2) && segment2.Equals(p.segment1)) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Parallel(" + segment1.ToString() + ", " + segment2.ToString() + "): " + justification;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CreateTransitiveParallel(Parallel parallel1, Parallel parallel2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // Did either of these congruences come from the other?
            // CTA: We don't need this anymore since use is restricted by class TransitiveSubstitution
            if (parallel1.HasRelationPredecessor(parallel2) || parallel2.HasRelationPredecessor(parallel1)) return newGrounded;

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel1);
            antecedent.Add(parallel2);

            //
            // Create the consequent clause
            //
            Segment shared = parallel1.SharedSegment(parallel2);
           
            AlgebraicParallel newAP = new AlgebraicParallel(parallel1.OtherSegment(shared), parallel2.OtherSegment(shared), "Transitivity");

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAP));

            return newGrounded;
        }
    }
}
