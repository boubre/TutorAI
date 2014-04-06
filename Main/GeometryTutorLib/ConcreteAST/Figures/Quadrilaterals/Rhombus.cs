using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Rhombus : Parallelogram
    {
        public Rhombus(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public Rhombus(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(top.Length, left.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + left);
            }
            if (!Utilities.CompareValues(top.Length, right.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + right);
            }
            if (!Utilities.CompareValues(top.Length, bottom.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + bottom);
            }
        }

        public override bool StructurallyEquals(Object obj)
        {
            Rhombus thatRhom = obj as Rhombus;
            if (thatRhom == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            Rhombus thatRhom = obj as Rhombus;
            if (thatRhom == null) return false;

            if (thatRhom is Square) return false;

            return base.StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Rhombus(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
