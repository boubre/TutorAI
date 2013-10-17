using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class SumAnglesInTriangle : Theorem
    {
        private readonly static string NAME = "Sum of Measure of Three Angles in Triangle is 180";

        public SumAnglesInTriangle() { }

        // 
        // Triangle(A, B, C) -> m\angle ABC + m\angle CAB + m\angle BCA = 180^o
        //
        public static List<GroundedClause> Instantiate(GroundedClause c)
        {
            List<GroundedClause> newGrounded = new List<GroundedClause>();

            ConcreteTriangle tri = c as ConcreteTriangle;
            if (tri == null) return newGrounded;

            // Generate, by definition the sum of the three angles equal 180^o
            ConcreteAngle a1 = new ConcreteAngle(tri.Point1, tri.Point2, tri.Point3);
            ConcreteAngle a2 = new ConcreteAngle(tri.Point3, tri.Point1, tri.Point2);
            ConcreteAngle a3 = new ConcreteAngle(tri.Point2, tri.Point3, tri.Point1);
            Addition add = new Addition(a1, a2);
            Addition overallAdd = new Addition(add, a3);
            NumericValue value = new NumericValue(180); // Sum is 180^o
            AngleMeasureEquation eq = new AngleMeasureEquation(overallAdd, value, NAME);

            newGrounded.Add(eq);

            tri.AddSuccessor(eq);
            eq.AddPredecessor(Utilities.MakeList<GroundedClause>(tri));

            return newGrounded;
        }
    }
}