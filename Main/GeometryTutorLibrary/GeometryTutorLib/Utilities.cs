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

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
