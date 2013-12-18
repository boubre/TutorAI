using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GeometryTutorLib
{
    public static class Utilities
    {
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
        // e.g. 2 -> { {}, {0}, {1}, {0, 1} }
        //
        private static readonly int GREATER = -1;
        private static readonly int EQUAL = 0;
        private static readonly int LESS = 1;
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

        public static List<List<int>> ConstructPowerSetWithNoEmpty(int n, int maxCardinality)
        {
            // Construct the powerset and remove the emptyset
            List<List<int>> powerset = ConstructRestrictedPowerSet(n, maxCardinality);
            powerset.RemoveAt(0);

            // Sort so the smallest sets are first and sets of the same size are compared based on elements.
            powerset.Sort(CompareTwoSets);

            return powerset;
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
