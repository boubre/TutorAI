using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A segment defined by two points.
    /// </summary>
    public class ConcreteSegment : ConcreteFigure
    {
        public ConcretePoint Point1 { get; private set; }
        public ConcretePoint Point2 { get; private set; }
        public double Length { get; private set; }
        public double Slope { get; private set; }

        /// <summary>
        /// Create a new ConcreteSegment. 
        /// </summary>
        /// <param name="p1">A point defining the segment.</param>
        /// <param name="p2">Another point defining the segment.</param>
        public ConcreteSegment(ConcretePoint p1, ConcretePoint p2) : base()
        {
            Point1 = p1;
            Point2 = p2;
            Length = ConcretePoint.calcDistance(p1, p2);
            Slope = (p2.Y - p1.Y) / (p2.X - p1.X);

            Point1.getSuperFigures().Add(this);
            Point2.getSuperFigures().Add(this);
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteSegment [l=");
            sb.Append(Length);
            sb.Append(']');
            sb.AppendLine();
            Point1.BuildUnparse(sb, tabDepth + 1);
            Point2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            ConcreteSegment segment = obj as ConcreteSegment;
            if (segment == null) return false;
            return (segment.Point1.Equals(Point1) && segment.Point2.Equals(Point2)) ||
                   (segment.Point1.Equals(Point2) && segment.Point2.Equals(Point1));
        }

        public ConcretePoint SharedVertex(ConcreteSegment s)
        {
            if (Point1.Equals(s.Point1)) return Point1;
            if (Point1.Equals(s.Point2)) return Point1;
            if (Point2.Equals(s.Point1)) return Point2;
            if (Point2.Equals(s.Point2)) return Point2;
            return null;
        }

        public ConcretePoint OtherPoint(ConcretePoint p)
        {
            if (p.Equals(Point1)) return Point2;
            if (p.Equals(Point2)) return Point1;

            return null;
        }

        public bool HasPoint(ConcretePoint p)
        {
            return Point1.Equals(p) || Point2.Equals(p);
        }

        public override bool Contains(GroundedClause target)
        {
            return this.Equals(target);
        }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            ConcreteSegment other = (ConcreteSegment)(this.MemberwiseClone());
            other.Point1 = (ConcretePoint)Point1.DeepCopy();
            other.Point2 = (ConcretePoint)Point2.DeepCopy();

            return other;
        }

        //
        // Each segment is congruent to itself
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause gc)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            ConcreteSegment segment = gc as ConcreteSegment;

            if (segment == null) return newGrounded;

            ConcreteCongruentSegments ccss = new ConcreteCongruentSegments(segment, segment, "Reflexive");

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(segment);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, ccss));

            return newGrounded;
        }

        public override string ToString() { return "Segment(" + Point1.ToString() + ", " + Point2.ToString() + ")"; }
    }
}
