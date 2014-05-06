using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArcArcBisector : Bisector
    {
        public ArcArcIntersection bisected { get; private set; }

        public ArcArcBisector(ArcArcIntersection b) : base()
        {
            bisected = b;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ArcArcBisector ab = obj as ArcArcBisector;
            if (ab == null) return false;
            return bisected.StructurallyEquals(ab.bisected);
        }

        public override bool Equals(Object obj)
        {
            ArcArcBisector ab = obj as ArcArcBisector;
            if (ab == null) return false;
            return bisected.Equals(ab.bisected);
        }

        public override string ToString()
        {
            return "ArcArcBisector(" + bisected.ToString() + ") at " + bisected.intersect + ")";
        }
    }
}
