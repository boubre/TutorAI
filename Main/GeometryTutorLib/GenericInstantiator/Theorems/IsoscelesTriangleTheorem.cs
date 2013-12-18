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

            if (!(c is IsoscelesTriangle)) return newGrounded;

            IsoscelesTriangle iso = c as IsoscelesTriangle;

            GeometricCongruentAngles newCongSegs = new GeometricCongruentAngles(iso.baseAngleOppositeLeg1, iso.baseAngleOppositeLeg2, NAME); // ... thisDescriptor)

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(iso);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newCongSegs));

            return newGrounded;
        }
    }
}