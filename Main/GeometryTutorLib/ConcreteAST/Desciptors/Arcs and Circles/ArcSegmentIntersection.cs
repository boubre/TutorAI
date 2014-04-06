using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArcSegmentIntersection : ArcIntersection
    {
        public Segment segment { get; protected set; }
        private bool isTangent;

        public ArcSegmentIntersection(Point p, Arc a, Segment thatSegment) : base(p, a)
        {
            segment = thatSegment;
            isTangent = CalcTangency();

            // Find the intersection points
            Point pt1, pt2;
            arc.theCircle.Intersection(segment, out pt1, out pt2);
            intersection1 = pt1;
            intersection2 = pt2;
        }

        //
        // If the segment intersects at a single point on the circle AND the given radius (perpendicular) is in the middle of this arc.
        //
        private bool CalcTangency()
        {
            // Is the segment tangent to the circle?
            Segment radius = arc.theCircle.IsTangent(segment);

            if (radius == null) return false;

            //
            // Is the radius inside the minor arc? That is, is the radius applicable to this arc or not?
            //
            Point ptOnCircle = radius.OtherPoint(arc.theCircle.center);

            return Arc.BetweenMinor(ptOnCircle, arc);
        }

        //
        // If the segment intersects at a single point and does not pass through the arc.
        //
        public override bool IsTangent()
        {
            return isTangent;
        }

        //
        // Acquire the radii to the points of intersection
        //
        public void GetRadii(out Segment radius1, out Segment radius2)
        {
            radius1 = arc.theCircle.GetRadius(new Segment(arc.theCircle.center, intersection1));
            radius2 = arc.theCircle.GetRadius(new Segment(arc.theCircle.center, intersection1));

            // Make sure the first arg is non-null.
            if (radius1 == null && radius2 != null)
            {
                radius1 = radius2;
                radius2 = null;
            }
        }

        //
        // If the segment starts on this arc and extends outward.
        //
        public override bool StandsOn()
        {
            // Is one endpoint of the segment on the arc?
            return Arc.BetweenMinor(segment.Point1, arc) || Arc.BetweenMinor(segment.Point2, arc);
        }

        // If the segment / arc passes through this arc and extends outward.
        public override bool Crossing()
        {
            // Both endpoints are not on the arc.
            return !Arc.BetweenMinor(segment.Point1, arc) && !Arc.BetweenMinor(segment.Point2, arc);
        }

        public override bool StructurallyEquals(Object obj)
        {
            ArcSegmentIntersection inter = obj as ArcSegmentIntersection;
            if (inter == null) return false;
            return this.segment.StructurallyEquals(inter.segment) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.arc.StructurallyEquals(inter.arc);
        }

        public override bool Equals(Object obj)
        {
            ArcSegmentIntersection inter = obj as ArcSegmentIntersection;
            if (inter == null) return false;
            return this.segment.Equals(inter.segment) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.arc.StructurallyEquals(inter.arc);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "ArcSegmentIntersection(" + intersect.ToString() + ", " + arc.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}