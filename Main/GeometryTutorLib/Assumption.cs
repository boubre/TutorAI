﻿using System;
using System.Collections.Generic;

namespace GeometryTutorLib
{
    /// <summary>
    /// This class is used to represent a mathematical assumption (axiom, definition, or theorem) that the user can enable or disable.
    /// </summary>
    public class Assumption
    {
        /// <summary>
        /// Determines if an Assumption is an axiom, a definition, or a theorem.
        /// </summary>
        public enum AssumptionType { Axiom = 0, Definition, Theorem };

        public String Name { get; private set; }
        public AssumptionType Type { get; private set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// All assumptions that are recognized by the back end.
        /// </summary>
        private static Dictionary<string, Assumption> Assumptions = null;

        /// <summary>
        /// Create a new Assumption.
        /// </summary>
        /// <param name="name">The name of the Assumption (such as Segment Addition).</param>
        /// <param name="type">The type of this Assumption.</param>
        private Assumption(String name, AssumptionType type)
        {
            this.Name = name;
            this.Type = type;
            Enabled = true;
        }

        /// <summary>
        /// Return the list of all possible Assumptions. If the list does not yet exist it will be created.
        /// </summary>
        /// <returns>A list of all assumptions.</returns>
        public static Dictionary<string, Assumption> GetAssumptions()
        {
            if (Assumptions == null)
            {
                CreateAssumptions();
            }
            return Assumptions;
        }

        /// <summary>
        /// Create a list of all assumptions. Each assumption is enabled by default.
        /// </summary>
        private static void CreateAssumptions()
        {
            var assumptions = new List<Assumption>();

            assumptions.Add(new Assumption("Segment Addition", AssumptionType.Axiom));
            assumptions.Add(new Assumption("Angle Addition", AssumptionType.Axiom));
            assumptions.Add(new Assumption("Corresponding Angles of Parallel Lines", AssumptionType.Axiom));
            assumptions.Add(new Assumption("SSS", AssumptionType.Axiom));
            assumptions.Add(new Assumption("SAS", AssumptionType.Axiom));
            assumptions.Add(new Assumption("ASA", AssumptionType.Axiom));
            assumptions.Add(new Assumption("AA Similarity", AssumptionType.Axiom));

            assumptions.Add(new Assumption("Altitude", AssumptionType.Definition));
            assumptions.Add(new Assumption("Angle Bisector", AssumptionType.Definition));
            assumptions.Add(new Assumption("Isosceles Triangle", AssumptionType.Definition));
            assumptions.Add(new Assumption("Median", AssumptionType.Definition));
            assumptions.Add(new Assumption("Midpoint", AssumptionType.Definition));
            assumptions.Add(new Assumption("Right Triangle", AssumptionType.Definition));
            assumptions.Add(new Assumption("Segment Bisector", AssumptionType.Definition));
            assumptions.Add(new Assumption("Stright Angle", AssumptionType.Definition));

            assumptions.Add(new Assumption("Midpoint Theorem", AssumptionType.Theorem));
            assumptions.Add(new Assumption("Angle Bisector Theorem", AssumptionType.Theorem));
            for (int i = 3; i <= 8; i++)
                assumptions.Add(new Assumption("1-"+i, AssumptionType.Theorem));
            for (int i = 2; i <= 7; i++)
                assumptions.Add(new Assumption("2-"+i, AssumptionType.Theorem));
            assumptions.Add(new Assumption("2-10", AssumptionType.Theorem));
            assumptions.Add(new Assumption("2-11", AssumptionType.Theorem));
            assumptions.Add(new Assumption("2-11 Corollary 4", AssumptionType.Theorem));
            assumptions.Add(new Assumption("2-12", AssumptionType.Theorem));
            assumptions.Add(new Assumption("3-1", AssumptionType.Theorem));
            assumptions.Add(new Assumption("3-1 Corollary 1", AssumptionType.Theorem));
            assumptions.Add(new Assumption("3-1 Corollary 2", AssumptionType.Theorem));
            assumptions.Add(new Assumption("3-1 Corollary 3", AssumptionType.Theorem));
            assumptions.Add(new Assumption("3-2", AssumptionType.Theorem));
            assumptions.Add(new Assumption("HL Theorem", AssumptionType.Theorem));
            assumptions.Add(new Assumption("SAS Similarity Theorem", AssumptionType.Theorem));
            assumptions.Add(new Assumption("SSS Similarity Theorem", AssumptionType.Theorem));
            assumptions.Add(new Assumption("Triangle Proportionality Theorem", AssumptionType.Theorem));

            //Add all assumptions in the list to the dictionary
            Assumptions = new Dictionary<string, Assumption>();
            foreach (Assumption a in assumptions)
            {
                Assumptions.Add(a.Name, a);
            }
        }

        public override string ToString()
        {
            string assumptionType;
            switch (Type)
            {
                case Assumption.AssumptionType.Axiom:
                    assumptionType = "Axiom";
                    break;
                case Assumption.AssumptionType.Definition:
                    assumptionType = "Def";
                    break;
                case Assumption.AssumptionType.Theorem:
                    assumptionType = "Thm";
                    break;
                default:
                    assumptionType = "";
                    break;
            }
            return assumptionType + ": " + Name;
        }
    }
}