using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Rectangle : Parallelogram
    {
        public Rectangle(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public Rectangle(Segment a, Segment b, Segment c, Segment d) : base(a, b, c, d)
        {
            foreach (Angle angle in angles)
            {
                if (!Utilities.CompareValues(angle.measure, 90))
                {
                    throw new ArgumentException("Quadrilateral is not a Rectangle; angle does not measure 90^o: " + angle);
                }
            }
        }

        public override bool StructurallyEquals(Object obj)
        {
            Rectangle thatRect = obj as Rectangle;
            if (thatRect == null) return false;

            //return base.StructurallyEquals(obj);
            return base.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Rectangle(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                  bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
