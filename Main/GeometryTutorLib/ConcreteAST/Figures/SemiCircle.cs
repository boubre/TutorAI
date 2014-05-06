using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Semicircle : Arc
    {
        public Segment diameter { get; private set; }

        public Semicircle(Circle circle, Point e1, Point e2, Segment d) : this(circle, e1, e2, new List<Point>(), new List<Point>(), d) { }

        public Semicircle(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts, Segment d)
            : base(circle, e1, e2, minorPts, majorPts)
        {
            diameter = d;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Semicircle semi = obj as Semicircle;
            if (semi == null) return false;

            return this.diameter.StructurallyEquals(semi.diameter) && base.StructurallyEquals(obj);
        }

        public override bool Equals(object obj)
        {
            Semicircle semi = obj as Semicircle;
            if (semi == null) return false;

            return this.diameter.Equals(semi.diameter) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "SemiCircle(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "), Diameter(" + diameter + "))";
        }
    }
}