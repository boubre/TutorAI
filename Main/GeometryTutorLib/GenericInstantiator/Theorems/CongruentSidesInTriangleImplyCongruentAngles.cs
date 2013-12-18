using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentSidesInTriangleImplyCongruentSegments : Theorem
    {
        private readonly static string NAME = "If two segments of a triangle are congruent, then the angles opposite those segments are congruent.";

        private CongruentSidesInTriangleImplyCongruentSegments() { }
        private static readonly CongruentSidesInTriangleImplyCongruentSegments thisDescriptor = new CongruentSidesInTriangleImplyCongruentSegments();

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return (c is CongruentSegments) || (c is Triangle);
        }

        private static List<CongruentSegments> candSegs = new List<CongruentSegments>();
        private static List<Triangle> candTris = new List<Triangle>();

        //
        //       A
        //      / \
        //     B---C
        //
        // Triangle(A, B, C), Congruent(Segment(A, B), Segment(A, C)) -> Congruent(\angle ABC, \angle ACB)
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
                            newGrounded.Add(GenerateCongruentAngles(candTris[t], css));

                            // There should be only one possible Isosceles triangle from this congruent Segments
                            // So we can remove this relationship and triangle from consideration
                            candTris.RemoveAt(t);

                            return newGrounded;
                        }
                    }

                    candSegs.Add(css);
                }

                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            else if (c is Triangle)
            {
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
                            newGrounded.Add(GenerateCongruentAngles(newTriangle, candSegs[cs]));

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
        // Just generate the new angle congruence
        //
        private static KeyValuePair<List<GroundedClause>, GroundedClause> GenerateCongruentAngles(Triangle tri, CongruentSegments css)
        {
            GeometricCongruentAngles newConAngs = new GeometricCongruentAngles(tri.GetOppositeAngle(css.cs1), tri.GetOppositeAngle(css.cs2), NAME); // ..., thisDescriptor)

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(css);
            antecedent.Add(tri);

            return new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newConAngs);
        }
    }
}