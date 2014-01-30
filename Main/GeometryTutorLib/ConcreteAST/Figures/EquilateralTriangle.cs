using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class EquilateralTriangle : IsoscelesTriangle
    {
        /// <summary>
        /// Create a new equilateral triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public EquilateralTriangle(Segment a, Segment b, Segment c, string just) : base(a, b, c)
        {
            provenIsosceles = true;
            provenEquilateral = true;
        }
        public EquilateralTriangle(Triangle t) : base(t.SegmentA, t.SegmentA, t.SegmentA)
        {
            provenIsosceles = true;
            provenEquilateral = true;
        }

        new internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteEquilateralTriangle");
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

        public override bool Equals(Object obj)
        {
            EquilateralTriangle triangle = obj as EquilateralTriangle;
            if (triangle == null) return false;
            return base.Equals(obj);
        }
    }
}
