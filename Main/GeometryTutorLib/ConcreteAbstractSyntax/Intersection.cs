using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class Intersection : Descriptor
    {
        public ConcretePoint intersect { get; private set; }
        public ConcreteSegment lhs { get; private set; }
        public ConcreteSegment rhs { get; private set; }
        public bool isPerpendicular { get; private set; }

        public Intersection() : base() { }

        public Intersection(ConcretePoint i, ConcreteSegment l, ConcreteSegment r, string just) : base()
        {
            intersect = i;
            lhs = l;
            rhs = r;
            isPerpendicular = false;
            justification = just;
        }

        public Intersection(ConcretePoint i, ConcreteSegment l, ConcreteSegment r, bool bPerpendicular, string just)
            : base()
        {
            intersect = i;
            lhs = l;
            rhs = r;
            isPerpendicular = bPerpendicular;
            justification = just;
        }

        public void setPerpendicular(bool bPerpendicular)
        {
            isPerpendicular = bPerpendicular;
        }

        public override string ToString()
        {
            return "Intersect(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }

        public override bool Equals(Object obj)
        {
            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return intersect.Equals(inter.intersect) && lhs.Equals(inter.lhs) && rhs.Equals(inter.rhs);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}