using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class IsoscelesTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Isosceles Triangle";

        private IsoscelesTriangleDefinition() { }
        private static readonly IsoscelesTriangleDefinition thisDescriptor = new IsoscelesTriangleDefinition();

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return (c is CongruentSegments) || (c is Triangle);
        }

        private static List<CongruentSegments> candSegs = new List<CongruentSegments>();
        private static List<Triangle> candTris = new List<Triangle>();

        //
        // In order for two triangles to be isosceles, we require the following:
        //
        //    Triangle(A, B, C), Congruent(Segment(A, B), Segment(B, C)) -> IsoscelesTriangle(A, B, C)
        //
        //  This does not generate a new clause explicitly; it simply strengthens the existent object
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!MayUnifyWith(c)) return newGrounded;

            //
            // Unify
            //
            if (c is CongruentSegments)
            {
                CongruentSegments css = c as CongruentSegments;

                // Only generate or add to possible congruent pairs if this is a non-reflexive relation
                if (!css.IsReflexive())
                {
                    for (int t = 0; t < candTris.Count; t++)
                    {
                        if (candTris[t].HasSegment(css.cs1) && candTris[t].HasSegment(css.cs2))
                        {
//                            candTris[t].MakeIsosceles();

                            newGrounded.Add(StregnthenToIsosceles(candTris[t], css));

                            // There should be only one possible Isosceles triangle from this congruent segments
                            // So we can remove this relationship and triangle from consideration
                            candTris.RemoveAt(t);

                            return newGrounded;
                        }
                    }

                    candSegs.Add(c as CongruentSegments);
                }

                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            else if (c is Triangle)
            {
                // Don't strengthen an Isosceles to an Isosceles triangle
                if (c is IsoscelesTriangle) return InstantiateGivenDefinition(c as IsoscelesTriangle);

                Triangle newTriangle = c as Triangle;

                //
                // Do any of the congruent segment pairs merit calling this new triangle isosceles?
                //
                for (int cs = 0; cs < candSegs.Count; cs++)
                {
                    // No need to check for this, in theory, since we never add any reflexive expressions to the list
                    if (!candSegs[cs].IsReflexive())
                    {
                        if (newTriangle.HasSegment(candSegs[cs].cs1) && newTriangle.HasSegment(candSegs[cs].cs2))
                        {
//                            newTriangle.MakeIsosceles();

                            newGrounded.Add(StregnthenToIsosceles(newTriangle, candSegs[cs]));

                            return newGrounded;
                        }
                    }
                }

                // Add the the list of candidates if it was not determined isosceles now.
                candTris.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // DO NOT generate a new clause, instead, report the result and generate all applicable
        // clauses attributed to this strengthening of a triangle from scalene to isosceles
        //
        private static KeyValuePair<List<GroundedClause>, GroundedClause> StregnthenToIsosceles(Triangle tri, CongruentSegments ccss)
        {
            Strengthened newStrengthened = new Strengthened(tri, new IsoscelesTriangle(tri, "Strengthened"), NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(ccss);
            antecedent.Add(tri);

            return new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newStrengthened);
        }

        //
        // IsoscelesTriangle(A, B, C) -> Congruent(Segment(A, B), Segment(A, C))
        //
        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateGivenDefinition(IsoscelesTriangle iso)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            GeometricCongruentSegments gcs = new GeometricCongruentSegments(iso.leg1, iso.leg2, NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(iso);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gcs));

            return newGrounded;
        }
    }
}