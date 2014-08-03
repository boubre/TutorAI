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

            thisAtomicRegion = new ShapeAtomicRegion(this);
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
        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
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
            return Arc.BetweenMinor(pt, new MinorArc(theCircle, endpoint1, middlePoint)) ||
                   Arc.BetweenMinor(pt, new MinorArc(theCircle, endpoint2, middlePoint));
        }

        public override bool PointLiesStrictlyOn(Point pt)
        {
            if (pt.StructurallyEquals(middlePoint)) return true;

            return Arc.StrictlyBetweenMinor(pt, new MinorArc(theCircle, endpoint1, middlePoint)) ||
                   Arc.StrictlyBetweenMinor(pt, new MinorArc(theCircle, endpoint2, middlePoint));
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

            // 'this' semicircle's included points are contained in the major arc point list
            // If thatSemi's middle is in the major arc point list, the two semicircles form arcs on the same side of the diameter
            if (this.arcMajorPoints.Contains(thatSemi.middlePoint)) return true;
            else return false;
        }

        public bool AngleIsInscribed(Angle angle)
        {
            if (!this.theCircle.IsInscribed(angle)) return false;

            // Verify that angle points match diameter endpoints
            Point endpt1, endpt2;

            endpt1 = angle.ray1.Point1.StructurallyEquals(angle.GetVertex()) ? angle.ray1.Point2 : angle.ray1.Point1;
            endpt2 = angle.ray2.Point1.StructurallyEquals(angle.GetVertex()) ? angle.ray2.Point2 : angle.ray2.Point1;

            if (!this.diameter.HasPoint(endpt1) || !this.diameter.HasPoint(endpt2)) return false;

            // Verify that the vertex is within the semicircle
            if (!this.arcMajorPoints.Contains(angle.GetVertex())) return false;

            return true;
        }

        public override Point Midpoint()
        {
            Point midpt = theCircle.Midpoint(endpoint1, endpoint2);

            if (!this.PointLiesOn(midpt)) midpt = theCircle.OppositePoint(midpt);

            return midpt;
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

        public override string CheapPrettyString()
        {
            return "Semicircle(" + endpoint1.SimpleToString() + middlePoint.SimpleToString() + endpoint2.SimpleToString() + ")";
        }
    }
}