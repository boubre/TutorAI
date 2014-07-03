﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public enum ConnectionType { SEGMENT, ARC }

    //
    // Aggregation class for each segment of an atomic region.
    //
    public class Connection
    {
        public Point endpoint1;
        public Point endpoint2;
        public ConnectionType type;

        // The shape which has this connection. 
        public Figure segmentOrArc;

        public Connection(Point e1, Point e2, ConnectionType t, Figure so)
        {
            endpoint1 = e1;
            endpoint2 = e2;
            type = t;
            segmentOrArc = so;
        }

        public bool HasPoint(Point p) { return endpoint1.Equals(p) || endpoint2.Equals(p); }

        public Point OtherEndpoint(Point p)
        {
            if (endpoint1.StructurallyEquals(p)) return endpoint2;
            if (endpoint2.StructurallyEquals(p)) return endpoint1;
            return null;
        }

        public override string ToString()
        {
            return "< " + endpoint1.name + ", " + endpoint2.name + "(" + type + ") >";
        }

        public bool StructurallyEquals(Connection that)
        {
            if (!this.HasPoint(that.endpoint1) || !this.HasPoint(that.endpoint2)) return false;

            if (type != that.type) return false;

            return segmentOrArc.StructurallyEquals(that.segmentOrArc);
        }

        //
        // Create an approximation of the arc by using a set number of arcs.
        //
        public List<Segment> Segmentize()
        {
            if (this.type == ConnectionType.SEGMENT) return Utilities.MakeList<Segment>(new Segment(this.endpoint1, this.endpoint2));

            return segmentOrArc.Segmentize();
        }

        public bool PointLiesOn(Point pt)
        {
            if (this.type == ConnectionType.SEGMENT)
            {
                return Segment.Between(pt, this.endpoint1, this.endpoint2);
            }
            else if (this.type == ConnectionType.ARC)
            {
                return (segmentOrArc as Arc).PointLiesOn(pt);
            }

            return false;
        }

        //
        // Find the intersection points between this conenction and that; 2 points may result. (2 with arc / segment)
        //
        public void FindIntersection(List<Point> figurePoints, Connection that, out Point pt1, out Point pt2)
        {
            if (that.type == ConnectionType.ARC)
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Arc, out pt1, out pt2);
            }
            else
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Segment, out pt1, out pt2);
            }

            Segment thatSeg = that.segmentOrArc as Segment;
            Arc thatArc = that.segmentOrArc as Arc;

            Segment thisSeg = this.segmentOrArc as Segment;
            Arc thisArc = this.segmentOrArc as Arc;

            //
            // Normalize the points to the points in the drawing.
            //
            if (thisSeg != null && thatSeg != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisSeg, thatSeg);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisSeg, thatSeg);
            }
            else if (thisSeg != null && thatArc != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisSeg, thatArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisSeg, thatArc);
            }
            else if (thisArc != null && thatSeg != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thatSeg, thisArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thatSeg, thisArc);
            }
            else if (thisArc != null && thatArc != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisArc, thatArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisArc, thatArc);
            }
        }
        public void FindIntersection(Connection that, out Point pt1, out Point pt2)
        {
            if (that.type == ConnectionType.ARC)
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Arc, out pt1, out pt2);
            }
            else
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Segment, out pt1, out pt2);
            }
        }

        //
        // If that is a segment which is smaller and is a subsegment of this.
        // If that is an arc which is smaller and is a subsrc of this.
        //
        public bool IsSubConnection(Connection that)
        {
            if (that.type != this.type) return false;

            if (this.type == ConnectionType.ARC)
            {
                return (this.segmentOrArc as Arc).HasSubArc(that.segmentOrArc as Arc);
            }
            else if (this.type == ConnectionType.SEGMENT)
            {
                return (this.segmentOrArc as Segment).HasSubSegment(that.segmentOrArc as Segment);
            }

            return false;
        }

        //
        // If one of the endpoints of that is inside of this; and vice versa.
        //
        public bool Overlap(Connection that)
        {
            if (that.type != this.type) return false;

            if (this.type == ConnectionType.ARC)
            {
                return (this.segmentOrArc as Arc).PointLiesOn(this.endpoint1) ||
                       (this.segmentOrArc as Arc).PointLiesOn(this.endpoint2);
            }
            else if (this.type == ConnectionType.SEGMENT)
            {
                return Segment.Between(that.endpoint1, this.endpoint1, this.endpoint2) ||
                       Segment.Between(that.endpoint2, this.endpoint1, this.endpoint2);
            }

            return false;
        }

        //
        // Does the segment or arc stand on the segment or arg? That is, the intersection point lies on the end of this or that?
        //
        public bool StandsOnNotEndpoint(Connection that)
        {
            if (StandsOnEndpoint(that)) return false;

            Point pt1 = null;
            Point pt2 = null;

            this.FindIntersection(that, out pt1, out pt2);

            if (pt2 != null) return false;

            if (this.HasPoint(pt1) && that.PointLiesOn(pt1)) return true;
            if (that.HasPoint(pt1) && this.PointLiesOn(pt1)) return true;

            return false;
        }

        public bool StandsOnEndpoint(Connection that)
        {
            Point pt1 = null;
            Point pt2 = null;

            this.FindIntersection(that, out pt1, out pt2);

            if (pt2 != null) return false;

            return this.HasPoint(pt1) && that.HasPoint(pt1);
        }

        //
        // Intersects at a single point (not at the endpoint of either connection).
        //
        public bool Crosses(Connection that)
        {
            Point pt1 = null;
            Point pt2 = null;

            this.FindIntersection(that, out pt1, out pt2);

            if (pt2 != null) return false;

            if (this.HasPoint(pt1)) return false;
            if (that.HasPoint(pt1)) return false;

            return true;
        }

        public bool DefinesArcSegmentRegion(Connection that)
        {
            if (this.type == ConnectionType.ARC && that.type == ConnectionType.ARC) return false;
            if (this.type == ConnectionType.SEGMENT && that.type == ConnectionType.SEGMENT) return false;

            // Endpoints align.
            return this.HasPoint(that.endpoint1) && this.HasPoint(that.endpoint2);
        }
    }
}