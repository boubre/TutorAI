using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ConcreteCongruentSegments : ConcreteCongruent
    {
        public ConcreteSegment cs1 { get; private set; }
        public ConcreteSegment cs2 { get; private set; }

        public ConcreteCongruentSegments(ConcreteSegment s1, ConcreteSegment s2, string just) : base()
        {
            cs1 = s1;
            cs2 = s2;
            justification = just;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public Boolean HasSegment(ConcreteSegment cs)
        {
            return cs1.Equals(cs) || cs2.Equals(cs);
        }

        public ConcreteSegment GetFirstSegment() { return cs1; }
        public ConcreteSegment GetSecondSegment() { return cs2; }

        public bool IsReflexive()
        {
            return cs1.Equals(cs2);
        }

        public bool LinksTriangles(ConcreteTriangle ct1, ConcreteTriangle ct2)
        {
            return ct1.HasSegment(cs1) && ct2.HasSegment(cs2) || ct1.HasSegment(cs2) && ct2.HasSegment(cs1);
        }

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public bool SharedVertex(ConcreteCongruentSegments ccs)
        {
            return ccs.cs1.SharedVertex(cs1) != null || ccs.cs1.SharedVertex(cs2) != null ||
                   ccs.cs2.SharedVertex(cs1) != null || ccs.cs2.SharedVertex(cs2) != null;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is ConcreteCongruentSegments)) return false;

            ConcreteCongruentSegments ccs = (ConcreteCongruentSegments)obj;

            return (cs1.Equals(ccs.cs1) && cs2.Equals(ccs.cs2)) || (cs1.Equals(ccs.cs2) && cs2.Equals(ccs.cs1));
        }

        public override string ToString()
        {
            return "Congruent(" + cs1.ToString() + ", " + cs2.ToString() + "): " + justification;
        }
    }
}
