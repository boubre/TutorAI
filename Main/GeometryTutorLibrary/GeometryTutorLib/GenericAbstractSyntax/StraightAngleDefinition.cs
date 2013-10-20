using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class StraightAngleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Straight Angle";

        public StraightAngleDefinition() { }

        //
        // Collinear(A, B, C, D, ...) -> Angle(A, B, C), Angle(A, B, D), Angle(A, C, D), Angle(B, C, D),...
        // All angles will have measure 180^o
        // There will be nC3 resulting clauses.
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is ConcreteCollinear)) return null;

            ConcreteCollinear cc = (ConcreteCollinear)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            for (int i = 0; i < cc.points.Count - 2; i++)
            {
                for (int j = i + 1; j < cc.points.Count - 1; j++)
                {
                    if (i != j)
                    {
                        for (int k = j + 1; k < cc.points.Count; k++)
                        {
                            if (j != k)
                            {
                                ConcreteAngle newAngle = new ConcreteAngle(cc.points[i], cc.points[j], cc.points[k]);
                                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(cc);
                                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newAngle));
                                GroundedClause.ConstructClauseLinks(antecedent, newAngle);
                            }
                        }
                    }
                }
            }

            return newGrounded;
        }
    }
}