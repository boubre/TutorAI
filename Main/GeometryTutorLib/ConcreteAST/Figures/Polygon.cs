﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a general polygon (which consists of n >= 3 segments)
    /// </summary>
    public class Polygon : Figure
    {
        public const int MAX_POLYGON_SIDES = 6;

        //
        // Indices to access a polygon array container
        //
        public const int MIN_POLY_INDEX = 0;
        public const int TRIANGLE_INDEX = 0;
        public const int QUADRILATERAL_INDEX = 1;
        public const int PENTAGON_INDEX = 2;
        public const int HEXAGON_INDEX = 3;
        public const int SEPTAGON_INDEX = 4;
        public const int OCTAGON_INDEX = 5;
        public const int NONAGON_INDEX = 6;
        public const int DECAGON_INDEX = 7;
        public const int MAX_INC_POLY_INDEX = MAX_POLYGON_SIDES - 3; // For use with <=
        public const int MAX_EXC_POLY_INDEX = MAX_POLYGON_SIDES - 2; // For use with <  and  array allocation size

        public static int GetPolygonIndex(int numSides) { return numSides - 3; }

        public List<Point> points { get; protected set; }
        public List<Segment> orderedSides { get; protected set; }
        public List<Angle> angles { get; protected set; }

        public double area { get; protected set; }

        public Polygon() { }

        public Polygon(Segment s1, Segment s2, Segment s3)
        {
            orderedSides = new List<Segment>();
            orderedSides.Add(s1);
            orderedSides.Add(s2);
            orderedSides.Add(s3);

            KeyValuePair<List<Point>, List<Angle>> pair = MakePointsAngle(orderedSides);

            points = pair.Key;
            angles = pair.Value;

            thisAtomicRegion = new ShapeAtomicRegion(this);
        }

        protected Polygon(List<Segment> segs, List<Point> pts, List<Angle> angs)
        {
            orderedSides = segs;
            points = pts;
            angles = angs;

            thisAtomicRegion = new ShapeAtomicRegion(this);
        }

        public bool HasSegment(Segment thatSegment)
        {
            return Utilities.HasStructurally<Segment>(orderedSides, thatSegment);
        }

        public override bool PointLiesInside(Point pt)
        {
            if (pt == null) return false;

            if (PointLiesOn(pt)) return false;
            
            return IsInPolygon(pt);
        }

        public override bool PointLiesInOrOn(Point pt)
        {
            if (IsInPolygon(pt)) return true;

            foreach (Segment side in orderedSides)
            {
                if (side.PointLiesOnAndBetweenEndpoints(pt)) return true;
            }

            return false;
        }
        public override bool PointLiesOn(Point pt)
        {
            foreach (Segment side in orderedSides)
            {
                if (side.PointLiesOnAndBetweenEndpoints(pt)) return true;
            }

            return false;
        }
        public override Polygon GetPolygonalized() { return this; }

        public override void FindIntersection(Segment that, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            Point foundInter = null;
            List<Point> intersections = new List<Point>();
            foreach (Segment side in orderedSides)
            {
                if (side.IsCollinearWith(that))
                {
                    if (that.PointLiesOnAndBetweenEndpoints(side.Point1)) Utilities.AddStructurallyUnique<Point>(intersections, side.Point1);
                    if (that.PointLiesOnAndBetweenEndpoints(side.Point2)) Utilities.AddStructurallyUnique<Point>(intersections, side.Point2);

                    if (side.PointLiesOnAndBetweenEndpoints(that.Point1)) Utilities.AddStructurallyUnique<Point>(intersections, that.Point1);
                    if (side.PointLiesOnAndBetweenEndpoints(that.Point2)) Utilities.AddStructurallyUnique<Point>(intersections, that.Point2);
                }
                else
                {
                    foundInter = side.FindIntersection(that);

                    // Is the intersection in the middle of the segments?
                    if (side.PointLiesOnAndBetweenEndpoints(foundInter) && that.PointLiesOnAndBetweenEndpoints(foundInter))
                    {
                        // A segment may intersect a polygon through up to 2 vertices creating 4 intersections.
                        if (!Utilities.HasStructurally<Point>(intersections, foundInter)) intersections.Add(foundInter);
                    }
                }
            }
            if (!(this is ConcavePolygon) && intersections.Count > 2)
            {
                throw new Exception("A segment intersecting a polygon may have up to 2 intersection points, not: " + intersections.Count);
            }

            if (intersections.Any()) inter1 = intersections[0];
            if (intersections.Count > 1) inter2 = intersections[1];
        }

        //
        // Return the side that overlaps.
        //
        public Segment Overlap(Segment that)
        {
            foreach (Segment side in orderedSides)
            {
                if (side.CoincidingWithOverlap(that)) return side;
            }

            return null;
        }

        /// <summary>
        /// Make a set of connections for atomic region analysis.
        /// </summary>
        /// <returns></returns>
        public override List<Connection> MakeAtomicConnections()
        {
            List<Connection> connections = new List<Connection>();

            foreach (Segment side in orderedSides)
            {
                connections.Add(new Connection(side.Point1, side.Point2, ConnectionType.SEGMENT, side));
            }

            return connections;
        }

        /// <summary>
        /// Determines if the given point is inside the polygon; http://alienryderflex.com/polygon/
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public bool IsInPolygon(Point thatPoint)
        {
            if (thatPoint == null)
            {
                System.Diagnostics.Debug.WriteLine(new System.Diagnostics.StackTrace(true).ToString());
                throw new ArgumentException("Null passed to isInPolygon");
            }

            bool result = false;
            int j = points.Count - 1;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < thatPoint.Y &&
                    points[j].Y >= thatPoint.Y || points[j].Y < thatPoint.Y &&
                                                  points[i].Y >= thatPoint.Y)
                {
                    if (points[i].X + (thatPoint.Y - points[i].Y) / (points[j].Y - points[i].Y) * (points[j].X - points[i].X) < thatPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        //
        // Is this polygon stronger than that?
        // That is, triangle -> isosceles -> equilateral.
        //
        public virtual bool IsStrongerThan(Polygon that) { return false; }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Polygon(");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].ToString());
                if (p < points.Count - 1) str.Append(", ");
            }
            str.Append(")");

            return str.ToString();
        }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Polygon(");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].CheapPrettyString());
            }
            str.Append(")");

            return str.ToString();
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Parallel arrays of (1) vertices and (2) segments that share that given vertex.
        public static Polygon ConstructPolygon(List<Point> vertices, List<KeyValuePair<Segment, Segment>> pairs)
        {
            List<Segment> orderedSides = new List<Segment>();

            // Follow the trail of sides starting at one of the first sides
            Segment currentSide = pairs[0].Key;
            orderedSides.Add(currentSide);
            Point currentVertex = currentSide.Point1;

            for (int v = 1; v < vertices.Count; v++)
            {
                // Where is the current vertex located?
                int nextVertexIndex = vertices.IndexOf(currentVertex);

                // Find the next side to follow.
                if (pairs[nextVertexIndex].Key.StructurallyEquals(currentSide))
                {
                    currentSide = pairs[nextVertexIndex].Value;
                }
                else
                {
                    currentSide = pairs[nextVertexIndex].Key;
                }
                orderedSides.Add(currentSide);

                // Find the next vertex (moving along the next side)
                currentVertex = currentSide.OtherPoint(currentVertex);
            }

            if (orderedSides.Count != vertices.Count)
            {
                throw new Exception("Construction of new polygon failed.");
            }

            return ActuallyConstructThePolygonObject(orderedSides);
        }

        private static Polygon ActuallyConstructThePolygonObject(List<Segment> orderedSides)
        {
            //
            // Check for lines that are actually collinear (and can be compressed into a single segment).
            //
            bool change = true;
            while (change)
            {
                change = false;
                for (int s = 0; s < orderedSides.Count; s++)
                {
                    Segment first = orderedSides[s];
                    Segment second = orderedSides[(s + 1) % orderedSides.Count];
                    Point shared = first.SharedVertex(second);

                    // We know these lines share an endpoint and that they are collinear.
                    if (first.IsCollinearWith(second))
                    {
                        Segment newSegment = new Segment(first.OtherPoint(shared), second.OtherPoint(shared));

                        // Replace the two original lines with the new line.
                        orderedSides.Insert(s, newSegment);
                        orderedSides.Remove(first);
                        orderedSides.Remove(second);
                        change = true;
                    }
                }
            }

            KeyValuePair<List<Point>, List<Angle>> pair = MakePointsAngle(orderedSides);

            // If the polygon is concave, make that object.
            if (IsConcavePolygon(pair.Key)) return new ConcavePolygon(orderedSides, pair.Key, pair.Value);

            // Otherwise, make the other polygons
            switch (orderedSides.Count)
            {
                case 3:
                    return new Triangle(orderedSides);
                case 4:
                    return Quadrilateral.GenerateQuadrilateral(orderedSides);
                default:
                    return new Polygon(orderedSides, pair.Key, pair.Value);
            }

            //return null;
        }

        //
        // Return True if the polygon is convex.
        // http://blog.csharphelper.com/2010/01/04/determine-whether-a-polygon-is-convex-in-c.aspx
        //
        protected static bool IsConcavePolygon(List<Point> orderedPts)
        {
            // For each set of three adjacent points A, B, C,
            // find the dot product AB · BC. If the sign of
            // all the dot products is the same, the angles
            // are all positive or negative (depending on the
            // order in which we visit them) so the polygon
            // is convex.
            bool got_negative = false;
            bool got_positive = false;
            int B, C;
            for (int A = 0; A < orderedPts.Count; A++)
            {
                B = (A + 1) % orderedPts.Count;
                C = (B + 1) % orderedPts.Count;

                // Create normalized vectors and find the cross-product.
                Point vec1 = Point.MakeVector(orderedPts[A], orderedPts[B]);
                Point vec2 = Point.MakeVector(orderedPts[B], orderedPts[C]);

                double cross_product = Point.CrossProduct(vec1, vec2);

                if (cross_product < 0)
                {
                    got_negative = true;
                }
                else if (cross_product > 0)
                {
                    got_positive = true;
                }
                if (got_negative && got_positive) return true;
            }

            // If we got this far, the polygon is convex.
            return false;
        }

        protected static KeyValuePair<List<Point>, List<Angle>> MakePointsAngle(List<Segment> orderedSides)
        {
            List<Point> points = new List<Point>();
            List<Angle> angles = new List<Angle>();

            for (int s = 0; s < orderedSides.Count; s++)
            {
                Point vertex = orderedSides[s].SharedVertex(orderedSides[s + 1 == orderedSides.Count ? 0 : s + 1]);

                points.Add(vertex);
                angles.Add(new Angle(orderedSides[s], orderedSides[s + 1 == orderedSides.Count ? 0 : s + 1]));
            }

            return new KeyValuePair<List<Point>, List<Angle>>(points, angles);
        }

        //
        // Returns whether this list of segments can be used to construct a valid polygon.
        // Criteria:
        //   * All vertices are exactly of degree 2.
        //
        // We assume filtration of crossing and coinciding segments
        //
        public static Polygon MakePolygon(List<Segment> theseSegs)
        {
            // Parallel arrays of (1) vertices and (2) segments that share the given vertex.
            List<Point> vertices = new List<Point>();
            List<KeyValuePair<Segment, Segment>> pairs = new List<KeyValuePair<Segment, Segment>>();

            for (int s1 = 0; s1 < theseSegs.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < theseSegs.Count; s2++)
                {
                    Point vertex = theseSegs[s1].SharedVertex(theseSegs[s2]);

                    // We have a shared vertex
                    if (vertex != null)
                    {
                        // Shared vertices must be unique among all combinations of segments
                        // if this vertex is already in the list, we don't have a polygon
                        if (vertices.Contains(vertex))
                        {
                            return null;
                        }

                        // We have a candidate vertex: save the vertex and the 2 segments which created it.
                        vertices.Add(vertex);
                        pairs.Add(new KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, GeometryTutorLib.ConcreteAST.Segment>(theseSegs[s1], theseSegs[s2]));
                    }
                }
            }

            // We must have the same number of vertices as input segments (ensures degree 2 for all vertices)
            if (vertices.Count != theseSegs.Count) return null;

            //
            // Walk around the set of ordered sides.
            // Those sides should not pass through a previous vertex.
            //
            //
            //        /\
            //       /  \
            //      /    \____
            //     /    / \   |
            //    /____/   \__|
            //
            //
            foreach (Segment side in theseSegs)
            {
                foreach (Point vertex in vertices)
                {
                    if (side.PointLiesOnAndExactlyBetweenEndpoints(vertex))
                    {
                        return null;
                    }
                }
            }

            // If we are given the sides already ordered, just make the polygon straight-away.
            Polygon simple = MakeOrderedPolygon(theseSegs);
            if (simple != null) return simple;

            // These segments make a polygon; the Polygon class will order the segments appropriately.
            return Polygon.ConstructPolygon(vertices, pairs);
        }

        //
        // If we are given the sides already ordered, just make the polygon stright-away.
        //
        public static Polygon MakeOrderedPolygon(List<Segment> theseSegs)
        {
            for (int s = 0; s < theseSegs.Count; s++)
            {
                if (theseSegs[s].SharedVertex(theseSegs[(s+1) % theseSegs.Count]) == null) return null;
            }

            return ActuallyConstructThePolygonObject(theseSegs);
        }

        //
        // The center lies inside the polygon and there are no intersection points with the sides.
        //
        private bool ContainsCircle(Circle that)
        {
            foreach (Segment side in orderedSides)
            {
                Point pt1 = null;
                Point pt2 = null;
                that.FindIntersection(side, out pt1, out pt2);
                if (pt1 != null && pt2 != null) return false;
            }

            return that.PointLiesInside(that.center);
        }

        private bool ContainsPolygon(Polygon that)
        {
            //
            // All points are interior to the polygon.
            //
            foreach (Point thatPt in that.points)
            {
                if (!this.PointLiesInOrOn(thatPt)) return false;
            }

            //
            // Check that all intersections so that there are no crossings.
            //
            foreach (Segment thisSide in this.orderedSides)
            {
                foreach (Segment thatSide in that.orderedSides)
                {
                    if (thisSide.Crosses(thatSide)) return false;
                }
            }

            return true;
        }

        //
        // that Polygon lies within this circle.
        //
        private bool ContainsSector(Sector that)
        {
            if (!this.PointLiesInOrOn(that.theArc.endpoint1)) return false;
            if (!this.PointLiesInOrOn(that.theArc.endpoint2)) return false;

            if (!this.PointLiesInOrOn(that.theArc.theCircle.center)) return false;

            if (!this.PointLiesInOrOn(that.theArc.theCircle.Midpoint(that.theArc.endpoint1, that.theArc.endpoint2))) return false;

            // Check all point approximations for containment.
            //List<Point> approx = that.GetFigureAsAtomicRegion().GetVertices();
            //foreach (Point pt in approx)
            //{
            //    if (!this.PointLiesInOrOn(pt)) return false;
            //}

            return true;
        }

        //
        // A shape within this shape?
        //
        public override bool Contains(Figure that)
        {
            if (that is Circle) return ContainsCircle(that as Circle);
            if (that is Polygon) return ContainsPolygon(that as Polygon);
            if (that is Sector) return ContainsSector(that as Sector);

            return false;
        }

        //
        // Find the points of intersection of two polygons.
        //
        public List<Point> FindIntersections(Polygon that)
        {
            List<Point> intersections = new List<Point>();

            foreach (Segment thatSide in that.orderedSides)
            {
                Point pt1 = null;
                Point pt2 = null;

                this.FindIntersection(thatSide, out pt1, out pt2);

                if (pt1 != null) Utilities.AddStructurallyUnique<Point>(intersections, pt1);
                if (pt2 != null) Utilities.AddStructurallyUnique<Point>(intersections, pt2);
            }

            return intersections;
        }

        //
        // Find the points of intersection of a polygon and a circle.
        //
        public List<Point> FindIntersections(Circle that)
        {
            List<Point> intersections = new List<Point>();

            foreach (Segment side in orderedSides)
            {
                Point pt1 = null;
                Point pt2 = null;

                that.FindIntersection(side, out pt1, out pt2);

                if (pt1 != null && side.PointLiesOnAndBetweenEndpoints(pt1)) Utilities.AddStructurallyUnique<Point>(intersections, pt1);
                if (pt2 != null && side.PointLiesOnAndBetweenEndpoints(pt2)) Utilities.AddStructurallyUnique<Point>(intersections, pt2);
            }

            return intersections;
        }

        public bool HasSamePoints(Polygon that)
        {
            if (that == null) return false;

            if (this.points.Count != that.points.Count) return false;

            return Utilities.EqualSets<Point>(this.points, that.points);
        }

        public override bool StructurallyEquals(Object obj)
        {
            Polygon thatPoly = obj as Polygon;
            if (thatPoly == null) return false;

            if (thatPoly is Quadrilateral) return false;

            foreach (Point pt in thatPoly.points)
            {
                if (!Utilities.HasStructurally<Point>(this.points, pt)) return false; 
            }

            return true;
        }

        public override bool Equals(Object obj)
        {
            Polygon thatPoly = obj as Polygon;
            if (thatPoly == null) return false;

            foreach (Point pt in thatPoly.points)
            {
                if (!(this.points.Contains(pt))) return false;
            }

            return true;
        }
    }
}
