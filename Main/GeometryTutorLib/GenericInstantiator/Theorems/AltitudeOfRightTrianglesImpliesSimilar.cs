using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AltitudeOfRightTrianglesImpliesSimilar : Theorem
    {
        private readonly static string NAME = "If the altitude is drawn to the hypotenuse of a right triangle, then the two triangles formed are similar to the original triangle and to each other.";

        private AltitudeOfRightTrianglesImpliesSimilar() { }

        private static readonly AltitudeOfRightTrianglesImpliesSimilar thisDescriptor = new AltitudeOfRightTrianglesImpliesSimilar();

        private static List<Altitude> candidateAltitudes = new List<Altitude>();
        private static List<RightTriangle> candRightTriangles = new List<RightTriangle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateAltitudes.Clear();
            candRightTriangles.Clear();
            candidateStrengthened.Clear();
        }

        //
        //   T
        //   |\
        //   | \
        //   |  \ N <-----------Right Angle
        //   | / \
        //   |/___\
        //   U     S
        //
        //  Altitude(Segment(U, N), Triangle(S, U, T)), RightTriangle(S, U, T) -> Similar(RightTriangle(S, U, T), RightTriangle(S, N, U)),
        //                                                                        Similar(RightTriangle(S, N, U), RightTriangle(U, N, T))
        //                                                                        Similar(RightTriangle(U, N, T), RightTriangle(S, U, T))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Altitude) && !(c is RightTriangle) && !(c is Strengthened)) return newGrounded;

            if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is RightTriangle)) return newGrounded;

                foreach (Altitude altitude in candidateAltitudes)
                {
                    newGrounded.AddRange(InstantiateRight(streng.strengthened as RightTriangle, altitude, streng));
                }

                candidateStrengthened.Add(streng);
            }
            else if (c is RightTriangle)
            {
                RightTriangle rt = c as RightTriangle;

                foreach (Altitude altitude in candidateAltitudes)
                {
                    newGrounded.AddRange(InstantiateRight(rt, altitude, rt));
                }

                candRightTriangles.Add(rt);
            }
            else if (c is Altitude)
            {
                Altitude altitude = c as Altitude;

                foreach (RightTriangle rt in candRightTriangles)
                {
                    newGrounded.AddRange(InstantiateRight(rt, altitude, rt));
                }

                foreach (Strengthened stren in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateRight(stren.strengthened as RightTriangle, altitude, stren));
                }

                candidateAltitudes.Add(altitude);
            }

            return newGrounded;
        }

        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateRight(RightTriangle rt, Altitude altitude, GroundedClause original)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The altitude must connect the vertex defining the right angle and the opposite side.
            if (!altitude.segment.HasPoint(rt.rightAngle.GetVertex())) return newGrounded;

            // The other point of the altitude must lie on the opposite side of the triangle
            Point altPointOppRightAngle = altitude.segment.OtherPoint(rt.rightAngle.GetVertex());

            Segment oppositeSide = rt.GetOppositeSide(rt.rightAngle);

            if (!Segment.Between(altPointOppRightAngle, oppositeSide.Point1, oppositeSide.Point2)) return newGrounded;

            //
            // Find the two smaller right triangles in the candidate list (which should be in the list at this point)
            //
            RightTriangle first = null;
            RightTriangle second = null;
            foreach (RightTriangle smallerRT in candRightTriangles)
            {
                if (smallerRT.IsDefinedBy(rt, altitude))
                {
                    if (first == null)
                    {
                        first = smallerRT;
                    }
                    else
                    {
                        second = smallerRT;
                        break;
                    }
                }
            }

            // CTA: We did not check to see points aligned, but these are the original triangles from the figure
            GeometricSimilarTriangles gsts1 = new GeometricSimilarTriangles(rt, first, NAME);
            GeometricSimilarTriangles gsts2 = new GeometricSimilarTriangles(rt, second, NAME);
            GeometricSimilarTriangles gsts3 = new GeometricSimilarTriangles(first, second, NAME);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(altitude);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gsts1));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gsts2));
            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, gsts3));

            return newGrounded;
        }
    }
}