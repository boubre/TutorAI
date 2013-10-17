using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class VerticalAnglesTheorem : Theorem
    {
        private readonly static string NAME = "Vertical Angles Theorem";

        public VerticalAnglesTheorem() { }

        //
        // Intersect(X, Segment(A, B), Segment(C, D)) -> Congruent(Angle(A, X, C), Angle(B, X, D)),
        //                                               Congruent(Angle(A, X, D), Angle(C, X, B))
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            if (!(c is Intersection)) return new List<GroundedClause>();

            Intersection inter = (Intersection)c;
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            // Congruent(Angle(A, X, C), Angle(B, X, D))
            ConcreteAngle ang1Set1 = new ConcreteAngle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1);
            ConcreteAngle ang2Set1 = new ConcreteAngle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2);
            ConcreteCongruentAngles cca1 = new ConcreteCongruentAngles(ang1Set1, ang2Set1, NAME);
            newGrounded.Add(cca1);
            inter.AddSuccessor(cca1);
            cca1.AddPredecessor(Utilities.MakeList<GroundedClause>(inter));


            // Congruent(Angle(A, X, D), Angle(C, X, B))
            ConcreteAngle ang1Set2 = new ConcreteAngle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2);
            ConcreteAngle ang2Set2 = new ConcreteAngle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1);
            ConcreteCongruentAngles cca2 = new ConcreteCongruentAngles(ang1Set2, ang2Set2, NAME);
            newGrounded.Add(cca2);
            inter.AddSuccessor(cca1);
            cca2.AddPredecessor(Utilities.MakeList<GroundedClause>(inter));

            return newGrounded;
        }
    }
}