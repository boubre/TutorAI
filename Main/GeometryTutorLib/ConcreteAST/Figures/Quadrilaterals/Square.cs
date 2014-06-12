using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Square : Rhombus
    {
        public Square(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public Square(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(topLeftAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + topLeftAngle);
            }
            if (!Utilities.CompareValues(topRightAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + topRightAngle);
            }

            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + bottomLeftAngle);
            }

            if (!Utilities.CompareValues(bottomRightAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + bottomRightAngle);
            }
        }

        public override bool StructurallyEquals(Object obj)
        {
            Square thatSquare = obj as Square;
            if (thatSquare == null) return false;

            //return base.StructurallyEquals(obj);
            return this.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Square(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                               bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }

    }
}
