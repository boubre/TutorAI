using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    public class PathGenerator
    {
        // Matrix to indicate if, from a node, we can reach another node.
        private bool[,] reachable;
//        private List<Path>[,] paths;
        private List<PebblerHyperEdge> edges;
        private SharedPebbledNodeList sharedEdgeList;

        public PathGenerator(int n, SharedPebbledNodeList sharedData)
        {
            reachable = new bool[n, n];

            edges = new List<PebblerHyperEdge>();

            problems = new List<Problem>();
            sharedEdgeList = sharedData;

            pathHashMap = new PathHashMap(n);
        }

        private List<Problem> problems;
        private PathHashMap pathHashMap;

        //
        // If U |- n is a problem:
        //   let v \in U such that V |- v is a problem then
        //   generate a new problem: (U \ v) \bigcup V |- n provided: (U \ v) \bigcup V != { n } AND 
        //
        public void GeneratePaths(Problem firstNewProblem)
        {
            List<Problem> worklist = new List<Problem>();
            worklist.Add(firstNewProblem);

            while (worklist.Any())
            {
                // Acquire a new element to work on
                Problem thisNewProblem = worklist[0];
                worklist.RemoveAt(0);



                // Acquire the old problems in which we can combine with the problem under consideration
                List<Problem> oldApplicableProblems = pathHashMap.Get(thisNewProblem.goal);

                if (oldApplicableProblems != null)
                {
                    foreach (Problem oldProblem in oldApplicableProblems)
                    {
                        // Can we combine? That is, is the new goal in the old source; even more specifically,
                        // avoid a cycle by checking that the old goal is not in the thisNew source
                        if (!thisNewProblem.InSource(oldProblem.goal) && !thisNewProblem.InPath(oldProblem.goal))
                        {
                            // To avoid a cycle, avoid deducing this same node
                            if (!oldProblem.HasGoal(thisNewProblem.goal))
                            {
                                Utilities.AddUnique(worklist, oldProblem.CombineAndCreateNewProblem(thisNewProblem));
                            }
                        }
                    }
                }

                // Add this new edge as a problem, but avoid redundancy
                if (Utilities.AddUnique(problems, thisNewProblem))
                {
                    pathHashMap.Put(thisNewProblem);
                }
            }
        }

        //public void GeneratePaths(Problem firstNewProblem)
        //{
        //    List<Problem> worklist = new List<Problem>();
        //    worklist.Add(firstNewProblem);

        //    while (worklist.Any())
        //    {
        //        // Acquire a new element to work on
        //        Problem thisNewProblem = worklist[0];
        //        worklist.RemoveAt(0);

        //        foreach (Problem oldProblem in problems)
        //        {
        //            // Can we combine? That is, is the new goal in the old source; even more specifically,
        //            // avoid a cycle by checking that the old goal is not in the thisNew source
        //            if (oldProblem.InSource(thisNewProblem.goal) && !thisNewProblem.InSource(oldProblem.goal) && !thisNewProblem.InPath(oldProblem.goal))
        //            {
        //                // To avoid a cycle, avoid deducing this same node
        //                if (!oldProblem.HasGoal(thisNewProblem.goal))
        //                {
        //                    Utilities.AddUnique(worklist, oldProblem.CombineAndCreateNewProblem(thisNewProblem));
        //                }
        //            }
        //        }

        //        // Add this new edge as a problem
        //        Utilities.AddUnique(problems, thisNewProblem);
        //    }
        //}

        //
        // Adds a hyperedge to the reachability matrix as well as the predecessor matrix
        //
        public void AddEdge(PebblerHyperEdge edge)
        {
            Debug.WriteLine("Considering Edge: " + edge.ToString());
            AddToReachability(edge);
            //AddToForest(edge);

            edges.Add(edge);
            GeneratePaths(new Problem(edge.sourceNodes, edge.targetNode, edges.Count));
        }

        //
        // Consumer thread executes this method
        //
        public void GenerateAllPaths()
        {
            while (!sharedEdgeList.IsReadingAndWritingComplete()) // busy waiting?
            {
                // Acquire the new edge
                PebblerHyperEdge edge = sharedEdgeList.ReadEdge();

                // Create the corresponding problems
                Problem newProblem = new Problem(edge.sourceNodes, edge.targetNode, problems.Count);

                // Create the problem paths / solutions
                GeneratePaths(newProblem);
            }

            Debug.WriteLine("-------------------");
            foreach (Problem problem in problems)
            {
                Debug.WriteLine(problem.ToString());
            }

            //Debug.WriteLine("Path Map (Size: " + pathHashMap.size + "): \n" + pathHashMap.ToString());
         }

        public List<Problem> GetPaths()
        {
            return problems;
        }

        //
        // Adds to the forest under the following specification:
        // if a_1, ..., a_n  |- b is a pebbled edge then we add this edge
        // as a node in the tree if b exists as a leaf (root b with children a_1...a_n)
        // otherwise we add a new tree rooted at b with children a_1...a_n
        //
        //public void AddToForest(PebblerHyperEdge edge)
        //{
        //    if (reversePaths.AddToLeaf(edge.targetNode, edge.sourceNodes) == 0)
        //    {
        //        reversePaths.AddNewTree(edge.targetNode, edge.sourceNodes);
        //    }
        //    Debug.WriteLine("Forest: \n" + reversePaths.ToString());
        //}

        //
        // Adds a hyperedge to the reachability matrix; [r, c] indicates is there exists a path from r -> c
        //
        private void AddToReachability(PebblerHyperEdge edge)
        {
            //
            // for each node in the source list, indicate that they can reach the target.
            //
            // Create a single edge path
            Path newP = new Path(Utilities.MakeList<PebblerHyperEdge>(edge));
            foreach (int src in edge.sourceNodes)
            {
                reachable[src, edge.targetNode] = true;
                Debug.WriteLine("Setting <" + src + ", " + edge.targetNode + ">");

                //paths[src, edge.targetNode].Add(newP); // What about target?
            }

            //
            // Any predecessor of a source node can now reach the target.
            //
            foreach (int src in edge.sourceNodes)
            {
                for (int r = 0; r < reachable.GetLength(0); r++)
                {
                    if (reachable[r, src])
                    {
                        reachable[r, edge.targetNode] = true;
                        //Debug.WriteLine("Deep Setting Predecessor <" + r + ", " + edge.targetNode + ">");

                        // DeepPredecessorSet(r, src, edge);

                        //Debug.WriteLine("After deep Setting Predecessor (Paths): \n" + this.ToString());
                    }
                }
            }

            //
            // Any successor of the target can now be reached by the source nodes
            //
            foreach (int src in edge.sourceNodes)
            {
                for (int r = 0; r < reachable.GetLength(0); r++)
                {
                    if (reachable[edge.targetNode, r])
                    {
                        reachable[src, r] = true;
                        //Debug.WriteLine("Deep Setting Successor <" + src + ", " + r + ">");

                        // DeepSuccessorSet(src, r, edge);

                        //Debug.WriteLine("After deep Setting Successor (Paths): \n" + this.ToString());
                    }
                }
            }
        }

        //
        // Any predecessor of a source node can now reach the target.
        //
        //private void DeepPredecessorSet(int predOfSource, int source, PebblerHyperEdge edge)
        //{
        //    List<Path> allNewPaths = new List<Path>();
        //    List<Path> tempAllPaths = new List<Path>();

        //    // This is the base for all new paths
        //    foreach (Path singleRtoSrcPath in paths[predOfSource, source])
        //    {
        //        Path newPath = new Path(singleRtoSrcPath);
        //        newPath.AddToPath(edge);
        //        allNewPaths.Add(newPath);

        //    }

        //    // Collect all other path combinations from r to otherSource
        //    // Does so in a powerset-style construction
        //    foreach (int otherSrc in edge.sourceNodes)
        //    {
        //        if (source != otherSrc)
        //        {
        //            for (int c = 0; c < paths.GetLength(1); c++)
        //            {
        //                foreach (Path singleRtoOtherSrcPath in paths[c, otherSrc])
        //                {
        //                    foreach (Path newPath in allNewPaths)
        //                    {
        //                        // Make a copy
        //                        Path newPathCopy = new Path(newPath);

        //                        newPathCopy.AddToPath(singleRtoOtherSrcPath);
        //                        tempAllPaths.Add(newPathCopy);
        //                    }
        //                }
        //            }

        //            // if no predecessors of the otherSrc, don't overwrite the established paths
        //            if (tempAllPaths.Any()) allNewPaths = tempAllPaths;
        //        }
        //    }

        //    //paths[predOfSource, edge.targetNode].AddRange(allNewPaths);
        //}

        ////
        //// Any successor of the target can now be reached by the source nodes
        ////
        //private void DeepSuccessorSet(int source, int succOfTarget, PebblerHyperEdge edge)
        //{
        //    List<Path> allNewPaths = new List<Path>();

        //    // Collect all path combinations from src to target
        //    // Does so in a powerset-style construction

        //    // Initialize the powerset construction with the first source node
        //    allNewPaths = paths[edge.sourceNodes[0], edge.targetNode];

        //    for (int i = 1; i < edge.sourceNodes.Count; i++)
        //    {
        //        List<Path> tempAllPaths = new List<Path>();
        //        foreach (Path newPath in allNewPaths)
        //        {
        //            foreach (Path singleSrcToTargetPath in paths[edge.sourceNodes[i], edge.targetNode])
        //            {
        //                // Make a copy
        //                Path newPathCopy = new Path(newPath);

        //                newPathCopy.AddToPath(singleSrcToTargetPath);
        //                tempAllPaths.Add(newPathCopy);
        //            }
        //        }
        //        allNewPaths = tempAllPaths;
        //    }

        //    //
        //    // Add the new edge (source to target) to all the paths
        //    //
        //    foreach (Path newPath in allNewPaths)
        //    {
        //        newPath.AddToPath(edge);
        //    }

        //    //
        //    // For all nodes reachable from target to c, define the new path
        //    //
        //    for (int c = 0; c < reachable.GetLength(1); c++)
        //    {
        //        if (paths[edge.targetNode, c].Any())
        //        {
        //            List<Path> tempAllPaths = new List<Path>();
        //            foreach (Path newPath in allNewPaths)
        //            {
        //                foreach (Path singleTargetToRPath in paths[edge.targetNode, c])
        //                {
        //                    // Make a copy
        //                    Path newPathCopy = new Path(newPath);

        //                    newPathCopy.AddToPath(singleTargetToRPath);
        //                    tempAllPaths.Add(newPathCopy);
        //                }
        //            }
        //            paths[source, c].AddRange(tempAllPaths);
        //        }
        //    }
        //}

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            // Print the reachability matrix
            s.Append("   ");
            for (int c = 0; c < reachable.GetLength(1); c++)
            {
                s.Append((c % 10) + " ");
            }
            s.AppendLine();
            for (int r = 0; r < reachable.GetLength(0); r++)
            {
                s.Append(r + ": ");
                for (int c = 0; c < reachable.GetLength(1); c++)
                {
                    s.Append((reachable[r, c] ? "T" : "F") + " ");
                }
                s.AppendLine();
            }


            //
            // Print all paths
            //
            //s.AppendLine();
            //for (int r = 0; r < reachable.GetLength(0); r++)
            //{
            //    for (int c = 0; c < reachable.GetLength(1); c++)
            //    {
            //        if (paths[r, c].Any())
            //        {
            //            s.AppendLine("<" + r + ", " + c + ">: ");
            //            foreach (Path path in paths[r, c])
            //            {
            //                s.AppendLine("\t" + path.ToString());
            //            }
            //        }
            //    }
            //    s.AppendLine();
            //}

            return s.ToString();
        }
    }
}
