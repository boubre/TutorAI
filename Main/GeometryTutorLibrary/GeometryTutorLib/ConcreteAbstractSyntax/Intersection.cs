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

        public Intersection() { }

        public Intersection(ConcretePoint i, ConcreteSegment l, ConcreteSegment r, string just)
        {
            intersect = i;
            lhs = l;
            rhs = r;
            justification = just;
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
    }
}