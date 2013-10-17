using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeometryTutorLib
{
    class Utilities
    {
        // Makes a list containing a single element
        public static List<T> MakeList<T>(T obj)
        {
            List<T> l = new List<T>();

            l.Add(obj);
            
            return l;
        }
    }
}
