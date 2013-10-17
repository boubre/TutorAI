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
        public ConcreteCongruentAngles(ConcreteAngle a1, ConcreteAngle a2, string just)
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

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public bool LinksTriangles(ConcreteTriangle ct1, ConcreteTriangle ct2)
        {
            return ct1.HasAngle(ca1) && ct2.HasAngle(ca2) || ct1.HasAngle(ca2) && ct2.HasAngle(ca1);
        }

        public override string ToString()
        {
            return "Congruent(" + ca1.ToString() + ", " + ca2.ToString() + "): " + justification;
        }
    }
}
