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
        private UndirectedPlanarGraph.PlanarGraph graph;

        // The list of minimal cycles, filaments, and isolated points.
        public List<Primitive> primitives { get; private set; }

        public MinimalBasisCalculator(UndirectedPlanarGraph.PlanarGraph g)
        {
            graph = g;

            primitives = new List<Primitive>();

            ExtractPrimitives();
        }

        //
        // Since the previous point is not known, we select the next point which is closest to -90 degrees (in the 4th quadrant)
        // Note: no possible points will be in the 2nd and 3rd quadrants w.r.t the start point.
        //
        //private Point GetStartNode(Point minPoint)
        //{
        //    // The lexicographically minimum point.
        //    int minIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(minPoint, UndirectedGraph.NodePointType.REAL));

        //    // Information that will change along with the current candidate next point. 
        //    double currentAngle = 2 * Math.PI; // This will be overwritten
        //    Point currentNextPoint = null;

        //    foreach (int neighborIndex in graph.nodes[minIndex].neighbors)
        //    {
        //        // If a neighbor node creates an angle closer to -90, use it as the new next.
        //        double tempAngle = GetStandardAngleWithCenter(minPoint, graph.nodes[neighborIndex].thePoint);
        //        if (tempAngle < currentAngle)
        //        {
        //            currentAngle = tempAngle;
        //            currentNextPoint = graph.nodes[neighborIndex].thePoint;
        //        }
        //    }

        //    return currentNextPoint;
        //}

        //
        // We want our first vector to be downward (-90 degrees std unit circle)
        //
        private Point GetFirstNeighbor(Point currentPt)
        {
            Point imaginaryPrevPt = new Point("", currentPt.X, currentPt.Y + 1);
            Point prevCurrVector = Point.MakeVector(imaginaryPrevPt, currentPt);

            // We want the point that creates the smallest angle w.r.t. to the stdVector

            // Information that will change along with the current candidate next point. 
            double currentAngle = 360; // This will be overwritten
            Point currentNextPoint = null;

            // Index of the current point so we can get its neighbors.
            int currentPtIndex = graph.IndexOf(currentPt);

            foreach (UndirectedPlanarGraph.PlanarGraphEdge edge in graph.nodes[currentPtIndex].edges)
            {
                int neighborIndex = graph.IndexOf(edge.target);
                Point neighbor = graph.nodes[neighborIndex].thePoint;

                // Create a vector of the current point with it's neighbor
                Point currentNeighborVector = Point.MakeVector(currentPt, neighbor);

                // Cross product of the two vectors to determine if we have an angle that is < 180 or > 180.
                double crossProduct = Point.CrossProduct(prevCurrVector, currentNeighborVector);

                double angleMeasure = Point.AngleBetween(prevCurrVector, currentNeighborVector);

                // if (GeometryTutorLib.Utilities.GreaterThan(crossProduct, 0)) angleMeasure = angleMeasure;
                if (crossProduct < 0) angleMeasure = angleMeasure + 180;
                if (GeometryTutorLib.Utilities.CompareValues(crossProduct, 0)) angleMeasure = 180;

                if (angleMeasure < currentAngle)
                {
                    currentAngle = angleMeasure;
                    currentNextPoint = neighbor;
                }
            }

            return currentNextPoint;
        }

        //
        // With respect to the given vector (based on prevPt and currentPt), return the tightest counter-clockwise neighbor.
        //
        private Point GetTightestCounterClockwiseNeighbor(Point prevPt, Point currentPt)
        {
            Point prevCurrVector = Point.MakeVector(prevPt, currentPt);

            // We want the point that creates the smallest angle w.r.t. to the stdVector

            // Information that will change along with the current candidate next point. 
            double currentAngle = 360; // This will be overwritten
            Point currentNextPoint = null;

            // Index of the current point so we can get its neighbors.
            int prevPtIndex = graph.IndexOf(prevPt);
            int currentPtIndex = graph.IndexOf(currentPt);

            foreach (UndirectedPlanarGraph.PlanarGraphEdge edge in graph.nodes[currentPtIndex].edges)
            {
                int neighborIndex = graph.IndexOf(edge.target);

                if (prevPtIndex != neighborIndex)
                {
                    Point neighbor = graph.nodes[neighborIndex].thePoint;

                    // Create a vector of the current point with it's neighbor
                    Point currentNeighborVector = Point.MakeVector(currentPt, neighbor);

                    // Cross product of the two vectors to determine if we have an angle that is < 180 or > 180.
                    double crossProduct = Point.CrossProduct(prevCurrVector, currentNeighborVector);

                    double angleMeasure = Point.AngleBetween(Point.GetOppositeVector(prevCurrVector), currentNeighborVector);

                    // if (GeometryTutorLib.Utilities.GreaterThan(crossProduct, 0)) angleMeasure = angleMeasure;
                    if (crossProduct < 0) angleMeasure = angleMeasure + 180;
                    if (GeometryTutorLib.Utilities.CompareValues(crossProduct, 0)) angleMeasure = 180;

                    if (angleMeasure < currentAngle)
                    {
                        currentAngle = angleMeasure;
                        currentNextPoint = neighbor;
                    }
                }
            }

            return currentNextPoint;
        }

        private void ExtractPrimitives()
        {
            //
            // Lexicographically sorted heap of all points in the graph.
            //
            OrderedPointList heap = new OrderedPointList();
            for (int gIndex = 0; gIndex < graph.Count; gIndex++)
            {
                heap.Add(graph.nodes[gIndex].thePoint);
            }

            Debug.WriteLine(heap);

            //
            // Exhaustively analyze all points in the graph.
            //
            while (!heap.IsEmpty())
            {
                Point v0 = heap.PeekMin();
                int v0Index = graph.IndexOf(v0);

                switch(graph.nodes[v0Index].NodeDegree())
                {
                    case 0:
                        // Isolated point
                        ExtractIsolatedPoint(v0, heap);
                        break;

                    case 1:
                        // Filament: start at this node and indicate the next point is its only neighbor
                        ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
                        break;

                    default:
                        // filament or minimal cycle
                        ExtractPrimitive(v0, heap);
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
            Debug.WriteLine(primitives[primitives.Count-1].ToString());
        }

        void ExtractFilament (Point v0, Point v1, OrderedPointList heap)
        {
            int v0Index = graph.IndexOf(v0);

            if (graph.IsCycleEdge(v0, v1))
            {
                if (graph.nodes[v0Index].NodeDegree() >= 3)
                {
                    graph.RemoveEdge(v0, v1);
                    v0 = v1;
                    v0Index = graph.IndexOf(v0);
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[v0Index].edges[0].target;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    v1 = graph.nodes[v0Index].edges[0].target;
                    
                    if (graph.IsCycleEdge(v0, v1))
                    {
                        heap.Remove(v0);
                        graph.RemoveEdge(v0, v1);
                        graph.RemoveNode(v0);
                        v0 = v1;
                        v0Index = graph.IndexOf(v0);
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

                    v0Index = graph.IndexOf(v0);
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[v0Index].edges[0].target;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    primitive.Add(v0);
                    v1 = graph.nodes[v0Index].edges[0].target;
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
                Debug.WriteLine(primitive.ToString());
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
            Point v1 = GetFirstNeighbor(v0); //  GetClockwiseMost(new Point("", v0.X, v0.Y + 1), v0);
            Point vPrev = v0;
            Point vCurr = v1;

            int v0Index = graph.IndexOf(v0);
            int v1Index = graph.IndexOf(v1);

            // Loop until we have a cycle or we have a null (filament)
            while (vCurr != null && !vCurr.Equals(v0) && !visited.Contains(vCurr))
            {
                sequence.Add(vCurr);
                visited.Add(vCurr);
                Point vNext = GetTightestCounterClockwiseNeighbor(vPrev, vCurr);
                vPrev = vCurr;
                vCurr = vNext;
            }

            //
            // Filament: hit an endpoint
            //
            if (vCurr == null)
            {
                // Filament found, not necessarily rooted at v0.
                ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
            }
            //
            // Minimal cycle found.
            //
            else if (vCurr.Equals(v0))
            {
                MinimalCycle primitive = new MinimalCycle();

                primitive.AddAll(sequence);

                primitives.Add(primitive);
                Debug.WriteLine(primitive.ToString());

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
                    ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
                }

                //
                // indices may have changed; update.
                //
                v1Index = graph.IndexOf(v1);
                if (v1Index != -1)
                {
                    if (graph.nodes[v1Index].NodeDegree() == 1)
                    {
                        // Remove the filament rooted at v1.
                        ExtractFilament(v1, graph.nodes[v1Index].edges[0].target, heap);
                    }
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
                    if (graph.nodes[v0Index].edges[0].target.Equals(v1))
                    {
                        v1 = v0;
                        v0 = graph.nodes[v0Index].edges[1].target;
                    }
                    else
                    {
                        v1 = v0;
                        v0 = graph.nodes[v0Index].edges[0].target;
                    }

                    // Find the next v0 index
                    v0Index = graph.IndexOf(v0);
                }
                ExtractFilament(v0, v1, heap);
            }
        }

        //private static double DotPerp(Point p1, Point p2)
        //{
        //    return p1.X * p2.Y - p2.X * p1.Y;
        //}

        //private Point GetCounterClockwiseMost(Point vPrev, Point vCurr)
        //{
        //    int vPrevIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vPrev, UndirectedGraph.NodePointType.REAL));
        //    int vCurrIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vCurr, UndirectedGraph.NodePointType.REAL));

        //    if (graph.nodes[vCurrIndex].NodeDegree() == 0) return null;
            
        //    Point dCurr = new Point("", vCurr.X - vPrev.X, vCurr.Y - vPrev.Y);
            
        //    //
        //    // adjacent Point of vcurr not equal to vprev;
        //    //
        //    Point vNext = null;
        //    int vNextStartIndex = -1;
        //    foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
        //    {
        //        if (vAdjIndex != vPrevIndex)
        //        {
        //            vNext = graph.nodes[vAdjIndex].thePoint;
        //            vNextStartIndex = vAdjIndex;
        //            break;
        //        }
        //    }

        //    // If we exit here, we have a filament endpoint
        //    if (vNextStartIndex == -1) return null;

        //    Point dNext = new Point("", vNext.X - vCurr.X, vNext.Y - vCurr.Y);
        //    double vCurrIsConvex = DotPerp(dNext, dCurr);

        //    foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
        //    {
        //        if (vAdjIndex != vPrevIndex && vAdjIndex != vNextStartIndex)
        //        {
        //            Point dAdj = new Point("", graph.nodes[vAdjIndex].thePoint.X - graph.nodes[vCurrIndex].thePoint.X,
        //                                       graph.nodes[vAdjIndex].thePoint.Y - graph.nodes[vCurrIndex].thePoint.Y);
        //            if (vCurrIsConvex > 0)
        //            {
        //                if (DotPerp(dCurr, dAdj) > 0 && DotPerp(dNext, dAdj) > 0)
        //                {
        //                    vNext = graph.nodes[vAdjIndex].thePoint;
        //                    dNext = dAdj;
        //                    vCurrIsConvex = DotPerp(dNext, dCurr);
        //                }
        //            }
        //            else
        //            {
        //                if (DotPerp(dCurr, dAdj) > 0 || DotPerp(dNext, dAdj) > 0)
        //                {
        //                    vNext = graph.nodes[vAdjIndex].thePoint;
        //                    dNext = dAdj;
        //                    vCurrIsConvex = DotPerp(dNext, dCurr);
        //                }
        //            }
        //        }
        //    }

        //    return vNext;
        //}



        //private Point GetClockwiseMost(Point vPrev, Point vCurr)
        //{
        //    int vPrevIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vPrev, UndirectedGraph.NodePointType.REAL));
        //    int vCurrIndex = graph.nodes.IndexOf(new UndirectedGraph.PlanarGraphNode(vCurr, UndirectedGraph.NodePointType.REAL));

        //    if (graph.nodes[vCurrIndex].NodeDegree() == 0) return null;

        //    Point dCurr = new Point("", vCurr.X - vPrev.X, vCurr.Y - vPrev.Y);

        //    //
        //    // adjacent Point of vcurr not equal to vprev;
        //    //
        //    Point vNext = null;
        //    int vNextStartIndex = -1;
        //    foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
        //    {
        //        if (vAdjIndex != vPrevIndex)
        //        {
        //            vNext = graph.nodes[vAdjIndex].thePoint;
        //            vNextStartIndex = vAdjIndex;
        //            break;
        //        }
        //    }

        //    // If we exit here, we have a filament endpoint
        //    if (vNextStartIndex == -1) return null;

        //    Point dNext = new Point("", vNext.X - vCurr.X, vNext.Y - vCurr.Y);
        //    double vCurrIsConvex = DotPerp(dNext, dCurr);

        //    //
        //    // Search all adjacent nodes in the graph.
        //    //
        //    foreach (int vAdjIndex in graph.nodes[vCurrIndex].neighbors)
        //    {
        //        if (vAdjIndex != vPrevIndex && vAdjIndex != vNextStartIndex)
        //        {
        //            Point dAdj = new Point("", graph.nodes[vAdjIndex].thePoint.X - graph.nodes[vCurrIndex].thePoint.X,
        //                                       graph.nodes[vAdjIndex].thePoint.Y - graph.nodes[vCurrIndex].thePoint.Y);
        //            if (vCurrIsConvex > 0)
        //            {
        //                if (DotPerp(dCurr, dAdj) < 0 || DotPerp(dNext, dAdj) < 0)
        //                {
        //                    vNext = graph.nodes[vAdjIndex].thePoint;
        //                    dNext = dAdj;
        //                    vCurrIsConvex = DotPerp(dNext, dCurr);
        //                }
        //            }
        //            else
        //            {
        //                if (DotPerp(dCurr, dAdj) < 0 && DotPerp(dNext, dAdj) < 0)
        //                {
        //                    vNext = graph.nodes[vAdjIndex].thePoint;
        //                    dNext = dAdj;
        //                    vCurrIsConvex = DotPerp(dNext, dCurr);
        //                }
        //            }
        //        }
        //    }

        //    return vNext;
        //}
    }
}