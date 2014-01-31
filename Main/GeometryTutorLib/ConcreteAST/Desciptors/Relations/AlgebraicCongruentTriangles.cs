using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicCongruentTriangles : CongruentTriangles
    {
        public AlgebraicCongruentTriangles(Triangle t1, Triangle t2) : base(t1, t2) { }

        public new void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString() { return "AlgebraicCongruent(" + ct1.ToString() + ", " + ct2.ToString() + ") " + justification; }
    }
}
