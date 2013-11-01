using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class VerticalAnglesTheorem : Theorem
    {
        private readonly static string NAME = "Vertical Angles Theorem";

        public VerticalAnglesTheorem() { }

        //
        // Intersect(X, Segment(A, B), Segment(C, D)) -> Congruent(Angle(A, X, C), Angle(B, X, D)),
        //                                               Congruent(Angle(A, X, D), Angle(C, X, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is Intersection)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            Intersection inter = (Intersection)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(inter);

            // Congruent(Angle(A, X, C), Angle(B, X, D))
            ConcreteAngle ang1Set1 = new ConcreteAngle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1);
            ConcreteAngle ang2Set1 = new ConcreteAngle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2);
            ConcreteCongruentAngles cca1 = new ConcreteCongruentAngles(ang1Set1, ang2Set1, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cca1));

            // Congruent(Angle(A, X, D), Angle(C, X, B))
            ConcreteAngle ang1Set2 = new ConcreteAngle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2);
            ConcreteAngle ang2Set2 = new ConcreteAngle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1);
            ConcreteCongruentAngles cca2 = new ConcreteCongruentAngles(ang1Set2, ang2Set2, NAME);
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cca2));

            return newGrounded;
        }
    }
}