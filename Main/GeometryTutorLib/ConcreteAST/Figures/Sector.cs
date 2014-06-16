﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Sector : Figure
    {
        public Arc theArc { get; protected set; }

        public Sector(Arc a)
        {
            theArc = a;
        }

        public override bool PointLiesInOrOn(Point pt)
        {
            // Radii
            KeyValuePair<Segment, Segment> radii = theArc.GetRadii();
            if (radii.Key.PointIsOnAndBetweenEndpoints(pt) || radii.Value.PointIsOnAndBetweenEndpoints(pt)) return true;

            // Interior
            if (this.PointLiesInside(pt)) return true;

            // Arc
            if (theArc is MajorArc) return Arc.BetweenMajor(pt, theArc as MajorArc);
            else if (theArc is MinorArc) return Arc.BetweenMinor(pt, theArc as MinorArc);

            return false;
        }
        public override List<Point> GetApproximatingPoints() { return theArc.GetApproximatingPoints(); }

        public override Polygon GetPolygonalized()
        {
            if (polygonalized != null) return polygonalized;

            List<Segment> sides = new List<Segment>(theArc.GetApproximatingSegments());

            sides.Add(new Segment(theArc.theCircle.center, theArc.endpoint1));
            sides.Add(new Segment(theArc.theCircle.center, theArc.endpoint2));

            // Make a polygon out of the radii and the sector
            polygonalized = Polygon.MakePolygon(sides);

            return polygonalized;
        }

        public override List<Segment> Segmentize()
        {
            List<Segment> segments = new List<Segment>();

            // Add radii
            segments.Add(new Segment(theArc.theCircle.center, theArc.endpoint1));
            segments.Add(new Segment(theArc.theCircle.center, theArc.endpoint2));

            // Segmentize the arc
            segments.AddRange(theArc.Segmentize());

            return segments;
        }

        private bool AreClockwise(Point v1, Point v2) { return -v1.X * v2.Y + v1.Y * v2.X > 0; }

        //
        // Point must be in the given circle and then, specifically in the specified angle
        //
        public override bool PointLiesInside(Point pt)
        {
            // Is the point in the sector's circle?
            if (!theArc.theCircle.PointLiesInside(pt)) return false;

            Point relPoint = Point.MakeVector(theArc.theCircle.center, pt);

            if (theArc is MinorArc)
            {
                return !AreClockwise(theArc.endpoint1, relPoint) && AreClockwise(theArc.endpoint2, relPoint);
            }

            // Negation of MinorArc
            if (theArc is MajorArc)
            {
                return !AreClockwise(theArc.endpoint1, relPoint) && !AreClockwise(theArc.endpoint2, relPoint);
            }

            if (new Segment(theArc.theCircle.center, theArc.endpoint1).PointIsOnAndBetweenEndpoints(pt)) return false;
            if (new Segment(theArc.theCircle.center, theArc.endpoint2).PointIsOnAndBetweenEndpoints(pt)) return false;

            return false;
        }

        //
        // Area-Related Computations
        //
        protected double Area(double radAngleMeasure, double radius)
        {
            return 0.5 * radius * radius * radAngleMeasure;
        }
        protected double RationalArea(double radAngleMeasure, double radius)
        {
            return Area(radAngleMeasure, radius) / Math.PI;
        }
        public override bool IsComputableArea() { return true; }
        public virtual bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Central Angle
            if (known.GetAngleMeasure(this.theArc.GetCentralAngle()) < 0) return false;

            // Radius / Circle 
            return theArc.theCircle.CanAreaBeComputed(known);
        }
        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Central Angle; this is minor arc measure by default
            double angleMeasure = Angle.toRadians(known.GetAngleMeasure(this.theArc.GetCentralAngle()));

            if (angleMeasure <= 0) return -1;

            // Make a major arc measure, if needed.
            if (theArc is MajorArc) angleMeasure = 2 * Math.PI - angleMeasure;

            // Radius / Circle
            double circArea = theArc.theCircle.GetArea(known);

            if (circArea <= 0) return -1;

            // The area is a proportion of the circle defined by the angle.
            return (angleMeasure / (2 * Math.PI)) * circArea;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.StructurallyEquals(sector.theArc);
        }

        public override bool Equals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.Equals(sector.theArc);
        }

        public override string ToString() { return "Sector(" + theArc + ")"; }
    }
}