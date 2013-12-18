using System;
using System.Collections.Generic;

namespace GeometryTutorLib.Pebbler
{
    //
    // Implements a multi-hashtable in which an entry may appear more than once in the table.
    // That is, a path contains potentially many source nodes. For each source node, we hash and add the path to the table.
    // Hence, a path with n source nodes is hashed n times; this allows for fast access
    // Collisions are thus handled by chaining
    //
    public class HyperEdgeMultiMap
    {
        private readonly int TABLE_SIZE;
        private List<PebblerHyperEdge>[] table;
        public int size { get; private set; }

        // If the user specifies the size, we will never have to rehash
        public HyperEdgeMultiMap(int sz)
        {
            size = 0;
            TABLE_SIZE = sz;
            
            table = new List<PebblerHyperEdge>[TABLE_SIZE];
        }

        //
        // Add the PebblerHyperEdge to all source node hash values
        //
        public void Put(PebblerHyperEdge e)
        {
            long hashVal = (e.targetNode % TABLE_SIZE);

            if (table[hashVal] == null)
            {
                table[hashVal] = new List<PebblerHyperEdge>();
            }

            Utilities.AddUnique<PebblerHyperEdge>(table[hashVal], e);

            size++;
        }

        // Another option to acquire the pertinent problems
        public List<PebblerHyperEdge> GetBasedOnGoal(int goalNodeIndex)
        {
            if (goalNodeIndex < 0 || goalNodeIndex >= TABLE_SIZE)
            {
                throw new ArgumentException("HyperEdgeMultimap::Get::key(" + goalNodeIndex + ")");
            }

            return table[goalNodeIndex];
        }

        public override string ToString()
        {
            String retS = "";

            for (int ell = 0; ell < TABLE_SIZE; ell++)
            {
                if (table[ell] != null)
                {
                    retS += ell + ":\n"; 
                    foreach (PebblerHyperEdge PebblerHyperEdge in table[ell])
                    {
                        retS += PebblerHyperEdge.ToString() + "\n";
                    }
                }
            }

            return retS;
        }
    }
}