using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class TransversalPerpendicularToParallelImplyBothPerpendicular : Theorem
    {
        private readonly static string NAME = "If a transversal is perpendicular to one of two parallel lines, then it is perpendicular to the other one also.";

        public TransversalPerpendicularToParallelImplyBothPerpendicular() { }

        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

        //
        // Perpendicular(Segment(E, F), Segment(C, D)),
        // Parallel(Segment(E, F), Segment(A, B)),
        // Intersection(M, Segment(A, B), Segment(C, D)) -> Perpendicular(Segment(A, B), Segment(C, D)) 
        //
        //                                   E       B
        //                                   |       |
        //                              C----|-------|--------D
        //                                   | N     | M
        //                                   |       |
        //                                   F       A
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Parallel) && !(c is Perpendicular) && !(c is Intersection)) return newGrounded;

            if (c is Parallel)
            {
                Parallel newParallel = c as Parallel;

                foreach (Perpendicular perp in candidatePerpendicular)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(perp, newParallel, inter, perp));
                    }
                }

                candidateParallel.Add(newParallel);
            }
            else if (c is Perpendicular)
            {
                Perpendicular newPerp = c as Perpendicular;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(newPerp, parallel, inter, newPerp));
                    }
                }

                candidatePerpendicular.Add(newPerp);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Perpendicular perp in candidatePerpendicular)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(perp, parallel, newIntersection, perp));
                    }
                }

                candidateIntersection.Add(newIntersection);
            }
            else if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is Perpendicular)) return newGrounded;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        if (!inter.Equals(streng.original))
                        {
                            newGrounded.AddRange(CheckAndGeneratePerpendicular(streng.strengthened as Perpendicular, parallel, inter, streng));
                        }
                    }
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> CheckAndGeneratePerpendicular(Perpendicular perp, Parallel parallel, Intersection inter, GroundedClause originalPerp)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The perpendicular intersection must refer to one of the parallel segments
            Segment shared = perp.CommonSegment(parallel);
            if (shared == null) return newGrounded;

            // The other intersection must refer to a segment in the parallel pair
            Segment otherShared = inter.CommonSegment(parallel);
            if (otherShared == null) return newGrounded;

            // The two shared segments must be distinct
            if (shared.Equals(otherShared)) return newGrounded;

            // Transversals must align
            if (!inter.OtherSegment(otherShared).Equals(perp.OtherSegment(shared))) return newGrounded;

            // Strengthen the old intersection to be perpendicular
            Perpendicular newPerp = new Perpendicular(inter, NAME);
            Strengthened strengthenedPerp = new Strengthened(inter, newPerp, NAME);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(originalPerp);
            antecedent.Add(parallel);
            antecedent.Add(inter);

            newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, strengthenedPerp));

            return newGrounded;
        }
    }
}