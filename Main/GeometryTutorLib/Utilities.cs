﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GeometryTutorLib
{
    public static class Utilities
    {
        public static readonly bool DEBUG = true;
        public static readonly bool CONSTRUCTION_DEBUG = false;  // Generating clauses when analyzing input figure
        public static readonly bool PEBBLING_DEBUG = true; // Hypergraph edges and pebbled nodes
        public static readonly bool PROBLEM_GEN_DEBUG = false; // Generating the actual problems

        // Given a sorted list, insert the element from the back to the front.
        public static void InsertOrdered(List<int> list, int value)
        {
            // Special Cases
            if (!list.Any() || value > list[list.Count - 1])
            {
                list.Add(value);
                return;
            }
            if (value < list[0])
            {
                list.Insert(0, value);
                return;
            }

            // General Case
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (value > list[i])
                {
                    list.Insert(i + 1, value);
                }
            }
        }

        // Acquire the index of the clause in the hypergraph based only on structure
        public static int StructuralIndex(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, int> graph, ConcreteAST.GroundedClause g)
        {
            //
            // Handle general case
            //
            List<Hypergraph.HyperNode<ConcreteAST.GroundedClause, int>> vertices = graph.vertices;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data.StructurallyEquals(g)) return v;

                if (vertices[v].data is ConcreteAST.Strengthened)
                {
                    if ((vertices[v].data as ConcreteAST.Strengthened).strengthened.StructurallyEquals(g)) return v;
                }
            }

            //
            // Handle strengthening by seeing if the clause is found without a 'strengthening' component
            //
            ConcreteAST.Strengthened streng = g as ConcreteAST.Strengthened;
            if (streng != null)
            {
                int index = StructuralIndex(graph, streng.strengthened);
                if (index != -1) return index;
            }

            return -1;
        }

        // Ensure uniqueness of additions
        public static void AddUniqueStructurally(List<GeometryTutorLib.ConcreteAST.Figure> figures, GeometryTutorLib.ConcreteAST.Figure f)
        {
            foreach (GeometryTutorLib.ConcreteAST.Figure figure in figures)
            {
                if (figure.StructurallyEquals(f)) return;
            }
            figures.Add(f);
        }

        // -1 is an error
        public static int IntegerRatio(double x, double y)
        {
            return Utilities.CompareValues(x / y, Math.Floor(x / y)) ? (int)Math.Floor(x / y) : -1;
        }

        // -1 is an error
        // A reasonable value for geometry problems must be less than 10 for a ratio
        // This is arbitrarily chosen and can be modeified
        private static readonly int RATIO_MAX = 10;
        public static KeyValuePair<int, int> RationalRatio(double x, double y)
        {
            for (int numer = 2; numer < RATIO_MAX; numer++)
            {
                for (int denom = 1; denom < RATIO_MAX; denom++)
                {
                    if (numer != denom)
                    {
                        if (Utilities.CompareValues(x / y, (double)(numer) / denom))
                        {
                            int gcd = GCD(numer, denom);
                            return numer > denom ? new KeyValuePair<int, int>(numer / gcd, denom / gcd)
                                                 : new KeyValuePair<int, int>(denom / gcd, numer / gcd);
                        }
                    }
                }
            }

            return new KeyValuePair<int,int>(-1, -1);
        }
        public static KeyValuePair<int, int> RationalRatio(double x)
        {
            for (int val = 2; val < RATIO_MAX; val++)
            {
                // Do we acquire an integer?
                if (Utilities.CompareValues(x * val, Math.Floor(x * val)))
                {
                    int gcd = Utilities.GCD(val, (int)Math.Round(x * val));
                    return x < 1 ? new KeyValuePair<int, int>(val / gcd, (int)Math.Round(x * val) / gcd) :
                                   new KeyValuePair<int, int>((int)Math.Round(x * val) / gcd, val / gcd);
                }
            }

            return new KeyValuePair<int, int>(-1, -1);
        }

        // Makes a list containing a single element
        public static List<T> MakeList<T>(T obj)
        {
            List<T> l = new List<T>();

            l.Add(obj);

            return l;
        }

        // Makes a list containing a single element
        public static bool AddUnique<T>(List<T> list, T obj)
        {
            if (list.Contains(obj)) return false;

            list.Add(obj);
            return true;
        }

        // Makes a list containing a single element
        public static void AddUniqueList<T>(List<T> list, List<T> objList)
        {
            foreach (T o in objList)
            {
                AddUnique<T>(list, o);
            }
        }

        // Is smaller \subseteq larger
        public static bool Subset<T>(List<T> larger, List<T> smaller)
        {
            foreach (T o in smaller)
            {
                if (!larger.Contains(o)) return false;
            }

            return true;
        }

        // Is set1 \equals set2
        public static bool EqualSets<T>(List<T> set1, List<T> set2)
        {
            if (set1.Count != set2.Count) return false;

            return Subset<T>(set1, set2); // redundant since we checked same size && Subset<T>(set2, set1);
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static readonly double EPSILON = 0.0001;
        public static bool CompareValues(double a, double b)
        {
            return Math.Abs(a - b) < EPSILON;
        }

        //
        // Constructs an integer representation of the powerset based on input value integer n
        // e.g. 4 -> { {}, {0}, {1}, {2}, {3}, {0, 1}, {0, 2}, {0, 3}, {1, 2}, {1, 3}, {2, 3}, {0, 1, 2}, {0, 1, 3}, {0, 2, 3}, {1, 2, 3}, {0, 1, 2, 3} }
        //
        private static readonly int GREATER = 1;
        private static readonly int EQUAL = 0;
        private static readonly int LESS = -1;
        private static int CompareTwoSets(List<int> set1, List<int> set2)
        {
            // Discriminate based on set size foremost
            if (set1.Count < set2.Count) return LESS;
            if (set1.Count > set2.Count) return GREATER;

            for (int i = 0; i < set1.Count && i < set2.Count; i++)
            {
                if (set1[i] < set2[i]) return LESS;
                if (set1[i] > set2[i]) return GREATER;
            }

            return EQUAL;
        }

        private static List<List<int>> ConstructRestrictedPowerSet(int n, int maxCardinality)
        {
            if (n <= 0) return Utilities.MakeList<List<int>>(new List<int>());

            List<List<int>> powerset = ConstructRestrictedPowerSet(n - 1, maxCardinality);
            List<List<int>> newCopies = new List<List<int>>();

            foreach (List<int> intlist in powerset)
            {
                if (intlist.Count < maxCardinality)
                {
                    // Make a copy, add to copy, add to overall list
                    List<int> copy = new List<int>(intlist);
                    copy.Add(n - 1); // We are dealing with indices, subtract 1
                    newCopies.Add(copy);
                }
            }

            powerset.AddRange(newCopies);

            return powerset;
        }

        // A memoized copy of all the powersets. 10 is large for this, we expect max of 5.
        // Note, we use a matrix since maxCardinality may change
        // We maintain ONLY an array because we are using this for a specific purpse in this project
        public static List<List<int>>[] memoized = new List<List<int>>[10];
        public static List<string>[] memoizedCompressed = new List<string>[10];
        private static void ConstructPowerSetWithNoEmptyHelper(int n, int maxCardinality)
        {
            if (memoized[n] != null) return;

            // Construct the powerset and remove the emptyset
            List<List<int>> powerset = ConstructRestrictedPowerSet(n, maxCardinality);
            powerset.RemoveAt(0);

            // Sort so the smallest sets are first and sets of the same size are compared based on elements.
            powerset.Sort(CompareTwoSets);

            // Now remove the singleton sets
            powerset.RemoveRange(0, n);

            // Save this construction
            memoized[n] = powerset;

            // Save the compressed versions
            List<string> compressed = new List<string>();
            powerset.ForEach(subset => compressed.Add(CompressUniqueIntegerList(subset)));
            memoizedCompressed[n] = compressed;
        }
        public static List<List<int>> ConstructPowerSetWithNoEmpty(int n, int maxCardinality)
        {
            ConstructPowerSetWithNoEmptyHelper(n, maxCardinality);

            return memoized[n];
        }

        public static List<string> ConstructPowerSetStringsWithNoEmpty(int n, int maxCardinality)
        {
            ConstructPowerSetWithNoEmptyHelper(n, maxCardinality);

            return memoizedCompressed[n];
        }
        // Unchecked, we assume a unique list of integers
        // Takes an integer list and compresses it into a string: { 0 1 2 } -> 012
        // Note, this is only a useful encoding for unit digits (like with powersets above)
        public static string CompressUniqueIntegerList(List<int> list)
        {
            string compressed = "";
            list.ForEach(item => compressed += item);
            return compressed;
        }
        // Splits a compressed string (from above) into two parts: the substring we have already processed and the tail we have yet to process
        // 012 -> < 01, 2>
        public static KeyValuePair<string, int> SplitStringIntoKnownToProcess(string s)
        {
            return new KeyValuePair<string, int>(s.Substring(0, s.Length - 1), Convert.ToInt32(s[s.Length - 1]) - 48);
        }
        // Decompresses a string of integers directly into an integer list: 012 -> { 0, 1, 2 }
        public static List<int> DecompressStringToList(string s)
        {
            List<int> intList = new List<int>();
            foreach (char c in s) intList.Add(Convert.ToInt32(c) - 48);
            return intList;
        }
    }

    public class Stopwatch
    {
        public static readonly bool IsHighResolution = false;
        public static readonly long Frequency = TimeSpan.TicksPerSecond;

        public TimeSpan Elapsed
        {
            get
            {
                if (!this.StartUtc.HasValue)
                {
                    return TimeSpan.Zero;
                }
                if (!this.EndUtc.HasValue)
                {
                    return (DateTime.UtcNow - this.StartUtc.Value);
                }
                return (this.EndUtc.Value - this.StartUtc.Value);
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return this.ElapsedTicks / TimeSpan.TicksPerMillisecond;
            }
        }
        public long ElapsedTicks { get { return this.Elapsed.Ticks; } }
        public bool IsRunning { get; private set; }
        private DateTime? StartUtc { get; set; }
        private DateTime? EndUtc { get; set; }

        public static long GetTimestamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        public void Reset()
        {
            Stop();
            this.EndUtc = null;
            this.StartUtc = null;
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }
            if ((this.StartUtc.HasValue) &&
                (this.EndUtc.HasValue))
            {
                // Resume the timer from its previous state
                this.StartUtc = this.StartUtc.Value +
                    (DateTime.UtcNow - this.EndUtc.Value);
            }
            else
            {
                // Start a new time-interval from scratch
                this.StartUtc = DateTime.UtcNow;
            }
            this.IsRunning = true;
            this.EndUtc = null;
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.EndUtc = DateTime.UtcNow;
            }
        }

        public static Stopwatch StartNew()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }
    }
}
