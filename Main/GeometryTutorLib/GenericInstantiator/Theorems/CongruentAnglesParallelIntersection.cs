using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;


namespace GeometryTutorLib.GenericInstantiator
{


    public class CongruentAnglesParallelIntersection : Theorem
    {
        private readonly static string NAME = "Congruent Alternate Interior Angles from Parallel Intersection";

        public CongruentAnglesParallelIntersection() { }

        private static List<Intersection> candIntersection = new List<Intersection>(); //All intersections found
        private static List<Parallel> candParallel = new List<Parallel>();  //All parallel sets found

        private static List<GroundedClause> antecedent;
        

        //TODO: this currently describes something else
        // Intersect(X, Segment(A, B), Segment(C, D)) -> Congruent(Angle(A, X, C), Angle(B, X, D)),
        //                                               Congruent(Angle(A, X, D), Angle(C, X, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            
            //Exit if c is neither a parallel set nor an intersection
            if (!(c is Parallel) && !(c is Intersection)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<Intersection> foundCand = new List<Intersection>(); //Variable holding intersections that will used for theorem
            Parallel foundParallelSet = null;
            ConcreteSegment foundTransversal;

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Parallel)
            {
                Parallel newParallel = (Parallel)c;
                candParallel.Add((Parallel)c);

                //Create a list of all segments in the intersection list by individual segment and list of intersecting segments
                var query1 = candIntersection.GroupBy(m => m.lhs, m => m.rhs).Concat(candIntersection.GroupBy(m => m.rhs, m => m.lhs));

                //Iterate through all segments intersected by each key segment
                foreach (var group in query1)
                {
                    if (group.Contains(newParallel.segment1) && group.Contains(newParallel.segment2))
                    {
                        //If a segment that intersected both parallel lines was found, find the intersection objects.  
                        var query2 = candIntersection.Where(m => m.lhs.Equals(group.Key)).Concat(candIntersection.Where(m => m.rhs.Equals(group.Key)));
                        var query3 = candIntersection.Where(m => m.lhs.Equals(newParallel.segment1) || m.lhs.Equals(newParallel.segment2) || m.rhs.Equals(newParallel.segment1) || m.rhs.Equals(newParallel.segment2));
                        foundCand.AddRange(query3);

                        foundParallelSet = newParallel;
                        foundTransversal = group.Key;
                        

                        antecedent = Utilities.MakeList<GroundedClause>(newParallel); //Add parallel set to antecedents
                        
                    }
                }

            }
            else if (c is Intersection)
            {

                candIntersection.Add((Intersection)c);
                Intersection newintersect = (Intersection)c;

                var query1 = candIntersection.GroupBy(m => m.lhs, m => m.rhs).Concat(candIntersection.GroupBy(m => m.rhs, m => m.lhs));

                foreach (Parallel p in candParallel)
                {
                    foreach (var group in query1)
                    {
                        if (group.Contains(p.segment1) && group.Contains(p.segment2))
                        {
                            var query2 = candIntersection.Where(m => m.lhs.Equals(group.Key)).Concat(candIntersection.Where(m => m.rhs.Equals(group.Key)));
                            var query3 = candIntersection.Where(m => m.lhs.Equals(p.segment1) || m.lhs.Equals(p.segment2) || m.rhs.Equals(p.segment1) || m.rhs.Equals(p.segment2));
                            foundCand.AddRange(query3);

                            foundParallelSet = p;
                            foundTransversal = group.Key;
                            antecedent = Utilities.MakeList<GroundedClause>(p);
                           
                        }
                    }
                }

            }

            
            //TODO: Make sure there will only be one set of intersections found at a time
            if (foundCand.Count() > 1)
            {
                antecedent.AddRange((IEnumerable<GroundedClause>)(foundCand));  //Add the two intersections to antecedent
                ConcreteCongruentAngles cca1;
                ConcreteCongruentAngles cca2;

                int seg1index;
                int seg2index;

                //Match first and second intersection points with first and second segments
                if (foundCand[0].lhs == foundParallelSet.segment1 || foundCand[0].rhs == foundParallelSet.segment1)
                {
                    seg1index = 0;
                    seg2index = 1;
                }
                else
                {
                    seg1index = 1;
                    seg2index = 0;
                }

                
                ConcreteAngle ang1Seg1 = new ConcreteAngle(foundParallelSet.segment1.Point1, foundCand[seg1index].intersect, foundCand[seg2index].intersect);
                ConcreteAngle ang2Seg1 = new ConcreteAngle(foundParallelSet.segment1.Point2, foundCand[seg1index].intersect, foundCand[seg2index].intersect);
                ConcreteAngle ang1Seg2 = new ConcreteAngle(foundParallelSet.segment2.Point1, foundCand[seg2index].intersect, foundCand[seg1index].intersect);
                ConcreteAngle ang2Seg2 = new ConcreteAngle(foundParallelSet.segment2.Point2, foundCand[seg2index].intersect, foundCand[seg1index].intersect);
                
               

                //Decide which angles match, cover perpendicular case
                if (ang1Seg1.measure <= ang2Seg1.measure)
                {
                    if (ang1Seg2.measure <= ang2Seg2.measure)
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Seg1, ang1Seg2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Seg1, ang2Seg2, NAME);
                    }
                    else
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Seg1, ang2Seg2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Seg1, ang1Seg2, NAME);
                    }
                }
                else
                {
                    if (ang1Seg2.measure <= ang2Seg2.measure)
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Seg1, ang2Seg2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Seg1, ang1Seg2, NAME);
                    }
                    else
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Seg1, ang1Seg2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Seg1, ang2Seg2, NAME);
                    }
                }

                //Add the two new congruent angle sets
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cca1));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, cca2));
            
            }

            return newGrounded;
        }
    }
}