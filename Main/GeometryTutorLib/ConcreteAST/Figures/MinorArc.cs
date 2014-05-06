using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class MinorArc : Arc
    {
        public MinorArc(Circle circle, Point e1, Point e2) : this(circle, e1, e2, new List<Point>(), new List<Point>()) { }

        public MinorArc(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts) : base(circle, e1, e2, minorPts, majorPts) { }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            MinorArc arc = obj as MinorArc;
            if (arc == null) return false;

            return this.theCircle.StructurallyEquals(arc.theCircle) && this.endpoint1.StructurallyEquals(arc.endpoint1)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint2);
        }

        public override bool Equals(Object obj)
        {
            MinorArc arc = obj as MinorArc;
            if (arc == null) return false;

            // Check equality of arc minor / major points?

            return this.theCircle.Equals(arc.theCircle) && this.endpoint1.Equals(arc.endpoint1)
                                                        && this.endpoint2.Equals(arc.endpoint2);
        }

        public override string ToString() { return "MinorArc(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "))"; }
    }
}