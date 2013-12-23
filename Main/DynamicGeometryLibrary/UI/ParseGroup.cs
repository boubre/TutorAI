using System.Collections.Generic;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    public class ParseGroup
    {
        private static List<ParseGroup> Groups = null;

        public List<Assumption> Assumptions { get; set; }
        public string Name { get; set; }

        private ParseGroup(string Name)
        {
            this.Name = Name;
            Assumptions = new List<Assumption>();
            Groups.Add(this);
        }

        public static ParseGroup AddParseGroup(string Name)
        {
            if (Groups == null)
            {
                CreatePredefGroups();
            }
            return new ParseGroup(Name);
        }

        public static ParseGroup AddParseGroup(string Name, List<Assumption> Assumptions)
        {
            ParseGroup pg = AddParseGroup(Name);
            pg.Assumptions.AddRange(Assumptions);
            return pg;
        }

        public static List<ParseGroup> GetParseGroups()
        {
            if (Groups == null)
            {
                CreatePredefGroups();
            }
            return Groups;
        }

        public override string ToString()
        {
            return Name;
        }

        private static void CreatePredefGroups()
        {
            Groups = new List<ParseGroup>();
            List<Assumption> assumptions = Assumption.GetAssumptions();

            //All assumptions
            ParseGroup pg = new ParseGroup("All");
            pg.Assumptions.AddRange(assumptions);

            //Axioms
            pg = new ParseGroup("Axioms");
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Axiom)
                {
                    pg.Assumptions.Add(a);
                }
            }

            //Definitions
            pg = new ParseGroup("Definitions");
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Definition)
                {
                    pg.Assumptions.Add(a);
                }
            }

            //Theorems
            pg = new ParseGroup("Theorems");
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Theorem)
                {
                    pg.Assumptions.Add(a);
                }
            }
        }
    }
}
