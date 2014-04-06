using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A circle is defined by a center and radius.
    /// </summary>
    public class Circle : Figure
    {
        public Point center { get; private set; }
        public double radius { get; private set; }

        // We define a secant to be a segment that passes through a circle (and contains a chord)
        // < Original secant segment, chord >
        public Dictionary<Segment, Segment> secants { get; private set; }

        // We define a chord to strictly be a segment that has BOTH endpoints on the circle (and does not extend).
        public List<Segment> chords { get; private set; }

        // Any radii defined by the figure.
        public List<Segment> radii { get; private set; }

        // A diameter is a special chord that passes through the center of the circle.
        public List<Segment> diameters { get; private set; }

        // Tangents intersect the circle at one point; the pair is <tangent, radius> where radius creates the 90^o angle.
        public Dictionary<Segment, Segment> tangents { get; private set; }

        // Triangles that are circumscribed about the circle. 
        public List<Triangle> circumscribedTris { get; private set; }

        // Triangles that are insrcibed in the circle. 
        public List<Triangle> inscribedTris { get; private set; }

        // Quadrilaterals that are circumscribed about the circle. 
        public List<Quadrilateral> circumscribedQuads { get; private set; }

        // Quadrilaterals that are insrcibed in the circle. 
        public List<Quadrilateral> inscribedQuads { get; private set; }

        /// <summary>
        /// Create a new ConcreteSegment. 
        /// </summary>
        /// <param name="p1">A point defining the segment.</param>
        /// <param name="p2">Another point defining the segment.</param>
        public Circle(Point center, double r) : base()
        {
            this.center = center;
            radius = r;

            secants = new Dictionary<Segment, Segment>();
            chords = new List<Segment>();
            radii = new List<Segment>();
            diameters = new List<Segment>();
            tangents = new Dictionary<Segment, Segment>();

            inscribedTris = new List<Triangle>();
            circumscribedTris = new List<Triangle>();

            inscribedQuads = new List<Quadrilateral>();
            circumscribedQuads = new List<Quadrilateral>();

            Utilities.AddUniqueStructurally(this.center.getSuperFigures(), this);
        }

        public Segment GetRadius(Segment r)
        {
            int index = radii.IndexOf(r);
            return index == -1 ? null : radii[index];
        }

        //
        // For each triangle, it is inscribed in the circle? Is it circumscribed.
        //
        public void AnalyzePolygons()
        {
            foreach (Triangle tri in Triangle.figureTriangles)
            {
                List<Segment> triSegments = tri.GetSegments();

                if (PolygonCircumscribesCircle(triSegments)) circumscribedTris.Add(tri);
                else if (CircleCircumscribesPolygon(triSegments)) inscribedTris.Add(tri);
            }

            foreach (Quadrilateral quad in Quadrilateral.figureQuadrilaterals)
            {
                if (PolygonCircumscribesCircle(quad.segments)) circumscribedQuads.Add(quad);
                else if (CircleCircumscribesPolygon(quad.segments)) inscribedQuads.Add(quad);
            }
        }

        //
        // The input polygon is in the form of segments (so it can be used by a polygon)
        //
        public bool PolygonCircumscribesCircle(List<Segment> segments)
        {
            // All of the sides of the polygon must be tangent to the circle to be circumscribed about the circle.
            foreach (Segment segment in segments)
            {
                if (this.IsTangent(segment) == null) return false;
            }

            return true;
        }

        //
        // The input polygon is in the form of segments (so it can be used by a polygon)
        //
        public bool CircleCircumscribesPolygon(List<Segment> segments)
        {
            // All of the vertices of the polygon must be on the circle to be inscribed in the circle.
            // That is, all of the segments must be chords.
            foreach (Segment segment in segments)
            {
                if (!this.IsChord(segment)) return false;
            }

            return true;
        }

        //
        // Determine all applicable secants, tangent, and chords for this circle
        //
        public void AnalyzeSegments()
        {
            foreach (Segment segment in Segment.figureSegments)
            {
                Segment tangentRadius = IsTangent(segment);
                if (tangentRadius != null) tangents.Add(segment, tangentRadius);

                else if (IsChord(segment)) Utilities.AddUnique<Segment>(chords, segment);
                else
                {
                    Segment chord;
                    if (IsSecant(segment, out chord))
                    {
                        // Add to the secants for this circle.
                        secants.Add(segment, chord);

                        // Also add to the chord list.
                        Utilities.AddUnique<Segment>(chords, chord);
                    }
                }

                // Is a radius the result of a segment starting at the center and extending outward?
                // We collect all other types below.
                Segment radius = IsRadius(segment);
                if (radius != null) Utilities.AddUnique<Segment>(radii, radius);
            }

            // Now that we have all the chords for this triangle, which are diameters?
            foreach (Segment chord in chords)
            {
                // The center needs to be the midpoint, but verifying the center is on the chord suffices in this context.
                if (chord.PointIsOnAndExactlyBetweenEndpoints(this.center))
                {
                    // Add to diameters....
                    Utilities.AddUnique<Segment>(diameters, chord);

                    // but also collect radii
                    Utilities.AddUnique<Segment>(radii, Segment.GetFigureSegment(this.center, chord.Point1));
                    Utilities.AddUnique<Segment>(radii, Segment.GetFigureSegment(this.center, chord.Point2));
                }
            }
        }

        //
        // Determine tangency of the given segment.
        // Indicate tangency by returning the segment which creates the 90^0 angle.
        //
        public Segment IsTangent(Segment segment)
        {
            // Acquire the line perpendicular to the segment that passes through the center of the circle.
            Segment perpendicular = segment.GetPerpendicular(this.center);

            // Is this perpendicular segment a radius? Check length
            if (!Utilities.CompareValues(perpendicular.Length, this.radius)) return null;

            //
            // The intersection between the perpendicular and the segment must be within the endpoints of the segment.
            //
            Point intersection = segment.FindIntersection(perpendicular);

            return segment.PointIsOnAndBetweenEndpoints(intersection) ? perpendicular : null;
        }

        //
        // Does the given segment pass through the circle so that it acts as a diameter (or contains a diameter)?
        //
        private bool ContainsDiameter(Segment segment)
        {
            if (!segment.PointIsOnAndBetweenEndpoints(this.center)) return false;

            // the endpoints of the segment must be on or outside the circle.
            double distance = Point.calcDistance(this.center, segment.Point1);
            if (distance < this.radius) return false;

            distance = Point.calcDistance(this.center, segment.Point2);
            if (distance < this.radius) return false;

            return true;
        }


        //
        // Given the secant, there is a midpoint along the secant (wrt to the circle), given the distance,
        // find the two points of intersection between the secant and the circle.
        // Return the resultant chord segment.
        //
        private Segment ConstructChord(Segment secantSegment, Point midpt, double distance)
        {
            //                distance
            //      circPt1    _____   circPt2
            //
            // Find the exact coordinates of the two 'circ' points.
            //
            double deltaX = 0;
            double deltaY = 0;
            if (secantSegment.IsVertical())
            {
                deltaX = 0;
                deltaY = distance;
            }
            else if (secantSegment.IsHorizontal())
            {
                deltaX = distance;
                deltaY = 0;
            }
            else
            {
                deltaX = Math.Sqrt(Math.Pow(distance, 2) / (1 + Math.Pow(secantSegment.Slope, 2)));
                deltaY = secantSegment.Slope * deltaX;
            }
            Point circPt1 = Point.GetFigurePoint(new Point("", midpt.X + deltaX, midpt.Y + deltaY));

            // intersection is the midpoint of circPt1 and pt2.
            Point circPt2 = Point.GetFigurePoint(new Point("", 2 * midpt.X - circPt1.X, 2 * midpt.Y - circPt1.Y));

            // Create the actual chord
            return Segment.GetFigureSegment(circPt1, circPt2);
        }

        //
        // Determine if the segment passes through the circle (we know it is not a chord since they have been filtered).
        //
        private bool IsSecant(Segment segment, out Segment chord)
        {
            // Make it null and overwrite when necessary.
            chord = null;

            if (ContainsDiameter(segment))
            {
                chord = ConstructChord(segment, this.center, this.radius);
                return true;
            }

            // Acquire the line perpendicular to the segment that passes through the center of the circle.
            Segment perpendicular = segment.GetPerpendicular(this.center);

            // Is this perpendicular segment a radius? If so, it's tangent, not a secant
            if (Utilities.CompareValues(perpendicular.Length, this.radius)) return false;

            // Filter the fact that there are no intersections
            if (perpendicular.Length > this.radius) return false;

            //            1/2 chord length
            //                 _____   circPoint
            //                |    /
            //                |   /
            // perp.Length    |  / radius
            //                | /
            //                |/
            // Determine the half-chord length via Pyhtagorean Theorem.
            double halfChordLength = Math.Sqrt(Math.Pow(this.radius, 2) - Math.Pow(perpendicular.Length, 2));

            chord = ConstructChord(segment, perpendicular.OtherPoint(this.center), halfChordLength);

            return true;
        }

        //
        // Is this a direct radius segment where one endpoint originates at the origin and extends outward?
        // Return the exact radius.
        private Segment IsRadius(Segment segment)
        {
            // The segment must originate from the circle center.
            if (!segment.HasPoint(this.center)) return null;

            // The segment must be at least as long as a radius.
            if (segment.Length < this.radius) return null;

            Point nonCenterPt = segment.OtherPoint(this.center);

            // Check for a direct radius.
            if (this.PointIsOn(nonCenterPt)) return segment;

            //
            // Check for an extended segment.
            //
            //                radius
            //      center    _____   circPt
            //
            // Find the exact coordinates of the 'circ' points.
            //
            double deltaX = 0;
            double deltaY = 0;
            if (segment.IsVertical())
            {
                deltaX = 0;
                deltaY = this.radius;
            }
            else if (segment.IsHorizontal())
            {
                deltaX = this.radius;
                deltaY = 0;
            }
            else
            {
                deltaX = Math.Sqrt(Math.Pow(this.radius, 2) / (1 + Math.Pow(segment.Slope, 2)));
                deltaY = segment.Slope * deltaX;
            }
            Point circPt1 = Point.GetFigurePoint(new Point("", this.center.X + deltaX, this.center.Y + deltaY));

            // intersection is the midpoint of circPt1 and pt2.
            Point circPt2 = Point.GetFigurePoint(new Point("", 2 * this.center.X - circPt1.X, 2 * this.center.Y - circPt1.Y));

            // Return the radius which the segment defines (we generated two points, which is in the middle of the given segment endpoints)
            Segment radius = null;
            if (segment.PointIsOnAndExactlyBetweenEndpoints(circPt1))
            {
                radius = Segment.GetFigureSegment(this.center, circPt1);
            }
            else if (segment.PointIsOnAndExactlyBetweenEndpoints(circPt2))
            {
                radius = Segment.GetFigureSegment(this.center, circPt2);
            }
            else
            {
                throw new ArgumentException("Expected to find candidate points involved in radius as being valid: " + circPt1 + " " + circPt2);
            }

            return radius;
        }

        //
        // Find the points of intersection of two circles; may be 0, 1, or 2.
        //
        public void Intersection(Segment thatSegment, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            //
            // 1 point of intersection: tangent
            //
            Segment radius = IsTangent(thatSegment);

            if (radius != null)
            {
                inter1 = radius.OtherPoint(this.center);
                return;
            }

            //
            // 2 points of intersection: secant / chord
            //
            Segment chord;
            if (IsSecant(thatSegment, out chord))
            {
                inter1 = chord.Point1;
                inter2 = chord.Point2;
                return;
            }

            //
            // Is it a weird segment intersection?
            // That is, where an endpoint lies in the circle and one outside. (Pacman smoking a cigarette).
            //
            Point interiorPt = null;
            Point exteriorPt = null;
            if (PointIsInterior(thatSegment.Point1) && PointIsExterior(thatSegment.Point2))
            {
                interiorPt = thatSegment.Point1;
                exteriorPt = thatSegment.Point2;
            }
            else if (PointIsInterior(thatSegment.Point2) && PointIsExterior(thatSegment.Point1))
            {
                interiorPt = thatSegment.Point2;
                exteriorPt = thatSegment.Point1;
            }

            // 0 intersection points
            if (interiorPt == null && exteriorPt == null) return;

            // Extend the segment to create a secant segment to acquire a chord
            // and the point of intersection (so we can leverage previous code).                
            // Take a point on the line which is guaranteed to be greater than the length of the diameter of the circle.

            // Use the slope and the orientation of the interior / exterior point to find the new point.
            double deltaX = 0;
            double deltaY = 0;
            if (thatSegment.IsVertical())
            {
                deltaX = 0;
                deltaY = this.radius * 2;
            }
            else if (thatSegment.IsHorizontal())
            {
                deltaX = this.radius * 2;
                deltaY = 0;
            }
            else
            {
                deltaX = Math.Sqrt(Math.Pow(this.radius * 2, 2) / (1 + Math.Pow(thatSegment.Slope, 2)));
                deltaY = thatSegment.Slope * deltaX;
            }

            Point option1 = Point.GetFigurePoint(new Point("", interiorPt.X + deltaX, interiorPt.Y + deltaY));

            // intersection is the midpoint of circPt1 and pt2.
            Point option2 = Point.GetFigurePoint(new Point("", 2 * interiorPt.X - option1.X, 2 * interiorPt.Y - option1.Y));

            // We want the point which is on the 'other' side of the interior point.

            Segment secant = null;
            if (thatSegment.PointIsOnAndExactlyBetweenEndpoints(option1))
            {
                secant = new Segment(option1, exteriorPt);
            }
            else if (thatSegment.PointIsOnAndExactlyBetweenEndpoints(option2))
            {
                secant = new Segment(option2, exteriorPt);
            }
            else throw new Exception("Unexpected result with secant calculation: " + this + " " + thatSegment);

            Segment genChord = null;
            if (IsSecant(thatSegment, out genChord))
            {
                // find the point on the chord to return (between interior and exterior
                if (thatSegment.PointIsOnAndExactlyBetweenEndpoints(genChord.Point1))
                {
                    inter1 = genChord.Point1;
                }
                else if (thatSegment.PointIsOnAndExactlyBetweenEndpoints(genChord.Point2))
                {
                    inter1 = genChord.Point2;
                }
                else throw new Exception("Unexpected result with secant / chord calculation: " + this + " " + thatSegment);
            }

            return;
        }

        //
        // Find the points of intersection of two circles; may be 0, 1, or 2.
        // Uses the technique found here: http://mathworld.wolfram.com/Circle-CircleIntersection.html
        //
        public void Intersection(Circle thatCircle, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;
            double R = this.radius;
            double r = thatCircle.radius;
            double d = Point.calcDistance(this.center, thatCircle.center);

            // 0 points of intersection
            if (d > R + r) return;

            // Tangent: 1 point of intersection
            if (Utilities.CompareValues(d, R + r))
            {
                Segment thisRadius = this.IsRadius(new Segment(this.center, thatCircle.center));
                inter1 = thisRadius.OtherPoint(this.center);
                return;
            }

            //
            // Two points of intersection.
            //
            double x = (Math.Pow(d, 2) - Math.Pow(r, 2) + Math.Pow(R, 2)) / (2 * d);
            double y = Math.Sqrt(Math.Pow(R, 2) - Math.Pow(x, 2));

            // The calculation assumes we have centers on the y-axis (and this Circle at the origin).
            inter1 = new Point("", x - this.center.X, y - this.center.Y);
            inter1 = Point.GetFigurePoint(inter1);

            inter2 = new Point("", x - this.center.X, -y - this.center.Y);
            inter2 = Point.GetFigurePoint(inter2);
        }

        //
        // Are the segment endpoints directly on the circle? 
        //
        private bool IsChord(Segment segment)
        {
            return this.PointIsOn(segment.Point1) && this.PointIsOn(segment.Point2);
        }

        //
        // Determine if the given point is on the circle via substitution into (x1 - x2)^2 + (y1 - y2)^2 = r^2
        //
        public bool PointIsOn(Point pt)
        {
            return Utilities.CompareValues(Math.Pow(center.X - pt.X, 2) + Math.Pow(center.Y - pt.Y, 2), Math.Pow(this.radius, 2));
        }

        //
        // Determine if the given point is a point in the interioir of the circle: via substitution into (x1 - x2)^2 + (y1 - y2)^2 = r^2
        //
        public bool PointIsInterior(Point pt)
        {
            return Utilities.LessThan(Point.calcDistance(this.center, pt), this.radius);
        }
        public bool PointIsExterior(Point pt)
        {
            return Utilities.GreaterThan(Point.calcDistance(this.center, pt), this.radius);
        }

        //
        // Concentric circles share the same center, but radii differ.
        //
        public bool AreConcentric(Circle thatCircle)
        {
            return this.center.StructurallyEquals(thatCircle.center) && !Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        //
        // Orthogonal Circles intersect at 90^0: radii connecting to intersection point are perpendicular
        //
        public bool AreOrthognal(Circle thatCircle)
        {
            // Find the intersection points
            Point inter1;
            Point inter2;
            Intersection(thatCircle, out inter1, out inter2);

            // If the circles intersect at 0 points.
            if (inter1 == null) return false;

            // If the circles intersect at 1 point they are tangent
            if (inter2 == null) return false;

            // Create two radii, one for each circle; arbitrarily choose the first point (both work)
            Segment radiusThis = new Segment(this.center, inter1);
            Segment radiusThat = new Segment(this.center, inter1);

            return radiusThis.IsPerpendicularTo(radiusThat);
        }

        //
        // Tangent circle have 1 intersection point
        //
        public Point AreTangent(Circle thatCircle)
        {
            // Find the intersection points
            Point inter1;
            Point inter2;
            Intersection(thatCircle, out inter1, out inter2);

            // If the circles have one valid point of intersection.
            if (inter1 != null && inter2 == null) return inter1;

            return null;
        }

        //
        // Maintain a public repository of all triangles objects in the figure.
        //
        public static void Clear()
        {
            figureCircles.Clear();
        }
        public static List<Circle> figureCircles = new List<Circle>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Circle) figureCircles.Add(clause as Circle);
        }
        public static Circle GetFigureCircle(Point cen, double rad)
        {
            Circle candCircle = new Circle(cen, rad);

            // Search for exact segment first
            foreach (Circle circle in figureCircles)
            {
                if (circle.StructurallyEquals(candCircle)) return circle;
            }

            return null;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Circle thatCircle = obj as Circle;
            if (thatCircle == null) return false;

            return thatCircle.center.StructurallyEquals(center) && Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        public override bool Equals(Object obj)
        {
            Circle thatCircle = obj as Circle;
            if (thatCircle == null) return false;

            return thatCircle.center.Equals(center) && Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        public override string ToString()
        {
            return "Circle(" + this.center + ": r = " + this.radius + ")";
        }
    }
}