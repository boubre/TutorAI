using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public class Triangle : Figure
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }
        public Point Point3 { get; private set; }

        public Segment SegmentA { get; private set; }
        public Segment SegmentB { get; private set; }
        public Segment SegmentC { get; private set; }
        public Angle AngleA { get; private set; }
        public Angle AngleB { get; private set; }
        public Angle AngleC { get; private set; }
        protected bool isRight;
        public bool givenRight { get; set; }
        public bool provenRight { get; protected set; }
        public Angle rightAngle { get; protected set; }
        protected bool isIsosceles;
        public bool provenIsosceles { get; protected set; }
        protected bool isEquilateral;
        public bool provenEquilateral { get; protected set; }
        private List<Triangle> congruencePairs;
        public bool WasDeducedCongruent(Triangle that) { return congruencePairs.Contains(that); }
        private List<Triangle> similarPairs;
        public bool WasDeducedSimilar(Triangle that) { return similarPairs.Contains(that); }

        public override bool Strengthened() { return provenIsosceles || provenRight || provenEquilateral; }

        /// <summary>
        /// Create a new triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public Triangle(Segment a, Segment b, Segment c, string just) : base()
        {
            justification = just;
            SegmentA = a;
            SegmentB = b;
            SegmentC = c;

            Point1 = SegmentA.Point1;
            Point2 = SegmentA.Point2;
            Point3 = Point1.Equals(SegmentB.Point1) || Point2.Equals(SegmentB.Point1) ? SegmentB.Point2 : SegmentB.Point1;

            AngleA = new Angle(Point1, Point2, Point3);
            AngleB = new Angle(Point2, Point3, Point1);
            AngleC = new Angle(Point3, Point1, Point2);

            isRight = isRightTriangle();
            provenRight = false;
            givenRight = false;
            isIsosceles = IsIsosceles();
            provenIsosceles = false;
            isEquilateral = IsEquilateral();
            provenEquilateral = false;

            congruencePairs = new List<Triangle>();
            similarPairs = new List<Triangle>();

            addSuperFigureToDependencies();
        }

        public Triangle(Point a, Point b, Point c) : base()
        {
            Point1 = a;
            Point2 = b;
            Point3 = c;

            SegmentA = new Segment(a, b);
            SegmentB = new Segment(a, c);
            SegmentC = new Segment(b, c);

            AngleA = new Angle(Point1, Point2, Point3);
            AngleB = new Angle(Point2, Point3, Point1);
            AngleC = new Angle(Point3, Point1, Point2);

            isRight = isRightTriangle();
            provenRight = false;
            givenRight = false;
            isIsosceles = IsIsosceles();
            provenIsosceles = false;
            isEquilateral = IsEquilateral();
            provenEquilateral = false;

            congruencePairs = new List<Triangle>();
            similarPairs = new List<Triangle>();

            addSuperFigureToDependencies();
        }

        public Triangle(List<Point> pts) : this(pts[0], pts[1], pts[2]) { }

        protected void addSuperFigureToDependencies()
        {
            Utilities.AddUniqueStructurally(SegmentA.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(SegmentB.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(SegmentC.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point1.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point2.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point3.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleA.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleB.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleC.getSuperFigures(), this);
        }

        public void SetProvenToBeRight()
        {
            provenRight = true;
        }

        public void SetProvenToBeIsosceles()
        {
            provenIsosceles = true;
        }

        public void SetProvenToBeEquilateral()
        {
            provenEquilateral = true;
        }

        public List<Angle> GetAngles()
        {
            List<Angle> angles = new List<Angle>();
            angles.Add(AngleA);
            angles.Add(AngleB);
            angles.Add(AngleC);
            return angles;
        }

        public List<Point> GetPoints()
        {
            List<Point> pts = new List<Point>();
            pts.Add(Point1);
            pts.Add(Point2);
            pts.Add(Point3);
            return pts;
        }

        public List<Segment> GetSegments()
        {
            List<Segment> segments = new List<Segment>();
            segments.Add(SegmentA);
            segments.Add(SegmentB);
            segments.Add(SegmentC);
            return segments;
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcreteTriangle [right=");
            sb.Append(isRight);
            sb.Append(']');
            sb.AppendLine();
            SegmentA.BuildUnparse(sb, tabDepth + 1);
            SegmentB.BuildUnparse(sb, tabDepth + 1);
            SegmentC.BuildUnparse(sb, tabDepth + 1);
        }

        /// <summary>
        /// Determines if this is a right traingle.
        /// </summary>
        /// <returns>TRUE if this is a right triangle.</returns>
        public bool isRightTriangle()
        {
            return Utilities.CompareValues(AngleA.measure, 90) ||
                   Utilities.CompareValues(AngleB.measure, 90) ||
                   Utilities.CompareValues(AngleC.measure, 90);

            //bool right = false;
            //Segment[] segments = new Segment[3];
            //segments[0] = SegmentA;
            //segments[1] = SegmentB;
            //segments[2] = SegmentC;

            ////Compare vector representations of lines to see if dot product is 0.
            //for (int i = 0; i < 3; i++)
            //{
            //    int j = (i + 1) % 3;
            //    double v1x = segments[i].Point1.X - segments[i].Point2.X;
            //    double v1y = segments[i].Point1.Y - segments[i].Point2.Y;
            //    double v2x = segments[j].Point1.X - segments[j].Point2.X;
            //    double v2y = segments[j].Point1.Y - segments[j].Point2.Y;
            //    right = right || (v1x * v2x + v1y * v2y) == 0;
            //    if ((v1x * v2x + v1y * v2y) < EPSILON) // == 0
            //    {
            //        Point vertex = segments[i].SharedVertex(segments[j]);
            //        Point other1 = segments[i].OtherPoint(vertex);
            //        Point other2 = segments[j].OtherPoint(vertex);
            //        rightAngle = new Angle(other1, vertex, other2);
            //        return true;
            //    }
            //}
            //return false;
        }

        /// <summary>
        /// Determines if this is an isosceles traingle.
        /// </summary>
        /// <returns>TRUE if this is an isosceles triangle.</returns>
        private bool IsIsosceles()
        {
            Segment[] segments = new Segment[3];
            segments[0] = SegmentA;
            segments[1] = SegmentB;
            segments[2] = SegmentC;

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length == segments[i + 1 < segments.Length ? i + 1 : 0].Length)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if this is an equilateral traingle.
        /// </summary>
        /// <returns>TRUE if this is an equilateral triangle.</returns>
        private bool IsEquilateral()
        {
            return Utilities.CompareValues(SegmentA.Length, SegmentB.Length) &&
                   Utilities.CompareValues(SegmentB.Length, SegmentC.Length);
        }

        public Angle GetOppositeAngle(Segment s)
        {
            Point oppVertex = this.OtherPoint(s);

            if (oppVertex.Equals(AngleA.GetVertex())) return AngleA;
            if (oppVertex.Equals(AngleB.GetVertex())) return AngleB;
            if (oppVertex.Equals(AngleC.GetVertex())) return AngleC;

            return null;
        }

        public Segment GetOppositeSide(Angle angle)
        {
            Point vertex = angle.GetVertex();

            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment GetOppositeSide(Point vertex)
        {
            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment OtherSide(Angle a)
        {
            Point vertex = a.GetVertex();

            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment GetSegmentWithPointOnOrExtends(Point pt)
        {
            if (SegmentA.PointIsOn(pt)) return SegmentA;
            if (SegmentB.PointIsOn(pt)) return SegmentB;
            if (SegmentC.PointIsOn(pt)) return SegmentC;

            return null;
        }

        public Segment GetSegmentWithPointDirectlyOn(Point pt)
        {
            if (Segment.Between(pt, SegmentA.Point1, SegmentA.Point2)) return SegmentA;
            if (Segment.Between(pt, SegmentB.Point1, SegmentB.Point2)) return SegmentB;
            if (Segment.Between(pt, SegmentC.Point1, SegmentC.Point2)) return SegmentC;

            return null;
        }

        public void MakeIsosceles()
        {
            if (!IsIsosceles())
            {
                Console.WriteLine("Deduced fact that this triangle is isosceles does not match empirical information for " + this.ToString());
            }

            provenIsosceles = true;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public bool LiesOn(Segment cs)
        {
            return SegmentA.IsCollinearWith(cs) || SegmentB.IsCollinearWith(cs) || SegmentC.IsCollinearWith(cs);
        }

        public bool HasSegment(Segment cs)
        {
            return SegmentA.Equals(cs) || SegmentB.Equals(cs) || SegmentC.Equals(cs);
        }

        // Does this triangle have this specific angle with these vertices
        private bool HasThisSpecificAngle(Angle ca)
        {
            return (HasSegment(ca.ray1) && HasSegment(ca.ray2)); // Could call SharedVertex
        }

        // Does the given angle correspond to any of the angles?
        public bool ExtendsAnAngle(Angle ca)
        {
            return ExtendsSpecificAngle(ca) != null;
        }

        // Does the given angle correspond to any of the angles?
        public Angle ExtendsSpecificAngle(Angle ca)
        {
            if (AngleA.Equates(ca)) return AngleA;
            if (AngleB.Equates(ca)) return AngleB;
            if (AngleC.Equates(ca)) return AngleC;

            return null;
        }

        // Does the given angle correspond to any of the angles?
        public Angle GetAngleWithVertex(Point pt)
        {
            if (AngleA.GetVertex().Equals(pt)) return AngleA;
            if (AngleB.GetVertex().Equals(pt)) return AngleB;
            if (AngleC.GetVertex().Equals(pt)) return AngleC;

            return null;
        }

        //
        // Check directly if this angle is in the triangle
        // Also check indirectly that the given angle is an extension (subangle) of this angle
        // That is, all the rays are shared and the vertex is shared, but the endpoint of rays may be different
        //
        public bool HasAngle(Angle ca)
        {
            return HasThisSpecificAngle(ca) || ExtendsAnAngle(ca);
        }

        public Segment SharesSide(Triangle cs)
        {
            if (SegmentA.Equals(cs.SegmentA) || SegmentA.Equals(cs.SegmentB) || SegmentA.Equals(cs.SegmentC))
            {
                return SegmentA;
            }
            if (SegmentB.Equals(cs.SegmentA) || SegmentB.Equals(cs.SegmentB) || SegmentB.Equals(cs.SegmentC))
            {
                return SegmentB;
            }
            if (SegmentC.Equals(cs.SegmentA) || SegmentC.Equals(cs.SegmentB) || SegmentC.Equals(cs.SegmentC))
            {
                return SegmentC;
            }

            return null;
        }

        //
        // Acquire the particular angle which belongs to this triangle (of a congruence)
        //
        public Angle AngleBelongs(CongruentAngles ccas)
        {
            if (HasAngle(ccas.ca1)) return ccas.ca1;
            if (HasAngle(ccas.ca2)) return ccas.ca2;
            return null;
        }

        //
        // Acquire the particular angle which belongs to this triangle (of a congruence)
        //
        public Angle OtherAngle(Angle thatAngle1, Angle thatAngle2)
        {
            if (AngleA.Equates(thatAngle1) && AngleB.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleB.Equates(thatAngle1)) return AngleC;
            if (AngleB.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleB.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleA;
            if (AngleA.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleB;

            return null;
        }

        public bool IsIncludedAngle(Segment s1, Segment s2, Angle a)
        {
            if (!HasSegment(s1) || !HasSegment(s2) && !HasAngle(a)) return false;

            // If the shared vertex between the segments is the vertex of this given angle, then
            // the angle is the included angle as desired
            return s1.SharedVertex(s2).Equals(a.GetVertex());
        }

        // Of the congruent pair, return the segment that applies to this triangle
        public Segment GetSegment(CongruentSegments ccss)
        {
            if (HasSegment(ccss.cs1)) return ccss.cs1;
            if (HasSegment(ccss.cs2)) return ccss.cs2;

            return null;
        }

        // Of the propportional pair, return the segment that applies to this triangle
        public Segment GetSegment(ProportionalSegments prop)
        {
            if (HasSegment(prop.smallerSegment)) return prop.smallerSegment;
            if (HasSegment(prop.largerSegment)) return prop.largerSegment;

            return null;
        }

        public bool HasPoint(Point p)
        {
            if (Point1.Equals(p)) return true;
            if (Point2.Equals(p)) return true;
            if (Point3.Equals(p)) return true;

            return false;
        }
        
        public Point OtherPoint(Segment cs)
        {
            if (!cs.HasPoint(Point1)) return Point1;
            if (!cs.HasPoint(Point2)) return Point2;
            if (!cs.HasPoint(Point3)) return Point3;

            return null;
        }

        public Point OtherPoint(Point p1, Point p2)
        {
            if (SegmentA.HasPoint(p1) && SegmentA.HasPoint(p2)) return OtherPoint(SegmentA);
            if (SegmentB.HasPoint(p1) && SegmentB.HasPoint(p2)) return OtherPoint(SegmentB);
            if (SegmentC.HasPoint(p1) && SegmentC.HasPoint(p2)) return OtherPoint(SegmentC);

            return null;
        }

        public KeyValuePair<Angle, Angle> GetAcuteAngles()
        {
            if (AngleA.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleB, AngleC);
            if (AngleB.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleC);
            if (AngleC.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleB);

            return new KeyValuePair<Angle,Angle>(null, null);
        }

        //
        // Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
        // the angle, using only the triangles vertices to represent the angle
        //
        public Angle NormalizeAngle(Angle extendedAngle)
        {
            return ExtendsSpecificAngle(extendedAngle);
        }

        //
        // Returns the longest side of the triangle; arbitary choice if equal and longest
        //
        public Segment GetLongestSide()
        {
            if (SegmentA.Length > SegmentB.Length)
            {
                if (SegmentA.Length > SegmentC.Length)
                {
                    return SegmentA;
                }
            }
            else if (SegmentB.Length > SegmentC.Length)
            {
                return SegmentB;
            }

            return SegmentC;
        }

        //
        // return the hypotenuse if we know we have a right triangle
        //
        public Segment GetHypotenuse()
        {
            if (!isRight) return null;

            return GetLongestSide();
        }

        //
        // Two sides known, return the third side
        //
        public Segment OtherSide(Segment s1, Segment s2)
        {
            if (!HasSegment(s1) || !HasSegment(s2)) return null;
            if (!SegmentA.Equals(s1) && !SegmentA.Equals(s2)) return SegmentA;
            if (!SegmentB.Equals(s1) && !SegmentB.Equals(s2)) return SegmentB;
            if (!SegmentC.Equals(s1) && !SegmentC.Equals(s2)) return SegmentC;
            return null;
        }

        // 
        // Triangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C),
        //                      Angle(A, B, C), Angle(B, C, A), Angle(C, A, B)
        //
        // RightTriangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C), m\angle ABC = 90^o
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Triangle tri = c as Triangle;
            if (tri == null) return newGrounded;

            // Generate the FOL for segments
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
            tri.SegmentA.SetJustification("Intrinsic");
            tri.SegmentB.SetJustification("Intrinsic");
            tri.SegmentC.SetJustification("Intrinsic");
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.SegmentA));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.SegmentB));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.SegmentC));

            tri.AngleA.SetJustification("Intrinsic");
            tri.AngleB.SetJustification("Intrinsic");
            tri.AngleC.SetJustification("Intrinsic");
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.AngleA));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.AngleB));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, tri.AngleC));

            // If this is a right triangle, generate the FOL equation
            if (tri.provenRight)
            {
                newGrounded.AddRange(Angle.Instantiate(tri, tri.rightAngle));
            }

            return newGrounded;
        }

        //
        // Is the given angle directly exterior to this triangle?
        //
        // The triangle must share a side with the angle, the non-shared side must extend the adjacent side
        public bool HasExteriorAngle(Angle extAngle)
        {
            // Disallow any angle in this triangle (since it technically satisfies the requirements)
            if (HasAngle(extAngle)) return false;

            // Determine which angle in the triangle has the same vertex as the input angle
            Angle triangleAngle = GetAngleWithVertex(extAngle.GetVertex());

            if (triangleAngle == null) return false;

            // Acquire the ray that is shared between the angle and the triangle
            Segment sharedSegment = triangleAngle.SharedRay(extAngle);

            if (sharedSegment == null) return false;

            // Acquire the other side of the triangle
            Segment otherTriangleSegment = triangleAngle.OtherRay(sharedSegment);

            if (otherTriangleSegment == null) return false;

            // Acquire the ray that is not shared
            Segment exteriorSegment = extAngle.OtherRay(sharedSegment);

            if (exteriorSegment == null) return false;

            //           DISALLOW                                     ALLOW
            //              /                                           /
            //             / \                                         / \
            //            /TRI\                                       /TRI\
            //           /-----\                                     /-----\
            //                 /                                            \
            //                /                                              \
            //               /                                                \
            return otherTriangleSegment.IsCollinearWith(exteriorSegment);
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public KeyValuePair<Segment, Segment> OtherSides(Segment candidate)
        {
            if (SegmentA.Equals(candidate)) return new KeyValuePair<Segment,Segment>(SegmentB, SegmentC);
            if (SegmentB.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentC);
            if (SegmentC.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentB);

            return new KeyValuePair<Segment, Segment>(null, null);
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public bool CoincidesWithASide(Segment candidate)
        {
            if (SegmentA.IsCollinearWith(candidate)) return true;
            if (SegmentB.IsCollinearWith(candidate)) return true;
            if (SegmentC.IsCollinearWith(candidate)) return true;

            return false;
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public Segment DoesParallelCoincideWith(Parallel p)
        {
            if (CoincidesWithASide(p.segment1)) return p.segment1;
            if (CoincidesWithASide(p.segment2)) return p.segment2;

            return null;
        }

        //
        // Given a point on the triangle, return the two angles not at that vertex
        //
        public void AcquireRemoteAngles(Point givenVertex, out Angle remote1, out Angle remote2)
        {
            remote1 = null;
            remote2 = null;

            if (!HasPoint(givenVertex)) return;

            if (AngleA.GetVertex().Equals(givenVertex))
            {
                remote1 = AngleB;
                remote2 = AngleC;
            }
            else if (AngleB.GetVertex().Equals(givenVertex))
            {
                remote1 = AngleA;
                remote2 = AngleC;
            }
            else
            {
                remote1 = AngleA;
                remote2 = AngleB;
            }
        }

        //
        // Have we deduced a congrunence between this triangle and t already?
        //
        public bool HasEstablishedCongruence(Triangle t)
        {
            return congruencePairs.Contains(t);
        }

        // Add to the list of congruent triangles
        public void AddCongruentTriangle(Triangle t)
        {
            congruencePairs.Add(t);
        }

        //
        // Have we deduced a similarity between this triangle and t already?
        //
        public bool HasEstablishedSimilarity(Triangle t)
        {
            return similarPairs.Contains(t);
        }

        // Add to the list of similar triangles
        public void AddSimilarTriangle(Triangle t)
        {
            similarPairs.Add(t);
        }

        public Point GetVertexOn(Segment thatSegment)
        {
            if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentB, thatSegment)) return SegmentA.SharedVertex(SegmentB);
            if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentC, thatSegment)) return SegmentA.SharedVertex(SegmentC);
            if (Segment.IntersectAtSamePoint(this.SegmentB, this.SegmentC, thatSegment)) return SegmentB.SharedVertex(SegmentC);

            return null;
        }

        //
        // Is this triangle congruent to the given triangle in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Triangle thatTriangle)
        {
            bool[] congruent = new bool[3];
            List<Segment> thisSegments = GetSegments();
            List<Segment> thatSegments = thatTriangle.GetSegments();

            for (int thisS = 0; thisS < thisSegments.Count; thisS++)
            {
                int thatS = 0;
                for ( ; thatS < thatSegments.Count; thatS++)
                {
                    if (thisSegments[thisS].CoordinateCongruent(thatSegments[thatS]))
                    {
                        congruent[thisS] = true;
                        break;
                    }
                }

                if (thatS == thatSegments.Count) return false;
            }

            return !congruent.Contains(false);
        }

        //
        // Is this triangle similar to the given triangle in terms of the coordinatization from the UI?
        //
        public bool CoordinateSimilar(Triangle thatTriangle)
        {
            bool[] congruentAngle = new bool[3];
            List<Angle> thisAngles = GetAngles();
            List<Angle> thatAngles = thatTriangle.GetAngles();

            for (int thisS = 0; thisS < thisAngles.Count; thisS++)
            {
                int thatS = 0;
                for (; thatS < thatAngles.Count; thatS++)
                {
                    if (thisAngles[thisS].CoordinateCongruent(thatAngles[thatS]))
                    {
                        congruentAngle[thisS] = true;
                        break;
                    }
                }

                if (thatS == thatAngles.Count) return false;
            }

            return !congruentAngle.Contains(false) && !CoordinateCongruent(thatTriangle); // CTA: Congruence is stronger than Similarity
        }

        //
        // Can this triangle be strengthened to Isosceles, Equilateral, Right, or Right Isosceles?
        //
        public static List<Strengthened> CanBeStrengthened(Triangle thatTriangle)
        {
            List<Strengthened> strengthened = new List<Strengthened>();

            //
            // Equilateral cannot be strengthened
            //
            if (thatTriangle is EquilateralTriangle) return strengthened;

            if (thatTriangle is IsoscelesTriangle)
            {
                //
                // Isosceles -> Equilateral
                //
                if (thatTriangle.isEquilateral)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new EquilateralTriangle(thatTriangle, "Precomputed"), "Precomputed"));
                }

                //
                // Isosceles -> Right Isosceles
                //
                if (thatTriangle.isRight)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new RightTriangle(thatTriangle, "Precomputed"), "Precomputed"));
                }

                return strengthened;
            }

            //
            // Scalene -> Isosceles
            //
            if (thatTriangle.isIsosceles)
            {
                strengthened.Add(new Strengthened(thatTriangle, new IsoscelesTriangle(thatTriangle, "Precomputed"), "Precomputed"));
            }

            //
            // Scalene -> Equilateral
            //
            if (thatTriangle.isEquilateral)
            {
                strengthened.Add(new Strengthened(thatTriangle, new EquilateralTriangle(thatTriangle, "Precomputed"), "Precomputed"));
            }

            //
            // Scalene -> Right
            //
            if (!(thatTriangle is RightTriangle))
            {
                if (thatTriangle.isRight)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new RightTriangle(thatTriangle, "Precomputed"), "Precomputed"));
                }
            }

            return strengthened;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Triangle tri = gc as Triangle;
            if (gc == null) return false;

            // Handles isosceles, right, or equilateral
            if (!this.StructurallyEquals(gc)) return false;

            // Ensure we know the original has been 'proven' (given) to be a particular type of triangle
            if (tri.provenIsosceles) this.provenIsosceles = true;
            if (tri.provenEquilateral) this.provenEquilateral = true;
            if (tri.provenRight) this.provenRight = true;

            return true;
        }

        public bool CoordinateMedian(Segment thatSegment)
        {
            //
            // Two sides must intersect the median at a single point
            //
            Point midptIntersection = null;
            Point coincidingIntersection = null;
            Segment oppSide = null;
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
            {
                coincidingIntersection = SegmentA.FindIntersection(SegmentB);
                midptIntersection = SegmentC.FindIntersection(thatSegment);
                oppSide = SegmentC;
            }
            else if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
            {
                coincidingIntersection = SegmentA.FindIntersection(SegmentC);
                midptIntersection = SegmentB.FindIntersection(thatSegment);
                oppSide = SegmentB;
            }
            else if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
            {
                coincidingIntersection = SegmentB.FindIntersection(SegmentC);
                midptIntersection = SegmentA.FindIntersection(thatSegment);
                oppSide = SegmentA;
            }

            if (midptIntersection == null || oppSide == null) return false;

            // It is possible for the segment to be parallel to the opposite side; results in NaN.
            if (midptIntersection.X == double.NaN || midptIntersection.Y == double.NaN) return false;

            if (!Segment.Between(coincidingIntersection, thatSegment.Point1, thatSegment.Point2)) return false;

            if (!oppSide.PointIsOnAndBetweenEndpoints(midptIntersection)) return false;

            // Midpoint of the remaining side needs to align
            return midptIntersection.Equals(oppSide.Midpoint());

            //
            // This commented code finds the direct median if the endpoints are as would be expected.
            //
            //Point oppPoint = null;
            //if (this.HasPoint(thatSegment.Point1))
            //{
            //    oppPoint = thatSegment.Point2;
            //}
            //else if (this.HasPoint(thatSegment.Point2))
            //{
            //    oppPoint = thatSegment.Point1;
            //}

            //if (oppPoint == null) return false;

            //if (SegmentA.Midpoint().Equals(oppPoint)) return true;
            //if (SegmentB.Midpoint().Equals(oppPoint)) return true;
            //if (SegmentC.Midpoint().Equals(oppPoint)) return true;

            //return false;
        }

        //
        // Is this segment an altitude based on the coordinates (precomputation)
        //
        public bool CoordinateAltitude(Segment thatSegment)
        {
            //
            // Two sides must intersect the given segment at a single point
            //
            Point otherIntersection = null;
            Point thisIntersection = null;
            Segment oppSide = null;
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
            {
                thisIntersection = SegmentA.FindIntersection(SegmentB);
                otherIntersection = SegmentC.FindIntersection(thatSegment);
                oppSide = SegmentC;
            }
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
            {
                thisIntersection = SegmentA.FindIntersection(SegmentC);
                otherIntersection = SegmentB.FindIntersection(thatSegment);
                oppSide = SegmentB;
            }
            if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
            {
                thisIntersection = SegmentB.FindIntersection(SegmentC);
                otherIntersection = SegmentA.FindIntersection(thatSegment);
                oppSide = SegmentA;
            }

            if (otherIntersection == null || oppSide == null) return false;

            // We require a perpendicular intersection
            return Utilities.CompareValues((new Angle(thisIntersection, otherIntersection, oppSide.Point1)).measure, 90);
        }

        public override bool StructurallyEquals(Object obj)
        {
            Triangle thatTriangle = obj as Triangle;
            if (thatTriangle == null) return false;

            return thatTriangle.HasPoint(this.Point1) && thatTriangle.HasPoint(this.Point2) && thatTriangle.HasPoint(this.Point3);
        }

        public override bool Equals(Object obj)
        {
            if (this == obj) return true;

            // An isosceles triangle is not the same as a scalene triangle object even if the points are the same.
            IsoscelesTriangle isoTriangle = obj as IsoscelesTriangle;
            if (isoTriangle != null && !(this is IsoscelesTriangle)) return false;

            // An isosceles triangle is not the same as a scalene triangle object even if the points are the same.
            EquilateralTriangle equiTriangle = obj as EquilateralTriangle;
            if (equiTriangle != null && !(this is IsoscelesTriangle)) return false;

            Triangle triangle = obj as Triangle;
            if (triangle == null) return false;

            // Is this a strengthened version?
            if (triangle.provenIsosceles != this.provenIsosceles) return false;

            // Is this a strenghtened version?
            if (triangle.provenRight != this.provenRight) return false;

            return StructurallyEquals(obj) && base.Equals(obj);
        }

        //
        // Is the given clause an intrinsic component of this triangle?
        //
        public override bool Covers(GroundedClause gc)
        {
            if (gc is Angle) return this.HasAngle(gc as Angle);
            else if (gc is Point) return this.HasPoint(gc as Point);
            else if (gc is Segment) return this.HasSegment(gc as Segment);
            else if (gc is Triangle) return this.StructurallyEquals(gc);

            // A triangle covers an intersection if one vertex covers the intersection and one segment is part of the triangle
            Intersection inter = gc as Intersection;
            if (inter == null) return false;
            return this.HasPoint(inter.intersect) && (this.HasSegment(inter.lhs) || this.HasSegment(inter.rhs));
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (provenIsosceles)
            {
                str.Append("Isosceles ");
            }
            else if (provenEquilateral)
            {
                str.Append("Equilateral ");
            }
            if (provenRight)
            {
                str.Append("Right ");
            }
            str.Append("Triangle(" + Point1.ToString() + ", " + Point2.ToString() + ", " + Point3.ToString() + "): " + justification);
            return str.ToString();
        }
    }
}
