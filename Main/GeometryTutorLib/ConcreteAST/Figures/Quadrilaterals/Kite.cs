using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Kite : Quadrilateral
    {
        public Segment pairASegment1 { get; private set; }
        public Segment pairASegment2 { get; private set; }
        public Segment pairBSegment1 { get; private set; }
        public Segment pairBSegment2 { get; private set; }

        public Kite(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public Kite(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (Utilities.CompareValues(left.Length, top.Length) && Utilities.CompareValues(right.Length, bottom.Length))
            {
                pairASegment1 = left;
                pairASegment2 = top;

                pairBSegment1 = right;
                pairBSegment2 = bottom;
            }
            else if (Utilities.CompareValues(left.Length, bottom.Length) && Utilities.CompareValues(right.Length, top.Length))
            {
                pairASegment1 = left;
                pairASegment2 = bottom;

                pairBSegment1 = right;
                pairBSegment2 = top;
            }
            else
            {
                throw new ArgumentException("Quadrilateral does not define a kite; no two adjacent sides are equal lengths: " + this);
            }
        }

        public override bool StructurallyEquals(Object obj)
        {
            Kite thatKite = obj as Kite;
            if (thatKite == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Kite(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                             bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
