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

        // Polygons that are circumscribed about the circle. 
        public List<GeometryTutorLib.ConcreteAST.Polygon>[] circumPolys { get; private set; }

        // Polygons that are inscribed in the circle. 
        public List<GeometryTutorLib.ConcreteAST.Polygon>[] inscribedPolys { get; private set; }

        // The list of points from the UI which involve this circle.
        public List<Point> pointsOnCircle { get; private set; }

        // The minor Arcs of this circle (based on pointsOnCircle list)
        public List<MinorArc> arcs { get; private set; }

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

            inscribedPolys = new List<Polygon>[Polygon.MAX_EXC_POLY_INDEX];
            circumPolys = new List<Polygon>[Polygon.MAX_EXC_POLY_INDEX];
            for (int n = Polygon.MIN_POLY_INDEX; n < Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                inscribedPolys[n] = new List<GeometryTutorLib.ConcreteAST.Polygon>();
                circumPolys[n] = new List<GeometryTutorLib.ConcreteAST.Polygon>();
            }

            pointsOnCircle = new List<Point>();
            arcs = new List<MinorArc>();

            Utilities.AddUniqueStructurally(this.center.getSuperFigures(), this);
        }

        public void AddArc(MinorArc mArc) { arcs.Add(mArc); }
        public void SetPointsOnCircle(List<Point> pts) { OrderPoints(pts); }

        //
        // For arcs, order the points so that there is a consistency: A, B, C, D-> B between AC, B between AD, etc.
        // Only need to order the points if there are more than three points
        //
        private void OrderPoints(List<Point> points)
        {
            List<KeyValuePair<double, Point>> pointAngleMap = new List<KeyValuePair<double, Point>>();

            foreach (Point point in points)
            {
                double deltaX = point.X - this.center.X;
                double deltaY = point.Y - this.center.Y;

                double radianAngle = System.Math.Atan2(deltaY, deltaX);

                // Find the correct quadrant the point lies in to find the exact angle (w.r.t. unit circle)
                // fourth quadrant
                if (deltaX > 0 && deltaY < 0) radianAngle += 2 * Math.PI;
                // second  or third quadrant
                if (deltaX < 0) radianAngle += Math.PI;
                // on the Y-axis (below x-axis)
                if (GeometryTutorLib.Utilities.CompareValues(deltaX, 0) && deltaY < 0) radianAngle += Math.PI;

                // Angles are between 0 and 2pi
                // insert the point into the correct position (starting from the back); insertion sort-style
                int index;
                for (index = 0; index < pointAngleMap.Count; index++)
                {
                    if (radianAngle > pointAngleMap[index].Key) break;
                }
                pointAngleMap.Insert(index, new KeyValuePair<double, Point>(radianAngle, point));
            }

            //
            // Put all the points in the final ordered list
            //
            foreach (KeyValuePair<double, Point> pair in pointAngleMap)
            {
                pointsOnCircle.Add(pair.Value);
            }
        }

        public Segment GetRadius(Segment r)
        {
            if (r == null) return null;
            int index = radii.IndexOf(r);
            return index == -1 ? null : radii[index];
        }

        //
        // For each triangle, it is inscribed in the circle? Is it circumscribed.
        //
        public void AnalyzePolygon(Polygon poly)
        {
            int index = Polygon.GetPolygonIndex(poly.orderedSides.Count);

            if (PolygonCircumscribesCircle(poly.orderedSides)) circumPolys[index].Add(poly);
            if (CircleCircumscribesPolygon(poly.orderedSides)) inscribedPolys[index].Add(poly);
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
        // Determine if this segment is applicable to the circle: secants, tangent, and chords.
        //
        public void AnalyzeSegment(Segment thatSegment)
        {
            Segment tangentRadius = IsTangent(thatSegment);
            if (tangentRadius != null) tangents.Add(thatSegment, tangentRadius);

            else if (IsChord(thatSegment)) Utilities.AddUnique<Segment>(chords, thatSegment);
            else
            {
                Segment chord;
                if (IsSecant(thatSegment, out chord))
                {
                    // Add to the secants for this circle.
                    secants.Add(thatSegment, chord);

                    // Also add to the chord list.
                    Utilities.AddUnique<Segment>(chords, chord);
                }
            }

            // Is a radius the result of a segment starting at the center and extending outward?
            // We collect all other types below.
            Segment radius = IsRadius(thatSegment);
            if (radius != null) Utilities.AddUnique<Segment>(radii, radius);
        }

        //
        // Determine all applicable secants, tangent, and chords for this circle
        //
        public void CleanUp()
        {
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
        public void FindIntersection(Segment ts, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            GeometryTutorLib.ConcreteAST.Segment s = new GeometryTutorLib.ConcreteAST.Segment(ts.Point1, ts.Point2);

            // SEE: http://stackoverflow.com/questions/1073336/circle-line-collision-detection

            // We have line AB, cicle center C, and radius R.
            double lengthAB = s.Length;
            double[] D = { (ts.Point2.X - ts.Point1.X) / lengthAB, (ts.Point2.Y - ts.Point1.Y) / lengthAB }; //Direction vector from A to B

            // Now the line equation is x = D[0]*t + A.X, y = D[1]*t + A.Y with 0 <= t <= 1.
            double t = D[0] * (this.center.X - ts.Point1.X) + D[1] * (this.center.Y - ts.Point1.Y); //Closest point to circle center
            double[] E = { t * D[0] + ts.Point1.X, t * D[1] + ts.Point1.Y }; //The point described by t.

            double lengthEC = System.Math.Sqrt(System.Math.Pow(E[0] - this.center.X, 2) + System.Math.Pow(E[1] - this.center.Y, 2));

            // Possible Intersection?
            if (Utilities.LessThan(lengthEC, this.radius))
            {
                // Compute distance from t to circle intersection point
                double dt = System.Math.Sqrt(System.Math.Pow(this.radius, 2) - System.Math.Pow(lengthEC, 2));

                // First intersection
                inter1 = Point.GetFigurePoint(new Point("", (t - dt) * D[0] + ts.Point1.X, (t - dt) * D[1] + ts.Point1.Y));

                // Second intersection
                inter2 = Point.GetFigurePoint(new Point("", (t + dt) * D[0] + ts.Point1.X, (t + dt) * D[1] + ts.Point1.Y));
            }
            //
            // Tangent point (E)
            //
            else if (Utilities.CompareValues(lengthEC, this.radius))
            {
                // First intersection
                inter1 = Point.GetFigurePoint(new Point("", E[0], E[1]));
            }
        }

        //
        // Find the points of intersection of two circles; may be 0, 1, or 2.
        // Uses the technique found here: http://mathworld.wolfram.com/Circle-CircleIntersection.html
        //
        public void FindIntersection(Circle thatCircle, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            // SEE: http://stackoverflow.com/questions/3349125/circle-circle-intersection-points

            // Distance between centers
            double d = System.Math.Sqrt(System.Math.Pow(thatCircle.center.X - this.center.X, 2) +
                                        System.Math.Pow(thatCircle.center.Y - this.center.Y, 2));

            // Separate circles
            if (d > this.radius + thatCircle.radius) { }

            // One circle contained in the other
            else if (d < System.Math.Abs(this.radius - thatCircle.radius)) { }

            // Coinciding circles
            else if (d == 0 && this.radius == thatCircle.radius) { }

            // We have intersection(s)!
            else
            {
                // Distance from center of this to midpt of intersections
                double a = (System.Math.Pow(this.radius, 2) - System.Math.Pow(thatCircle.radius, 2) + System.Math.Pow(d, 2)) / (2 * d);

                // Midpoint of the intersections
                double[] midpt = { this.center.X + a * (thatCircle.center.X - this.center.X) / d, this.center.Y + a * (thatCircle.center.Y - this.center.Y) / d };

                // Distance from midpoint to intersections
                double h = System.Math.Sqrt(System.Math.Pow(this.radius, 2) - System.Math.Pow(a, 2));

                // Only one intersection
                if (h == 0)
                {
                    inter1 = Point.GetFigurePoint(new Point("", midpt[0], midpt[1]));
                }
                // Two intersections
                else
                {
                    inter1 = Point.GetFigurePoint(new Point("",
                        midpt[0] + h * (thatCircle.center.Y - this.center.Y) / d, midpt[1] - h * (thatCircle.center.X - this.center.X) / d));

                    inter2 = Point.GetFigurePoint(new Point("",
                        midpt[0] - h * (thatCircle.center.Y - this.center.Y) / d, midpt[1] + h * (thatCircle.center.X - this.center.X) / d));
                }
            }
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
            FindIntersection(thatCircle, out inter1, out inter2);

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
            FindIntersection(thatCircle, out inter1, out inter2);

            // If the circles have one valid point of intersection.
            if (inter1 != null && inter2 == null) return inter1;

            return null;
        }

        //
        // Does the given segment contain a radius of this circle?
        //
        public bool ContainsRadiusWithin(Segment thatSegment)
        {
            foreach (Segment radius in radii)
            {
                if (thatSegment.HasSubSegment(radius)) return true;
            }

            return false;
        }

        //
        // Does the given segment contain a chord? Return the chord.
        //
        public Segment ContainsChord(Segment thatSegment)
        {
            foreach (KeyValuePair<Segment, Segment> pair in secants)
            {
                // Does the secant contain that segment? If so, is the chord contained in that Segment?
                if (pair.Key.HasSubSegment(thatSegment))
                {
                    if (thatSegment.HasSubSegment(pair.Value)) return pair.Value;
                }
            }

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

        public static List<Circle> GetFigureCirclesByRadius(Segment radius)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.radii.Contains(radius))
                {
                    circles.Add(circle);
                }
            }

            return circles;
        }

        public bool IsCentral(Angle angle)
        {
            if (this.center.StructurallyEquals(angle.GetVertex()))
            {
                // The rays need to contain radii of the circle.
                if (this.ContainsRadiusWithin(angle.ray1) && this.ContainsRadiusWithin(angle.ray2))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Circle> IsCentralAngle(Angle angle)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.IsCentral(angle))
                {
                    circles.Add(circle);
                }
            }

            return circles;
        }

        public bool IsInscribed(Angle angle)
        {
            // If the angle has vertex on the circle
            if (!this.PointIsOn(angle.GetVertex())) return false;

            // Do the rays contain chords?
            Segment chord1 = this.ContainsChord(angle.ray1);
            Segment chord2 = this.ContainsChord(angle.ray2);

            return chord1 != null && chord2 != null;
        }

        public static List<Circle> IsInscribedAngle(Angle angle)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.IsInscribed(angle)) circles.Add(circle);
            }

            return circles;
        }

        public static List<Circle> GetChordCircles(Segment segment)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.chords.Contains(segment)) circles.Add(circle);
            }

            return circles;
        }

        public static List<Circle> GetSecantCircles(Segment segment)
        {
            List<Circle> secCircles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.secants.ContainsKey(segment)) secCircles.Add(circle);
                if (circle.chords.Contains(segment)) secCircles.Add(circle);
            }

            return secCircles;
        }

        // A lookup of a chord based on the given secant.
        public Segment GetChord(Segment thatSegment)
        {
            Segment chord = null;

            secants.TryGetValue(thatSegment, out chord);

            return chord;
        }

        public bool HasArc(Arc arc)
        {
            return this.StructurallyEquals(arc.theCircle);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

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