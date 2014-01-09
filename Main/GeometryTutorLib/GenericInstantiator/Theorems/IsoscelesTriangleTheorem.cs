using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class IsoscelesTriangleTheorem : Theorem
    {
        private readonly static string NAME = "Isosceles Triangle Theorem: Base Angles are Congruent";

        private IsoscelesTriangleTheorem() { }
        private static readonly IsoscelesTriangleTheorem thisDescriptor = new IsoscelesTriangleTheorem();

        //
        // In order for two triangles to be isosceles, we require the following:
        //    IsoscelesTriangle(A, B, C) -> \angle BAC \cong \angle BCA
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>,GroundedClause>>();

            if (c is IsoscelesTriangle) return InstantiateTheorem(c, c as IsoscelesTriangle);

            Strengthened streng = c as Strengthened;
            if (streng != null)
            {
                if (streng.strengthened is IsoscelesTriangle)
                {
                    return InstantiateTheorem(c, streng.strengthened as IsoscelesTriangle);
                }
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateTheorem(GroundedClause original, IsoscelesTriangle isoTri)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            GeometricCongruentAngles newCongSegs = new GeometricCongruentAngles(isoTri.baseAngleOppositeLeg1, isoTri.baseAngleOppositeLeg2, NAME); // ... thisDescriptor)

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newCongSegs));

            return newGrounded;
        }
    }
}