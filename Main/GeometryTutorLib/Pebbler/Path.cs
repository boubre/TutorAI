using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    // A wrapper class around a list of edges
    public class Path
    {
        private List<PebblerHyperEdge> path;

        public Path()
        {
            path = new List<PebblerHyperEdge>();
        }

        public Path(Path p)
        {
            path = new List<PebblerHyperEdge>(p.path);
        }

        public Path(List<PebblerHyperEdge> edges)
        {
            path = new List<PebblerHyperEdge>(edges);
        }

        public void AddToPath(PebblerHyperEdge edge)
        {
            Utilities.AddUnique<PebblerHyperEdge>(path, edge);
        }

        public void AddToPath(Path newPath)
        {
            Utilities.AddUniqueList<PebblerHyperEdge>(path, newPath.path);
        }

        public Path Copy()
        {
            return new Path(this);
        }

        public override string ToString()
        {
            string s = "";

            foreach (PebblerHyperEdge edge in path)
            {
                s += edge.ToString() + " ";
            }

            return "{ " + s + "}";
        }
    }
}
