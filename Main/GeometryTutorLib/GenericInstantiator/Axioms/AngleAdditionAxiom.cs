using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleAdditionAxiom : Axiom
    {
        private readonly static string NAME = "Angle Addition Axiom";

        // Candidate angles
        private static List<Angle> unifyCandAngles = new List<Angle>();

        //
        // Angle(A, B, C), Angle(C, B, D) -> Angle(A, B, C) + Angle(C, B, D) = Angle(A, B, D)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Angle)) return newGrounded;

            Angle newAngle = c as Angle;

            //
            // Determine if another angle in the candidate unify list can be combined with this new angle
            //
            foreach (Angle ang in unifyCandAngles)
            {
                newGrounded.AddRange(InstantiateAngles(newAngle, ang));
            }

            // Add this angle to the unifying candidates
            unifyCandAngles.Add(newAngle);

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateAngles(Angle angle1, Angle angle2)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // An angle may have multiple names
            if (angle1.Equates(angle2)) return newGrounded;

            if (!angle1.GetVertex().Equals(angle2.GetVertex())) return newGrounded;

            // Determine the shared segment if we have an adjacent situation
            Segment shared = angle1.IsAdjacentTo(angle2);

            if (shared == null) return newGrounded;

            //
            // If we combine these two angles, the result is a third angle, which, when measured,
            // would be less than 180; this is contradictory since we measuare angles greedily and no circular angle is measured as > 180 
            //
            if (angle1.measure + angle2.measure > 180) return newGrounded;

            // Angle(A, B, C), Angle(C, B, D) -> Angle(A, B, C) + Angle(C, B, D) = Angle(A, B, D)
            Point vertex = angle1.GetVertex();
            Point exteriorPt1 = angle2.OtherPoint(shared);
            Point exteriorPt2 = angle1.OtherPoint(shared);
            Angle newAngle = new Angle(exteriorPt1, vertex, exteriorPt2);
            Addition sum = new Addition(angle1, angle2);
            GeometricAngleEquation geoAngEq = new GeometricAngleEquation(sum, newAngle, NAME);
            geoAngEq.MakeAxiomatic(); // This is an axiomatic equation

            // For hypergraph construction
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(angle1);
            antecedent.Add(angle2);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, geoAngEq));

            return newGrounded;
        }
    }
}