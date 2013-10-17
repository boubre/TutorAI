using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class Simplification : GenericRule
    {
        private static readonly string NAME = "Simplification";
        //
        // Given an equation, attempt to simplify algebraically using the following notions:
        //     A + A = B  -> 2A = B
        //     A + B = B + C -> A = C
        //     A + B = 2B + C -> A = B + C
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            Equation eq = c as Equation;
            // Do we have an equation?
            if (eq == null) return new List<GroundedClause>();

            List<GroundedClause> newGrounded = new List<GroundedClause>();

            // Remove all subtractions -> adding a negative instead
            // Distribute subtraction or multiplication over addition
            Equation flattened = eq.Flatten();

            //Console.WriteLine("Flattened: " + flattened);

            // Combine terms only on each side (do not cross =)
//            CombineLikeTerms(flattened);

            // Combine terms across the equal sign
//            CombineLikeTermsAcrossEqual(flattened);

            newGrounded.Add(flattened);
            
            return newGrounded;
        }

        //private static GroundedClause CombineLikeTerms(Equation eq)
        //{
        //}

        //private static GroundedClause CombineLikeTermsAcrossEqual(Equation flattened)
        //{
        //}
    }
}