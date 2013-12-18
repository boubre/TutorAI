using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleBisectorTheorem : Theorem
    {
        private readonly static string NAME = "Angle Bisector Theorem";

        public AngleBisectorTheorem() { }

        //
        // AngleBisector(SegmentA, D), Angle(C, A, B)) -> 2 m\angle CAD = m \angle CAB,
        //                                                2 m\angle BAD = m \angle CAB
        //
        //   A ________________________B
        //    |\
        //    | \ 
        //    |  \
        //    |   \
        //    |    \
        //    C     D
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is AngleBisector)) return newGrounded;

            AngleBisector angleBisector = c as AngleBisector;
            KeyValuePair<Angle, Angle> adjacentAngles = angleBisector.GetBisectedAngles();

            // Construct 2 m\angle CAD
            Multiplication product1 = new Multiplication(new NumericValue(2), adjacentAngles.Key);
            // Construct 2 m\angle BAD
            Multiplication product2 = new Multiplication(new NumericValue(2), adjacentAngles.Value);

            // 2X = AB
            GeometricAngleEquation newEq1 = new GeometricAngleEquation(product1, angleBisector.angle, NAME);
            GeometricAngleEquation newEq2 = new GeometricAngleEquation(product2, angleBisector.angle, NAME);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(angleBisector);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEq1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newEq2));

            return newGrounded;
        }
    }
}