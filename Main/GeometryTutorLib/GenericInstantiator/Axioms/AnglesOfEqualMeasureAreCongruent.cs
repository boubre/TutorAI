using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AnglesOfEqualMeasureAreCongruent : Axiom
    {
        private readonly static string NAME = "Angles of Equal Measure Are Congruent";

        private static List<AngleEquation> candiateAngleEquations = new List<AngleEquation>();

        // Resets all saved data.
        public static void Clear()
        {
            candiateAngleEquations.Clear();
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();



            return newGrounded;
        }
    }
}