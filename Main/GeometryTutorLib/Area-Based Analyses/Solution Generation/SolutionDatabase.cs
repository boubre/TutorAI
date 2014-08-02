using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class SolutionDatabase
    {
        // private Dictionary<IndexList, SolutionAgg> Solutions;
        private Dictionary<IndexList, SolutionAgg> rootSolutions;
        private Dictionary<IndexList, SolutionAgg> extendedSolutions;

        public SolutionDatabase(int size)
        {
            rootSolutions = new Dictionary<IndexList,SolutionAgg>();
            extendedSolutions = new Dictionary<IndexList,SolutionAgg>(size);
        }

        public bool TryGetValue(IndexList indices, out SolutionAgg solutionAgg)
        {
            if (rootSolutions.TryGetValue(indices, out solutionAgg)) return true;

            if (extendedSolutions.TryGetValue(indices, out solutionAgg)) return true;

            solutionAgg = null;
            return false;
        }

        public bool Contains(SolutionAgg solution)
        {
            return Contains(solution.atomIndices);
        }

        public bool Contains(IndexList indices)
        {
            return rootSolutions.ContainsKey(indices) || extendedSolutions.ContainsKey(indices);
        }

        public bool AddRootSolution(SolutionAgg solution)
        {
            return AddSolution(rootSolutions, solution);
        }

        public bool AddExtendedSolution(SolutionAgg solution)
        {
            if (rootSolutions.ContainsKey(solution.atomIndices)) return false;

            return AddSolution(extendedSolutions, solution);
        }

        public List<SolutionAgg> GetRootSolutions()
        {
            return new List<SolutionAgg>(rootSolutions.Values);
        }

        public List<SolutionAgg> GetExtendedSolutions()
        {
            return new List<SolutionAgg>(extendedSolutions.Values);
        }

        public int GetNumComputable()
        {
            int computable = 0;
            foreach (KeyValuePair<IndexList, SolutionAgg> pair in rootSolutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.COMPUTABLE) computable++;
            }
            foreach (KeyValuePair<IndexList, SolutionAgg> pair in extendedSolutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.COMPUTABLE) computable++;
            }

            return computable;
        }

        public int GetNumIncomputable()
        {
            int incomputable = 0;

            foreach (KeyValuePair<IndexList, SolutionAgg> pair in rootSolutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.INCOMPUTABLE) incomputable++;
            }
            foreach (KeyValuePair<IndexList, SolutionAgg> pair in extendedSolutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.INCOMPUTABLE) incomputable++;
            }

            return incomputable;
        }

        //
        // Acquire a single solution equation and area value.
        //
        public KeyValuePair<ComplexRegionEquation, double> GetSolution(List<Atomizer.AtomicRegion> figureAtoms, List<Atomizer.AtomicRegion> desiredRegions)
        {
            IndexList indices = IndexList.AcquireAtomicRegionIndices(figureAtoms, desiredRegions);

            SolutionAgg solutionAgg = null;
            if (!rootSolutions.TryGetValue(indices, out solutionAgg))
            {
                extendedSolutions.TryGetValue(indices, out solutionAgg);
            }

            if (solutionAgg == null)
            {
                throw new ArgumentException("Could not find a solution in the database.");
            }

            return new KeyValuePair<ComplexRegionEquation, double>(solutionAgg.solEq, solutionAgg.solArea);
        }

        //
        // Adds an equation, if it does not exist.
        // If an equation for the region already exists, take the shortest one or the one that is computable.
        //
        private bool AddSolution(Dictionary<IndexList, SolutionAgg> solDictionary, SolutionAgg that)
        {
            //
            // Does this equation NOT exist in the database?
            //
            SolutionAgg existentAgg = null;
            if (!solDictionary.TryGetValue(that.atomIndices, out existentAgg))
            {
                // Add this solution to the database.
                solDictionary.Add(that.atomIndices, that);

                return true;
            }
            //
            // The equation already exists in the database.
            //

            // Favor a straight-forward calculation of the area (no manipulations to acquire the value).
            if (existentAgg.IsDirectArea()) return false;

            // Favor a coomputable equation over incomputable.
            if (existentAgg.solType == SolutionAgg.SolutionType.INCOMPUTABLE && that.solType == SolutionAgg.SolutionType.COMPUTABLE)
            {
                solDictionary[that.atomIndices] = that;
                return true;
            }
            // Again, favor a computable solution over not.
            else if (existentAgg.solType == SolutionAgg.SolutionType.COMPUTABLE && that.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
            {
                // NO-OP
            }
            // The computability is the same for both equations.
            else if (existentAgg.solType == that.solType || existentAgg.solType == SolutionAgg.SolutionType.UNKNOWN)
            {
                if (!Utilities.CompareValues(existentAgg.solArea, that.solArea))
                {
                    throw new Exception("Area for region " + existentAgg.atomIndices.ToString() +
                                        " was calculated now as (" + existentAgg.solArea + ") AND before (" + that.solArea + ")");
                }

                if (existentAgg.solEq.Length > that.solEq.Length)
                {
                    solDictionary[that.atomIndices] = that;
                    return true;
                }
            }

            return false;
        }
    }
}