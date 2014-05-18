using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;

namespace LiveGeometry.AtomicRegionIdentifier
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public class MinimalBasisCalculator
    {
        // The graph we use as the basis for region identification.
        private UndirectedGraph.PlanarGraph graph;

        // The list of minimal cycles, filaments, and isolated points.
        public List<Primitive> primitives { get; private set; }

        public MinimalBasisCalculator(UndirectedGraph.PlanarGraph g)
        {
            graph = g;

            primitives = new List<Primitive>();

            ExtractPrimitives();
        }

        private void ExtractPrimitives()
        {
            //
            // Lexicographically sorted heap of all points in the graph.
            //
            OrderedPointList heap = new OrderedPointList();
            for (int gIndex = 0; gIndex < graph.Count; gIndex++)
            {
                heap.Add(graph.nodes[gIndex].thePoint, gIndex);
            }

            Debug.WriteLine(heap);

            //
            // Exhaustively analyze all points in the graph.
            //
            while (!heap.IsEmpty())
            {
                KeyValuePair<Point, int> v0Pair = heap.PeekMin();

                switch(graph.nodes[v0Pair.Value].NodeDegree())
                {
                    case 0:
                        // Isolated point
                        ExtractIsolatedPoint(v0Pair.Key, heap);
                        break;

                    case 1:
                        // Filament: start at this node and indicate the next point is its only neighbor
                        ExtractFilament(v0Pair.Key, graph.nodes[graph.nodes[v0Pair.Value].neighbors[0]].thePoint, heap);
                        break;

                    default:
                        // filament or minimal cycle
                        ExtractPrimitive(v0Pair.Key, heap);
                        break;
                }
            }
        }

        //
        // Remove the isolated point from the graph and heap; add to list of primitives.
        //
        void ExtractIsolatedPoint (Point v0, OrderedPointList heap)
        {
            heap.Remove(v0);

            graph.RemoveNode(v0);

            primitives.Add(new IsolatedPoint(v0));
        }

        void ExtractFilament (Point v0, Point v1, OrderedPointList heap)
        {
            int v0Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v0, UndirectedGraph.NodePointType.REAL));

            if (graph.IsCycleEdge(v0, v1))
            {
                if (graph.nodes[v0Index].NodeDegree() >= 3)
                {
                    graph.RemoveEdge(v0, v1);
                    v0 = v1;
                    v0Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v0, UndirectedGraph.NodePointType.REAL));
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    v1 = graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint;
                    
                    if (graph.IsCycleEdge(v0, v1))
                    {
                        heap.Remove(v0);
                        graph.RemoveEdge(v0, v1);
                        graph.RemoveNode(v0);
                        v0 = v1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (graph.nodes[v0Index].NodeDegree() == 0)
                {
                    heap.Remove(v0);
                    graph.RemoveNode(v0);
                }
            }
            else
            {
                Filament primitive = new Filament();

                if (graph.nodes[v0Index].NodeDegree() >= 3)
                {
                    primitive.Add(v0);
                    graph.RemoveEdge(v0,v1);
                    v0 = v1;

                    v0Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v0, UndirectedGraph.NodePointType.REAL));
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    primitive.Add(v0);
                    v1 = graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint;
                    heap.Remove(v0);
                    graph.RemoveEdge(v0, v1);
                    graph.RemoveNode(v0);
                    v0 = v1;
                }

                primitive.Add(v0);

                if (graph.nodes[v0Index].NodeDegree() == 0)
                {
                    heap.Remove(v0);
                    graph.RemoveEdge(v0, v1);
                    graph.RemoveNode(v0);
                }
                
                primitives.Add(primitive);
            }
        }

        //
        // Extract a minimal cycle or a filament
        //
        void ExtractPrimitive(Point v0, OrderedPointList heap)
        {
            List<Point> visited = new List<Point>();
            List<Point> sequence = new List<Point>();

            sequence.Add(v0);

            // Create an initial line as (downward) vertical w.r.t. v0; v1 is based on the vertical line through v0
            Point v1 = GetClockwiseMost(new Point("", v0.X, v0.Y + 1), v0);
            Point vPrev = v0;
            Point vCurr = v1;

            int v0Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v0, UndirectedGraph.NodePointType.REAL));
            int v1Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v1, UndirectedGraph.NodePointType.REAL));

            // Loop until we have a cycle or we have a null (filament)
            while (vCurr != null && !vCurr.Equals(v0) && !visited.Contains(vCurr))
            {
                sequence.Add(vCurr);
                visited.Add(vCurr);
                Point vNext = GetCounterClockwiseMost(vPrev, vCurr);
                vPrev = vCurr;
                vCurr = vNext;
            }

            //
            // Filament: hit an endpoint
            //
            if (vCurr == null)
            {
                // Filament found, not necessarily rooted at v0.
                ExtractFilament(v0, graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint, heap);
            }
            //
            // Minimal cycle found.
            //
            else if (vCurr.Equals(v0))
            {
                MinimalCycle primitive = new MinimalCycle();

                primitive.AddAll(sequence);

                primitives.Add(primitive);

                // Mark that these edges are a part of a cycle
                for (int p = 0; p < sequence.Count; p++)
                {
                    graph.MarkCycleEdge(sequence[p], sequence[p+1 < sequence.Count ? p+1 : 0]);
                }

                graph.RemoveEdge(v0, v1);

                //
                // Check filaments for v0 and v1
                //
                if (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    // Remove the filament rooted at v0.
                    ExtractFilament(v0, graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint, heap);
                }

                if (graph.nodes[v1Index].NodeDegree() == 1)
                {
                    // Remove the filament rooted at v1.
                    ExtractFilament(v1, graph.nodes[graph.nodes[v1Index].neighbors[1]].thePoint, heap);
                }
            }
            //
            // vCurr was visited earlier
            //
            else
            {
                // A cycle has been found, but is not guaranteed to be a minimal
                // cycle. This implies v0 is part of a filament. Locate the
                // starting point for the filament by traversing from v0 away
                // from the initial v1.
                while (graph.nodes[v0Index].NodeDegree() == 2)
                {
                    // Choose between the the two neighbors
                    if (graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint.Equals(v1))
                    {
                        v1 = v0;
                        v0 = graph.nodes[graph.nodes[v0Index].neighbors[1]].thePoint;
                    }
                    else
                    {
                        v1 = v0;
                        v0 = graph.nodes[graph.nodes[v0Index].neighbors[0]].thePoint;
                    }

                    // Find the next v0 index
                    v0Index = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(v0, UndirectedGraph.NodePointType.REAL));
                }
                ExtractFilament(v0, v1, heap);
            }
        }

        private static double DotPerp(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        private Point GetCounterClockwiseMost(Point vPrev, Point vCurr)
        {
            int vPrevIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vPrev, UndirectedGraph.NodePointType.REAL));
            int vCurrIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vCurr, UndirectedGraph.NodePointType.REAL));

            if (graph.nodes[vCurrIndex].NodeDegree() == 0) return null;
            
            Point dCurr = new Point("", vCurr.X - vPrev.X, vCurr.Y - vPrev.Y);
            
            //
            // adjacent Point of vcurr not equal to vprev;
            //
            Point vNext = null;
            int vNextStartIndex = -1;
            foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
            {
                if (vAdjIndex != vPrevIndex)
                {
                    vNext = graph.nodes[vAdjIndex].thePoint;
                    vNextStartIndex = vAdjIndex;
                    break;
                }
            }

            // If we exit here, we have a filament endpoint
            if (vNextStartIndex == -1) return null;

            Point dNext = new Point("", vNext.X - vCurr.X, vNext.Y - vCurr.Y);
            double vCurrIsConvex = DotPerp(dNext, dCurr);

            foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
            {
                if (vAdjIndex != vPrevIndex && vAdjIndex != vNextStartIndex)
                {
                    Point dAdj = new Point("", graph.nodes[vAdjIndex].thePoint.X - graph.nodes[vCurrIndex].thePoint.X,
                                               graph.nodes[vAdjIndex].thePoint.Y - graph.nodes[vCurrIndex].thePoint.Y);
                    if (vCurrIsConvex > 0)
                    {
                        if (DotPerp(dCurr, dAdj) > 0 && DotPerp(dNext, dAdj) > 0)
                        {
                            vNext = graph.nodes[vAdjIndex].thePoint;
                            dNext = dAdj;
                            vCurrIsConvex = DotPerp(dNext, dCurr);
                        }
                    }
                    else
                    {
                        if (DotPerp(dCurr, dAdj) > 0 || DotPerp(dNext, dAdj) > 0)
                        {
                            vNext = graph.nodes[vAdjIndex].thePoint;
                            dNext = dAdj;
                            vCurrIsConvex = DotPerp(dNext, dCurr);
                        }
                    }
                }
            }

            return vNext;
        }



        private Point GetClockwiseMost(Point vPrev, Point vCurr)
        {
            int vPrevIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vPrev, UndirectedGraph.NodePointType.REAL));
            int vCurrIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vCurr, UndirectedGraph.NodePointType.REAL));

            if (graph.nodes[vCurrIndex].NodeDegree() == 0) return null;

            Point dCurr = new Point("", vCurr.X - vPrev.X, vCurr.Y - vPrev.Y);

            //
            // adjacent Point of vcurr not equal to vprev;
            //
            Point vNext = null;
            int vNextStartIndex = -1;
            foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
            {
                if (vAdjIndex != vPrevIndex)
                {
                    vNext = graph.nodes[vAdjIndex].thePoint;
                    vNextStartIndex = vAdjIndex;
                    break;
                }
            }

            // If we exit here, we have a filament endpoint
            if (vNextStartIndex == -1) return null;

            Point dNext = new Point("", vNext.X - vCurr.X, vNext.Y - vCurr.Y);
            double vCurrIsConvex = DotPerp(dNext, dCurr);

            //
            // Search all adjacent nodes in the graph.
            //
            foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
            {
                if (vAdjIndex != vPrevIndex && vAdjIndex != vNextStartIndex)
                {
                    Point dAdj = new Point("", graph.nodes[vAdjIndex].thePoint.X - graph.nodes[vCurrIndex].thePoint.X,
                                               graph.nodes[vAdjIndex].thePoint.Y - graph.nodes[vCurrIndex].thePoint.Y);
                    if (vCurrIsConvex > 0)
                    {
                        if (DotPerp(dCurr, dAdj) < 0 || DotPerp(dNext, dAdj) < 0)
                        {
                            vNext = graph.nodes[vAdjIndex].thePoint;
                            dNext = dAdj;
                            vCurrIsConvex = DotPerp(dNext, dCurr);
                        }
                    }
                    else
                    {
                        if (DotPerp(dCurr, dAdj) < 0 && DotPerp(dNext, dAdj) < 0)
                        {
                            vNext = graph.nodes[vAdjIndex].thePoint;
                            dNext = dAdj;
                            vCurrIsConvex = DotPerp(dNext, dCurr);
                        }
                    }
                }
            }

            return vNext;
        }
    }
}