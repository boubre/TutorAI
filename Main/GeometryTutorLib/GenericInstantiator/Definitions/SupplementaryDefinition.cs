using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SupplementaryDefinition : Definition
    {
        private readonly static string NAME = "Definition of Supplementary";

        private static List<Intersection> candidateIntersections = new List<Intersection>();
        private static List<Angle> candidateAngles = new List<Angle>();

        public static void Clear()
        {
            candidateAngles.Clear();
            candidateIntersections.Clear();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause clause)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (clause is Intersection)
            {
                newGrounded.AddRange(InstantiateToSupplementary(clause as Intersection));
            }

            return newGrounded;
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
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateToSupplementary(Intersection inter)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The situation looks like this:
            //  |
            //  |
            //  |_______
            //
            if (inter.StandsOnEndpoint()) return newGrounded;

            // The situation looks like this:
            //       |
            //       |
            //  _____|_______
            //
            if (inter.StandsOn())
            {
                Point up = null;
                Point left = null;
                Point right = null;
                if (inter.lhs.HasPoint(inter.intersect))
                {
                    up = inter.lhs.OtherPoint(inter.intersect);
                    left = inter.rhs.Point1;
                    right = inter.rhs.Point2;
                }
                else
                {
                    up = inter.rhs.OtherPoint(inter.intersect);
                    left = inter.lhs.Point1;
                    right = inter.lhs.Point2;
                }

                // Gets the single angle object from the original figure
                Angle newAngle1 = Angle.AcquireFigureAngle(new Angle(left, inter.intersect, up));
                Angle newAngle2 = Angle.AcquireFigureAngle(new Angle(right, inter.intersect, up));

                Supplementary supp = new Supplementary(newAngle1, newAngle2, NAME);
                supp.SetNotASourceNode();
                supp.SetNotAGoalNode();
                supp.SetClearDefinition();

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(MakeAntecedent(inter, supp.angle1, supp.angle2), supp));
            }

            //
            // The situation is standard and results in 4 supplementary relationships
            //
            else
            {
                Angle newAngle1 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
                Angle newAngle2 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
                Angle newAngle3 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
                Angle newAngle4 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));

                List<Supplementary> newSupps = new List<Supplementary>();
                newSupps.Add(new Supplementary(newAngle1, newAngle2, NAME));
                newSupps.Add(new Supplementary(newAngle2, newAngle4, NAME));
                newSupps.Add(new Supplementary(newAngle3, newAngle4, NAME));
                newSupps.Add(new Supplementary(newAngle3, newAngle1, NAME));

                foreach (Supplementary supp in newSupps)
                {
                    supp.SetNotASourceNode();
                    supp.SetNotAGoalNode();
                    supp.SetClearDefinition();

                    newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(MakeAntecedent(inter, supp.angle1, supp.angle2), supp));
                }
            }

            return newGrounded;
        }

        private static List<GroundedClause> MakeAntecedent(Intersection inter, Angle angle1, Angle angle2)
        {
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(inter);
            antecedent.Add(angle1);
            antecedent.Add(angle2);
            return antecedent;
        }
    }
}