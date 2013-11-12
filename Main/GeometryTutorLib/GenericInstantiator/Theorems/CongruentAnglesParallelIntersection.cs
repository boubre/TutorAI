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
            List<Intersection> foundCand = new List<Intersection>(); //Variable holding intersections that will used for theorem
            
            //Exit if c is neither a parallel set nor an intersection
            if (!(c is Parallel) && !(c is Intersection)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Parallel)
            {
                Parallel newParallel = (Parallel)c;
                candParallel.Add((Parallel)c);

                antecedent = Utilities.MakeList<GroundedClause>(newParallel);  //TODO: Find out what antecedent is used for

                //Find all intersections that contain one of the parallel segments in newParallel
                IEnumerable<Intersection> query = (IEnumerable<Intersection>)candIntersection.Where(m => m.lhs == newParallel.segment1 || m.rhs == newParallel.segment1 || m.lhs == newParallel.segment2 || m.rhs == newParallel.segment2);
                
                //Make list of intersections in new list that contain an identical value on the left and right side 
                foreach (Intersection inter in query)
                {
                    //TODO: Make more logical search
                    if ((query.Any(m => m.rhs == inter.lhs || m.lhs == inter.rhs || m.lhs == inter.lhs && m.rhs != inter.rhs || m.rhs == inter.rhs && m.lhs != inter.lhs)))
                    {
                        foundCand.Add(inter);  //This will be the pair of intersections on the set of parallel lines
                    }
                }

            }
            else if (c is Intersection)
            {

                candIntersection.Add((Intersection)c);
                Intersection newintersect = (Intersection)c;

                antecedent = Utilities.MakeList<GroundedClause>(newintersect);

                //Make a list of intersections that have one of the segments contained in the new intersection
                IEnumerable<Intersection> intersectsList = (IEnumerable<Intersection>)candIntersection.Where(m => m.lhs == newintersect.lhs || m.lhs == newintersect.rhs || m.rhs == newintersect.lhs || m.rhs == newintersect.rhs);


                foreach (Parallel p in candParallel)
                {
                    foreach (Intersection inter in intersectsList)
                    {
                        ////TODO: Make more logical search
                        if (intersectsList.Any(m => m.rhs == inter.lhs || m.lhs == inter.rhs || m.lhs == inter.lhs && m.rhs != inter.rhs || m.rhs == inter.rhs && m.lhs != inter.lhs))
                        {
                            foundCand.Add(inter);
                        }
                    }
                }

            }

            

            //TODO: Make sure there will only be one set of intersections found at a time
            if (foundCand.Count() > 1)
            {

                ConcreteCongruentAngles cca1;
                ConcreteCongruentAngles cca2;
                
                //TODO: Not sure how math works here
                ConcreteAngle ang1Set1 = new ConcreteAngle(foundCand[0].lhs.Point1, foundCand[0].intersect, foundCand[0].rhs.Point1);
                ConcreteAngle ang2Set1 = new ConcreteAngle(foundCand[0].lhs.Point2, foundCand[0].intersect, foundCand[0].rhs.Point1);
                ConcreteAngle ang1Set2 = new ConcreteAngle(foundCand[1].lhs.Point1, foundCand[1].intersect, foundCand[1].rhs.Point1);
                ConcreteAngle ang2Set2 = new ConcreteAngle(foundCand[1].lhs.Point2, foundCand[1].intersect, foundCand[1].rhs.Point1);

                //Decide which angles match, cover perpendicular case
                if (ang1Set1.measure <= ang2Set1.measure)
                {
                    if (ang1Set2.measure <= ang2Set2.measure)
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Set1, ang1Set2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Set1, ang2Set2, NAME);
                    }
                    else
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Set1, ang2Set2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Set1, ang1Set2, NAME);
                    }
                }
                else
                {
                    if (ang1Set2.measure <= ang2Set2.measure)
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Set1, ang2Set2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Set1, ang1Set2, NAME);
                    }
                    else
                    {
                        cca1 = new ConcreteCongruentAngles(ang1Set1, ang1Set2, NAME);
                        cca2 = new ConcreteCongruentAngles(ang2Set1, ang2Set2, NAME);
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