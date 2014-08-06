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
            List<AtomicRegion> originalAtoms = new List<AtomicRegion>();

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
                    circle.AddAtomicRegion(atom);
                }
                originalAtoms.AddRange(circleAtoms);
            }

            //
            // Make all of the polygons an atomic region.
            // Also, convert any concave polygon into atoms by extending sides inward. 
            //
            for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                {
                    // Handle any concave polygons.
                    ConcavePolygon concave = poly as ConcavePolygon;
                    if (concave != null)
                    {
                        List<AtomicRegion> concaveAtoms = concave.Atomize(figurePoints);
                        originalAtoms.AddRange(concaveAtoms);
                        poly.AddAtomicRegions(concaveAtoms);
                    }

                    // Basic polygon: make it a shape atomic region.
                    else
                    {
                        ShapeAtomicRegion shapeAtom = new ShapeAtomicRegion(poly);
                        shapeAtom.AddOwner(poly);
                        originalAtoms.Add(shapeAtom);
                    }
                }
            }

            //
            // Since circles and Concave Polygons were atomized, there may be duplicate atomic regions.
            //
            originalAtoms = RemoveDuplicates(originalAtoms);
            List<AtomicRegion> workingAtoms = new List<AtomicRegion>(originalAtoms);

            //
            // Combine all of the atomic regions together.
            //
            List<AtomicRegion> composed = ComposeAllRegions(figurePoints, workingAtoms);
            composed = RemoveRedundantSemicircle(composed);

            //
            // Run the graph-based algorithm one last time to identify any pathological regions (exterior to all shapes).
            //
            // Identify those pathological regions as well as any lost (major arcs).
            //
            List<AtomicRegion> lost = new List<AtomicRegion>();
            List<AtomicRegion> pathological = new List<AtomicRegion>();
            List<AtomicRegion> pathologicalID = IdentifyPathological(figurePoints, composed, circles);
            pathologicalID = RemoveRedundantSemicircle(pathologicalID);
            foreach (AtomicRegion atom in composed)
            {
                if (!pathologicalID.Contains(atom))
                {
                    bool containment = false;
                    foreach (AtomicRegion pathAtom in pathologicalID)
                    {
                        if (atom.Contains(pathAtom))
                        {
                            containment = true;
                            break;
                        }
                    }
                    if (!containment) lost.Add(atom);
                }
            }

            foreach (AtomicRegion atom in pathologicalID)
            {
                if (!composed.Contains(atom)) pathological.Add(atom);
            }

            List<AtomicRegion> finalAtomSet = new List<AtomicRegion>();
            finalAtomSet.AddRange(composed);
            finalAtomSet.AddRange(pathological);

            return finalAtomSet;
        }

        //
        // Remove any semicircles that are not truly atomic.
        //
        private static List<AtomicRegion> RemoveRedundantSemicircle(List<AtomicRegion> originalAtoms)
        {
            List<AtomicRegion> trueAtoms = new List<AtomicRegion>();
            foreach (AtomicRegion atom in originalAtoms)
            {
                bool add = true;

                ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    Sector sector = shapeAtom.shape as Sector;
                    if (sector != null && sector.theArc is Semicircle)
                    {
                        foreach (AtomicRegion checkAtom in originalAtoms)
                        {
                            if (!atom.Equals(checkAtom) && shapeAtom.Contains(checkAtom))
                            {
                                add = false;
                                break;
                            }
                        }
                    }
                }

                if (add) trueAtoms.Add(atom);
            }

            return trueAtoms;
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

        //
        // Main routine traversing the list of all shapes to determine if the given shape is an outer shape. If it is, process it.
        //
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
                    foreach (AtomicRegion newAtom in newAtoms)
                    {
                        if (!knownAtomicRegions.Contains(newAtom)) knownAtomicRegions.Add(newAtom);
                    }
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
            if (!intersectingSet.Any() && !containedSet.Any()) return new List<AtomicRegion>();

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
                else
                {
                    newContainedAtoms.Add(containedAtom);
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
                                //if (!outerBounds.ExteriorTo(agg.thisConn))
                                //{
                                //    if (agg.thisConn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, agg.thisConn.segmentOrArc as Arc);
                                //    if (agg.thisConn.type == ConnectionType.SEGMENT)
                                //        GeometryTutorLib.Utilities.AddUnique<GeometryTutorLib.ConcreteAST.Segment>(segments, agg.thisConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);
                                //}

                                if (!outerBounds.NotInteriorTo(agg.thatConn))
                                {
                                    if (agg.thatConn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, agg.thatConn.segmentOrArc as Arc);
                                    if (agg.thatConn.type == ConnectionType.SEGMENT)
                                        GeometryTutorLib.Utilities.AddUnique<GeometryTutorLib.ConcreteAST.Segment>(segments, agg.thatConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);
                                }

                                GeometryTutorLib.Utilities.AddUnique<Point>(points, agg.intersection1);
                            }
                        }
                        if (agg.intersection2 != null)
                        {
                            if (outerBounds.PointLiesOnOrInside(agg.intersection2))
                            {
                                //
                                // These arcs / segments were added from the first intersection point.
                                //
                                //if (agg.thisConn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, agg.thisConn.segmentOrArc as Arc);
                                //if (agg.thisConn.type == ConnectionType.SEGMENT)
                                //    GeometryTutorLib.Utilities.AddUnique<GeometryTutorLib.ConcreteAST.Segment>(segments, agg.thisConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                //if (agg.thatConn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, agg.thatConn.segmentOrArc as Arc);
                                //if (agg.thatConn.type == ConnectionType.SEGMENT)
                                //    GeometryTutorLib.Utilities.AddUnique<GeometryTutorLib.ConcreteAST.Segment>(segments, agg.thatConn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);

                                GeometryTutorLib.Utilities.AddUnique<Point>(points, agg.intersection2);
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
                List<Point> verts = contained.GetVertices();
                GeometryTutorLib.Utilities.AddUniqueList<Point>(points, verts);

                foreach (Connection conn in contained.connections)
                {
                    if (conn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, conn.segmentOrArc as Arc);
                    if (conn.type == ConnectionType.SEGMENT)
                    {
                        GeometryTutorLib.Utilities.AddUnique<GeometryTutorLib.ConcreteAST.Segment>(segments, conn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment);
                    }
                }
            }


            //
            // Find all intersections...among segments and arcs.
            //
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                segment.ClearCollinear();
            }
            foreach (GeometryTutorLib.ConcreteAST.Arc arc in arcs)
            {
                arc.ClearCollinear();
            }

            // Segment-Segment
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    Point intersection = segments[s1].FindIntersection(segments[s2]);
                    intersection = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, intersection);

                    if (intersection != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(intersection))
                        {
                            segments[s1].AddCollinearPoint(intersection);
                            segments[s2].AddCollinearPoint(intersection);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, intersection);
                        }
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
                    pt1 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt1);
                    pt2 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt2);

                    if (pt1 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            segment.AddCollinearPoint(pt1);
                            arc.AddCollinearPoint(pt1);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt1);
                        }
                    }
                    if (pt2 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            segment.AddCollinearPoint(pt2);
                            arc.AddCollinearPoint(pt2);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt2);
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
                    pt1 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt1);
                    pt2 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt2);

                    if (pt1 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            arcs[a1].AddCollinearPoint(pt1);
                            arcs[a2].AddCollinearPoint(pt1);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt1);
                        }
                    }
                    if (pt2 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            arcs[a1].AddCollinearPoint(pt2);
                            arcs[a2].AddCollinearPoint(pt2);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt2);
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
                    if (outerBounds.PointLiesInOrOn(segment.collinear[p]) && outerBounds.PointLiesInOrOn(segment.collinear[p + 1]))
                    {
                        graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p + 1],
                                                new GeometryTutorLib.ConcreteAST.Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                                GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                    }
                }
            }

            foreach (Arc arc in arcs)
            {
                List<Point> applicCollinear = arc.GetOrderedCollinearPointsByEndpoints();

                int upperBound = applicCollinear.Count - 1;
                if (arc is Semicircle) upperBound = applicCollinear.Count;

                for (int p = 0; p < upperBound; p++)
                {
                    int nextindex = (p+1) == applicCollinear.Count ? 0 : (p+1);
                    if (outerBounds.PointLiesInOrOn(applicCollinear[p]) && outerBounds.PointLiesInOrOn(applicCollinear[nextindex]))
                    {
                        graph.AddUndirectedEdge(applicCollinear[p], applicCollinear[nextindex],
                                                new GeometryTutorLib.ConcreteAST.Segment(applicCollinear[p], applicCollinear[nextindex]).Length,
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
                
                // Indicate that the given boundary shape owns all of the new regions within.
                ShapeAtomicRegion shapeAtom = outerBounds as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    shapeAtom.shape.AddAtomicRegion(boundedAtom);
                }
            }

            return boundedAtoms;
        }

        private static List<AtomicRegion> IdentifyPathological(List<Point> figurePoints, List<AtomicRegion> atoms, List<GeometryTutorLib.ConcreteAST.Circle> circles)
        {
            List<GeometryTutorLib.ConcreteAST.Point> points = new List<GeometryTutorLib.ConcreteAST.Point>();
            List<GeometryTutorLib.ConcreteAST.Segment> segments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<GeometryTutorLib.ConcreteAST.Arc> arcs = new List<GeometryTutorLib.ConcreteAST.Arc>();

            // Collect segments and arcs.
            foreach (AtomicRegion atom in atoms)
            {
                foreach (Connection conn in atom.connections)
                {
                    if (conn.type == ConnectionType.SEGMENT)
                    {
                        GeometryTutorLib.ConcreteAST.Segment seg = conn.segmentOrArc as GeometryTutorLib.ConcreteAST.Segment;
                        if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.Segment>(segments, seg))
                        {
                            seg.ClearCollinear();
                            segments.Add(seg);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, seg.Point1);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, seg.Point2);
                        }
                    }
                    else if (conn.type == ConnectionType.ARC)
                    {
                        GeometryTutorLib.ConcreteAST.Arc arc = conn.segmentOrArc as GeometryTutorLib.ConcreteAST.Arc;
                        if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.Arc>(arcs, arc))
                        {
                            arc.ClearCollinear();
                            arcs.Add(arc);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint1);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint2);
                        }
                    }
                }
            }

            // Combine all points into a single list.
            GeometryTutorLib.Utilities.AddUniqueList<Point>(points, figurePoints);

            //
            //
            // Find all intersections...among segments and arcs.
            //
            //
            // Segment-Segment
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    Point intersection = segments[s1].FindIntersection(segments[s2]);
                    intersection = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, intersection, segments[s1], segments[s2]);

                    if (intersection != null)
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
                    pt1 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt1, segment, arc);
                    pt2 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt2, segment, arc);

                    if (pt1 != null)
                    {
                        segment.AddCollinearPoint(pt1);
                        arc.AddCollinearPoint(pt1);
                    }
                    if (pt2 != null)
                    {
                        segment.AddCollinearPoint(pt2);
                        arc.AddCollinearPoint(pt2);
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
                    pt1 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt1, arcs[a1], arcs[a2]);
                    pt2 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt2, arcs[a1], arcs[a2]);

                    if (pt1 != null)
                    {
                        arcs[a1].AddCollinearPoint(pt1);
                        arcs[a2].AddCollinearPoint(pt1);
                    }
                    if (pt2 != null)
                    {
                        arcs[a1].AddCollinearPoint(pt2);
                        arcs[a2].AddCollinearPoint(pt2);
                    }
                }
            }

            return AcquireAtomicRegionsFromGraph(points, segments, arcs, circles);
        }

        private static List<AtomicRegion> AcquireAtomicRegionsFromGraph(List<Point> points, List<GeometryTutorLib.ConcreteAST.Segment> segments,
                                                                        List<Arc> arcs, List<GeometryTutorLib.ConcreteAST.Circle> circles)
        {
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
            //
            foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
            {
                for (int p = 0; p < segment.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p + 1],
                                            new GeometryTutorLib.ConcreteAST.Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                }
            }

            foreach (Arc arc in arcs)
            {
                for (int p = 0; p < arc.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(arc.collinear[p], arc.collinear[p + 1],
                                            new GeometryTutorLib.ConcreteAST.Segment(arc.collinear[p], arc.collinear[p + 1]).Length,
                                            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
                }
            }

            //
            // Convert the planar graph to atomic regions.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();

            return PrimitiveToRegionConverter.Convert(graph, primitives, circles);
        }
    }
}