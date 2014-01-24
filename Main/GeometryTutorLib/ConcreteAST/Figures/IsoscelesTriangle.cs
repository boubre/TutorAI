using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments where 2 are of equal length
    /// </summary>
    public class IsoscelesTriangle : Triangle
    {
        //
        // Although some of this information is redundant to what is stored in the superclass, it makes the information easily accessible
        //
        public Segment leg1 { get; private set; }
        public Segment leg2 { get; private set; }
        public Segment baseSegment { get; private set; }
        public Angle baseAngleOppositeLeg1 { get; private set; }
        public Angle baseAngleOppositeLeg2 { get; private set; }
        public Angle vertexAngle { get; private set; }

        private void DetermineIsoscelesValues()
        {
            Segment[] segments = new Segment[3];
            segments[0] = SegmentA;
            segments[1] = SegmentB;
            segments[2] = SegmentC;

            // Find the two congruent segments
            for (int i = 0; i < segments.Length; i++)
            {
                int otherSegment = i + 1 < segments.Length ? i + 1 : 0;
                if (Utilities.CompareValues(segments[i].Length, segments[otherSegment].Length))
                {
                    leg1 = segments[i];
                    leg2 = segments[otherSegment];

                    baseAngleOppositeLeg1 = GetOppositeAngle(leg1);
                    baseAngleOppositeLeg2 = GetOppositeAngle(leg2);
                    vertexAngle = OtherAngle(baseAngleOppositeLeg1, baseAngleOppositeLeg2);
                    baseSegment = GetOppositeSide(vertexAngle);

                    break;
                }
            }
        }

        /// <summary>
        /// Create a new isosceles triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public IsoscelesTriangle(Segment a, Segment b, Segment c, string just) : base(a, b, c, just)
        {
            if (!this.isEquilateral)
            {
                DetermineIsoscelesValues();
            }
            provenIsosceles = true;
        }

        public IsoscelesTriangle(Triangle tri, string just) : base(tri.SegmentA, tri.SegmentB, tri.SegmentC, just)
        {
            DetermineIsoscelesValues();
            provenIsosceles = true;
        }

        public Angle GetVertexAngle()
        {
            return vertexAngle;
        }

        new internal void BuildUnparse(StringBuilder sb, int tabDepth)
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

        public override bool Equals(Object obj)
        {
            IsoscelesTriangle triangle = obj as IsoscelesTriangle;
            if (triangle == null) return false;
            return base.Equals(obj);
        }

    }
}
