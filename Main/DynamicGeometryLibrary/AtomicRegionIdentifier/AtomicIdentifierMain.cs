using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry.TutorParser;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace LiveGeometry.AtomicRegionIdentifier
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public static class AtomicIdentifierMain
    {
        public static List<AtomicRegion> GetAtomicRegions(List<Point> figurePoints,
                                                          List<GeometryTutorLib.ConcreteAST.Circle> circles,
                                                          List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            //
            // Convert all circles to atomic regions identifying their chord / radii substructure.
            //
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                List<AtomicRegion> circleAtoms = circle.Atomize(figurePoints);

                // Make this circle the owner of the atomic regions.
                foreach (AtomicRegion atom in circleAtoms)
                {
                    atom.AddOwner(circle);
                }
                atoms.AddRange(circleAtoms);
            }

            //
            // Make all of the polygons an atomic region.
            // Also, convert any concave polygon into atoms by extending sides inward. 
            //
            for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                {
                    ConcavePolygon concave = poly as ConcavePolygon;
                    if (concave != null)
                    {
                        List<AtomicRegion> concaveAtoms = concave.Atomize(figurePoints);
                        
                    }
                    ShapeAtomicRegion shapeAtom = new ShapeAtomicRegion(poly);
                    shapeAtom.AddOwner(poly);
                    atoms.Add(shapeAtom);
                }
            }

            //
            // Since circles and Concave Polygons were atomized, there may be duplicate atomic regions.
            //
            atoms = RemoveDuplicates(atoms);

            //
            // Combine all of the atomic regions together.
            //
            atoms = ComposeAllRegions(figurePoints, atoms);

            return atoms;
        }

        //
        // Remove all duplicate regions; this transfers all ownerships of the region to the persistent region.
        //
        private static List<AtomicRegion> RemoveDuplicates(List<AtomicRegion> atoms)
        {
            List<AtomicRegion> unique = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atoms)
            {
                // If this atom exists in the list, transfer ownership; this is NOT hierarchical.
                int index = unique.IndexOf(atom);

                if (index == -1) unique.Add(atom);
                else
                {
                    unique[index].AddOwners(atom.owners);
                }
            }

            return unique;
        }

        //
        // Find all regions that overlap this region.
        //
        private static void GetInterestingRegions(List<AtomicRegion> atoms, AtomicRegion theAtom,
                                                  out List<AtomicRegion> intersecting, out List<AtomicRegion> contained)
        {
            intersecting = new List<AtomicRegion>();
            contained = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atoms)
            {
                // An atom should not intersect itself.
                if (!theAtom.Equals(atom))
                {
                    if (theAtom.Contains(atom))
                    {
                        contained.Add(atom);
                    }
                    //else if (theAtom.StrictlyContains(atom))
                    //{
                    //    contained.Add(theAtom);
                    //}
                    else if (theAtom.OverlapsWith(atom))
                    {
                        intersecting.Add(atom);
                    }
                }
            }
        }

        ////
        //// Combine atomic regions: Use an on-demand combining process.
        ////    Collect all 
        ////
        //private static List<AtomicRegion> ComposeAllRegions(List<Point> figurePoints, List<AtomicRegion> atoms, AtomicRegion outerBounds)
        //{
        //    List<AtomicRegion> worklist = new List<AtomicRegion>(atoms);
        //    List<AtomicRegion> completed = new List<AtomicRegion>();
            
        //    bool change = true;
        //    while (change)
        //    {
        //        // Choose an arbitrary atomic region
        //        AtomicRegion current = worklist[0];
        //        worklist.RemoveAt(0);

        //        // Collect all interesting regions for this region: those that intersect with it and those that are contained inside.
        //        List<AtomicRegion> intersecting = null;
        //        List<AtomicRegion> contained = null;
        //        GetInterestingRegions(worklist, current, out intersecting, out contained);

        //        //
        //        // Recursively determine all interactions within the bounds of this atomic region.
        //        //
        //        // Combine all the intersecting and contained into one list.
        //        List<AtomicRegion> interesting = new List<AtomicRegion>(intersecting);
        //        interesting.AddRange(contained);
        //        List<AtomicRegion> newRegions = ComposeAllRegions(figurePoints, interesting, current);

        //        change = newRegions.Any();

        //        if (!change) completed.Add(current);
        //    }

        //    return completed;
        //}

        private static List<AtomicRegion> knownAtomicRegions = new List<AtomicRegion>();
        private static List<AtomicRegion> ComposeAllRegions(List<Point> figurePoints, List<AtomicRegion> atoms)
        {
            knownAtomicRegions.Clear();
            List<AtomicRegion> workingAtoms = new List<AtomicRegion>(atoms);

            //
            // Pre-calculate any simple atomic regions and remove them from further processing.
            //
            foreach (AtomicRegion currentAtom in atoms)
            {
                List<AtomicRegion> intersecting = null;
                List<AtomicRegion> containedSet = null;
                GetInterestingRegions(atoms, currentAtom, out intersecting, out containedSet);

                if (!intersecting.Any() && !containedSet.Any())
                {
                    knownAtomicRegions.Add(currentAtom);
                    workingAtoms.Remove(currentAtom);
                }
            }

            //
            // Can we eliminate any region from analysis since the region has been completely covered by known atomic regions?
            //


            //
            // Combine all regions using the given atoms as a boundary.
            //
            foreach (AtomicRegion currentAtom in workingAtoms)
            {
                List<AtomicRegion> allRegions = new List<AtomicRegion>(workingAtoms);
                allRegions.AddRange(knownAtomicRegions);

                List<AtomicRegion> newAtoms = ComposeSingleRegion(figurePoints, allRegions, currentAtom);

                // The boundary atom is replaced by all of the newAtoms
                if (newAtoms.Any())
                {
                    // Check for redundant 
                    knownAtomicRegions.AddRange(newAtoms);
                }
                // This is a true atomic region that cannot be split.
                else
                {
                    knownAtomicRegions.Add(currentAtom);
                }
            }
            return knownAtomicRegions;
        }

        //
        // Using a single atomic region as a set of bounds:
        //    (1) Find all interesting regions (contained and intersecting)
        //    (2) Recursively compose all contained regions.
        //    (3) Combine all into the original boundary region. 
        //
        private static List<AtomicRegion> ComposeSingleRegion(List<Point> figurePoints, List<AtomicRegion> atoms, AtomicRegion outerBounds)
        {
            //
            // Collect all interesting regions for this region: those that intersect with it and those that are contained inside.
            //
            List<AtomicRegion> intersectingSet = null;
            List<AtomicRegion> containedSet = null;
            GetInterestingRegions(atoms, outerBounds, out intersectingSet, out containedSet);

            // If we have have no interactions, this is a known atomic region.
            if (!intersectingSet.Any() && !intersectingSet.Any()) return new List<AtomicRegion>();

            //
            // Recur on all containing regions.
            //
            List<AtomicRegion> regions;
            List<AtomicRegion> newContainedAtoms = new List<AtomicRegion>();
            foreach (AtomicRegion containedAtom in containedSet)
            {
                if (!knownAtomicRegions.Contains(containedAtom))
                {
                    // Get all regions using containedAtom as the boundary region. 
                    regions = ComposeSingleRegion(figurePoints, atoms, containedAtom);
                    newContainedAtoms.AddRange(regions);

                    foreach (AtomicRegion newAtom in regions)
                    {
                        if (!knownAtomicRegions.Contains(newAtom)) knownAtomicRegions.Add(newAtom);
                    }
                }
            }

            //
            // Now that all contained regions are atomized, combine ALL intersections and atomic regions.
            //
            //  Collect all segments and arcs (with explicit endpoints).
            //  Extend only if they do not touch the sides of the boundaries.
            //
            
            // inside of the boundaries; determine all intersection points.
            //
            //  (1) All intersecting regions.
            //      (a) For all vertices inside the boundaries, extend to the closest atom.
            //      (b) For all sides that pass through determine any intersections.
            //  (2) All contained atoms
            //      (a) For each side of a region, extend to the closest region.
            //      (b) If a single circle or concentric circles, extend a diameter from the closest point inside the region, through the center. 
            //      (c) If several non-intersecting circles, extend diameters through the centers of each pair.
            //
            List<GeometryTutorLib.ConcreteAST.Point> points = new List<GeometryTutorLib.ConcreteAST.Point>();
            List<GeometryTutorLib.ConcreteAST.Segment> segments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<GeometryTutorLib.ConcreteAST.Arc> arcs = new List<GeometryTutorLib.ConcreteAST.Arc>();

            //
            // Add the outer boundaries.
            //
            points.AddRange(outerBounds.GetVertices());
            foreach (Connection boundaryConn in outerBounds.connections)
            {
                if (boundaryConn.type == ConnectionType.ARC) arcs.Add(boundaryConn.segmentOrArc as Arc);
                if (boundaryConn.type == ConnectionType.SEGMENT) segments.Add(boundaryConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);
            }

            //
            // Regions that intersect the boundaries; selectively take connections.
            //
            foreach (AtomicRegion intersecting in intersectingSet)
            {
                List<AtomicRegion.IntersectionAgg> intersections = outerBounds.GetIntersections(figurePoints, intersecting);

                // Determine which intersections are interior to the boundaries.
                foreach (AtomicRegion.IntersectionAgg agg in intersections)
                {
                    if (agg.overlap) { /* No-op */ }
                    else
                    {
                        if (agg.intersection1 != null)
                        {
                            if (outerBounds.PointLiesOnOrInside(agg.intersection1))
                            {
                                if (agg.thisConn.type == ConnectionType.ARC) arcs.Add(agg.thisConn.segmentOrArc as Arc);
                                if (agg.thisConn.type == ConnectionType.SEGMENT) segments.Add(agg.thisConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                if (agg.thatConn.type == ConnectionType.ARC) arcs.Add(agg.thatConn.segmentOrArc as Arc);
                                if (agg.thatConn.type == ConnectionType.SEGMENT) segments.Add(agg.thatConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                points.Add(agg.intersection1);
                            }
                        }
                        if (agg.intersection2 != null)
                        {
                            if (outerBounds.PointLiesOnOrInside(agg.intersection2))
                            {
                                if (agg.thisConn.type == ConnectionType.ARC) arcs.Add(agg.thisConn.segmentOrArc as Arc);
                                if (agg.thisConn.type == ConnectionType.SEGMENT) segments.Add(agg.thisConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                if (agg.thatConn.type == ConnectionType.ARC) arcs.Add(agg.thatConn.segmentOrArc as Arc);
                                if (agg.thatConn.type == ConnectionType.SEGMENT) segments.Add(agg.thatConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                points.Add(agg.intersection2);
                            }
                        }
                    }
                }
            }

            //
            // Deal with contained regions.
            //
            // TO BE COMPLETED: Deal with isolated circles.
            foreach (AtomicRegion contained in newContainedAtoms)
            {
                foreach (Connection conn in contained.connections)
                {
                    if (conn.type == ConnectionType.ARC) arcs.Add(conn.segmentOrArc as Arc);
                    if (conn.type == ConnectionType.SEGMENT) segments.Add(conn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);
                }
            }


            //
            // Find all intersections...among segments and arcs.
            //
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                segment.ClearCollinear();
            }

            // Segment-Segment
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    Point intersection = segments[s1].FindIntersection(segments[s2]);
                    intersection = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, intersection);

                    if (outerBounds.PointLiesOnOrInside(intersection))
                    {
                        segments[s1].AddCollinearPoint(intersection);
                        segments[s2].AddCollinearPoint(intersection);
                    }
                }
            }

            // Segment-Arc
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                foreach (GeometryTutorLib.ConcreteAST.Arc arc in arcs)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arc.theCircle.FindIntersection(segment, out pt1, out pt2);

                    if (pt1 == null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            segment.AddCollinearPoint(pt1);
                            arc.AddCollinearPoint(pt1);
                        }
                    }
                    if (pt2 == null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            segment.AddCollinearPoint(pt2);
                            arc.AddCollinearPoint(pt2);
                        }
                    }
                }
            }

            // Arc-Arc
            for (int a1 = 0; a1 < arcs.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < arcs.Count; a2++)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arcs[a1].theCircle.FindIntersection(arcs[a2].theCircle, out pt1, out pt2);

                    if (pt1 == null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            arcs[a1].AddCollinearPoint(pt1);
                            arcs[a2].AddCollinearPoint(pt1);
                        }
                    }
                    if (pt2 == null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            arcs[a1].AddCollinearPoint(pt2);
                            arcs[a2].AddCollinearPoint(pt2);
                        }
                    }
                }
            }

            //
            // Construct the Planar graph for atomic region identification.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            // Add the points as nodes in the graph.
            foreach (Point pt in points)
            {
                graph.AddNode(pt);
            }

            //
            // Edges are based on all the collinear relationships.
            // To ensure we are taking ONLY the closest extended intersections, choose ONLY the 1 point around the actual endpoints of the arc or segment.
            //
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                for (int p = 0; p < segment.collinear.Count - 1; p++)
                {
                    if (segment.HasPoint(segment.collinear[p]) || segment.HasPoint(segment.collinear[p + 1]))
                    {
                        graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p + 1],
                                                new GeometryTutorLib.ConcreteAST.Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                                GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                    }
                }
            }

            foreach (Arc arc in arcs)
            {
                for (int p = 0; p < arc.collinear.Count - 1; p++)
                {
                    if (arc.HasEndpoint(arc.collinear[p]) || arc.HasEndpoint(arc.collinear[p + 1]))
                    {
                        graph.AddUndirectedEdge(arc.collinear[p], arc.collinear[p + 1],
                                                new GeometryTutorLib.ConcreteAST.Segment(arc.collinear[p], arc.collinear[p + 1]).Length,
                                                GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
                    }
                }
            }

            //
            // Collect the circles from the arcs.
            //
            List<GeometryTutorLib.ConcreteAST.Circle> circles = new List<GeometryTutorLib.ConcreteAST.Circle>();
            foreach (Arc arc in arcs)
            {
                GeometryTutorLib.Utilities.AddStructurallyUnique<GeometryTutorLib.ConcreteAST.Circle>(circles, arc.theCircle);
            }

            //
            // Convert the planar graph to atomic regions.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();
            List<AtomicRegion> boundedAtoms = PrimitiveToRegionConverter.Convert(graph, primitives, circles);

            //
            // Determine ownership of the atomic regions.
            //
            foreach (AtomicRegion boundedAtom in boundedAtoms)
            {
                boundedAtom.AddOwners(outerBounds.owners);
            }

            return boundedAtoms;
        }
    }
}