using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class PerpendicularImplyCongruentAdjacentAngles : Theorem
    {
        private readonly static string NAME = "Perpendicular Segments Imply Congruent Adjacent Angles";

        public PerpendicularImplyCongruentAdjacentAngles() { }

        private static List<Perpendicular> candPerpendicular = new List<Perpendicular>();
        private static List<Angle> candAngles = new List<Angle>();

        // Resets all saved data.
        public static void Clear()
        {
            candPerpendicular.Clear();
            candAngles.Clear();
        }

        //
        // Perpendicular(Segment(A, B), Segment(C, D)), Angle(A, M, D), Angle(D, M, B) -> Congruent(Angle(A, M, D), Angle(D, M, B)) 
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                                       A
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Angle) && !(c is Perpendicular)) return newGrounded;

            if (c is Angle)
            {
                Angle newAngle = c as Angle;

                // Find two candidate lines cut by the same transversal
                foreach (Perpendicular perp in candPerpendicular)
                {
                    foreach (Angle oldAngle in candAngles)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicularImplyCongruentAdjacent(perp, oldAngle, newAngle));
                    }
                }

                candAngles.Add(newAngle);
            }
            else if (c is Perpendicular)
            {
                Perpendicular newPerpendicular = c as Perpendicular;

                // Avoid generating if the situation is:
                //
                //   |
                //   |
                //   |_
                //   |_|_________
                //
                if (newPerpendicular.StandsOnEndpoint()) return newGrounded;

                for (int i = 0; i < candAngles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candAngles.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicularImplyCongruentAdjacent(newPerpendicular, candAngles[i], candAngles[j]));
                    }
                }

                candPerpendicular.Add(newPerpendicular);
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckAndGeneratePerpendicularImplyCongruentAdjacent(Perpendicular perp, Angle angle1, Angle angle2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!Utilities.CompareValues(angle1.measure, angle2.measure)) return newGrounded;

            // The given angles must belong to the intersection. That is, the vertex must align and all rays must overlay the intersection.
            if (!(perp.InducesNonStraightAngle(angle1) && perp.InducesNonStraightAngle(angle1))) return newGrounded;

            //
            // Now we have perpendicular -> congruent angles scenario
            //
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(angle1, angle2, NAME);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(perp);
            antecedent.Add(angle1);
            antecedent.Add(angle2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gcas));

            return newGrounded;
        }
    }
}