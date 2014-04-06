using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class IsoscelesTrapezoid : Trapezoid
    {
        public IsoscelesTrapezoid(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public IsoscelesTrapezoid(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(leftLeg.Length, rightLeg.Length))
            {
                throw new ArgumentException("Trapezoid does not define an isosceles trapezoid; sides are not equal length: " + this);
            }
        }

        public override bool StructurallyEquals(Object obj)
        {
            IsoscelesTrapezoid thatTrap = obj as IsoscelesTrapezoid;
            if (thatTrap == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "IsoscelesTrapezoid(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                           bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
