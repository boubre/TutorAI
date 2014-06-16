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

        //
        // Area-Related Computations
        //
        protected double Area(double radius)
        {
            return 0.5 * radius * radius * Math.PI;
        }
        protected double RationalArea(double radius)
        {
            return Area(radius) / Math.PI;
        }
        public override bool IsComputableArea() { return true; }
        public virtual bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Radius / Circle 
            return theCircle.CanAreaBeComputed(known);
        }
        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Radius / Circle
            double circArea = theCircle.GetArea(known);

            if (circArea <= 0) return -1;

            // The area is a proportion of the circle defined by the angle.
            return 0.5 * circArea;
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