using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public class ConcreteTriangle : ConcreteFigure
    {
        public ConcretePoint Point1 { get; private set; }
        public ConcretePoint Point2 { get; private set; }
        public ConcretePoint Point3 { get; private set; }

        public ConcreteSegment SegmentA { get; private set; }
        public ConcreteSegment SegmentB { get; private set; }
        public ConcreteSegment SegmentC { get; private set; }
        public bool isRight             { get; private set; }
        public ConcreteAngle rightAngle { get; private set; }
        public bool isIsosceles         { get; private set; }
        private readonly double EPSILON = 0.00001;


        /// <summary>
        /// Create a new triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public ConcreteTriangle(ConcreteSegment a, ConcreteSegment b, ConcreteSegment c, string just)
        {
            justification = just;
            SegmentA = a;
            SegmentB = b;
            SegmentC = c;
            isRight = isRightTriangle();
            isIsosceles = false; // Being isosceles must be given in the problem IsIsosceles();

            Point1 = SegmentA.Point1;
            Point2 = SegmentA.Point2;
            Point3 = Point1.Equals(SegmentB.Point1) || Point2.Equals(SegmentB.Point1) ? SegmentB.Point2 : SegmentB.Point1;
        }

        public ConcreteTriangle(ConcretePoint a, ConcretePoint b, ConcretePoint c)
        {
            Point1 = a;
            Point2 = b;
            Point3 = c;

            SegmentA = new ConcreteSegment(a, b);
            SegmentB = new ConcreteSegment(a, c);
            SegmentC = new ConcreteSegment(b, c);

            isRight = isRightTriangle();
            isIsosceles = false; // Being isosceles must be given in the problem IsIsosceles();
        }

        public ConcreteTriangle(List<ConcretePoint> pts)
        {
            Point1 = pts.ElementAt(0);
            Point2 = pts.ElementAt(1);
            Point3 = pts.ElementAt(2);

            SegmentA = new ConcreteSegment(Point1, Point2);
            SegmentB = new ConcreteSegment(Point1, Point3);
            SegmentC = new ConcreteSegment(Point2, Point3);

            isRight = isRightTriangle();
            isIsosceles = false; // Being isosceles must be given in the problem IsIsosceles();
        }

        public List<ConcretePoint> GetPoints()
        {
            List<ConcretePoint> pts = new List<ConcretePoint>();
            pts.Add(Point1);
            pts.Add(Point2);
            pts.Add(Point3);
            return pts;
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
        private bool isRightTriangle()
        {
            bool right = false;
            ConcreteSegment[] segments = new ConcreteSegment[3];
            segments[0] = SegmentA;
            segments[1] = SegmentB;
            segments[2] = SegmentC;

            //Compare vector representations of lines to see if dot product is 0.
            for (int i = 0; i < 3; i++)
            {
                int j = (i + 1) % 3;
                double v1x = segments[i].Point1.X - segments[i].Point2.X;
                double v1y = segments[i].Point1.Y - segments[i].Point2.Y;
                double v2x = segments[j].Point1.X - segments[j].Point2.X;
                double v2y = segments[j].Point1.Y - segments[j].Point2.Y;
                right = right || (v1x * v2x + v1y * v2y) == 0;
                if ((v1x * v2x + v1y * v2y) < EPSILON) // == 0
                {
                    ConcretePoint vertex = segments[i].SharedVertex(segments[j]);
                    ConcretePoint other1 = segments[i].OtherPoint(vertex);
                    ConcretePoint other2 = segments[j].OtherPoint(vertex);
                    rightAngle = new ConcreteAngle(other1, vertex, other2);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if this is an isosceles traingle.
        /// </summary>
        /// <returns>TRUE if this is an isosceles triangle.</returns>
        private bool IsIsosceles()
        {
            ConcreteSegment[] segments = new ConcreteSegment[3];
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

        public void MakeIsosceles()
        {
            if (!IsIsosceles())
            {
                //Console.WriteLine("Deduced fact that this triangle is isosceles does not match empirical information for " + this.ToString());
            }

            isIsosceles = true;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            ConcreteTriangle triangle = obj as ConcreteTriangle;
            if (triangle == null) return false;
            return (triangle.Point1.Equals(Point1) || triangle.Point1.Equals(Point2) || triangle.Point1.Equals(Point3)) &&
                   (triangle.Point2.Equals(Point1) || triangle.Point2.Equals(Point2) || triangle.Point2.Equals(Point3)) &&
                   (triangle.Point3.Equals(Point1) || triangle.Point3.Equals(Point2) || triangle.Point3.Equals(Point3));
//            return triangle.SegmentA.Equals(SegmentA) && triangle.SegmentB.Equals(SegmentB) && triangle.SegmentC.Equals(SegmentC);
        }

        public bool HasSegment(ConcreteSegment cs)
        {
            return SegmentA.Equals(cs) || SegmentB.Equals(cs) || SegmentC.Equals(cs);
        }

        public bool HasAngle(ConcreteAngle ca)
        {
            return (HasSegment(ca.ray1) && HasSegment(ca.ray2)); // Could call SharedVertex
        }

        public ConcreteSegment SharesSide(ConcreteTriangle cs)
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
        // Acquire the particular side which belongs to this triangle (of a congruence)
        //
        public ConcreteSegment SegmentBelongs(ConcreteCongruentSegments ccss)
        {
            if (HasSegment(ccss.cs1)) return ccss.cs1;
            if (HasSegment(ccss.cs2)) return ccss.cs2;
            return null;
        }

        //
        // Acquire the particular angle which belongs to this triangle (of a congruence)
        //
        public ConcreteAngle AngleBelongs(ConcreteCongruentAngles ccas)
        {
            if (HasAngle(ccas.ca1)) return ccas.ca1;
            if (HasAngle(ccas.ca2)) return ccas.ca2;
            return null;
        }

        public bool IsIncludedAngle(ConcreteSegment s1, ConcreteSegment s2, ConcreteAngle a)
        {
            if (!HasSegment(s1) || !HasSegment(s2) && !HasAngle(a)) return false;

            // If the shared vertex between the segments is the vertex of this given angle, then
            // the angle is the included angle as desired
            return s1.SharedVertex(s2).Equals(a.GetVertex());
        }
        
        public ConcretePoint OtherPoint(ConcreteSegment cs)
        {
            if (!cs.HasPoint(Point1)) return Point1;
            if (!cs.HasPoint(Point2)) return Point2;
            if (!cs.HasPoint(Point3)) return Point3;

            return null;
        }

        //
        // Two sides known, return the third side
        //
        public ConcreteSegment OtherSide(ConcreteSegment s1, ConcreteSegment s2)
        {
            if (!HasSegment(s1) || !HasSegment(s2)) return null;
            if (!SegmentA.Equals(s1) && !SegmentA.Equals(s2)) return SegmentA;
            if (!SegmentB.Equals(s1) && !SegmentB.Equals(s2)) return SegmentB;
            if (!SegmentC.Equals(s1) && !SegmentC.Equals(s2)) return SegmentC;
            return null;
        }

        // 
        // Triangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C)
        //
        // RightTriangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C), m\angle ABC = 90^o
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            ConcreteTriangle tri = c as ConcreteTriangle;
            if (tri == null) return newGrounded;

            // Generate the FOL for segments
            tri.SegmentA.SetJustification("Intrinsic");
            tri.SegmentB.SetJustification("Intrinsic");
            tri.SegmentC.SetJustification("Intrinsic");
            tri.AddSuccessor(tri.SegmentA);
            tri.AddSuccessor(tri.SegmentB);
            tri.AddSuccessor(tri.SegmentC);
            tri.SegmentA.AddPredecessor(Utilities.MakeList<GroundedClause>(tri));
            tri.SegmentB.AddPredecessor(Utilities.MakeList<GroundedClause>(tri));
            tri.SegmentB.AddPredecessor(Utilities.MakeList<GroundedClause>(tri));
            newGrounded.Add(tri.SegmentA);
            newGrounded.Add(tri.SegmentB);
            newGrounded.Add(tri.SegmentC);

            // If this is a right triangle, generate the FOL equation
            if (tri.isRight) newGrounded.AddRange(ConcreteAngle.Instantiate(tri, tri.rightAngle));

            return newGrounded;
        }

        public override string ToString() { return "Triangle(" + Point1.ToString() + ", " + Point2.ToString() + ", " + Point3.ToString() + ")"; }
    }
}
