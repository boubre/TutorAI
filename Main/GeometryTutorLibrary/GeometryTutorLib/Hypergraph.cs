using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericAbstractSyntax;

namespace GeometryTutorLib
{
    public class Hypergraph
    {
        // List of ground clauses
        List<ConcreteAbstractSyntax.GroundClause> grounds;
        // List of generic rules (definitions, theorems, axioms) that may be instantiated
        List<GenericAbstractSyntax.GenericRule> generics;

        public Hypergraph(List<ConcreteAbstractSyntax.GroundClause> grs, List<GenericAbstractSyntax.GenericRule> gens)
        {
            grounds = grs;
            generics = gens;
        }

        public void ConstructGraph()
        {
            //
            // The worklist which will initial ground clauses
            //
            List<ConcreteAbstractSyntax.GroundClause> groundWorklist = new List<ConcreteAbstractSyntax.GroundClause>(grounds);
            List<GroundClause> groundChecked = new List<GroundClause>();

            while (groundWorklist.Any())
            {
                GroundClause currentClause = groundWorklist.ElementAt(0);
                groundWorklist.RemoveAt(0);

                foreach (GenericRule gen in generics)
                {
                    // Does this clause unify with some clause in the generic?
                    if (gen.MayUnifyWith(currentClause))
                    {
                        // Can this generic now be completely instantiated with this new clause?
                        List<GroundClause> instList = gen.Instantiate(currentClause);
                        if (instList != null)
                        {
                            // If completely instantiated, add the deduced clauses to the worklist
                            groundWorklist.AddRange(instList);
                        }

                        // Make this ground clause a candidate for complete instantiation via unification
                        gen.AddCandidateClause(currentClause);
                    }
                }
                groundChecked.Add(currentClause);
            }
        }
    }
}