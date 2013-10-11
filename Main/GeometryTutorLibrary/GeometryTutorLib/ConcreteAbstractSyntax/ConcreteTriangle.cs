using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public class ConcreteTriangle : GroundedClause
    {
        public ConcreteSegment SegmentA { get; private set; }
        public ConcreteSegment SegmentB { get; private set; }
        public ConcreteSegment SegmentC { get; private set; }
        public bool isRight { get; private set; }

        /// <summary>
        /// Create a new triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public ConcreteTriangle(ConcreteSegment a, ConcreteSegment b, ConcreteSegment c)
        {
            SegmentA = a;
            SegmentB = b;
            SegmentC = c;
            isRight = isRightTriangle();
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteTriangle [right=");
            sb.Append(isRight);
            sb.Append(']');
            sb.AppendLine();
            SegmentA.BuildUnparse(sb, tabDepth + 1);
            SegmentB.BuildUnparse(sb, tabDepth + 1);
            SegmentC.BuildUnparse(sb, tabDepth + 1);
        }

        /// <summary>
        /// Determines if this is a right traingle.
        /// </summary>
        /// <returns>TRUE if this is a right triangle.</returns>
        private bool isRightTriangle()
        {
            bool right = false;
            ConcreteSegment[] segments = new ConcreteSegment[3];
            segments[0] = SegmentA;
            segments[1] = SegmentB;
            segments[2] = SegmentC;

            //Compare vector representations of lines to see if dot product is 0.
            for (int i = 0; i < 3; i++)
            {
                int j = (i + 1) % 3;
                double v1x = segments[i].Point1.X - segments[i].Point2.X;
                double v1y = segments[i].Point1.Y - segments[i].Point2.Y;
                double v2x = segments[j].Point1.X - segments[j].Point2.X;
                double v2y = segments[j].Point1.Y - segments[j].Point2.Y;
                right = right || (v1x * v2x + v1y * v2y) == 0;
            }
            return right;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            ConcreteTriangle triangle = obj as ConcreteTriangle;
            if (triangle == null) return false;
            return triangle.SegmentA.Equals(SegmentA) && triangle.SegmentB.Equals(SegmentB) && triangle.SegmentC.Equals(SegmentC);
        }
    }
}
