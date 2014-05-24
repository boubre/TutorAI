﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        private Polygon(List<Segment> segs, List<Point> pts, List<Angle> angs)
        {
            orderedSides = segs;
            points = pts;
            angles = angs;
        }

        public bool HasSegment(Segment thatSegment)
        {
            return Utilities.HasStructurally<Segment>(orderedSides, thatSegment);
        }

        /// <summary>
        /// Determines if the given point is inside the polygon; http://alienryderflex.com/polygon/
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public bool IsInConvexPolygon(Point thatPoint)
        {
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
                throw new Exception("Construction new polygon failed.");
            }

            KeyValuePair<List<Point>, List<Angle>> pair = MakePointsAngle(orderedSides);

            switch (vertices.Count)
            {
                case 3:
                    return new Triangle(orderedSides);
                case 4:
                    return Quadrilateral.GenerateQuadrilateral(orderedSides);
                default:
                    return new Polygon(orderedSides, pair.Key, pair.Value);
            }
        }

        private static KeyValuePair<List<Point>, List<Angle>> MakePointsAngle(List<Segment> orderedSides)
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
                        if (vertices.Contains(vertex)) return null;

                        // We have a candidate vertex: save the vertex and the 2 segments which created it.
                        vertices.Add(vertex);
                        pairs.Add(new KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, GeometryTutorLib.ConcreteAST.Segment>(theseSegs[s1], theseSegs[s2]));
                    }
                }
            }

            // We must have the same number of vertices as input segments (ensures degree 2 for all vertices)
            if (vertices.Count != theseSegs.Count) return null;

            // These segments make a polygon; the Polygon class will order the segments appropriately.
            return Polygon.ConstructPolygon(vertices, pairs);
        }

        public override bool StructurallyEquals(Object obj)
        {
            Polygon thatPoly = obj as Polygon;
            if (thatPoly == null) return false;

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
                if (this.points.Contains(pt)) return false;
            }

            return true;
        }
    }
}
