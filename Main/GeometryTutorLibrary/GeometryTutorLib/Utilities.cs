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
        // Makes a list containing a single element
        public static List<T> MakeList<T>(T obj)
        {
            List<T> l = new List<T>();

            l.Add(obj);

            return l;
        }

        // Makes a list containing a single element
        public static void AddUnique<T>(List<T> list, T obj)
        {
            if (list.Contains(obj)) return;

            list.Add(obj);
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
