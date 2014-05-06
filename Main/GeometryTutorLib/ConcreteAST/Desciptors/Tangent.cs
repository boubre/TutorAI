using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Tangent : Descriptor
    {
        public ArcIntersection intersection { get; protected set; }

        public Tangent(ArcIntersection that) : base()
        {
            if (!that.IsTangent())
            {
                throw new ArgumentException(that + " deduced tangent; it is NOT numerically.");
            }

            intersection = that;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.intersection.StructurallyEquals(tangent);
        }

        public override bool Equals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.intersection.Equals(tangent);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "Tangent(" + intersection.ToString() + ") " + justification;
        }
    }
}