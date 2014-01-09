using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Intersection : Descriptor
    {
        public Point intersect { get; protected set; }
        public Segment lhs { get; protected set; }
        public Segment rhs { get; protected set; }

        public Intersection() : base() { }

        public Intersection(Point i, Segment l, Segment r, string just)
            : base()
        {
            intersect = i;
            lhs = l;
            rhs = r;
            justification = just;
        }

        //
        // If an endpoint of one segment is on the other segment
        //
        public bool StandsOn()
        {
            return Segment.Between(rhs.Point1, lhs.Point1, lhs.Point2) ||
                   Segment.Between(rhs.Point2, lhs.Point1, lhs.Point2) ||
                   Segment.Between(lhs.Point1, rhs.Point1, rhs.Point2) ||
                   Segment.Between(lhs.Point2, rhs.Point1, rhs.Point2);
        }

        //
        // If an endpoint of one segment is on the other segment
        //
        public bool StandsOnEndpoint()
        {
            if (lhs.Point1.Equals(rhs.Point1) || lhs.Point1.Equals(rhs.Point2)) return true;
            if (lhs.Point2.Equals(rhs.Point1) || lhs.Point2.Equals(rhs.Point2)) return true;

            return false;
        }

        public bool Crossing()
        {
            return !StandsOn() && !StandsOnEndpoint();
        }

        public bool IsStraightAngleIntersection()
        {
            return !StandsOnEndpoint();
        }


        //
        // If an endpoint of one segment is on the other segment
        //
        public bool HasSegment(Segment s)
        {
            return lhs.StructurallyEquals(s) || rhs.StructurallyEquals(s);
        }

        public Segment GetCollinearSegment(Segment thatSegment)
        {
            if (lhs.IsCollinearWith(thatSegment)) return lhs;
            if (rhs.IsCollinearWith(thatSegment)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Segment CommonSegment(Intersection thatInter)
        {
            if (lhs.StructurallyEquals(thatInter.lhs) || lhs.IsCollinearWith(thatInter.lhs)) return lhs;
            if (lhs.StructurallyEquals(thatInter.rhs) || lhs.IsCollinearWith(thatInter.rhs)) return lhs;
            if (rhs.StructurallyEquals(thatInter.lhs) || rhs.IsCollinearWith(thatInter.lhs)) return rhs;
            if (rhs.StructurallyEquals(thatInter.rhs) || rhs.IsCollinearWith(thatInter.rhs)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Segment CommonSegment(Parallel thatParallel)
        {
            if (lhs.StructurallyEquals(thatParallel.segment1) || lhs.IsCollinearWith(thatParallel.segment1)) return lhs;
            if (lhs.StructurallyEquals(thatParallel.segment2) || lhs.IsCollinearWith(thatParallel.segment2)) return lhs;
            if (rhs.StructurallyEquals(thatParallel.segment1) || rhs.IsCollinearWith(thatParallel.segment1)) return rhs;
            if (rhs.StructurallyEquals(thatParallel.segment2) || rhs.IsCollinearWith(thatParallel.segment2)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public bool InducesNonStraightAngle(Angle thatAngle)
        {
            // The given vertex must match the intersection point of the two lines intersecting
            if (!intersect.Equals(thatAngle.GetVertex())) return false;

            if (thatAngle.Equates(new Angle(lhs.Point1, intersect, rhs.Point1))) return true;
            if (thatAngle.Equates(new Angle(lhs.Point1, intersect, rhs.Point2))) return true;
            if (thatAngle.Equates(new Angle(lhs.Point2, intersect, rhs.Point1))) return true;
            if (thatAngle.Equates(new Angle(lhs.Point2, intersect, rhs.Point2))) return true;

            return false;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Angle GetInducedNonStraightAngle(CongruentAngles congAngles)
        {
            if (this.InducesNonStraightAngle(congAngles.ca1)) return congAngles.ca1;
            if (this.InducesNonStraightAngle(congAngles.ca2)) return congAngles.ca2;

            return null;
        }
        public Angle GetInducedNonStraightAngle(Supplementary supp)
        {
            if (this.InducesNonStraightAngle(supp.angle1)) return supp.angle1;
            if (this.InducesNonStraightAngle(supp.angle2)) return supp.angle2;

            return null;
        }

        //
        // Are both angles induced by this intersection either as vertical angles or adjacent angles
        //
        public bool InducesBothAngles(CongruentAngles conAngles)
        {
            return this.InducesNonStraightAngle(conAngles.ca1) && this.InducesNonStraightAngle(conAngles.ca2);
        }

        public Segment OtherSegment(Segment thatSegment)
        {
            if (lhs.Equals(thatSegment)) return rhs;
            if (rhs.Equals(thatSegment)) return lhs;

            if (lhs.IsCollinearWith(thatSegment)) return rhs;
            if (rhs.IsCollinearWith(thatSegment)) return lhs;

            return null;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Perpendicular perp = gc as Perpendicular;
            if (perp == null) return false;
            return intersect.Equals(perp.intersect) && ((lhs.StructurallyEquals(perp.lhs) && rhs.StructurallyEquals(perp.rhs)) ||
                                                        (lhs.StructurallyEquals(perp.rhs) && rhs.StructurallyEquals(perp.lhs)));
        }

        public override bool Covers(GroundedClause gc)
        {
            if (gc is Point) return intersect.Equals(gc as Point) || lhs.Covers(gc) || rhs.Covers(gc);
            else if (gc is Segment) return lhs.Covers(gc) || rhs.Covers(gc);
            // An intersection covers a triangle if a triangle covers the intersection (the intersection
            // point is a vertex and a segment is a side of the triangle)
            else if (gc is Triangle) return (gc as Triangle).Covers(this);

            InMiddle im = gc as InMiddle;
            if (im == null) return false;
            return intersect.Covers(im.point) && (lhs.Covers(im.segment) || rhs.Covers(im.segment));
        }

        //
        // Is the given segment collinear with this intersection
        //
        public bool ImpliesRay(Segment s)
        {
            if (!intersect.Equals(s.Point1) && !intersect.Equals(s.Point2)) return false;

            return lhs.IsCollinearWith(s) || rhs.IsCollinearWith(s);
        }

        public bool DefinesBothRays(Segment thatSeg1, Segment thatSeg2)
        {
            return ImpliesRay(thatSeg1) && ImpliesRay(thatSeg2);
        }

        //
        // Returns the exact transversal between the intersections
        //
        public Segment AcquireTransversal(Intersection thatInter)
        {
            // The two intersections should not be at the same vertex
            if (intersect.Equals(thatInter.intersect)) return null;

            Segment common = CommonSegment(thatInter);
            return common != null ? new Segment(this.intersect, thatInter.intersect) : common;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return intersect.Equals(inter.intersect) && lhs.StructurallyEquals(inter.lhs) && rhs.StructurallyEquals(inter.rhs);
        }

        public override bool Equals(Object obj)
        {
            if (obj is Perpendicular) return (obj as Perpendicular).Equals(this);

            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return StructurallyEquals(inter) && base.Equals(inter);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Intersection(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }

        //  A      B
        //   \    /
        //    \  /
        //     \/
        //     /\ X
        //    /  \
        //   /    \
        //  C      D
        //
        // Intersection(X, Segment(A, D), Segment(B, C)) -> Supplementary(Angle(A, X, B), Angle(B, X, D))
        //                                                  Supplementary(Angle(B, X, D), Angle(D, X, C))
        //                                                  Supplementary(Angle(D, X, C), Angle(C, X, A))
        //                                                  Supplementary(Angle(C, X, A), Angle(A, X, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateSupplementary(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Intersection)) return newGrounded;

            Intersection newIntersection = c as Intersection;

            // The situation looks like this:
            //  |
            //  |
            //  |_______
            //
            if (newIntersection.StandsOnEndpoint()) return newGrounded;

            // The situation looks like this:
            //       |
            //       |
            //  _____|_______
            //
            if (newIntersection.StandsOn())
            {
                Point up = null;
                Point left = null;
                Point right = null;
                if (newIntersection.lhs.HasPoint(newIntersection.intersect))
                {
                    up = newIntersection.lhs.OtherPoint(newIntersection.intersect);
                    left = newIntersection.rhs.Point1;
                    right = newIntersection.rhs.Point2;
                }
                else
                {
                    up = newIntersection.rhs.OtherPoint(newIntersection.intersect);
                    left = newIntersection.lhs.Point1;
                    right = newIntersection.lhs.Point2;
                }

                Angle newAngle1 = new Angle(left, newIntersection.intersect, up);
                Angle newAngle2 = new Angle(right, newIntersection.intersect, up);

                Supplementary supp = new Supplementary(newAngle1, newAngle2, "Definition of Supplementary");
                supp.SetNotASourceNode();
                supp.SetNotAGoalNode();

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(Utilities.MakeList<GroundedClause>(newIntersection), supp));
            }

            //
            // The situation is standard and results in 4 supplementary relationships
            //
            else
            {
                Angle newAngle1 = new Angle(newIntersection.lhs.Point1, newIntersection.intersect, newIntersection.rhs.Point1);
                Angle newAngle2 = new Angle(newIntersection.lhs.Point1, newIntersection.intersect, newIntersection.rhs.Point2);
                Angle newAngle3 = new Angle(newIntersection.lhs.Point2, newIntersection.intersect, newIntersection.rhs.Point1);
                Angle newAngle4 = new Angle(newIntersection.lhs.Point2, newIntersection.intersect, newIntersection.rhs.Point2);

                Supplementary supp1 = new Supplementary(newAngle1, newAngle2, "Definition of Supplementary");
                Supplementary supp2 = new Supplementary(newAngle2, newAngle4, "Definition of Supplementary");
                Supplementary supp3 = new Supplementary(newAngle3, newAngle4, "Definition of Supplementary");
                Supplementary supp4 = new Supplementary(newAngle3, newAngle1, "Definition of Supplementary");
                supp1.SetNotASourceNode();
                supp1.SetNotAGoalNode();
                supp2.SetNotASourceNode();
                supp2.SetNotAGoalNode();
                supp3.SetNotASourceNode();
                supp3.SetNotAGoalNode();
                supp4.SetNotASourceNode();
                supp4.SetNotAGoalNode();

                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(newIntersection);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp1));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp2));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp3));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp4));
            }

            return newGrounded;
        }
    }
}