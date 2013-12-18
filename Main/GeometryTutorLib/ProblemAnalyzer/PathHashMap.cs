using System;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // Implements a multi-hashtable in which an entry may appear more than once in the table.
    // That is, a path contains potentially many source nodes. For each source node, we hash and add the path to the table.
    // Hence, a path with n source nodes is hashed n times; this allows for fast access
    // Collisions are thus handled by chaining
    //
    public class PathHashMap
    {
        private readonly int TABLE_SIZE;
        private List<Problem>[] table;
        public int size { get; private set; }

        // If the user specifies the size, we will never have to rehash
        public PathHashMap(int sz)
        {
            size = 0;
            TABLE_SIZE = sz;
            
            table = new List<Problem>[TABLE_SIZE];
        }

        //
        // Add the problem to all source node hash values
        //
        public void Put(Problem p)
        {
            foreach (int src in p.givens)
            {
                int hashVal = (src % TABLE_SIZE);

                if (table[hashVal] == null)
                {
                    table[hashVal] = new List<Problem>();
                }

                table[hashVal].Add(p);
            }

            size++;
        }

        //
        // Acquire a problem based on the goal only
        //
        public List<Problem> Get(Problem p)
        {
            return Get(p.goal);
        }

        // Another option to acquire the pertinent problems
        public List<Problem> Get(int key)
        {
            if (key < 0 || key >= TABLE_SIZE)
            {
                throw new ArgumentException("Hashtable::Get::key(" + key + ")");
            }

            return table[key];
        }

        public override string ToString()
        {
            String retS = "";

            for (int ell = 0; ell < TABLE_SIZE; ell++)
            {
                if (table[ell] != null)
                {
                    retS += ell + ":\n"; 
                    foreach (Problem problem in table[ell])
                    {
                        retS += problem.ToString() + "\n";
                    }
                }
            }

            return retS;
        }
    }
}