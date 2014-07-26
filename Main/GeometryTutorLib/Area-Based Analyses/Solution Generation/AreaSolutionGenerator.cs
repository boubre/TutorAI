using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaSolutionGenerator
    {
        private SolutionDatabase solutions;
        private List<Figure> topShapeForest;
        private List<AtomicRegion> figureAtoms;

        //
        // Initialize with the list of figures in the drawing. Do we need that knowledge?
        //
        public AreaSolutionGenerator(List<Figure> forest, List<AtomicRegion> atoms)
        {
            topShapeForest = forest;
            figureAtoms = atoms;

            // The initial size of the solution dictionary.
            int initSize = (int)Math.Min(Math.Pow(2, atoms.Count), Math.Pow(2, 12));

            solutions = new SolutionDatabase(initSize);
        }

        //
        // Assuming all solutions have been generated, acquire a single solution equation and area value.
        //
        public KeyValuePair<ComplexRegionEquation, double> GetSolution(List<AtomicRegion> desiredRegions)
        {
            return solutions.GetSolution(figureAtoms, desiredRegions);
        }

        //
        // Catalyst routine to the recursive solver: returns solution equation and actual area.
        //
        public void SolveAll(KnownMeasurementsAggregator known)
        {
            //
            // Preprocess any of the shape atoms to see if the area is computable.
            //
            for (int a = 0; a < figureAtoms.Count; a++)
            {
                ShapeAtomicRegion shapeAtom = figureAtoms[a] as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    double area = shapeAtom.GetArea(known);

                    if (area > 0)
                    {
                        Region atomRegion = new Region(figureAtoms[a]);

                        SolutionAgg agg = new SolutionAgg();

                        // The equation is the identity equation.
                        agg.solEq = new ComplexRegionEquation(atomRegion, atomRegion);
                        agg.solType = SolutionAgg.SolutionType.COMPUTABLE;
                        agg.solArea = area;
                        agg.atomIndices = new IndexList(a);

                        // Add this solution to the database.
                        solutions.AddRootSolution(agg);
                    }
                }
            }

            //
            // Using the atomic regions, explore all of the top-most shapes recursively.
            //
            for (int a = 0; a < figureAtoms.Count; a++)
            {
                IndexList atomIndexList = new IndexList(a);
                SolutionAgg agg = null;

                solutions.TryGetValue(atomIndexList, out agg);

                if (agg == null)
                {
                    Figure topShape = figureAtoms[a].GetTopMostShape();

                    // Shape Region?
                    ComplexRegionEquation startEq = new ComplexRegionEquation(null, new ShapeRegion(topShape));
                    double outerArea = topShape.GetArea(known);

                    // Invoke the recursive solver using the outermost region and catalyst.
                    //ProcessChildrenShapes(a, new ShapeRegion(topShape), topShape.Hierarchy(),
                    //             new List<TreeNode<Figure>>(),
                    //             startEq, outerArea, known);
                    SolveHelper(new ShapeRegion(topShape),
                                topShape.Hierarchy().Children(),
                                startEq, outerArea, known);
                }
                else if (agg.solType == SolutionAgg.SolutionType.COMPUTABLE)
                {
                    //solutions[atomIndexList] = agg;
                }
                else if (agg.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
                {
                    //solutions[atomIndexList] = agg;
                }
                else if (agg.solType == SolutionAgg.SolutionType.UNKNOWN)
                {
                    //TBD
                }
            }

            //
            // Subtraction of shapes extracts as many atomic regions as possible.the strict atomic regions, now compose those together.
            //
            ComposeAllRegions();
        }

        //
        // Given a shape that owns the atomic region, recur through the resulting atomic region
        //
        // From 
        //
        public void SolveHelper(Region currOuterRegion, List<TreeNode<Figure>> currHierarchyRoots,
                                ComplexRegionEquation currEquation, double currArea, KnownMeasurementsAggregator known)
        {
            IndexList currOuterRegionIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, currOuterRegion.atoms);

            // There is no outer region
            if (currOuterRegionIndices.IsEmpty()) return;

            //
            // We have reached this point by subtracting shapes, therefore, we have an equation.
            //
            SolutionAgg agg = new SolutionAgg();

            agg.solEq = new ComplexRegionEquation(currEquation);
            agg.solEq.SetTarget(currOuterRegion);
            agg.solType = currArea < 0 ? SolutionAgg.SolutionType.INCOMPUTABLE : SolutionAgg.SolutionType.COMPUTABLE;
            agg.solArea = currArea;
            agg.atomIndices = currOuterRegionIndices;

            //
            // Add this solution to the database.
            //
            solutions.AddRootSolution(agg);

            // Was this equation solving for a single atomic region? If so, leave. 
            if (currOuterRegion.IsAtomic()) return;

            //
            // Recursively explore EACH sub-shape root inside of the outer region.
            //
            foreach (TreeNode<Figure> shapeNode in currHierarchyRoots)
            {
                // A list omitting this shape
                List<TreeNode<Figure>> updatedHierarchy = new List<TreeNode<Figure>>(currHierarchyRoots);
                updatedHierarchy.Remove(shapeNode);

                // Process this shape
                ProcessShape(currOuterRegion, shapeNode, updatedHierarchy, currEquation, currArea, known);

                // Process the children
                ProcessChildrenShapes(currOuterRegion, shapeNode, updatedHierarchy, currEquation, currArea, known);
            }
        }

        //
        // Create the actual equation and continue processing recursively.
        //
        private void ProcessShape(Region currOuterRegion, TreeNode<Figure> currShape,
                                 List<TreeNode<Figure>> currHierarchyRoots, ComplexRegionEquation currEquation,
                                 double currArea, KnownMeasurementsAggregator known)
        {
            // Acquire the sub-shape.
            Figure currentFigure = currShape.GetData();

            // See what regions compose the subshape.
            ShapeRegion childShapeRegion = new ShapeRegion(currentFigure);

            // Make a copy of the current outer regions
            Region regionCopy = new Region(currOuterRegion);

            // Remove all regions from the outer region; if successful, recur on the new outer shape.
            if (regionCopy.Remove(childShapeRegion.atoms))
            {
                // Update the equation: copy and modify
                ComplexRegionEquation eqCopy = new ComplexRegionEquation(currEquation);
                eqCopy.AppendSubtraction(childShapeRegion);

                // Compute new area
                double currAreaCopy = currArea;
                if (currAreaCopy > 0)
                {
                    double currShapeArea = currentFigure.GetArea(known);

                    currAreaCopy = currShapeArea < 0 ? -1 : currAreaCopy - currShapeArea;
                }

                // Recur.
                SolveHelper(regionCopy, currHierarchyRoots, eqCopy, currAreaCopy, known);
            }
        }

        //
        // Process the child's shapes.
        //
        private void ProcessChildrenShapes(Region currOuterRegion, TreeNode<Figure> currShape,
                                           List<TreeNode<Figure>> currHierarchyRoots, ComplexRegionEquation currEquation,
                                           double currArea, KnownMeasurementsAggregator known)
        {
            foreach (TreeNode<Figure> childNode in currShape.Children())
            {
                // A copy of the children minus this shape.
                List<TreeNode<Figure>> childHierarchy = new List<TreeNode<Figure>>(currShape.Children());
                childHierarchy.Remove(childNode);

                // Add the hierarchy to the list of topmost hierarchical shapes.
                childHierarchy.AddRange(currHierarchyRoots);

                ProcessShape(currOuterRegion, childNode, childHierarchy, currEquation, currArea, known);
            }
        }



        //
        // Combine (through addition) any / all set of two equations in which there is no shared atomic region:
        //    (0 1 2) + (5 6) = (0 1 2 5 6)
        //
        // worklist-style fixpoint construction
        //
        private void ComposeAllRegions()
        {
            List<SolutionAgg> worklist = new List<SolutionAgg>(solutions.GetRootSolutions());

            while (worklist.Any())
            {
                SolutionAgg currentSol = worklist[0];
                worklist.RemoveAt(0);

                foreach (SolutionAgg otherSol in solutions.GetRootSolutions())
                {
                    HandleComposition(worklist, currentSol, otherSol);
                }

                foreach (SolutionAgg otherSol in solutions.GetExtendedSolutions())
                {
                    HandleComposition(worklist, currentSol, otherSol);
                }

                solutions.AddExtendedSolution(currentSol);
            }
        }

        private void HandleComposition(List<SolutionAgg> worklist, SolutionAgg first, SolutionAgg second)
        {
            // Addition of the two regions.
            SolutionAgg unionSol = HandleUnion(first, second);

            if (unionSol != null)
            {
                AddToWorklist(worklist, unionSol);
            }
            else
            {
                // Subtraction of the two regions.
                SolutionAgg diffSol = HandleDifference(first, second);
                if (diffSol != null) AddToWorklist(worklist, diffSol);
            }
        }

        //
        // If the solution is already in the database, update the solution in the database (if needed).
        // otherwise, add this solution to the worklist.
        //
        private void AddToWorklist(List<SolutionAgg> worklist, SolutionAgg solution)
        {
            // If the solution is already in the database, update the solution in the database (if needed). 
            if (solutions.Contains(solution))
            {
                solutions.AddExtendedSolution(solution);
            }
            else
            {
                // Is this region (set of indices) already in the worklist?
                // If not, add to the worklist.
                int worklistIndex = worklist.IndexOf(solution);

                if (worklistIndex == -1)
                {
                    worklist.Add(solution);
                    return;
                }

                //
                // If in the worklist, update the record with the shorter / computable version.
                // Same logic as the SolutionDatabase.
                //
                // Favor a straight-forward calculation of the area (no manipulations to acquire the value).
                if (worklist[worklistIndex].IsDirectArea()) return;

                // Favor a coomputable equation over incomputable.
                if (worklist[worklistIndex].solType == SolutionAgg.SolutionType.INCOMPUTABLE &&
                    solution.solType == SolutionAgg.SolutionType.COMPUTABLE)
                {
                    worklist[worklistIndex] = solution;
                    return;
                }
                // Again, favor a computable solution over not.
                else if (worklist[worklistIndex].solType == SolutionAgg.SolutionType.COMPUTABLE &&
                         solution.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
                {
                    return;
                }
                // The computability is the same for both equations.
                else if (worklist[worklistIndex].solType == solution.solType || worklist[worklistIndex].solType == SolutionAgg.SolutionType.UNKNOWN)
                {
                    if (!Utilities.CompareValues(worklist[worklistIndex].solArea, solution.solArea))
                    {
                        throw new Exception("Area for region " + worklist[worklistIndex].atomIndices.ToString() +
                                            " was calculated now as (" + worklist[worklistIndex].solArea + ") AND before (" + solution.solArea + ")");
                    }

                    if (worklist[worklistIndex].solEq.Length > solution.solEq.Length)
                    {
                        worklist[worklistIndex] = solution;
                        return;
                    }
                }
            }
        }

        private SolutionAgg HandleUnion(SolutionAgg first, SolutionAgg second)
        {
            // Can we combine the currentSol with this existent solution?
            IndexList unionIndices = IndexList.UnionIndices(first.atomIndices, second.atomIndices);

            // Disjoint union not possible.
            if (unionIndices == null) return null;

            //
            // Can combine; create a new solution / equation with addition.
            //
            SolutionAgg newSum = new SolutionAgg();
            newSum.atomIndices = unionIndices;
            newSum.solType = first.solType == SolutionAgg.SolutionType.COMPUTABLE &&
                             second.solType == SolutionAgg.SolutionType.COMPUTABLE ? SolutionAgg.SolutionType.COMPUTABLE : SolutionAgg.SolutionType.INCOMPUTABLE;
            newSum.solArea = newSum.solType == SolutionAgg.SolutionType.COMPUTABLE ? first.solArea + second.solArea : -1;
            newSum.solEq = new ComplexRegionEquation(MakeRegion(unionIndices.orderedIndices),
                                                     new ComplexRegionEquation.Binary(first.solEq.target, OperationT.ADDITION, second.solEq.target));
            return newSum;
        }

        private SolutionAgg HandleDifference(SolutionAgg first, SolutionAgg second)
        {
            // Can we combine the currentSol with this existent solution?
            IndexList diffIndices = IndexList.DifferenceIndices(first.atomIndices, second.atomIndices);

            // Disjoint union not possible.
            if (diffIndices == null) return null;

            //
            // Can combine; create a new solution / equation with addition.
            //
            SolutionAgg newSum = new SolutionAgg();
            newSum.atomIndices = diffIndices;
            newSum.solType = first.solType == SolutionAgg.SolutionType.COMPUTABLE &&
                             second.solType == SolutionAgg.SolutionType.COMPUTABLE ? SolutionAgg.SolutionType.COMPUTABLE : SolutionAgg.SolutionType.INCOMPUTABLE;
            if (newSum.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
            {
                newSum.solArea = -1;
            }
            else
            {
                newSum.solArea = first.solArea > second.solArea ? first.solArea - second.solArea : second.solArea - first.solArea;
            }

            if (first.atomIndices.Count > second.atomIndices.Count)
            {
                newSum.solEq = new ComplexRegionEquation(MakeRegion(diffIndices.orderedIndices),
                                                         new ComplexRegionEquation.Binary(first.solEq.target, OperationT.SUBTRACTION, second.solEq.target));
            }
            else
            {
                newSum.solEq = new ComplexRegionEquation(MakeRegion(diffIndices.orderedIndices),
                                                         new ComplexRegionEquation.Binary(second.solEq.target, OperationT.SUBTRACTION, first.solEq.target));
            }

            return newSum;
        }

        //
        // Makes a region out of a list of indices (of atomic regions).
        //
        private Region MakeRegion(List<int> indices)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            foreach (int index in indices)
            {
                atoms.Add(figureAtoms[index]);
            }

            //
            // CTA: Is this a shape region? Do we care?
            //

            return new Region(atoms);
        }













        ////
        //// On-Demand hypergraph construction.
        ////
        //public Hypergraph.Hypergraph<Region, SimpleRegionEquation> ConstructHypergraph()
        //{
        //    List<Region> worklist = new List<Region>();
        //    List<ShapeRegion> shapeRegions = new List<ShapeRegion>();

        //    // Add all the shapes to the worklist.
        //    foreach (Figure fig in ownerShapes)
        //    {
        //        ShapeRegion r = new ShapeRegion(fig.atoms, fig);

        //        shapeRegions.Add(r);

        //        worklist.Enqueue(r);
        //    }

        //    //
        //    // Deconstruct non-atomic regions, construct atomic regions.
        //    //
        //    while (worklist.Any())
        //    {
        //        Region currRegion = worklist.Dequeue();

        //        if (!currRegion.IsAtomic()) RegionDecomposition(currRegion, shapeRegions, worklist);

        //        if (currRegion.IsAtomic()) RegionComposition(currRegion, shapeRegions, worklist);
        //    }

        //    graph.DebugDumpClauses();

        //    Debug.WriteLine(graph.ToString());

        //    return graph;
        //}

        //// Ensures unique addition (no addition if the queue contains the element)
        //public void AddToWorklist(List<Region> worklist, Region r)
        //{
        //    foreach (Region wr in worklist)
        //    {
        //        if (wr.RegionDefinedBy(r.atoms)) return;
        //    }

        //    worklist.Enqueue(r);
        //}

        //// Acquire the ShapeRegion, if it exists, based on the set of atoms 
        //public ShapeRegion GetShapeRegion(List<ShapeRegion> shapeRegions, List<Atomizer.AtomicRegion> atoms)
        //{
        //    foreach (ShapeRegion sr in shapeRegions)
        //    {
        //        if (sr.RegionDefinedBy(atoms)) return sr;
        //    }

        //    return null;
        //}

        ////
        //// Take the curr region and deconstruct it into all its atoms to place in the hypergraph.
        ////
        //private void RegionDecomposition(Region currRegion, List<ShapeRegion> shapeRegions, List<Region> worklist)
        //{
        //    for (int a = 0; a < currRegion.atoms.Count; a++)
        //    {
        //        AtomicRegion currAtom = currRegion.atoms[a];

        //        if (currAtom is ShapeAtomicRegion)
        //        {
        //            // Make the difference-set of atoms.
        //            List<AtomicRegion> diffSet = new List<AtomicRegion>(currRegion.atoms);
        //            diffSet.RemoveAt(a);

        //            // Deconstruct the shape: Shape = atoms \ atom
        //            Region diffRegion = GetShapeRegion(shapeRegions, diffSet);
        //            if (diffRegion == null) diffRegion = new Region(diffSet);

        //            Region atomRegion = GetShapeRegion(shapeRegions, Utilities.MakeList<AtomicRegion>(currAtom));
        //            if (atomRegion == null) atomRegion = new Region(currAtom);

        //            // Add to the worklist if we have added something new.
        //            if (CreateAddNodesEdges(currRegion, diffRegion, atomRegion))
        //            {
        //                // Add the smaller regions to the worklist.
        //                AddToWorklist(worklist, diffRegion);

        //                // They can't be broken down, but they can be constructed.
        //                AddToWorklist(worklist, atomRegion);
        //            }
        //        }
        //    }
        //}

        ////
        //// Look at all nodes in the graph. If we can combine this atomic region with any region, add edges and nodes.
        ////
        //private void RegionComposition(Region currAtomRegion, List<ShapeRegion> shapeRegions, List<Region> worklist)
        //{
        //    AtomicRegion atom = currAtomRegion.atoms[0];

        //    // Look at all nodes in the graph
        //    int graphSize = graph.Size();
        //    for (int n = 0; n < graphSize; n++)
        //    {
        //        Region node = graph.vertices[n].data;

        //        // If the region in the graph does NOT have this atom, perform construction.
        //        if (!node.HasAtom(atom))
        //        {
        //            // Make the sum-set of atoms.
        //            List<AtomicRegion> sumAtoms = new List<AtomicRegion>(node.atoms);
        //            sumAtoms.Add(atom);

        //            // Construct the shape: Shape = atoms + atom
        //            Region sumRegion = GetShapeRegion(shapeRegions, sumAtoms);
        //            if (sumRegion == null) sumRegion = new Region(sumAtoms);

        //            // Add to the worklist if we have added something new.
        //            if (CreateAddNodesEdges(sumRegion, node, currAtomRegion))
        //            {
        //                // Add the smaller regions to the worklist.
        //                worklist.Enqueue(sumRegion);
        //            }
        //        }
        //    }
        //}

        ////
        //// Return T / F if we added anything new to the hypergraph
        ////
        //private bool CreateAddNodesEdges(Region large, Region small, Region atom)
        //{
        //    return CreateAddNodes(large, small, atom) || CreateAddEdges(large, small, atom);
        //}

        ////
        //// Add nodes
        ////
        //private bool CreateAddNodes(Region large, Region small, Region atom)
        //{
        //    bool addedNode = false;

        //    if (graph.AddNode(large)) addedNode = true;
        //    if (graph.AddNode(small)) addedNode = true;
        //    if (graph.AddNode(atom)) addedNode = true;

        //    return addedNode;
        //}


        ////
        //// Add edges
        ////
        //private bool CreateAddEdges(Region large, Region small, Region atom)
        //{
        //    bool addedEdge = false;

        //    //
        //    // A = (A \ a) + a
        //    //
        //    SimpleRegionEquation sumAnnotation = new SimpleRegionEquation(large, small, OperationT.ADDITION, atom);
        //    List<Region> sources = new List<Region>();
        //    sources.Add(small);
        //    sources.Add(atom);
        //    if (graph.AddEdge(sources, large, sumAnnotation)) addedEdge = true;

        //    //
        //    // (A \ a) = A - a
        //    //
        //    SimpleRegionEquation diff1Annotation = new SimpleRegionEquation(small, large, OperationT.SUBTRACTION, atom);
        //    sources = new List<Region>();
        //    sources.Add(large);
        //    sources.Add(atom);
        //    if (graph.AddEdge(sources, small, diff1Annotation)) addedEdge = true;

        //    //
        //    // a = A - (A \ a)
        //    //
        //    SimpleRegionEquation diff2Annotation = new SimpleRegionEquation(atom, large, OperationT.SUBTRACTION, small);
        //    sources = new List<Region>();
        //    sources.Add(large);
        //    sources.Add(small);
        //    if (graph.AddEdge(sources, atom, diff2Annotation)) addedEdge = true;

        //    return addedEdge;
        //}


        ////
        //// Dynamic Programming Style Solution Construction; equation and actual area.
        ////
        //public KeyValuePair<ComplexRegionEquation, double> Solve(List<Atomizer.AtomicRegion> atoms, KnownMeasurementsAggregator known)
        //{
        //    // Construct the memoization data structure.
        //    if (solutions == null) solutions = new ComplexRegionEquation[graph.Size()];

        //    // The region for which we will construct an equation.
        //    Region desired = new Region(atoms);

        //    // Determine if this region is actually in the hypergraph.
        //    int startIndex = graph.GetNodeIndex(desired);

        //    if (startIndex == -1)
        //    {
        //        throw new ArgumentException("Desired region not found in area hypergraph: " + desired);
        //    }
        //    // For marking if we have visited the given node already
        //    bool[] visited = new bool[graph.Size()];

        //    // Traverse dymanically to construct all equations.
        //    return DynamicVisit(startIndex, visited, known);
        //}

        ////
        //// Graph traversal to find shapes and thus the resulting equation (solution).
        ////
        //// Dynamic Programming: return the first solution found (which will be the shortest)
        ////
        //private KeyValuePair<ComplexRegionEquation, double> DynamicVisit(int startIndex, bool[] visited, KnownMeasurementsAggregator known)
        //{
        //    // The actual Region object for this node.
        //    Region thisRegion = graph.vertices[startIndex].data;

        //    // Cut off search if we've been here before.
        //    if (visited[startIndex]) return new KeyValuePair<ComplexRegionEquation, double>(solutions[startIndex], thisRegion.GetKnownArea());

        //    // We've been here now.
        //    visited[startIndex] = true;

        //    //
        //    // Can we compute the area of this node directly?
        //    //
        //    double area = thisRegion.GetArea(known);
        //    if (area > 0)
        //    {
        //        thisRegion.SetKnownArea(area);
        //        solutions[startIndex] = new ComplexRegionEquation(thisRegion, thisRegion);
        //        return new KeyValuePair<ComplexRegionEquation, double>(solutions[startIndex], thisRegion.GetKnownArea());
        //    }

        //    //
        //    // Does any of the edges satisfy this equation? Investigate dynamically.
        //    //
        //    // Complex equation resulting from each outgoing edge.
        //    ComplexRegionEquation shortestEq = null;
        //    area = 0;
        //    foreach (Hypergraph.HyperEdge<SimpleRegionEquation> edge in graph.vertices[startIndex].targetEdges)
        //    {
        //        KeyValuePair<ComplexRegionEquation, double> src1Eq = DynamicVisit(edge.sourceNodes[0], visited, known);
        //        KeyValuePair<ComplexRegionEquation, double> src2Eq = DynamicVisit(edge.sourceNodes[1], visited, known);

        //        // Success, we found a valid area expression for edge.
        //        if (src1Eq.Key != null && src2Eq.Key != null)
        //        {
        //            // Create a copy of the anootation for a simple region equation for this edge.
        //            SimpleRegionEquation simpleEdgeEq = new SimpleRegionEquation(edge.annotation);

        //            //
        //            // Make one complex equation performing substitutions.
        //            //
        //            ComplexRegionEquation complexEdgeEq = new ComplexRegionEquation(simpleEdgeEq);
        //            complexEdgeEq.Substitute(src1Eq.Key.target, src1Eq.Key.expr);
        //            complexEdgeEq.Substitute(src2Eq.Key.target, src2Eq.Key.expr);

        //            // Pick the shortest equation possible.
        //            if (shortestEq == null) shortestEq = complexEdgeEq;
        //            else if (shortestEq.Length > complexEdgeEq.Length) shortestEq = complexEdgeEq;

        //            if (edge.annotation.op == OperationT.ADDITION)
        //            {
        //                area = src1Eq.Value + src2Eq.Value;
        //            }
        //            else if (edge.annotation.op == OperationT.SUBTRACTION)
        //            {
        //                area = src1Eq.Value - src2Eq.Value;
        //            }
        //        }
        //    }

        //    //if (shortestEq != null)
        //    //{
        //    //    thisRegion.SetKnownArea(area);
        //    //    solutions[startIndex] = new ComplexRegionEquation(thisRegion, thisRegion);
        //    //    return new KeyValuePair<ComplexRegionEquation, double>(solutions[startIndex], area);
        //    //}

        //    solutions[startIndex] = shortestEq;

        //    return new KeyValuePair<ComplexRegionEquation, double>(solutions[startIndex], area);
        //}

























        ////
        //// Depth-First
        ////
        //public ComplexRegionEquation TraceRegionArea(Region thatRegion)
        //{
        //    // Find this region in the hypergraph.
        //    int startIndex = graph.GetNodeIndex(thatRegion);

        //    if (startIndex == -1) throw new ArgumentException("Desired region not found in area hypergraph: " + thatRegion);

        //    // Modifiable region equation: region = region so that we substitute into the RHS. 
        //    ComplexRegionEquation eq = new ComplexRegionEquation(thatRegion, thatRegion);

        //    bool[] visited = new bool[graph.Size()];

        //    //    // Precompute any shapes in which we know we can compute the area; this is done in BuildNodes()
        //    //    solutions = new ComplexRegionEquation[graph.Size()];

        //    // Traverse depth-first to construct all equations.
        //    bool success = SimpleVisit(startIndex, eq, visited);


        //    return success ? solutions[startIndex] : null;
        //}

        ////
        //// Graph traversal to find shapes and thus the resulting equation (solution).
        ////
        //// Depth first: construct along the way
        //// Find only the SHORTEST equation (based on the number of regions involved in the equation).
        ////
        //private bool SimpleVisit(int regionIndex, ComplexRegionEquation currentEq, bool[] visited)
        //{
        //    //
        //    // Deal with memoizing: keep the shortest equation for this particular region (@ regionIndex)
        //    //
        //    if (visited[regionIndex]) return true;

        //    // We have now visited this node.
        //    visited[regionIndex] = true;

        //    // Is this partitcular region a shape?
        //    // If so, save the basis equation in memozied.
        //    if (graph.vertices[regionIndex].data is ShapeRegion) return true;

        //    // For all hyperedges leaving this node, follow the edge sources
        //    foreach (Hypergraph.HyperEdge<SimpleRegionEquation> edge in graph.vertices[regionIndex].targetEdges)
        //    {
        //        // Will contain two equations representing expressions for the source node
        //        ComplexRegionEquation[] edgeEqs = new ComplexRegionEquation[edge.sourceNodes.Count];

        //        // For actively substituting into.
        //        ComplexRegionEquation currentEqCopy = new ComplexRegionEquation(currentEq);

        //        // Area can be calcualted either directly or using the GeoTutor deductive engine.
        //        bool canCalcArea = true;
        //        for (int e = 0; e < edge.sourceNodes.Count; e++)
        //        {
        //            // If we have already visited this node, we already have an equation for it; use it.
        //            if (visited[edge.sourceNodes[e]])
        //            {
        //                // Check if we cannot calculate the region area.
        //                if (solutions[edge.sourceNodes[e]] == null)
        //                {
        //                    canCalcArea = false;
        //                    break;
        //                }

        //                // Otherwise, we use the memoized version of this region equation for this source node.
        //                edgeEqs[e] = solutions[edge.sourceNodes[e]];
        //            }
        //            // We don't have a memoized version; calculate it.
        //            else
        //            {
        //                // Create an equation: region = region so that we substitute into the RHS.
        //                Region srcRegion = graph.vertices[edge.sourceNodes[e]].data;
        //                edgeEqs[e] = new ComplexRegionEquation(srcRegion, srcRegion);

        //                // This source node is not a shape: we can't directly calculate its area.
        //                if (!SimpleVisit(edge.sourceNodes[e], edgeEqs[e], visited))
        //                {
        //                    canCalcArea = false;
        //                    break;
        //                }
        //            }
        //        }

        //        //
        //        // If we have a successful search from this edge, create the corresponding region equation.
        //        //
        //        if (canCalcArea)
        //        {
        //            // We can substitute the annotation along the edge into the edge's target region (expression).

        //            // to find (val)
        //            currentEqCopy.Substitute(graph.vertices[edge.targetNode].data,
        //                // to sub (for val)
        //                                     new ComplexRegionEquation.Binary(edgeEqs[0].expr,
        //                                                                      edge.annotation.op,
        //                                                                      edgeEqs[0].expr));
        //            // Choose the shortest solution for this region
        //            if (currentEq.Length > currentEqCopy.Length)
        //            {
        //                currentEq = currentEqCopy;
        //            }
        //        }
        //    }

        //    // Did we find a solution?
        //    if (currentEq.Length == int.MaxValue) return false;

        //    // Success; save the solution.
        //    solutions[regionIndex] = currentEq;

        //    return true;
        //}

        //public ComplexRegionEquation TraceRegionArea(List<AtomicRegion> atoms)
        //{
        //    if (solutions == null) solutions = new ComplexRegionEquation[graph.Size()];

        //    return TraceRegionArea(new Region(atoms));
        //}


        //public AreaOnDemandHypergraph(List<AtomicRegion> atoms, List<Circle> circles, List<Polygon>[] polygons, List<Sector> minorSectors, List<Sector> majorSectors)
        //{
        //    // Ensure the capacity of the powerset.
        //    graph = new Hypergraph.Hypergraph<Region, SimpleRegionEquation>((int)Math.Pow(2, atoms.Count));

        //    BuildNodes(atoms, circles, polygons, minorSectors, majorSectors);
        //    BuildEdges(atoms);

        //    visited = new bool[graph.Size()];

        //    // Precompute any shapes in which we know we can compute the area; this is done in BuildNodes()
        //    solutions = new ComplexRegionEquation[graph.Size()];
        //}

        ////
        //// Reset the fact we have not visited any nodes.
        ////
        //public void Clear()
        //{
        //    visited = new bool[graph.Size()];
        //}

        ////
        //// The graph nodes are the powerset of atomic nodes
        ////
        //private void BuildNodes(List<AtomicRegion> atoms, List<Circle> circles, List<Polygon>[] polygons, List<Sector> minorSectors, List<Sector> majorSectors)
        //{
        //    // Acquire an integer representation of the powerset of atomic nodes
        //    List<List<int>> powerset = Utilities.ConstructPowerSetWithNoEmpty(atoms.Count);

        //    // Construct each element of the powerset
        //    for (int s = 0; s < powerset.Count; s++)
        //    {
        //        List<AtomicRegion> theseAtoms = new List<AtomicRegion>();

        //        // Construct the individual element (set)
        //        foreach (int e in powerset[s])
        //        {
        //            theseAtoms.Add(atoms[e]);
        //        }

        //        //// Create the region; If this set of atoms union together to make a single shape, a ShapeRegion is constructed
        //        //Region newRegion = Region.MakeRegion(theseAtoms, circles, polygons, minorSectors, majorSectors);

        //        //// Add the new node to the area hypergraph
        //        //graph.AddNode(newRegion);
        //    }
        //}

        ////
        //// Find the index of the given set; search only between the given indices for speed.
        ////
        //private int GetPowerSetIndex(List<List<int>> powerset, List<int> wanted, int startIndex, int stopIndex)
        //{
        //    for (int i = startIndex; i < stopIndex && i < powerset.Count; i++)
        //    {
        //        if (Utilities.EqualOrderedSets(powerset[i], wanted)) return i;
        //    }

        //    return -1;
        //}

        ////
        //// There is one addition edge and two subtraction edges per set of 3 nodes.
        //// Build the edges top-down from complete set of atoms down to singletons.
        ////
        //private void BuildEdges(List<AtomicRegion> atoms)
        //{
        //    // Acquire an integer representation of the powerset of atomic nodes
        //    // This is memoized so it's fast.
        //    List<List<int>> powerset = Utilities.ConstructPowerSetWithNoEmpty(atoms.Count);

        //    //
        //    // For each layer (of particular subset size), establish all links.
        //    //
        //    int setIndex = atoms.Count; // Skip the singletons
        //    int currSetSize = 2;
        //    int prevLayerStartIndex = 0;
        //    while (setIndex < powerset.Count)
        //    {
        //        //
        //        // For each layer, look at each individual set and deconstruct
        //        //
        //        int layerSize = (int)Utilities.Combination(atoms.Count, currSetSize++);

        //        for (int layerIndex = 0; layerIndex < layerSize; layerIndex++)
        //        {
        //            int currentIndex = setIndex + layerIndex;

        //            // Look at each set
        //            // Take away, in turn, each element in the set to construct the desired edges.
        //            List<int> currentSet = powerset[currentIndex];
        //            foreach (int val in currentSet)
        //            {
        //                // Make a copy of this set and remove the element
        //                List<int> differenceSet = new List<int>(currentSet);
        //                differenceSet.Remove(val);

        //                int singletonIndex = val; // the index of a singleton corresponds to its value
        //                int differenceIndex = GetPowerSetIndex(powerset, differenceSet, prevLayerStartIndex, setIndex + layerSize);

        //                //
        //                // Build the edge for this 3 node combinations.
        //                //

        //                //
        //                // A = (A \ a) + a
        //                //
        //                SimpleRegionEquation sumAnn = new SimpleRegionEquation(graph.vertices[currentIndex].data,
        //                                                                       graph.vertices[differenceIndex].data, OperationT.ADDITION, graph.vertices[singletonIndex].data);
        //                List<int> sourceIndices = new List<int>();
        //                sourceIndices.Add(differenceIndex);
        //                sourceIndices.Add(singletonIndex);
        //                graph.AddIndexEdge(sourceIndices, currentIndex, sumAnn);

        //                //
        //                // (A \ a) = A - a
        //                //
        //                SimpleRegionEquation diffAnn1 = new SimpleRegionEquation(graph.vertices[differenceIndex].data,
        //                                                                         graph.vertices[currentIndex].data, OperationT.SUBTRACTION, graph.vertices[singletonIndex].data);
        //                sourceIndices = new List<int>();
        //                sourceIndices.Add(currentIndex);
        //                sourceIndices.Add(singletonIndex);
        //                graph.AddIndexEdge(sourceIndices, differenceIndex, diffAnn1);

        //                //
        //                // a = A - (A \ a)
        //                //
        //                SimpleRegionEquation diffAnn2 = new SimpleRegionEquation(graph.vertices[singletonIndex].data,
        //                                                                         graph.vertices[currentIndex].data, OperationT.SUBTRACTION, graph.vertices[differenceIndex].data);
        //                sourceIndices = new List<int>();
        //                sourceIndices.Add(currentIndex);
        //                sourceIndices.Add(differenceIndex);
        //                graph.AddIndexEdge(sourceIndices, singletonIndex, diffAnn2);
        //            }
        //        }
        //        prevLayerStartIndex = setIndex;
        //        setIndex += layerSize;
        //    }

        //}

        ////
        //// There is one addition edge and two subtraction edges per set of 3 nodes.
        //// Build the edges top-down from complete set of atoms down to singletons.
        ////
        //private void BuildEdges(List<AtomicRegion> atoms, bool[] marked)
        //{
        //    // We don't want edges connecting a singleton region to an 'empty' region.
        //    if (atoms.Count == 1) return;

        //    // The node for this set of list of atoms.
        //    Region atomsRegion = new Region(atoms);

        //    // Check to see if we have already visited this node and constructed the edges.
        //    int nodeIndex = graph.GetNodeIndex(atomsRegion);
        //    if (marked[nodeIndex]) return;

        //    foreach (AtomicRegion atom in atoms)
        //    {
        //        List<AtomicRegion> atomsMinusAtom = new List<AtomicRegion>(atoms);
        //        atomsMinusAtom.Remove(atom);

        //        Region aMinus1Region = new Region(atomsMinusAtom);
        //        Region atomRegion = new Region(atom);

        //        //
        //        // A = (A \ a) + a
        //        //
        //        SimpleRegionEquation sumAnnotation = new SimpleRegionEquation(atomsRegion, aMinus1Region, OperationT.ADDITION, atomRegion);
        //        List<Region> sources = new List<Region>();
        //        sources.Add(aMinus1Region);
        //        sources.Add(atomRegion);
        //        graph.AddEdge(sources, atomsRegion, sumAnnotation);

        //        //
        //        // (A \ a) = A - a
        //        //
        //        SimpleRegionEquation diff1Annotation = new SimpleRegionEquation(aMinus1Region, atomsRegion, OperationT.SUBTRACTION, atomRegion);
        //        sources = new List<Region>();
        //        sources.Add(atomsRegion);
        //        sources.Add(atomRegion);
        //        graph.AddEdge(sources, aMinus1Region, diff1Annotation);

        //        //
        //        // a = A - (A \ a)
        //        //
        //        SimpleRegionEquation diff2Annotation = new SimpleRegionEquation(atomRegion, atomsRegion, OperationT.SUBTRACTION, aMinus1Region);
        //        sources = new List<Region>();
        //        sources.Add(atomsRegion);
        //        sources.Add(aMinus1Region);
        //        graph.AddEdge(sources, atomRegion, diff2Annotation);

        //        //
        //        // Recursive call to construct edges with A \ a
        //        //
        //        BuildEdges(atomsMinusAtom, marked);
        //    }

        //    Debug.WriteLine(graph.EdgeCount());
        //    marked[nodeIndex] = true;
        //}

    }
}