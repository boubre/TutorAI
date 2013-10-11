using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments where 2 are of equal length
    /// </summary>
    public class ConcreteIsoscelesTriangle : ConcreteTriangle
    {
        /// <summary>
        /// Create a new isosceles triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public ConcreteIsoscelesTriangle(ConcreteSegment a, ConcreteSegment b, ConcreteSegment c)
            : base(a, b, c)
        {
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteIsoscelesTriangle [right=");
            sb.Append(isRight);
            sb.Append(']');
            sb.AppendLine();
            SegmentA.BuildUnparse(sb, tabDepth + 1);
            SegmentB.BuildUnparse(sb, tabDepth + 1);
            SegmentC.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(GroundedClause obj)
        {
            ConcreteIsoscelesTriangle triangle = obj as ConcreteIsoscelesTriangle;
            if (triangle == null) return false;
            return base.Equals(obj);
        }
    }
}
