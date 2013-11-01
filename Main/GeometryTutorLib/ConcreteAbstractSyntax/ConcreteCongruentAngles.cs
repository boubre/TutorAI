using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ConcreteCongruentAngles : ConcreteCongruent
    {
        public ConcreteAngle ca1 { get; private set; }
        public ConcreteAngle ca2 { get; private set; }

        public ConcreteAngle GetFirstAngle() { return ca1; }
        public ConcreteAngle GetSecondAngle() { return ca2; }

        //
        // Deduced Node Information
        //
        private List<ConcreteCongruentSegments> facts; // CTA: needed?
        public ConcreteCongruentAngles(ConcreteAngle a1, ConcreteAngle a2, string just) : base()
        {
            ca1 = a1;
            ca2 = a2;
            justification = just;
        }

        public override bool Equals(Object c)
        {
            if (!(c is ConcreteCongruentAngles)) return false;

            ConcreteCongruentAngles cca = (ConcreteCongruentAngles)c;

            return ca1.Equals(cca.ca1) && ca2.Equals(cca.ca2) || ca1.Equals(cca.ca2) && ca2.Equals(cca.ca1);
        }

        public override bool IsReflexive()
        {
            return ca1.Equals(ca2);
        }

        // Return the number of shared angles in both congruences
        public override int SharesNumClauses(ConcreteCongruent thatCC)
        {
            ConcreteCongruentAngles ccas = thatCC as ConcreteCongruentAngles;

            if (ccas == null) return 0;

            int numShared = ca1.Equals(ccas.ca1) || ca1.Equals(ccas.ca2) ? 1 : 0;
            numShared += ca2.Equals(ccas.ca1) || ca2.Equals(ccas.ca1) ? 1 : 0;

            return numShared;
        }

        // Return the shared angle in both congruences
        public ConcreteAngle SegmentShared(ConcreteCongruentAngles thatCC)
        {
            if (SharesNumClauses(thatCC) != 1) return null;

            return ca1.Equals(thatCC.ca1) || ca1.Equals(thatCC.ca2) ? ca1 : ca2;
        }

        // Given one of the angles in the pair, return the other
        public ConcreteAngle OtherAngle(ConcreteAngle cs)
        {
            if (cs.Equals(ca1)) return ca2;
            if (cs.Equals(ca2)) return ca1;
            return null;
        }

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public bool LinksTriangles(ConcreteTriangle ct1, ConcreteTriangle ct2)
        {
            return ct1.HasAngle(ca1) && ct2.HasAngle(ca2) || ct1.HasAngle(ca2) && ct2.HasAngle(ca1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Congruent(" + ca1.ToString() + ", " + ca2.ToString() + "): " + justification;
        }
    }
}
