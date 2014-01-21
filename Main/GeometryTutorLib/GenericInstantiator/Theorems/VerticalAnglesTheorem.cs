using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class VerticalAnglesTheorem : Theorem
    {
        private readonly static string NAME = "Vertical Angles Theorem";

        //
        // Intersect(X, Segment(A, B), Segment(C, D)) -> Congruent(Angle(A, X, C), Angle(B, X, D)),
        //                                               Congruent(Angle(A, X, D), Angle(C, X, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Intersection)) return newGrounded;

            Intersection inter = c as Intersection;

            //
            // Verify that this intersection is composed of two overlapping segments
            // That is, we do not allow a segment to stand on another:
            //      \
            //       \
            //        \
            //   ______\_______
            //
            if (inter.StandsOn()) return newGrounded;

            //
            // Congruent(Angle(A, X, C), Angle(B, X, D))
            //
            List<GroundedClause> antecedent1 = Utilities.MakeList<GroundedClause>(inter); 
            Angle ang1Set1 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
            Angle ang2Set1 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
            antecedent1.Add(ang1Set1);
            antecedent1.Add(ang2Set1);
            GeometricCongruentAngles cca1 = new GeometricCongruentAngles(ang1Set1, ang2Set1, NAME);
            cca1.MakeIntrinsic(); // This is an 'obvious' notion so it should be intrinsic to any figure
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent1, cca1));

            //
            // Congruent(Angle(A, X, D), Angle(C, X, B))
            //
            List<GroundedClause> antecedent2 = Utilities.MakeList<GroundedClause>(inter);
            Angle ang1Set2 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
            Angle ang2Set2 = Angle.AcquireFigureAngle(new Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
            antecedent2.Add(ang1Set2);
            antecedent2.Add(ang2Set2);
            GeometricCongruentAngles cca2 = new GeometricCongruentAngles(ang1Set2, ang2Set2, NAME);
            cca2.MakeIntrinsic(); // This is an 'obvious' notion so it should be intrinsic to any figure
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent2, cca2));

            return newGrounded;
        }
    }
}