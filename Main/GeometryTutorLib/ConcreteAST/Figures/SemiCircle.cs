using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public class Semicircle : Arc
    {
        public Segment diameter { get; private set; }
        public Point middlePoint { get; private set; }

        public Semicircle(Circle circle, Point e1, Point e2, Point m, Segment d) : this(circle, e1, e2, m, new List<Point>(), new List<Point>(), d) { }

        public Semicircle(Circle circle, Point e1, Point e2, Point m, List<Point> minorPts, List<Point> majorPts, Segment d)
            : base(circle, e1, e2, minorPts, majorPts)
        {
            diameter = d;
            middlePoint = m;
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

        public override bool PointLiesOn(Point pt)
        {
            return Arc.BetweenMinor(pt, this);
        }

        public override bool PointLiesStrictlyOn(Point pt)
        {
            return Arc.StrictlyBetweenMinor(pt, this);
        }

        public override bool HasSubArc(Arc that)
        {
            if (that is MajorArc) return false;

            return this.HasMinorSubArc(that);
        }

        public bool SameSideSemicircle(Semicircle thatSemi)
        {
            // First, the endpoints and the diameter must match
            if (!(this.diameter.StructurallyEquals(thatSemi.diameter) && base.StructurallyEquals(thatSemi))) return false;

            // 'this' semicircle's included points should be contained in the major arc point list
            // If thatSemi's middle point is also included in the major arc point list, the two semicircles form the same side
            if (this.arcMajorPoints.Contains(thatSemi.middlePoint)) return true;
            else return false;
        }

        /// <summary>
        /// Make a set of connections for atomic region analysis.
        /// </summary>
        /// <returns></returns>
        public override List<Connection> MakeAtomicConnections()
        {
            List<Segment> segments = this.Segmentize();
            List<Connection> connections = new List<Connection>();

            foreach (Segment approxSide in segments)
            {
                connections.Add(new Connection(approxSide.Point1, approxSide.Point2, ConnectionType.ARC,
                                               new MinorArc(this.theCircle, approxSide.Point1, approxSide.Point2)));
            }

            connections.Add(new Connection(diameter.Point1, diameter.Point2, ConnectionType.SEGMENT, this));

            return connections;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Semicircle semi = obj as Semicircle;
            if (semi == null) return false;

            return this.diameter.StructurallyEquals(semi.diameter) && this.middlePoint.StructurallyEquals(semi.middlePoint) && base.StructurallyEquals(obj);
        }

        public override bool Equals(object obj)
        {
            Semicircle semi = obj as Semicircle;
            if (semi == null) return false;

            return this.diameter.Equals(semi.diameter) && this.middlePoint.Equals(semi.middlePoint) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "SemiCircle(" + theCircle + "(" + endpoint1.ToString() + ", " + middlePoint.ToString() + ", " + endpoint2.ToString() + "), Diameter(" + diameter + "))";
        }
    }
}