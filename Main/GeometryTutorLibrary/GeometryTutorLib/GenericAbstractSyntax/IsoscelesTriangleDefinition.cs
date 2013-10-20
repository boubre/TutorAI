using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class IsoscelesTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Isosceles Triangle";

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return (c is ConcreteCongruentSegments) || (c is EqualSegments) || (c is ConcreteTriangle);
        }

        private static List<EqualSegments> unifyCandEqSegments = new List<EqualSegments>();
        private static List<ConcreteCongruentSegments> unifyCandCongSegments = new List<ConcreteCongruentSegments>();
        private static List<ConcreteTriangle> unifyCandTriangles = new List<ConcreteTriangle>();

        //
        // In order for two triangles to be isosceles, we require the following:
        //    Triangle(A, B, C), Equal(Segment(A, B), Segment(B, C)) -> IsoscelesTriangle(A, B, C)
        //
        //    Triangle(A, B, C), Congruent(Segment(A, B), Segment(B, C)) -> IsoscelesTriangle(A, B, C)
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!MayUnifyWith(c)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Do we have enough information for unification?
            //
            if (c is ConcreteCongruentSegments && !unifyCandTriangles.Any())
            {
                unifyCandCongSegments.Add((ConcreteCongruentSegments)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is EqualSegments && !unifyCandTriangles.Any())
            {
                unifyCandEqSegments.Add((EqualSegments)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is ConcreteTriangle && !(unifyCandEqSegments.Any() || unifyCandCongSegments.Any()))
            {
                unifyCandTriangles.Add((ConcreteTriangle)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            //
            // Unify
            //
            if (c is ConcreteCongruentSegments)
            {
                ConcreteCongruentSegments ccss = (ConcreteCongruentSegments)c;

                foreach (ConcreteTriangle tri in unifyCandTriangles)
                {
                    if (tri.HasSegment(ccss.cs1) && tri.HasSegment(ccss.cs2))
                    {
                        // This triangle is isosceles
                        tri.MakeIsosceles(); // Make any new nodes?

                        //Console.WriteLine("Is an Isosceles Triangle: " + tri.ToString());

                        // There should be only one possible Isosceles triangle from this congruent segments
                        break;
                    }
                }

                unifyCandCongSegments.Add((ConcreteCongruentSegments)c);
                return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();
            }
            else if (c is EqualSegments)
            {
                EqualSegments eqSegments = (EqualSegments)c;

                foreach (ConcreteTriangle tri in unifyCandTriangles)
                {
                    if (tri.HasSegment(eqSegments.segment1) && tri.HasSegment(eqSegments.segment2))
                    {
                        // This triangle is isosceles
                        tri.MakeIsosceles(); // Make any new nodes?

                        //Console.WriteLine("Is an Isosceles Triangle: " + tri.ToString());

                        // There should be only one possible Isosceles triangle from this congruent segments
                        break;
                    }
                }

                unifyCandEqSegments.Add((EqualSegments)c);
            }
            else if (c is ConcreteTriangle)
            {
                ConcreteTriangle newTriangle = (ConcreteTriangle)c;
                //
                // Do any of the congruent segment pairs merit calling this new triangle isosceles?
                //
                bool foundIso = false;
                foreach (ConcreteCongruentSegments ccss in unifyCandCongSegments)
                {
                    if (newTriangle.HasSegment(ccss.cs1) && newTriangle.HasSegment(ccss.cs2))
                    {
                        // This triangle is isosceles
                        newTriangle.MakeIsosceles(); // Make any new nodes?

                        //Console.WriteLine("Is an Isosceles Triangle: " + newTriangle.ToString());
                        foundIso = true;
                        break;
                    }
                }

                if (!foundIso)
                {
                    foreach (EqualSegments eqSegs in unifyCandEqSegments)
                    {
                        if (newTriangle.HasSegment(eqSegs.segment1) && newTriangle.HasSegment(eqSegs.segment2))
                        {
                            // This triangle is isosceles
                            newTriangle.MakeIsosceles(); // Make any new nodes?

                            //Console.WriteLine("Is an Isosceles Triangle: " + newTriangle.ToString());

                            break;
                        }
                    }
                }

                unifyCandTriangles.Add(newTriangle);
            }

            return newGrounded;
        }
    }
}