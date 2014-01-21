using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularDefinition : Definition
    {
        private readonly static string NAME = "Definition of Perpendicular";

        public static void Clear()
        {
            candidateIntersections.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            if ((clause is Perpendicular && !(clause is PerpendicularBisector)) || clause is Strengthened)
            {
                return InstantiateFromPerpendicular(clause);
            }

            if (clause is AngleEquation || clause is Intersection) return InstantiateToPerpendicular(clause);

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromPerpendicular(GroundedClause clause)
        {
            if (clause is Perpendicular) return InstantiateFromPerpendicular(clause, clause as Perpendicular);

            if ((clause as Strengthened).strengthened is Perpendicular && !((clause as Strengthened).strengthened is PerpendicularBisector))
            {
                return InstantiateFromPerpendicular(clause, (clause as Strengthened).strengthened as Perpendicular);
            }

            return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
        }

        // 
        // Perpendicular(B, Segment(A, B), Segment(B, C)) -> RightAngle(), RightAngle()
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateFromPerpendicular(GroundedClause original, Perpendicular perp)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            //          top
            //           |
            //           |_
            //           |_|____ right
            if (perp.StandsOnEndpoint())
            {
                Point top = perp.lhs.OtherPoint(perp.intersect);
                Point right = perp.rhs.OtherPoint(perp.intersect);

                Strengthened streng = new Strengthened(new Angle(top, perp.intersect, right), new RightAngle(top, perp.intersect, right, NAME), NAME);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, streng));
            }
            //          top
            //           |
            //           |_
            // left  ____|_|____ right
            else if (perp.StandsOn())
            {
                Point top = perp.lhs.Point1;
                Point center = perp.intersect;
                Point left = perp.rhs.Point1;
                Point right = perp.rhs.Point2;
                if (perp.lhs.PointIsOnAndExactlyBetweenEndpoints(perp.intersect))
                {
                    left = perp.lhs.Point1;
                    right = perp.lhs.Point2;
                    top = perp.rhs.OtherPoint(perp.intersect);
                }
                else if (perp.rhs.PointIsOnAndExactlyBetweenEndpoints(perp.intersect))
                {
                    left = perp.rhs.Point1;
                    right = perp.rhs.Point2;
                    top = perp.lhs.OtherPoint(perp.intersect);
                }
                else return newGrounded;

                Strengthened topRight = new Strengthened(new Angle(top, center, right), new RightAngle(top, center, right, NAME), NAME);
                Strengthened topLeft = new Strengthened(new Angle(top, center, left), new RightAngle(top, center, left, NAME), NAME);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, topRight));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, topLeft));
            }
            //          top
            //           |
            //           |_
            // left  ____|_|____ right
            //           |
            //           |
            //         bottom
            else if (perp.Crossing())
            {
                Point top = perp.lhs.Point1;
                Point bottom = perp.lhs.Point2;
                Point center = perp.intersect;
                Point left = perp.rhs.Point1;
                Point right = perp.rhs.Point2;

                Strengthened topRight = new Strengthened(new Angle(top, center, right), new RightAngle(top, center, right, NAME), NAME);
                Strengthened bottomRight = new Strengthened(new Angle(right, center, bottom), new RightAngle(right, center, bottom, NAME), NAME);
                Strengthened bottomLeft = new Strengthened(new Angle(left, center, bottom), new RightAngle(left, center, bottom, NAME), NAME);
                Strengthened topLeft = new Strengthened(new Angle(top, center, left), new RightAngle(top, center, left, NAME), NAME);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, topRight));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, bottomRight));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, bottomLeft));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, topLeft));
            }
            else return newGrounded;

            return newGrounded;
        }



        // 
        // Equation(m\angle ABC = 90) -> Perpendicular(B, Segment(A, B), Segment(B, C))
        //
        private static List<Intersection> candidateIntersections = new List<Intersection>();
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToPerpendicular(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Intersection)
            {
                // Since we receive intersections right away in the instantiation process, just store the intersections
                candidateIntersections.Add(clause as Intersection);
            }
            else if (clause is AngleEquation)
            {
                AngleEquation eq = clause as AngleEquation;

                // We need a basic angle equation containing the constant 90
                if (eq.GetAtomicity() != Equation.BOTH_ATOMIC) return newGrounded;
                if (!eq.Contains(new NumericValue(90))) return newGrounded;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateToPerpendicular(inter, eq));
                }
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToPerpendicular(Intersection inter, AngleEquation angEq)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Angle theAngle = (Angle)(angEq.lhs is NumericValue ? angEq.rhs : angEq.lhs);

            // The multiplier must be one for a perpendicular situation
            if (theAngle.multiplier != 1) return newGrounded;

            // This angle must apply to this intersection (same vertex as well as the segments inducing this angle)
            if (!inter.InducesNonStraightAngle(theAngle)) return newGrounded;

            // We are strengthening an intersection to a perpendicular 'labeling'
            Strengthened streng = new Strengthened(inter, new Perpendicular(inter, NAME), NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(angEq);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, streng));

            return newGrounded;
        }
    }
}