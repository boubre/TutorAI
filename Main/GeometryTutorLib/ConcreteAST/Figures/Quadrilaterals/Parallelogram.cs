using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Parallelogram : Quadrilateral
    {
        public Parallelogram(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public Parallelogram(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (!left.IsParallelWith(right))
            {
                throw new ArgumentException("Given opposing segments are not parallel: " + left + " " + right);
            }

            if (!top.IsParallelWith(bottom))
            {
                throw new ArgumentException("Given opposing segments are not parallel: " + top + " " + bottom);
            }
        }

        public bool IsStrictParallelogram()
        {
            if (this is Rhombus) return false;

            return true;
        }

        public override bool StructurallyEquals(Object obj)
        {
            //Parallelogram thatPara = obj as Parallelogram;
            //if (thatPara == null) return false;
            
            //Fix to prevent a false positive when obj is of a derived type:
            if (!Object.ReferenceEquals(this.GetType(), obj.GetType())) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            Parallelogram thatPara = obj as Parallelogram;
            if (thatPara == null) return false;

            if (thatPara is Rhombus) return false;

            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Parallelogram(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                      bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
