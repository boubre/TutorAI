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
    public class GeneratorHashtable
    {
        private int TABLE_SIZE;
        private List<Problem>[] table;
        public int size { get; private set; }
        private readonly double FULLNESS_RATIO = 0.75; // If this table exceeds a specific % of fullness, we rehash.

        // If the user specifies the size, we will never have to rehash
        public GeneratorHashtable(int sz)
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
            if ((double)(size + 1) / TABLE_SIZE > FULLNESS_RATIO) Rehash();

            long hashVal = (p.GetHashKey() % TABLE_SIZE);

            if (table[hashVal] == null)
            {
                table[hashVal] = new List<Problem>();
            }

            table[hashVal].Add(p);

            size++;
        }

        //
        // Acquire a problem
        //
        public bool Contains(Problem p)
        {
            long hashVal = p.GetHashKey() % TABLE_SIZE;

            return table[hashVal] == null ? false : table[hashVal].Contains(p);
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

        public void Rehash()
        {
            TABLE_SIZE *= 2;

            List<Problem>[] newTable = new List<Problem>[TABLE_SIZE];

            for (int pList = 0; pList < table.Length; pList++)
            {
                if (table[pList] != null)
                {
                    foreach (Problem problem in table[pList])
                    {
                        long hashVal = (problem.GetHashKey() % TABLE_SIZE);

                        if (newTable[hashVal] == null)
                        {
                            newTable[hashVal] = new List<Problem>();
                        }

                        newTable[hashVal].Add(problem);
                    }
                }
            }

            table = newTable;
        }

        public List<Problem> GetProblems()
        {
            List<Problem> allProblems = new List<Problem>();

            for (int pList = 0; pList < table.Length; pList++)
            {
                if (table[pList] != null)
                {
                    allProblems.AddRange(table[pList]);
                }
            }

            return allProblems;
        }
    }
}