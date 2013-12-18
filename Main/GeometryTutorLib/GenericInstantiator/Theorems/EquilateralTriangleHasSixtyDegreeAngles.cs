using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class EquilateralTriangleHasSixtyDegreeAngles : Theorem
    {
        private readonly static string NAME = "An equilateral triangle has three sixty degree angles.";

        public EquilateralTriangleHasSixtyDegreeAngles() { }

        //
        // EquilateralTriangle(A, B, C) -> Equation(m \angle ABC = 60),  
        //                                 Equation(m \angle BCA = 60),
        //                                 Equation(m \angle CAB = 60)
        //

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is EquilateralTriangle) && !(c is Strengthened)) return newGrounded;

            EquilateralTriangle eqTri = c as EquilateralTriangle;

            if (c is Strengthened)
            {
                eqTri = (c as Strengthened).strengthened as EquilateralTriangle; 
            }

            if (eqTri == null) return newGrounded;

            // EquilateralTriangle(A, B, C) ->
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(c);

            //                              -> Equation(m \angle ABC = 60),  
            //                                 Equation(m \angle BCA = 60),
            //                                 Equation(m \angle CAB = 60)
            GeometricAngleEquation eq1 = new GeometricAngleEquation(eqTri.AngleA, new NumericValue(60), NAME);
            GeometricAngleEquation eq2 = new GeometricAngleEquation(eqTri.AngleB, new NumericValue(60), NAME);
            GeometricAngleEquation eq3 = new GeometricAngleEquation(eqTri.AngleC, new NumericValue(60), NAME);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq2));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, eq3));

            return newGrounded;
        }
    }
}